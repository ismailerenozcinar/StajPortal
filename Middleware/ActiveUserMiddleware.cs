using Microsoft.AspNetCore.Identity;
using StajPortal.Models.Entities;

namespace StajPortal.Middleware
{
    /// <summary>
    /// Pasif kullanıcıları otomatik olarak çıkış yaptıran middleware
    /// </summary>
    public class ActiveUserMiddleware
    {
        private readonly RequestDelegate _next;

        public ActiveUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.GetUserAsync(context.User);
                
                // Kullanıcı pasifse çıkış yaptır
                if (user != null && !user.IsActive)
                {
                    await signInManager.SignOutAsync();
                    context.Response.Redirect("/Account/Login?deactivated=true");
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class ActiveUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseActiveUserCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ActiveUserMiddleware>();
        }
    }
}
