using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Overload.BusinessLogic.Dtos;

namespace Overload.BusinessLogic.Interfaces;

public interface ILessonService
{
    Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId);

    Task<LessonDto> GetLessonByIdAsync(Guid lessonId, Guid userId);
}
