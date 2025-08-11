using FinanceAdvisor.Web.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json;
using IdentityModel.Client;

namespace FinanceAdvisor.Web.Helpers
{
    public class TokenRefreshService : ITokenRefreshService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;

        public TokenRefreshService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
        }

        public async Task<bool> TryRefreshTokenAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            var refreshToken = await context.GetTokenAsync("refresh_token");
            var expiresAt = await context.GetTokenAsync("expires_at");

            if (string.IsNullOrEmpty(refreshToken) || !TokenExpired(expiresAt))
                return false;

            var disco = await _clientFactory.CreateClient().GetDiscoveryDocumentAsync("https://localhost:7150");

            var response = await _clientFactory.CreateClient().RequestRefreshTokenAsync(new IdentityModel.Client.RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "mvc_client",
                ClientSecret = "mvc_secret",
                RefreshToken = refreshToken
            });

            if (response.IsError) return false;

            var authInfo = await context.AuthenticateAsync("Cookies");

            authInfo.Properties.UpdateTokenValue("access_token", response.AccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", response.RefreshToken);
            authInfo.Properties.UpdateTokenValue("expires_at", DateTime.UtcNow.AddSeconds(response.ExpiresIn).ToString("o", CultureInfo.InvariantCulture));

            await context.SignInAsync("Cookies", authInfo.Principal, authInfo.Properties);
            return true;
        }

        private bool TokenExpired(string expiresAt)
        {
            var expiry = DateTime.Parse(expiresAt, null, DateTimeStyles.RoundtripKind);
            return expiry < DateTime.UtcNow;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            var accessToken = await context.GetTokenAsync("access_token");
            return accessToken;
        }
        
    }
}

