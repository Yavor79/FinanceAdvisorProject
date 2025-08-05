using Duende.IdentityServer.Configuration;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Validation;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class LoggingTokenService : DefaultTokenService
{
    public LoggingTokenService(
        IClaimsService claimsService,
        IReferenceTokenStore referenceTokenStore,
        ITokenCreationService tokenCreationService,
        IClock clock,
        IKeyMaterialService keyMaterialService,
        IdentityServerOptions options,
        ILogger<DefaultTokenService> logger)
        : base(claimsService, referenceTokenStore, tokenCreationService, clock, keyMaterialService, options, logger)
    {
    }

    public override async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
    {
        var token = await base.CreateAccessTokenAsync(request);

        // Log raw claims
        //Console.WriteLine("🎟️ Token Claims:");
        //foreach (var claim in token.Claims)
        //{
        //    Console.WriteLine($"- {claim.Type}: {claim.Value}");
        //}

        return token;
    }

    public override async Task<string> CreateSecurityTokenAsync(Token token)
    {
        var jwt = await base.CreateSecurityTokenAsync(token);

        //Console.WriteLine("🔐 JWT Access Token:");
        //Console.WriteLine(jwt);

        return jwt;
    }
}
