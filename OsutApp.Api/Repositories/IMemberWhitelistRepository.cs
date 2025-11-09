using OsutApp.Api.Models;

namespace OsutApp.Api.Repositories;

public interface IMemberWhitelistRepository
{
    Task<MemberWhitelist?> GetActiveByEmailAsync(string email);
    Task<IEnumerable<MemberWhitelist>> GetAllAsync();
    Task AddAsync(MemberWhitelist whitelist);
    Task UpdateAsync(MemberWhitelist whitelist);
    Task DeleteAsync(MemberWhitelist whitelist);
}