using System.Globalization;
using AutoMapper;
using Config.Settings;
using Contracts.Tokens;
using Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos.AuthToken;
using Models.Dtos.User;
using Models.Entities.Identity;
using Models.Enums;
using System.Security.Claims;
using Contracts.Crypto;
using Utilities.Exceptions;
using Utilities.Response;

namespace CoreApi.Controllers
{
    public class AuthorizationController(IMapper mapper, IAuthTokenService tokenService,
            ApplicationSettings applicationSettings, IUserService userService,
            IPasswordHasherService<User> passwordHasher)
        : BaseController
    {
        [HttpPost("Register"), AllowAnonymous]
        public async Task<ApiResponse<UserSignInDto>> Register(UserCreateDto userCreateDto, CancellationToken cancellationToken)
        {
            if (await userService.AnyAsync(a=>a.UserName== userCreateDto.Username,cancellationToken))
            {
                throw new DuplicateException("This user already exists.");
            }
            
            var user = mapper.Map<User>(userCreateDto);
            //password
            var hashPassword = passwordHasher.HashPassword(user, userCreateDto.Password);
            user.Password = hashPassword;
            user.CreatedOn = DateTime.Now;
            // Create User
            var result = await userService.AddAsync(user, cancellationToken);
            await userService.SaveChangesAsync(cancellationToken);
            //if (result.Succeeded is false)
            //{
            //    throw new Exception(result.Errors.FirstOrDefault()?.Description ?? "Can not register this user.");
            //}

            // Add custom claims
            var claims = await CreateClaimsAsync(user, cancellationToken);

            // Create token
            var token = CreateToken(claims);

            var userDto = mapper.Map<UserDto>(user);
            var userSignInDto = new UserSignInDto
            {
                UserDto = userDto,
                Token = token
            };

            // Update User
            user.LastLoginDate = DateTime.UtcNow;
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpirationTime = token.RefreshTokenExpiresIn;
            await userService.Update(user);
            await userService.SaveChangesAsync(cancellationToken);

            return new ApiResponse<UserSignInDto>(true, ApiResultBodyCode.Success, userSignInDto);
        }
       
        [HttpPost("Login"), AllowAnonymous]
        public async Task<ApiResponse<UserSignInDto>> Login(TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            if (tokenRequest.GrantType.Equals("password", StringComparison.OrdinalIgnoreCase) is false)
            {
                throw new BadRequestException("Grant type is not valid.");
            }

            var user = await userService.GetUserByUserName(tokenRequest.Username,cancellationToken);
            if (user is null)
            {
                throw new NotFoundException("Username or Password is incorrect.");
            }

            var verifyHashedPassword = passwordHasher.VerifyHashedPassword(user, user.Password, tokenRequest.Password);

            if (verifyHashedPassword == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("User Verification Failed.Username or Password is incorrect.");
            }

            if (verifyHashedPassword == PasswordVerificationResult.SuccessRehashNeeded)
            {
                throw new BadRequestException();
            }

            // Add custom claims
            var claims = await CreateClaimsAsync(user, cancellationToken);

            // Create token
            var token = CreateToken(claims);

            var userDto = mapper.Map<UserDto>(user);
            var userSignInDto = new UserSignInDto
            {
                UserDto = userDto,
                Token = token
            };

            // Update User
            user.LastLoginDate = DateTime.UtcNow;
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpirationTime = token.RefreshTokenExpiresIn;
            await userService.Update(user);
            await userService.SaveChangesAsync(cancellationToken);
            return new ApiResponse<UserSignInDto>(true, ApiResultBodyCode.Success, userSignInDto);
        }

        [HttpPost("RefreshToken"), AllowAnonymous]
        public async Task<ApiResponse<Token>> RefreshToken(TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            if (!tokenRequest.GrantType.Equals("refresh_token", StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException("Invalid client request.");
            }

            if (tokenRequest.AccessToken is null)
            {
                throw new BadRequestException("Invalid client request (AccessToken can not be empty).");
            }

            var principal = tokenService.GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
            var username = principal.Identity?.Name; //this is mapped to the Name claim by default
            if (username is null)
            {
                throw new BadRequestException("Invalid client request.");
            }

            var user = await userService.GetUserByUserName(username,cancellationToken);
            if (user == null || user.RefreshToken != tokenRequest.RefreshToken)
            {
                throw new BadRequestException("Invalid client request.");
            }

            if (user.RefreshTokenExpirationTime <= DateTime.UtcNow)
            {
                throw new TokenExpiredException("Refresh token expired.");
            }

            // Create Token
            var token = CreateToken(principal.Claims);

            // Update User
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpirationTime = token.RefreshTokenExpiresIn;
            await userService.Update(user);
            await userService.SaveChangesAsync(cancellationToken);

            return new ApiResponse<Token>(true, ApiResultBodyCode.Success, token);
        }

        private Token CreateToken(IEnumerable<Claim> claims)
        {
            return new Token
            {
                AccessToken = tokenService.GenerateAccessToken(claims),
                RefreshToken = tokenService.GenerateRefreshToken(),
                RefreshTokenExpiresIn = DateTime.UtcNow.AddDays(applicationSettings.JwtSetting.RefreshTokenExpirationDays),
                TokenType = "Bearer"
            };
        }
        private async Task<IEnumerable<Claim>> CreateClaimsAsync(User user, CancellationToken cancellationToken)
        {
            var roles = new List<string?>();
            // Add custom claims
            var claims = new List<Claim>
            {
                new(nameof(CurrentUser.Id), user.Id.ToString()),
                new(nameof(CurrentUser.Username), user.UserName),
                new(nameof(CurrentUser.Firstname), user.Firstname),
                new(nameof(CurrentUser.Lastname), user.Lastname),
                new(nameof(CurrentUser.Email), user.Email),
                new(nameof(CurrentUser.Birthdate), user.Birthdate.ToString(CultureInfo.CurrentCulture)),
                new(nameof(CurrentUser.Gender), user.Gender.ToString()),
                new(nameof(CurrentUser.Roles), string.Join(",",roles)),
            };

            return claims;
        }


    }
}
