using Duende.IdentityServer.Models;
using Duende.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    new IdentityResource[]
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource("roles", "Your roles", new[] { "role" })
    };


    public static IEnumerable<ApiScope> ApiScopes =>
    new[]
    {
        new ApiScope("finance_api", "Finance API")
    };

    // Important for the audience in the acsess token
    public static IEnumerable<ApiResource> ApiResources =>
        new[]
        {
        new ApiResource("finance_api", "Finance API")
        {
            Scopes = { "finance_api" },
            UserClaims = { "role" }
        }
        };

   
    public static IEnumerable<Client> Clients =>
        new[]
        {
            new Client
            {
                ClientId = "mvc_client",
                ClientName = "MVC Web App",
                AllowedGrantTypes = GrantTypes.Hybrid,
                RequirePkce = false,
                ClientSecrets = { new Secret("mvc_secret".Sha256()) },
                RedirectUris = { "https://localhost:7053/signin-oidc" },
                //PostLogoutRedirectUris = { "https://localhost:7053/signout-callback-oidc" },
                PostLogoutRedirectUris = { "https://localhost:7053/" },

                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "finance_api",
                    "roles"
                },

                AllowOfflineAccess = true, // refresh token
                AlwaysIncludeUserClaimsInIdToken = true,
                AlwaysSendClientClaims = true
            }
        };
}
