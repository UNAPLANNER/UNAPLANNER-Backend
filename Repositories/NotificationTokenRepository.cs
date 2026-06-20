using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;
// This class implements the INotificationTokenRepository interface, providing methods to manage notification tokens in the database, including adding new tokens, retrieving tokens by user ID and token value, getting active tokens for a user, and updating existing tokens.
public class NotificationTokenRepository : INotificationTokenRepository
{
    private readonly AppDbContext _context;
// The constructor initializes the repository with the application's database context, allowing it to perform database operations related to notification tokens.
    public NotificationTokenRepository(AppDbContext context)
    {
        _context = context;
    }
// This method adds a new notification token to the database and returns the added token, including its generated ID.
    public async Task<NotificationToken> AddAsync(NotificationToken token)
    {
        _context.NotificationTokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }
// This method retrieves a notification token by user ID and token value, returning null if the token does not exist.
    public async Task<NotificationToken?> GetByUserIdAndTokenAsync(int userId, string fcmToken)
    {
        return await _context.NotificationTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.FcmToken == fcmToken);
    }
// This method retrieves a list of active notification tokens for a specific user, returning only tokens that are marked as active.
    public async Task<List<NotificationToken>> GetActiveTokensByUserIdAsync(int userId)
    {
        return await _context.NotificationTokens
            .Where(t => t.UserId == userId && t.IsActive)
            .ToListAsync();
    }
// This method updates an existing notification token in the database, setting its LastUpdated property to the current date and time, and returns true if the update was successful.
    public async Task<bool> UpdateAsync(NotificationToken token)
    {
        token.LastUpdated = DateTime.Now;
        _context.NotificationTokens.Update(token);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task DeactivateOtherTokensAsync(int userId, string activeToken)
    {
        var oldTokens = await _context.NotificationTokens
            .Where(t => t.UserId == userId && t.FcmToken != activeToken && t.IsActive)
            .ToListAsync();

        foreach (var t in oldTokens)
        {
            t.IsActive = false;
            t.LastUpdated = DateTime.Now;
        }

        if (oldTokens.Count > 0)
            await _context.SaveChangesAsync();
    }
}
