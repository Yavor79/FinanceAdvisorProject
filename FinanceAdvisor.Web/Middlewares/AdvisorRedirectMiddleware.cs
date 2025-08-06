namespace FinanceAdvisor.Web.Middlewares
{
    

    public class AdvisorRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public AdvisorRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("? Middleware hit. Authenticated: " + context.User.Identity?.IsAuthenticated);

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var isAdvisor = context.User.Claims.Any(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" &&
                    c.Value == "Advisor");

                if (isAdvisor)
                {
                    Console.WriteLine("⭐ Advisor detected");
                    var path = context.Request.Path.Value ?? "";

                    // Prevent infinite redirect loop
                    //var isAlreadyInAdvisorArea = path.StartsWith("/Advisor", StringComparison.OrdinalIgnoreCase);
                    //if (!isAlreadyInAdvisorArea)
                    //{
                    //    Console.WriteLine("⭐ redirecting to /Advisor/AdvisorHome/Index");
                    //    context.Response.Redirect("/Advisor/AdvisorHome/Index");
                    //    return;
                    //}
                    if (path == "/" || path == "/Home" || path == "/Home/Index")
                    {
                        context.Response.Redirect("/Advisor/AdvisorHome/Index", false);
                        return;
                    }


                }


            }

            await _next(context);
        }
    }


}
