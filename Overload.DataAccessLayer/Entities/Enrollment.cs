using System;
using System.Collections.Generic;

namespace Overload.DataAccessLayer.Entities;

public partial class Enrollment
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CourseId { get; set; }

    public DateTime? EnrolledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public decimal? ProgressPercentage { get; set; }

    public DateTime? LastAccessedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
