using Contracts.Users;
using Data.Context;
using Data.Providers;
using Models.Entities.Identity;

namespace Services.Users
{
    public class UserService:Repository<User,int>,IUserService
    {
        public UserService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByUserName(string userName, CancellationToken cancellationToken)
        {
            return await FirstOrDefaultAsync(a => a.UserName == userName, cancellationToken);
        }
    }
}
