using FreelanceFlow.Backend.DTOs.Auth;
using FreelanceFlow.Backend.Helpers;

namespace FreelanceFlow.Backend.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto dto);
    Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(Guid userId);
}