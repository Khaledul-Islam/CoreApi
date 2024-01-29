using Data.Providers;
using Models.Entities.Identity;

namespace Contracts.Users
{
    public interface IUserService : IRepository<User, int>
    {
        Task<User?> GetUserByUserName(string userName, CancellationToken cancellationToken);
    }
}
