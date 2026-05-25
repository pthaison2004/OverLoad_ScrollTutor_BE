using System;
using System.Collections.Generic;

namespace Overload.DataAccessLayer.Entities;

public partial class UserLessonProgress
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid LessonId { get; set; }

    public decimal? LastScrollPercentage { get; set; }

    public int? UnlockedCheckpointIndex { get; set; }

    public bool? Completed { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? LastPositionSeconds { get; set; }

    public int? WatchTimeSeconds { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
