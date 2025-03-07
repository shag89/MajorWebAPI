using Microsoft.AspNetCore.Authorization;

namespace MajorWebAPI.Extensions
{
    // this IAuthorizationRequirement interface just use to Represents an authorization requirement.
    public class AgeRestrictionRequirement : IAuthorizationRequirement
    {
        public int UserAge { get; set; }
        public AgeRestrictionRequirement(int Age) => UserAge = Age;
    }
}
