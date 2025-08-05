namespace FinanceAdvisor.Web.Middlewares
{
    //public class AdminRedirectMiddleware
    //{
    //    private readonly RequestDelegate _next;

    //    public AdminRedirectMiddleware(RequestDelegate next)
    //    {
    //        _next = next;
    //    }

    //    public async Task Invoke(HttpContext context)
    //    {
    //        Console.WriteLine("///////////////////**************1");
    //        // Check if request is for homepage
    //        if (context.Request.Path == "/")
    //        {
    //            if(context.User.Identity?.IsAuthenticated == true) { Console.WriteLine("///////////////////**************5"); }
    //            Console.WriteLine("///////////////////**************2");
    //            if (context.User.IsInRole("Admin"))
    //            {
    //                Console.WriteLine("///////////////////**************3");
    //                context.Response.Redirect("/Admin/Home/Index");
    //                return;
    //            }
    //        }

    //        await _next(context);
    //    }
    //}

    public class AdminRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("? Middleware hit. Authenticated: " + context.User.Identity?.IsAuthenticated);

            if (context.User.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine("/////// Checking user claims for role:");
                foreach (var claim in context.User.Claims)
                {
                    Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                }

                var isAdmin = context.User.Claims.Any(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" &&
                    c.Value == "Admin");

                if (isAdmin)
                {
                    Console.WriteLine("⭐ Admin detected");
                    var path = context.Request.Path.Value ?? "";

                    // Prevent infinite redirect loop
                    var isAlreadyInAdminArea = path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase);
                    if (!isAlreadyInAdminArea)
                    {
                        Console.WriteLine("⭐ redirecting to /Admin/Home/Index");
                        context.Response.Redirect("/Admin/Home/Index");
                        return;
                    }

                }

                
            }

            await _next(context);
        }
    }


}
