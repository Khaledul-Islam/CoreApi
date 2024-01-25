using Models.Enums;
using System.Security.Cryptography;

namespace Models.Crypto
{
    public class PasswordHasherOptions
    {
        private static readonly RandomNumberGenerator _defaultRng = RandomNumberGenerator.Create(); // secure PRNG

        public PasswordHasherCompatibilityMode CompatibilityMode { get; set; } = PasswordHasherCompatibilityMode.IdentityV3;

        public int IterationCount { get; set; } = 10000;

        // for unit testing
        public RandomNumberGenerator Rng { get; set; } = _defaultRng;
    }

}
