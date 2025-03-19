using HospitalMVC;
using System.Data;
using System.Security.Claims;

namespace Utils
{
    public static class CheckRole
    {
        public static bool IsInRoles(ClaimsPrincipal user, string[] roles)
        {
            if (user.Identity.IsAuthenticated == false) return false;

            bool isInRoles = false;
            foreach (var role in roles) {
                if (user.IsInRole(role))
                {
                    isInRoles = true;
                }
            }

            return isInRoles;
        }

        public static string GetUserRole(ClaimsPrincipal user)
        {
            string[] allRoles = new string[] { 
                Constants.Admin,
                Constants.Manager,
                Constants.User,
                Constants.Doctor
            };

            foreach (var role in allRoles)
            {
                if (user.IsInRole(role))
                {
                    return role;
                }
            }

            throw new Exception("User is not in any role!");
        }
    }
}