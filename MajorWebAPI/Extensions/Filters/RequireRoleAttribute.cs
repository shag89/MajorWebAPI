using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MajorWebAPI.Extensions.Filters
{
    public class RequireRoleAttribute :ActionFilterAttribute
    {
        private readonly string _roleName;
        public RequireRoleAttribute(string roleName) 
        {
            _roleName = roleName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            var customClaim = user.FindFirst(c => c.Type == "DateOfBirth")?.Value;

            if (!user.Identity.IsAuthenticated || !user.IsInRole(_roleName))
            {
                // Return 403 Forbidden if the check fails
                context.Result = new ForbidResult();
            }
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //base.OnActionExecuted(context);
        }
    }
}
