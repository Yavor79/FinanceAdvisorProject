namespace FinanceAdvisor.Web.Helpers
{
    public interface ITokenRefreshService
    {
        Task<bool> TryRefreshTokenAsync();
        
    }

}
