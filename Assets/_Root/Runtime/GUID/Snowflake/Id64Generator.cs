using System;
using System.Collections.Generic;
using System.Threading;

namespace Pancake.Common
{
    /// <summary>
    /// Generated id is composed of
    /// <list type="bullet">
    /// <item><description>time - 41 bits (millisecond precision w/ a custom epoch gives us 69 years)</description></item>
    /// <item><description>configured machine id - 10 bits (5 bit worker id, 5 bits datacenter id) - gives us up to 1024 machines</description></item>
    /// <item><description>sequence number - 12 bits - rolls over every 4096 per machine (with protection to avoid rollover in the same ms)</description></item>
    /// </list>
    /// </summary>
    public class Id64Generator : IIdGenerator<long>
    {
        #region Private Constant

        /// <summary>
        /// 1 January 1970. Used to calculate timestamp (in milliseconds)
        /// </summary>
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private const long EPOCH = 1351606710465L;

        /// <summary>
        /// Number of bits allocated for a worker id in the generated identifier. 5 bits indicates values from 0 to 31
        /// </summary>
        private const int WORKER_ID_BITS = 5;

        /// <summary>
        /// Datacenter identifier this worker belongs to. 5 bits indicates values from 0 to 31
        /// </summary>
        private const int DATACENTER_ID_BITS = 5;

        /// <summary>
        /// Generator identifier. 10 bits indicates values from 0 to 1023
        /// </summary>
        private const int GENERATOR_ID_BITS = 10;

        /// <summary>
        /// Maximum generator identifier
        /// </summary>
        private const long MAX_GENERATOR_ID = -1 ^ (-1L << GENERATOR_ID_BITS);

        /// <summary>
        /// Maximum worker identifier
        /// </summary>
        private const long MAX_WORKER_ID = -1L ^ (-1L << WORKER_ID_BITS);

        /// <summary>
        /// Maximum datacenter identifier
        /// </summary>
        private const long MAX_DATACENTER_ID = -1L ^ (-1L << DATACENTER_ID_BITS);

        /// <summary>
        /// Number of bits allocated for sequence in the generated identifier
        /// </summary>
        private const int SEQUENCE_BITS = 12;

        private const int WORKER_ID_SHIFT = SEQUENCE_BITS;

        private const int DATACENTER_ID_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS;

        private const int TIMESTAMP_LEFT_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS + DATACENTER_ID_BITS;

        private const long SEQUENCE_MASK = -1L ^ (-1L << SEQUENCE_BITS);

        #endregion Private Constant

        #region Private Fields

        /// <summary>
        /// Object used as a monitor for threads synchronization.
        /// </summary>
        private readonly object _monitor = new object();

        /// <summary>
        /// The timestamp used to generate last id by the worker
        /// </summary>
        private long _lastTimestamp = -1L;

        private long _sequence = 0;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Indicates how many times the given generator had to wait 
        /// for next millisecond <see cref="TillNextMillis"/> since startup.
        /// </summary>
        public int NextMillisecondWait { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public Id64Generator(int generatorId = 0, int sequence = 0)
            : this(
                (int) (generatorId & MAX_WORKER_ID),
                (int) (generatorId >> WORKER_ID_BITS & MAX_DATACENTER_ID),
                sequence)
        {
            // sanity check for generatorId
            if (generatorId > MAX_GENERATOR_ID || generatorId < 0)
            {
                throw new InvalidOperationException(
                    string.Format("Generator Id can't be greater than {0} or less than 0", MAX_GENERATOR_ID));
            }
        }

        public Id64Generator(int workerId, int datacenterId, int sequence = 0)
        {
            // sanity check for workerId
            if (workerId > MAX_WORKER_ID || workerId < 0)
            {
                throw new InvalidOperationException(
                    string.Format("Worker Id can't be greater than {0} or less than 0", MAX_WORKER_ID));
            }

            // sanity check for datacenterId
            if (datacenterId > MAX_DATACENTER_ID || datacenterId < 0)
            {
                throw new InvalidOperationException(
                    string.Format("Datacenter Id can't be greater than {0} or less than 0", MAX_DATACENTER_ID));
            }

            WorkerId = workerId;
            DatacenterId = datacenterId;
            this._sequence = sequence;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The identifier of the worker
        /// </summary>
        public long WorkerId { get; private set; }

        /// <summary>
        /// Identifier of datacenter the worker belongs to
        /// </summary>
        public long DatacenterId { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public long GenerateId()
        {
            lock (_monitor)
            {
                return NextId();
            }
        }

        public IEnumerator<long> GetEnumerator()
        {
            while (true)
            {
                yield return GenerateId();
            }

            // ReSharper disable once IteratorNeverReturns
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Public Methods

        #region Private Properties

        private static long CurrentTime
        {
            get { return (long) (DateTime.UtcNow - Jan1St1970).TotalMilliseconds; }
        }

        #endregion Private Properties

        #region Public Methods

        #endregion Public Methods

        #region Private Static Methods

        #endregion Private Static Methods

        #region Private Methods

        private long TillNextMillis(long lastTimestamp)
        {
            NextMillisecondWait++;

            var timestamp = CurrentTime;

            SpinWait.SpinUntil(() => (timestamp = CurrentTime) > lastTimestamp);

            return timestamp;
        }

        private long NextId()
        {
            var timestamp = CurrentTime;

            if (timestamp < _lastTimestamp)
            {
                throw new InvalidOperationException(string.Format("Clock moved backwards. Refusing to generate id for {0} milliseconds", (_lastTimestamp - timestamp)));
            }

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SEQUENCE_MASK;
                if (_sequence == 0)
                {
                    timestamp = TillNextMillis(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;
            return ((timestamp - EPOCH) << TIMESTAMP_LEFT_SHIFT) |
                   (DatacenterId << DATACENTER_ID_SHIFT) |
                   (WorkerId << WORKER_ID_SHIFT) |
                   _sequence;
        }

        #endregion Private Methods
    }
}