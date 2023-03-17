using System;
using System.Security.Claims;

namespace VacationAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// Retrieves the user ID claim from the ClaimsPrincipal.
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                return Guid.Empty;
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Guid.Empty;
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Guid.Empty;
            }

            return userId;
        }
    }
}