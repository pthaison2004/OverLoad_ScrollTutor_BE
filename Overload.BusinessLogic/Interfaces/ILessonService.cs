using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Overload.BusinessLogic.Dtos;

namespace Overload.BusinessLogic.Interfaces;

public interface ILessonService
{
    Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId);

    Task<LessonDto> GetLessonByIdAsync(Guid lessonId, Guid userId);

    /// <summary>
    /// Gets the interactive steps content for a lesson.
    /// The content JSON is parsed from the lesson's Content field and cached.
    /// </summary>
    Task<LessonContentDto> GetLessonStepsAsync(Guid lessonId, Guid userId);

    /// <summary>
    /// Validates a user's answer at a specific checkpoint in a lesson.
    /// On success, updates the user's unlocked checkpoint index.
    /// </summary>
    Task<CheckpointResultDto> ValidateCheckpointAnswerAsync(Guid userId, ValidateCheckpointDto dto);
}
