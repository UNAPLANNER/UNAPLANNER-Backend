using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

// This interface defines the contract for managing notification tokens in the repository, including adding new tokens, retrieving tokens by user ID and token value, getting active tokens for a user, and updating existing tokens.
public interface INotificationTokenRepository
{
    Task<NotificationToken> AddAsync(NotificationToken token);
    Task<NotificationToken?> GetByUserIdAndTokenAsync(int userId, string fcmToken);
    Task<List<NotificationToken>> GetActiveTokensByUserIdAsync(int userId);
    Task<bool> UpdateAsync(NotificationToken token);
    Task DeactivateOtherTokensAsync(int userId, string activeToken);
}
