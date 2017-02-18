using System;

namespace Lenoard.Identifier
{
    /// <summary>
    /// Generates unique identifier with GUID.
    /// </summary>
    public class DefaultIdentityGenerator : IIdentityGenerator
    {
        /// <summary>
        /// Generate a new byte array that represents uniquely identifier.
        /// </summary>
        /// <returns>The byte array represents uniquely identifier.</returns>
        public Guid Generate()
        {
            return Guid.NewGuid();
        }
    }
}
