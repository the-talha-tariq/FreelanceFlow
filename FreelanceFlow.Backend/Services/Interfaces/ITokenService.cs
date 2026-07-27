using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Services.Interfaces;

public interface ITokenService
{
    /// <summary>Signed, short-lived JWT carrying the user's identity claims.</summary>
    string GenerateAccessToken(ApplicationUser user);

    /// <summary>Opaque, high-entropy random string. Caller is responsible for
    /// hashing it before persisting (see Helpers/TokenHasher).</summary>
    string GenerateRefreshToken();
}