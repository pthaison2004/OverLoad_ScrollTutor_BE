using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Overload.BusinessLogic.Dtos;
using Overload.BusinessLogic.Interfaces;
using Overload.DataAccessLayer.Entities;
using Overload.DataAccessLayer.Repositories;

namespace Overload.BusinessLogic.Services;

public class CourseService : ICourseService
{
    private readonly IRepository<Course> _repository;

    public CourseService(IRepository<Course> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CourseDto>> GetCoursesAsync()
    {
        var courses = await _repository.GetAllAsync();

        return courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description
        });
    }
}
