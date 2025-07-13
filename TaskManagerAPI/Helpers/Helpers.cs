using System.Security.Claims;

namespace TaskManagerAPI.Helpers {
    /// <summary>
    /// Helper class for common user-related utilities.
    /// </summary>
    public class Helpers {
        /// <summary>
        /// Checks whether the given user has an "admin" role based on Cognito groups.
        /// </summary>
        /// <param name="user">The authenticated user's claims principal.</param>
        /// <returns>True if the user belongs to the "admin" group; otherwise, false.</returns>
        /// <remarks>
        /// This method reads all claims with the key "cognito:groups" and checks
        /// if any of them contains the value "admin".
        /// </remarks>
        public static bool IsAdmin(ClaimsPrincipal user) {
            var roles = user.FindAll("cognito:groups").Select(r => r.Value).ToList();
            return roles.Contains("admin");
        }
    }
}
