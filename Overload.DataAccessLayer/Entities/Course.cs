using System;
using System.Collections.Generic;

namespace Overload.DataAccessLayer.Entities;

public partial class Course
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? Category { get; set; }

    public string? Level { get; set; }

    public bool? IsPublished { get; set; }

    public int? TotalDurationMinutes { get; set; }

    public int? TotalLessons { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
