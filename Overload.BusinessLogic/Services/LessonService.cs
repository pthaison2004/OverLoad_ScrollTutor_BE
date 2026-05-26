using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Overload.BusinessLogic.Dtos;
using Overload.BusinessLogic.Interfaces;
using Overload.DataAccessLayer.Entities;
using Overload.DataAccessLayer.Repositories;

namespace Overload.BusinessLogic.Services;

public class LessonService : ILessonService
{
    private readonly IRepository<Lesson> _lessonRepository;
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IRepository<UserLessonProgress> _progressRepository;
    private readonly IMemoryCache _cache;

    // Cache options: lesson content is cached for 30 minutes
    private static readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

    public LessonService(
        IRepository<Lesson> lessonRepository,
        IRepository<Enrollment> enrollmentRepository,
        IRepository<UserLessonProgress> progressRepository,
        IMemoryCache cache)
    {
        _lessonRepository = lessonRepository;
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
        _cache = cache;
    }

    public async Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        var lessons = await _lessonRepository.FindAsync(l => l.CourseId == courseId);
        return lessons.OrderBy(l => l.OrderIndex).Select(l => new LessonDto
        {
            Id = l.Id,
            CourseId = l.CourseId,
            Title = l.Title,
            Description = l.Description,
            Content = null, // Hide content in listing
            DurationMinutes = l.DurationMinutes,
            OrderIndex = l.OrderIndex,
            IsFree = l.IsFree ?? false
        });
    }

    public async Task<LessonDto> GetLessonByIdAsync(Guid lessonId, Guid userId)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
        {
            throw new ArgumentException("Lesson not found.");
        }

        var isFree = lesson.IsFree ?? false;
        var hasAccess = isFree;

        if (!hasAccess)
        {
            // Check enrollment
            var enrollments = await _enrollmentRepository.FindAsync(e => e.UserId == userId && e.CourseId == lesson.CourseId);
            if (enrollments.Any())
            {
                hasAccess = true;
            }
        }

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("You must enroll in this course to view this premium lesson.");
        }

        return new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Description = lesson.Description,
            Content = lesson.Content, // Populate content since they have access
            DurationMinutes = lesson.DurationMinutes,
            OrderIndex = lesson.OrderIndex,
            IsFree = isFree
        };
    }

    public async Task<LessonContentDto> GetLessonStepsAsync(Guid lessonId, Guid userId)
    {
        // 1. Verify access
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
        {
            throw new ArgumentException("Lesson not found.");
        }

        var isFree = lesson.IsFree ?? false;
        if (!isFree)
        {
            var enrollments = await _enrollmentRepository.FindAsync(
                e => e.UserId == userId && e.CourseId == lesson.CourseId);
            if (!enrollments.Any())
            {
                throw new UnauthorizedAccessException("You must enroll in this course to view this lesson's steps.");
            }
        }

        // 2. Parse steps from content JSON, with caching
        var cacheKey = $"lesson_steps_{lessonId}";
        if (!_cache.TryGetValue(cacheKey, out LessonContentDto? cachedContent))
        {
            cachedContent = ParseLessonContent(lesson);
            _cache.Set(cacheKey, cachedContent, _cacheOptions);
        }

        return cachedContent!;
    }

    public async Task<CheckpointResultDto> ValidateCheckpointAnswerAsync(Guid userId, ValidateCheckpointDto dto)
    {
        // 1. Get lesson and parse its content
        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);
        if (lesson == null)
        {
            throw new ArgumentException("Lesson not found.");
        }

        var cacheKey = $"lesson_steps_{dto.LessonId}";
        if (!_cache.TryGetValue(cacheKey, out LessonContentDto? content))
        {
            content = ParseLessonContent(lesson);
            _cache.Set(cacheKey, content, _cacheOptions);
        }

        // 2. Find the checkpoint
        var stepWithCheckpoint = content!.Steps
            .FirstOrDefault(s => s.Checkpoint != null && s.Checkpoint.CheckpointIndex == dto.CheckpointIndex);

        if (stepWithCheckpoint?.Checkpoint == null)
        {
            throw new ArgumentException($"Checkpoint with index {dto.CheckpointIndex} not found in this lesson.");
        }

        var checkpoint = stepWithCheckpoint.Checkpoint;

        // 3. Compare answer (case-insensitive trim comparison)
        var isCorrect = string.Equals(
            dto.UserAnswer.Trim(),
            checkpoint.CorrectAnswer.Trim(),
            StringComparison.OrdinalIgnoreCase);

        var result = new CheckpointResultDto
        {
            IsCorrect = isCorrect,
            Message = isCorrect
                ? "Correct! You can proceed to the next section."
                : "Incorrect. Please try again."
        };

        // 4. If correct, update the user's unlocked checkpoint index
        if (isCorrect)
        {
            var progressItems = await _progressRepository.FindAsync(
                p => p.UserId == userId && p.LessonId == dto.LessonId);
            var progress = progressItems.FirstOrDefault();

            var now = DateTime.UtcNow;
            var newIndex = dto.CheckpointIndex + 1;

            if (progress == null)
            {
                progress = new UserLessonProgress
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LessonId = dto.LessonId,
                    LastScrollPercentage = 0,
                    UnlockedCheckpointIndex = newIndex,
                    Completed = false,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await _progressRepository.AddAsync(progress);
            }
            else
            {
                // Only advance forward, never backwards
                if (newIndex > (progress.UnlockedCheckpointIndex ?? 0))
                {
                    progress.UnlockedCheckpointIndex = newIndex;
                    progress.UpdatedAt = now;
                }
            }

            await _progressRepository.SaveAsync();
            result.NewUnlockedCheckpointIndex = Math.Max(newIndex, progress.UnlockedCheckpointIndex ?? 0);
        }

        return result;
    }

    /// <summary>
    /// Parses the lesson's Content JSON field into a LessonContentDto.
    /// The Content field is expected to be a JSON object with "total_steps" and "steps" properties.
    /// </summary>
    private static LessonContentDto ParseLessonContent(Lesson lesson)
    {
        if (string.IsNullOrWhiteSpace(lesson.Content))
        {
            return new LessonContentDto
            {
                LessonId = lesson.Id,
                Title = lesson.Title,
                TotalSteps = 0,
                Steps = new List<LessonStepDto>()
            };
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Try to deserialize the content as a LessonContentDto
            var parsed = JsonSerializer.Deserialize<LessonContentDto>(lesson.Content, options);
            if (parsed != null)
            {
                parsed.LessonId = lesson.Id;
                parsed.Title = lesson.Title;
                return parsed;
            }
        }
        catch (JsonException)
        {
            // Content is not valid JSON steps format - return as a single narrative step
        }

        // Fallback: treat content as plain text narrative in a single step
        return new LessonContentDto
        {
            LessonId = lesson.Id,
            Title = lesson.Title,
            TotalSteps = 1,
            Steps = new List<LessonStepDto>
            {
                new LessonStepDto
                {
                    StepIndex = 1,
                    TriggerPercentage = 0,
                    Narrative = lesson.Content,
                    CodeAction = "none",
                    CodeSnippet = ""
                }
            }
        };
    }
}
