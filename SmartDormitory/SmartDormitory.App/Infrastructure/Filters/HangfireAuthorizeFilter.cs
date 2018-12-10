using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace SmartDormitory.App.Infrastructure.Filters
{
    public class HangfireAuthorizeFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
            {
                return httpContext.User.IsInRole("Administrator");
            }

            return false;
        }
    }
}
