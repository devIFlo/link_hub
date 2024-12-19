using LinkHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinkHub.Filters
{
    public class SetUserFullNameFilter : IAsyncActionFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SetUserFullNameFilter(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = await _userManager.GetUserAsync(context.HttpContext.User);

            if (user != null)
            {
                context.HttpContext.Items["FullName"] = user.FullName;
            }

            await next();
        }
    }
}