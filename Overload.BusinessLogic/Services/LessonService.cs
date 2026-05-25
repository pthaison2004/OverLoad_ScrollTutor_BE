using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Overload.BusinessLogic.Dtos;
using Overload.BusinessLogic.Interfaces;
using Overload.DataAccessLayer.Entities;
using Overload.DataAccessLayer.Repositories;

namespace Overload.BusinessLogic.Services;

public class LessonService : ILessonService
{
    private readonly IRepository<Lesson> _lessonRepository;
    private readonly IRepository<Enrollment> _enrollmentRepository;

    public LessonService(
        IRepository<Lesson> lessonRepository,
        IRepository<Enrollment> enrollmentRepository)
    {
        _lessonRepository = lessonRepository;
        _enrollmentRepository = enrollmentRepository;
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
}
