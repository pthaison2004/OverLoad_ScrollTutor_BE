using System.Collections.Generic;
using System.Threading.Tasks;
using Overload.BusinessLogic.Dtos;

namespace Overload.BusinessLogic.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetCoursesAsync();
}
