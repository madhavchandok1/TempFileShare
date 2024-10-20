using System.Security.Claims;

namespace TempFileShare.Utilities.Token
{
    public static class AutoMapper
    {
        public static string? GetUsername(this ClaimsPrincipal user)
        {
            Claim? claims = user.Claims.SingleOrDefault(x =>
            {
                return x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            });

            return claims?.Value;
        }
    }
}
