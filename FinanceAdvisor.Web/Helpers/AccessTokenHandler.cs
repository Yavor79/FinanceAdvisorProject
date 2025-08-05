using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace FinanceAdvisor.Web.Helpers
{
    public class AccessTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccessTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await _httpContextAccessor.HttpContext?.GetTokenAsync("access_token");
           

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                Console.WriteLine($"** Access Token Sent: {accessToken}"); // Safe partial log
            }
            else
            {
                Console.WriteLine("** No access token found in HttpContext.");
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
