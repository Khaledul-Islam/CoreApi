using Models.Enums;

namespace Contracts.Crypto
{
    public interface IPasswordHasherService<TUser> where TUser : class
    {
        string HashPassword(TUser user, string password);

        PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword);
    }

}
