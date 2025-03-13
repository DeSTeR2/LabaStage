using HospitalMVC;
using System.Security.Claims;

namespace Utils
{
    public static class CheckRole
    {
        public static bool IsInRoles(ClaimsPrincipal user, string[] roles)
        {
            bool isInRoles = false;
            foreach (var role in roles) {
                if (user.IsInRole(role))
                {
                    isInRoles = true;
                }
            }

            return isInRoles;
        }
    }
}