// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace FinanceAdvisor.Identity.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IIdentityServerInteractionService _interaction;

        public LogoutModel(SignInManager<User> signInManager, ILogger<LogoutModel> logger, IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
            _signInManager = signInManager;
            _logger = logger;
        }


        //public async Task<IActionResult> OnGet(string logoutId)
        //{
        //    var logout = await _interaction.GetLogoutContextAsync(logoutId);

        //    await HttpContext.SignOutAsync(); // logs out all schemes
        //    _logger.LogInformation("User logged out.");

        //    //return Redirect(logout?.PostLogoutRedirectUri ?? "/");
        //    return Redirect("https://localhost:7053/");
        //}

        public async Task<IActionResult> OnGet(string returnUrl = null)
        {

            // Sign out of ASP.NET Core Identity
            await _signInManager.SignOutAsync();

            // Sign out of IdentityServer (clears idsrv, idsrv.session)
            await HttpContext.SignOutAsync("Identity.Application"); // Same as SignInManager
            
            await HttpContext.SignOutAsync("idsrv");
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            _logger.LogInformation("User logged out.");
            return Redirect("https://localhost:7053/");
            //if (returnUrl != null)
            //{
            //    return Redirect(returnUrl);
            //}
            //else
            //{
            //    // This needs to be a redirect so that the browser performs a new
            //    // request and the identity for the user gets updated.
            //    return RedirectToPage();
            //}
        }
    }
}
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Duende.IdentityServer.Services;
//using Duende.IdentityServer.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authentication;

//public class LogoutModel : PageModel
//{
//    private readonly IIdentityServerInteractionService _interaction;
//    private readonly ILogger<LogoutModel> _logger;

//    public LogoutModel(IIdentityServerInteractionService interaction, ILogger<LogoutModel> logger)
//    {
//        _interaction = interaction;
//        _logger = logger;
//    }

//    public async Task<IActionResult> OnGet(string logoutId)
//    {
//        // Get the logout context
//        var logout = await _interaction.GetLogoutContextAsync(logoutId);


//        // Perform the logout immediately and redirect without confirmation
//        await HttpContext.SignOutAsync();
//        _logger.LogInformation("User logged out.");
//        return Redirect(logout?.PostLogoutRedirectUri ?? "/");
//    }
//}
