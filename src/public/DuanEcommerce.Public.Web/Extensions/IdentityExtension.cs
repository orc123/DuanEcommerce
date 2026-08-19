using System;
using System.Linq;
using System.Security.Claims;

namespace DuanEcommerce.Public.Web.Extensions;

public static class IdentityExtension
{
    public static string GetSpecificClaim(this ClaimsPrincipal claimsPrincipal, string claimnType)
    {
        var claim = ((ClaimsIdentity)claimsPrincipal.Identity)?.Claims.FirstOrDefault(x => x.Type == claimnType);

        return claim !=  null ? claim.Value : claimnType;
    }

    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal) 
        => Guid.Parse(claimsPrincipal.GetSpecificClaim(ClaimTypes.NameIdentifier));
}
