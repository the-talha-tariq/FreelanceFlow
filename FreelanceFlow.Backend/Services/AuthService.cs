using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using FreelanceFlow.Backend.DTOs.Auth;
using FreelanceFlow.Backend.Helpers;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Repositories.Interfaces;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
        {
            return ServiceResult<AuthResponseDto>.FailureResult("An account with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            Currency = dto.Currency
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
        {
            return ServiceResult<AuthResponseDto>.FailureResult(
                createResult.Errors.Select(e => e.Description));
        }

        var response = await GenerateAuthResponseAsync(user);
        return ServiceResult<AuthResponseDto>.SuccessResult(response);
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            return ServiceResult<AuthResponseDto>.FailureResult("Invalid email or password.");
        }

        var response = await GenerateAuthResponseAsync(user);
        return ServiceResult<AuthResponseDto>.SuccessResult(response);
    }

    public async Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        var hashed = TokenHasher.Hash(refreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(hashed);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return ServiceResult<AuthResponseDto>.FailureResult("Invalid or expired refresh token.");
        }

        var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user == null)
        {
            return ServiceResult<AuthResponseDto>.FailureResult("User not found.");
        }

        // Rotate: the presented refresh token is single-use.
        storedToken.IsRevoked = true;
        _refreshTokenRepository.Update(storedToken);

        var response = await GenerateAuthResponseAsync(user);
        return ServiceResult<AuthResponseDto>.SuccessResult(response);
    }

    public async Task LogoutAsync(Guid userId)
    {
        await _refreshTokenRepository.RevokeAllForUserAsync(userId);
        await _refreshTokenRepository.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenPlain = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = TokenHasher.Hash(refreshTokenPlain),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            AccessToken = accessToken,
            RefreshToken = refreshTokenPlain,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes)
        };
    }
}