using System.Net.Http.Headers;
using System.Net;
using FinanceAdvisor.Web.Helpers;

public static class HttpClientExtensions
{
    
    public static async Task<HttpResponseMessage> GetWithRefreshAsync(this HttpClient client, string requestUri, ITokenRefreshService tokenService)
    {
        var response = await client.GetAsync(requestUri);
        Console.WriteLine("Got response: ***************************");
        Console.WriteLine(response.StatusCode);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Cookies still valid but acsess token is expired
            // => get new one via the refresh token
            // Attempt refresh via IdentityServer here (silent token renewal logic)
            
            var newToken = await tokenService.TryRefreshTokenAsync();

            if (newToken)
            {
                response = await client.GetAsync(requestUri);

            }
            // refresh token is expired too
            // => response == Unauthorized 401
            // => forcefully sign out + try to log in the User (reauthenticate)
            // + delete the local browser Cookies
        }

        return response;
    }

    public static async Task<HttpResponseMessage> PostAsJsonWithRefreshAsync<T>(this HttpClient client, string requestUri, T content, ITokenRefreshService tokenService)
    {
        var response = await client.PostAsJsonAsync(requestUri, content);

        if (response.StatusCode == HttpStatusCode.Unauthorized && await tokenService.TryRefreshTokenAsync())
        {
            response = await client.PostAsJsonAsync(requestUri, content);
        }

        return response;
    }

    public static async Task<HttpResponseMessage> PutAsJsonWithRefreshAsync<T>(this HttpClient client, string requestUri, T content, ITokenRefreshService tokenService)
    {
        var response = await client.PutAsJsonAsync(requestUri, content);

        if (response.StatusCode == HttpStatusCode.Unauthorized && await tokenService.TryRefreshTokenAsync())
        {
            response = await client.PutAsJsonAsync(requestUri, content);
        }

        return response;
    }

    public static async Task<HttpResponseMessage> DeleteWithRefreshAsync(this HttpClient client, string requestUri, ITokenRefreshService tokenService)
    {
        var response = await client.DeleteAsync(requestUri);

        if (response.StatusCode == HttpStatusCode.Unauthorized && await tokenService.TryRefreshTokenAsync())
        {
            response = await client.DeleteAsync(requestUri);
        }

        return response;
    }

    public static async Task<HttpResponseMessage> PatchWithRefreshAsync<T>(this HttpClient client, string requestUri, T content, ITokenRefreshService tokenService)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
        {
            Content = JsonContent.Create(content)
        };

        var response = await client.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized && await tokenService.TryRefreshTokenAsync())
        {
            response = await client.SendAsync(request);
        }

        return response;
    }

    public static async Task<HttpResponseMessage> PatchWithRefreshAsync(this HttpClient client, string requestUri, ITokenRefreshService tokenService)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, requestUri);
        var response = await client.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized && await tokenService.TryRefreshTokenAsync())
        {
            response = await client.SendAsync(request);
        }

        return response;
    }
}
