using System;
using System.Collections.Generic;

namespace Overload.DataAccessLayer.Entities;

public partial class Lesson
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Content { get; set; } = null!;

    public int? DurationMinutes { get; set; }

    public int OrderIndex { get; set; }

    public bool? IsFree { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<UserLessonProgress> UserLessonProgresses { get; set; } = new List<UserLessonProgress>();
}
