using System.Security.Claims;

namespace TaskManagerAPI.Helpers
{
    public class Helpers
    {
        public static bool IsAdmin(ClaimsPrincipal user)
        {

            var roles = user.FindAll("cognito:groups").Select(r => r.Value).ToList();
            return roles.Contains("admin");
        }
    }
}
