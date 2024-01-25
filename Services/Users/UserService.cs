using Contracts.Users;
using Data.Context;
using Data.Providers;
using Models.Entities.Identity;

namespace Services.Users
{
    public class UserService(ApplicationDbContext context) : Repository<User, int>(context), IUserService
    {
        public async Task<User?> GetUserByUserName(string userName, CancellationToken cancellationToken)
        {
            return await FirstOrDefaultAsync(a => a.UserName == userName, cancellationToken);
        }
    }
}
