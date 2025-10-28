using System.Security.Claims;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            throw new ApplicationException("Users id is unavailable");
    }
}
// using System.Security.Claims;
//
// namespace BloodDonation.Infrastructure.Authentication;
//
// internal static class ClaimsPrincipalExtensions
// {
//     public static Guid GetUserId(this ClaimsPrincipal? principal)
//     {
//         string? userId =
//             principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
//             principal?.FindFirstValue("sub") ??
//             principal?.FindFirstValue("uid");
//
//         return Guid.TryParse(userId, out Guid parsedUserId)
//             ? parsedUserId
//             : throw new ApplicationException("Users id is unavailable");
//     }
// }
