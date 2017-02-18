using System;

namespace Lenoard.Identifier
{
    /// <summary>
    /// This interface represents ID generator.
    /// </summary>
    public interface IIdentityGenerator
    {
        /// <summary>
        /// Generates new identifier every time the method is called.
        /// </summary>
        /// <returns>The <see cref="Guid"/> represents uniquely identifier.</returns>
        Guid Generate();
    }
}
