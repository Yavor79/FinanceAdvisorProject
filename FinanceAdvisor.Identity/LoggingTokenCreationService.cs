using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

public class LoggingTokenCreationService : ITokenCreationService
{
    private readonly ITokenCreationService _inner;
    private readonly ILogger<LoggingTokenCreationService> _logger;

    public LoggingTokenCreationService(ILogger<LoggingTokenCreationService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        // Manually resolve the original DefaultTokenCreationService (bypass current registration)
        _inner = serviceProvider.GetRequiredService<DefaultTokenCreationService>();
    }

    public async Task<string> CreateTokenAsync(Token token)
    {
        var tokenValue = await _inner.CreateTokenAsync(token);
        //_logger.LogInformation("Access Token Created: {token}", tokenValue);
        //Console.WriteLine($"🚀 Access Token Created: {tokenValue}");
        return tokenValue;
    }

    
}
