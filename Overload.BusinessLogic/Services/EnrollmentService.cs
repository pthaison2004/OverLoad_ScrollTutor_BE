using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Overload.BusinessLogic.Dtos;
using Overload.BusinessLogic.Interfaces;
using Overload.DataAccessLayer.Entities;
using Overload.DataAccessLayer.Repositories;

namespace Overload.BusinessLogic.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IRepository<Course> _courseRepository;

    public EnrollmentService(
        IRepository<Enrollment> enrollmentRepository,
        IRepository<Course> courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<EnrollmentDto> EnrollAsync(Guid userId, Guid courseId)
    {
        // 1. Verify course exists
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new ArgumentException("Course not found.");
        }

        // 2. Check if already enrolled
        var existing = await _enrollmentRepository.FindAsync(e => e.UserId == userId && e.CourseId == courseId);
        if (existing.Any())
        {
            var en = existing.First();
            return new EnrollmentDto
            {
                Id = en.Id,
                UserId = en.UserId,
                CourseId = en.CourseId,
                CourseTitle = course.Title,
                CourseDescription = course.Description,
                EnrolledAt = en.EnrolledAt,
                CompletedAt = en.CompletedAt,
                ProgressPercentage = en.ProgressPercentage ?? 0,
                LastAccessedAt = en.LastAccessedAt
            };
        }

        // 3. Create new enrollment
        var now = DateTime.UtcNow;
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = now,
            CompletedAt = null,
            ProgressPercentage = 0,
            LastAccessedAt = now
        };

        await _enrollmentRepository.AddAsync(enrollment);
        await _enrollmentRepository.SaveAsync();

        return new EnrollmentDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId,
            CourseTitle = course.Title,
            CourseDescription = course.Description,
            EnrolledAt = enrollment.EnrolledAt,
            CompletedAt = enrollment.CompletedAt,
            ProgressPercentage = enrollment.ProgressPercentage ?? 0,
            LastAccessedAt = enrollment.LastAccessedAt
        };
    }

    public async Task<IEnumerable<EnrollmentDto>> GetMyEnrollmentsAsync(Guid userId)
    {
        var enrollments = await _enrollmentRepository.FindAsync(e => e.UserId == userId);
        var courses = await _courseRepository.GetAllAsync();
        var courseMap = courses.ToDictionary(c => c.Id);

        return enrollments.Select(e =>
        {
            courseMap.TryGetValue(e.CourseId, out var course);
            return new EnrollmentDto
            {
                Id = e.Id,
                UserId = e.UserId,
                CourseId = e.CourseId,
                CourseTitle = course?.Title ?? "Unknown Course",
                CourseDescription = course?.Description,
                EnrolledAt = e.EnrolledAt,
                CompletedAt = e.CompletedAt,
                ProgressPercentage = e.ProgressPercentage ?? 0,
                LastAccessedAt = e.LastAccessedAt
            };
        }).OrderByDescending(e => e.LastAccessedAt);
    }
}
