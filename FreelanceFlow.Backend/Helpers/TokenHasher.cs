using System.Security.Cryptography;
using System.Text;

namespace FreelanceFlow.Backend.Helpers;

public static class TokenHasher
{
    /// <summary>
    /// One-way hash for refresh tokens. Refresh tokens are bearer secrets
    /// like passwords — the raw value is only ever returned to the client
    /// once and never stored, only this hash is persisted.
    /// </summary>
    public static string Hash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}