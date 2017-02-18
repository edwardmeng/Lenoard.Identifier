using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

namespace Lenoard.Identifier
{
    /// <summary>
    /// A decentralized, k-ordered id generator
    /// <list>
    /// <item><description>64-bit timestamp - milliseconds since the epoch (Jan 1 1970)</description></item>
    /// <item><description>48-bit worker id; it can be MAC address or other identifier</description></item>
    /// <item><description>16-bit sequence # - usually 0 incremented when more than one id is requested in the same millisecond and reset to 0 when the clock ticks forward</description></item>
    /// </list>
    /// </summary>
    public class FlakeIdentityGenerator : IIdentityGenerator
    {
        #region Fields

        /// <summary>
        /// The default epoch used by the id generator.  1970-01-01T00:00:00Z
        /// </summary>
        private static readonly DateTime DefaultEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// The last tick.
        /// </summary>
        private long _lastTicks;
        private readonly long _epoch;

        /// <summary>
        /// The sequence within the same tick.
        /// </summary>
        private int _sequence;

        /// <summary>
        /// Object used as a monitor for threads synchronization.
        /// </summary>
        private readonly object _monitor = new object();

        // store the individual bytes instead of an array
        // so we do not incur the overhead of array indexing
        // and bound checks when generating id values
        private byte _identifier0;
        private byte _identifier1;
        private byte _identifier2;
        private byte _identifier3;
        private byte _identifier4;
        private byte _identifier5;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlakeIdentityGenerator"/> class.
        /// </summary>
        public FlakeIdentityGenerator()
        {
            var maxIdentifier = -1 ^ (-1 << 48);
#if NetCore
            var task = Dns.GetHostAddressesAsync(Dns.GetHostName());
            task.Wait();
            var addresses = task.Result;
#else
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
#endif
            var identifier = (from address in addresses
                where !IPAddress.IsLoopback(address) && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                let bytes = address.GetAddressBytes()
                select BitConverter.ToInt32(address.GetAddressBytes(), 0) % (maxIdentifier + 1)).Max();
            Init(identifier);
            _epoch = DefaultEpoch.Ticks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlakeIdentityGenerator"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The instance identifier. Only the 6 low order bytes will be used.
        /// </param>
        public FlakeIdentityGenerator(long identifier)
            : this(identifier, DefaultEpoch.Ticks)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlakeIdentityGenerator"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The instance identifier. Only the 6 low order bytes will be used.
        /// </param>
        /// <param name="epoch">The epoch.</param>
        public FlakeIdentityGenerator(long identifier, DateTime epoch)
            : this(identifier, epoch.Ticks)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlakeIdentityGenerator"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="epoch">The epoch.</param>
        public FlakeIdentityGenerator(long identifier, long epoch)
        {
            Init(identifier);
            _epoch = epoch == 0 ? DefaultEpoch.Ticks : epoch;
        }

        #endregion

        #region Methods

        private void Init(long identifier)
        {
            _identifier0 = (byte)(identifier >> (8 * 0) & 0xff);
            _identifier1 = (byte)(identifier >> (8 * 1) & 0xff);
            _identifier2 = (byte)(identifier >> (8 * 2) & 0xff);
            _identifier3 = (byte)(identifier >> (8 * 3) & 0xff);
            _identifier4 = (byte)(identifier >> (8 * 4) & 0xff);
            _identifier5 = (byte)(identifier >> (8 * 5) & 0xff);
        }

        public Guid Generate()
        {
            lock (_monitor)
            {
                HandleTime();

                var lastTimestamp = _lastTicks - _epoch;

                return new Guid(
                    (int)(lastTimestamp >> 32 & 0xFFFFFFFF),
                    (short)(lastTimestamp >> 16 & 0xFFFF),
                    (short)(lastTimestamp & 0xFFFF),
                    _identifier5,
                    _identifier4,
                    _identifier3,
                    _identifier2,
                    _identifier1,
                    _identifier0,
                    (byte)(_sequence >> 8 & 0xff),
                    (byte)(_sequence >> 0 & 0xff));
            }
        }

        private void HandleTime()
        {
            var ticks = DateTime.UtcNow.Ticks;

            if (_lastTicks < ticks)
            {
                _lastTicks = ticks;
                _sequence = 0;
            }
            else if (_lastTicks == ticks)
            {
                // overflow 16-bits ? need to generate over 65,535,000,000 id/second?
                // release build on Intel Core i7 - 3740QM @ 2.7GHz max out at
                // about 17,000,000 id/sec
                _sequence++;
                Debug.Assert(_sequence <= 65535);
            }
            else if (ticks < _lastTicks)
            {
                SpinWait.SpinUntil(() => _lastTicks <= DateTime.UtcNow.Ticks);

                _lastTicks = DateTime.UtcNow.Ticks;
                _sequence = 0;
            }
        }

        #endregion
    }
}
