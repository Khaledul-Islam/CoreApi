using Contracts.ContextAccessor;
using Microsoft.AspNetCore.Http;
using Models.Dtos.User;
using Models.Enums;

namespace Utilities.ContextAccessor
{
    public class CurrentPrinciple(IHttpContextAccessor httpContextAccessor) : ICurrentPrinciple
    {
        public CurrentUser GetCurrentUser()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                throw new NullReferenceException("httpContextAccessor.HttpContext null");
            }

            var currentUser = new CurrentUser();

            currentUser.Id = int.Parse(user.FindFirst(nameof(CurrentUser.Id))!.Value);
            currentUser.Username = user.FindFirst(nameof(CurrentUser.Username))!.Value;
            currentUser.Firstname = user.FindFirst(nameof(CurrentUser.Firstname))!.Value;
            currentUser.Lastname = user.FindFirst(nameof(CurrentUser.Lastname))!.Value;
            currentUser.Email = user.FindFirst(nameof(CurrentUser.Email))!.Value;
            currentUser.Birthdate = DateTime.Parse(user.FindFirst(nameof(CurrentUser.Birthdate))!.Value);
            currentUser.PhoneNumber = user.FindFirst(nameof(CurrentUser.PhoneNumber))!.Value;
            currentUser.Gender = Enum.Parse<GenderType>(user.FindFirst(nameof(CurrentUser.Gender))!.Value);
            //Roles = user.FindFirst(nameof(CurrentUser.Roles))!.Value.Split(',').ToList(),

            return currentUser;
        }
    }
}
