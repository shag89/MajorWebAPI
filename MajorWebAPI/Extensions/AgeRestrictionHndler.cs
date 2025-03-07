using Microsoft.AspNetCore.Authorization;

namespace MajorWebAPI.Extensions
{
    public class AgeRestrictionHndler : AuthorizationHandler<AgeRestrictionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AgeRestrictionRequirement requirement)
        {
            var birthDateClaim = context.User.FindFirst(c => c.Type == "DateOfBirth");
            if (birthDateClaim == null)
                return Task.CompletedTask;

            var birthDate = DateTime.Parse(birthDateClaim.Value);
            var userAge = DateTime.Today.Year - birthDate.Year;

            if (userAge >= requirement.UserAge)
            {
                context.Succeed(requirement); // If requirement fulfil, marks the requirement as fulfilled.
            }

            return Task.CompletedTask;
        }
    }
}
