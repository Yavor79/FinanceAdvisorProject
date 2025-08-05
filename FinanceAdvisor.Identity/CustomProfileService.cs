using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;

public class CustomProfileService : IProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;

    public CustomProfileService(UserManager<User> userManager, IUserClaimsPrincipalFactory<User> claimsFactory)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        var principal = await _claimsFactory.CreateAsync(user);

        var claims = principal.Claims.ToList();

        // Add role claims explicitly
        var roles = await _userManager.GetRolesAsync(user);
        Console.WriteLine("Roles");
        foreach (var role in roles)
        {
            Console.WriteLine(role);
        }
        //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(roles.Select(role => new Claim("role", role))); 

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }
}
