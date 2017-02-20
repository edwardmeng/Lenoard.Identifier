using System;
using System.Security.Cryptography;

namespace Lenoard.Identifier
{
    /// <summary>
    /// Generates Universally Unique Lexicographically Sortable Identifier.
    /// </summary>
    public class SortableIdentityGenerator : IIdentityGenerator
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        /// <summary>
        /// Generates new identifier every time the method is called.
        /// </summary>
        /// <returns>The <see cref="Guid"/> represents uniquely identifier.</returns>
        public Guid Generate()
        {
            var buffer = new byte[16];
            Array.Copy(BitConverter.GetBytes(DateTime.UtcNow.Ticks), 2, buffer, 0, 6);
            var random = new byte[10];
            _rng.GetBytes(random);
            Array.Copy(random, 0, buffer, 6, 10);
            return new Guid(buffer);
        }
    }
}
