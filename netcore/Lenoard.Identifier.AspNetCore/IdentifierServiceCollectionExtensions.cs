using System;
using Lenoard.Identifier;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentifierServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultIdentifier(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddIdentifier<DefaultIdentityGenerator>();
        }

        public static IServiceCollection AddSortableIdentifier(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddIdentifier<SortableIdentityGenerator>();
        }

        public static IServiceCollection AddIdentifier(this IServiceCollection services, IIdentityGenerator generator)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            return services.AddSingleton(generator);
        }

        public static IServiceCollection AddIdentifier<T>(this IServiceCollection services)
            where T : class, IIdentityGenerator
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddSingleton<IIdentityGenerator, T>();
        }

        public static IServiceCollection AddFlakeIdentifier(this IServiceCollection services, long identifier, long epoch)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddIdentifier(new FlakeIdentityGenerator(identifier, epoch));
        }

        public static IServiceCollection AddFlakeIdentifier(this IServiceCollection services, long identifier, DateTime epoch)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddIdentifier(new FlakeIdentityGenerator(identifier, epoch));
        }

        public static IServiceCollection AddFlakeIdentifier(this IServiceCollection services, long identifier)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddIdentifier(new FlakeIdentityGenerator(identifier));
        }

        public static IServiceCollection AddFlakeIdentifier(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return services.AddIdentifier(new FlakeIdentityGenerator());
        }
    }
}
