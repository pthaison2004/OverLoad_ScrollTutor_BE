using System;
using System.Threading.Tasks;
using Overload.BusinessLogic.Dtos;

namespace Overload.BusinessLogic.Interfaces;

public interface IProgressService
{
    Task UpdateScrollAsync(Guid userId, UpdateScrollDto dto);

    Task<ContinueLearningDto?> GetContinueLearningAsync(Guid userId);
}
