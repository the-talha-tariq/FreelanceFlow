using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Backend.Data;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Repositories.Interfaces;

namespace FreelanceFlow.Backend.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token) =>
        await DbSet.FirstOrDefaultAsync(t => t.Token == token);

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        var tokens = await DbSet.Where(t => t.UserId == userId && !t.IsRevoked).ToListAsync();
        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }
        // Caller is responsible for calling SaveChangesAsync() so this can
        // be batched together with other changes in the same unit of work.
    }
}