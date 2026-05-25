using System.Threading.Tasks;
using Overload.BusinessLogic.Dtos;

namespace Overload.BusinessLogic.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
