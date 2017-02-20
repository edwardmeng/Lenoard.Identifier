using Lenoard.Identifier.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    public static class IdentityApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCorrelation(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationMiddleware>();
        }
    }
}
