using System;
using System.ComponentModel.DataAnnotations;

namespace Overload.BusinessLogic.Dtos;

public class UpdateScrollDto
{
    [Required]
    public Guid LessonId { get; set; }

    [Range(0, 100)]
    public decimal ScrollPercentage { get; set; }

    public int CheckpointIndex { get; set; }
}
