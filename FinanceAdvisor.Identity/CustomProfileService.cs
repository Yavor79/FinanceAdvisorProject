using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using FinanceAdvisor.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

public class CustomProfileService : IProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly FinanceDbContext _dbContext;
    private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;

    public CustomProfileService(UserManager<User> userManager, IUserClaimsPrincipalFactory<User> claimsFactory, FinanceDbContext dbContext)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
        _dbContext = dbContext;
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

        claims.Add(new Claim("user_id", user.Id.ToString()));
        if (roles.Contains("Advisor"))
        {
            var advisor = await _dbContext.Advisors.FirstOrDefaultAsync(ad => ad.UserId == user.Id);

            if (advisor != null) 
            { 
                claims.Add(new Claim("advisor_id", advisor.AdvisorId.ToString()));
                claims.Add(new Claim("advisor_specialization", advisor.Specialization.ToString()));
            }


        }

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }
}
