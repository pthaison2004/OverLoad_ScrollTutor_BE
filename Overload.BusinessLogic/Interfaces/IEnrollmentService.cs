using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Overload.BusinessLogic.Dtos;

namespace Overload.BusinessLogic.Interfaces;

public interface IEnrollmentService
{
    Task<EnrollmentDto> EnrollAsync(Guid userId, Guid courseId);

    Task<IEnumerable<EnrollmentDto>> GetMyEnrollmentsAsync(Guid userId);
}
