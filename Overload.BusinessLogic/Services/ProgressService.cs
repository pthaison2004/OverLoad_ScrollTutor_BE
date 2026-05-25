using System;
using System.Linq;
using System.Threading.Tasks;
using Overload.BusinessLogic.Dtos;
using Overload.BusinessLogic.Interfaces;
using Overload.DataAccessLayer.Entities;
using Overload.DataAccessLayer.Repositories;

namespace Overload.BusinessLogic.Services;

public class ProgressService : IProgressService
{
    private readonly IRepository<UserLessonProgress> _progressRepository;
    private readonly IRepository<Lesson> _lessonRepository;
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IRepository<Course> _courseRepository;

    public ProgressService(
        IRepository<UserLessonProgress> progressRepository,
        IRepository<Lesson> lessonRepository,
        IRepository<Enrollment> enrollmentRepository,
        IRepository<Course> courseRepository)
    {
        _progressRepository = progressRepository;
        _lessonRepository = lessonRepository;
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
    }

    public async Task UpdateScrollAsync(Guid userId, UpdateScrollDto dto)
    {
        // 1. Validate if lesson exists
        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);
        if (lesson == null)
        {
            throw new ArgumentException($"Lesson with ID {dto.LessonId} does not exist.");
        }

        // 2. Find existing progress
        var progressItems = await _progressRepository.FindAsync(p => p.UserId == userId && p.LessonId == dto.LessonId);
        var progress = progressItems.FirstOrDefault();

        var now = DateTime.UtcNow;

        if (progress == null)
        {
            // Create new progress record
            progress = new UserLessonProgress
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LessonId = dto.LessonId,
                LastScrollPercentage = dto.ScrollPercentage,
                UnlockedCheckpointIndex = dto.CheckpointIndex,
                Completed = dto.ScrollPercentage >= 100,
                CompletedAt = dto.ScrollPercentage >= 100 ? now : null,
                CreatedAt = now,
                UpdatedAt = now
            };
            await _progressRepository.AddAsync(progress);
        }
        else
        {
            // Update existing progress record
            progress.LastScrollPercentage = dto.ScrollPercentage;
            
            // Keep the maximum checkpoint index reached
            progress.UnlockedCheckpointIndex = Math.Max(progress.UnlockedCheckpointIndex ?? 0, dto.CheckpointIndex);
            
            progress.UpdatedAt = now;

            // Mark as completed if 100% (or already completed)
            if (dto.ScrollPercentage >= 100 && (progress.Completed != true))
            {
                progress.Completed = true;
                progress.CompletedAt = now;
            }
        }

        // 3. Save progress
        await _progressRepository.SaveAsync();

        // 4. Update enrollment stats
        var enrollments = await _enrollmentRepository.FindAsync(e => e.UserId == userId && e.CourseId == lesson.CourseId);
        var enrollment = enrollments.FirstOrDefault();
        if (enrollment != null)
        {
            enrollment.LastAccessedAt = now;

            // Recalculate progress percentage
            var courseLessons = await _lessonRepository.FindAsync(l => l.CourseId == lesson.CourseId);
            var courseLessonIds = courseLessons.Select(l => l.Id).ToList();

            var userProgressItems = await _progressRepository.FindAsync(p => p.UserId == userId && courseLessonIds.Contains(p.LessonId));
            
            int completedLessons = userProgressItems.Count(p => p.Completed == true);
            int totalLessons = courseLessons.Count();

            enrollment.ProgressPercentage = totalLessons > 0 
                ? Math.Round((decimal)completedLessons * 100m / totalLessons, 2)
                : 0m;

            if (completedLessons == totalLessons && totalLessons > 0)
            {
                if (enrollment.CompletedAt == null)
                {
                    enrollment.CompletedAt = now;
                }
            }
            else
            {
                enrollment.CompletedAt = null;
            }

            _enrollmentRepository.Update(enrollment);
            await _enrollmentRepository.SaveAsync();
        }
    }

    public async Task<ContinueLearningDto?> GetContinueLearningAsync(Guid userId)
    {
        // 1. Get user enrollments ordered by active access date
        var enrollments = await _enrollmentRepository.FindAsync(e => e.UserId == userId);
        var activeEnrollments = enrollments.OrderByDescending(e => e.LastAccessedAt).ToList();

        var courses = await _courseRepository.GetAllAsync();
        var courseMap = courses.ToDictionary(c => c.Id);

        foreach (var enrollment in activeEnrollments)
        {
            if (!courseMap.TryGetValue(enrollment.CourseId, out var course))
            {
                continue;
            }

            // Get all lessons in the course
            var lessons = await _lessonRepository.FindAsync(l => l.CourseId == enrollment.CourseId);
            var sortedLessons = lessons.OrderBy(l => l.OrderIndex).ToList();
            if (!sortedLessons.Any())
            {
                continue;
            }

            // Get user progress for all course lessons
            var lessonIds = sortedLessons.Select(l => l.Id).ToList();
            var progresses = await _progressRepository.FindAsync(p => p.UserId == userId && lessonIds.Contains(p.LessonId));
            var progressMap = progresses.ToDictionary(p => p.LessonId);

            // Find the first uncompleted lesson
            Lesson? nextLesson = null;
            foreach (var lesson in sortedLessons)
            {
                if (!progressMap.TryGetValue(lesson.Id, out var prog) || prog.Completed != true)
                {
                    nextLesson = lesson;
                    break;
                }
            }

            // If we found an active lesson, return it
            if (nextLesson != null)
            {
                progressMap.TryGetValue(nextLesson.Id, out var activeProg);
                
                return new ContinueLearningDto
                {
                    CourseId = enrollment.CourseId,
                    CourseTitle = course.Title,
                    ProgressPercentage = enrollment.ProgressPercentage ?? 0m,
                    LessonId = nextLesson.Id,
                    LessonTitle = nextLesson.Title,
                    OrderIndex = nextLesson.OrderIndex,
                    LastScrollPercentage = activeProg?.LastScrollPercentage ?? 0m,
                    UnlockedCheckpointIndex = activeProg?.UnlockedCheckpointIndex ?? 0
                };
            }
        }

        return null;
    }
}
