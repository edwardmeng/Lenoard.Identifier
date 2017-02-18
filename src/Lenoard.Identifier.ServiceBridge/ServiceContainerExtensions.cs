using System;
using ServiceBridge;

namespace Lenoard.Identifier.ServiceBridge
{
    public static class ServiceContainerExtensions
    {
        public static IServiceContainer UseDefaultIdentifier(this IServiceContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.UseIdentifier<DefaultIdentityGenerator>();
        }

        public static IServiceContainer UseIdentifier(this IServiceContainer container, IIdentityGenerator generator)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            return container.RegisterInstance(generator);
        }

        public static IServiceContainer UseIdentifier<T>(this IServiceContainer container)
            where T : IIdentityGenerator
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.Register<IIdentityGenerator, T>();
        }

        public static IServiceContainer UseFlakeIdentifier(this IServiceContainer container, long identifier, long epoch)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.UseIdentifier(new FlakeIdentityGenerator(identifier, epoch));
        }

        public static IServiceContainer UseFlakeIdentifier(this IServiceContainer container, long identifier, DateTime epoch)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.UseIdentifier(new FlakeIdentityGenerator(identifier, epoch));
        }

        public static IServiceContainer UseFlakeIdentifier(this IServiceContainer container, long identifier)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.UseIdentifier(new FlakeIdentityGenerator(identifier));
        }

        public static IServiceContainer UseFlakeIdentifier(this IServiceContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.UseIdentifier(new FlakeIdentityGenerator());
        }
    }
}
