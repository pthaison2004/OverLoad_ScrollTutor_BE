using System;

namespace Overload.BusinessLogic.Dtos;

public class CourseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }
}
