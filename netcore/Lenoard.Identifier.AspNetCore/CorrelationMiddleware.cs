using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Lenoard.Identifier.AspNetCore
{
    internal class CorrelationMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Features.Set<ICorrelationFeature>(new CorrelationFeature
            {
                CorrelationIdentity = context.RequestServices.GetService<IIdentityGenerator>()?.Generate().ToString() ?? Guid.NewGuid().ToString()
            });
            await _next.Invoke(context);
        }
    }
}
