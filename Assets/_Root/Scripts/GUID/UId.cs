using System;

namespace Snorlax.Common
{
    /// <summary>
    /// Id is 16 bytes, based on Guid generation, used to uniquely identify some object 
    /// </summary>
    public struct UId : IEquatable<UId>
    {
        public static readonly UId Empty = new UId();

        private readonly ulong _key1;
        private readonly ulong _key2;

        /// <summary>
        /// Generate new id , based on Guid
        /// </summary>
        public static UId NewId => new UId(Guid.NewGuid().ToByteArray());

        /// <summary>
        /// Parse string value. If value is not valid retuens UId.Empty
        /// </summary>
        public static UId Parse(string value)
        {
            if (value == null || value.Length != 22) return Empty;
            try
            {
                return new UId(value);
            }
            catch
            {
                return Empty;
            }
        }

        /// <summary>
        /// Try to parse string value. return success
        /// </summary>
        public static bool TryParse(string value, out UId id)
        {
            id = Empty;
            if (value == null || value.Length != 22) return false;
            try
            {
                id = new UId(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// is id is empty
        /// </summary>
        public bool IsEmpty => (_key1 | _key2) == 0;

        /// <summary>
        /// construct id from 22 symbols Base64-encoded string   
        /// </summary>
        public UId(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length != 22) throw new FormatException("Invalid UId: value should be 22 symbols long " + value);

            try
            {
                var bytes = Convert.FromBase64String(value + "==");
                _key1 = (ulong)ToInt64(bytes, 0);
                _key2 = (ulong)ToInt64(bytes, 8);
            }
            catch
            {
                throw new FormatException("Invalid UId " + value);
            }
        }

        /// <summary>
        /// construct id from 16 bytes   
        /// </summary>
        public UId(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length != 16) throw new FormatException("Invalid UId: value should be 16 bytes long");

            _key1 = (ulong)ToInt64(value, 0);
            _key2 = (ulong)ToInt64(value, 8);
        }

        /// <summary>
        /// construct id using byte array data at specified index  
        /// </summary>
        public UId(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            _key1 = (ulong)ToInt64(value, startIndex);
            _key2 = (ulong)ToInt64(value, startIndex + 8);
        }

        /// <summary>
        /// construct id using 2 ulong values  
        /// </summary>
        public UId(ulong key1, ulong key2)
        {
            this._key1 = key1;
            this._key2 = key2;
        }

        /// <summary>
        /// construct id using 2 long values  
        /// </summary>
        public UId(long key1, long key2)
        {
            this._key1 = (ulong)key1;
            this._key2 = (ulong)key2;
        }

        //from byte array to long 
        private static long ToInt64(byte[] value, int offset)
        {
            if (offset > value.Length - 8) throw new FormatException("start index more than value.Length -8" + offset + ">=" + value.Length);

            return (uint)(value[offset] | value[offset + 1] << 8 | value[offset + 2] << 16 | value[offset + 3] << 24) |
                   (long)(value[offset + 4] | value[offset + 5] << 8 | value[offset + 6] << 16 | value[offset + 7] << 24) << 32;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) { return obj is UId && Equals((UId)obj); }

        /// <inheritdoc />
        public bool Equals(UId other) { return _key1 == other._key1 && _key2 == other._key2; }

        /// <inheritdoc />
        public override int GetHashCode() { return (int)_key1 ^ (int)(_key1 >> 32) ^ (int)_key2 ^ (int)(_key2 >> 32); }

        public static bool operator ==(UId a, UId b) { return a._key1 == b._key1 && a._key2 == b._key2; }

        public static bool operator !=(UId a, UId b) { return !(a == b); }

        /// <summary>
        /// Convert id to 22 symbols Base64-encoded string  
        /// </summary>
        public override string ToString() { return Convert.ToBase64String(ToByteArray()).Substring(0, 22); }

        /// <summary>
        /// Serilize id value to byte array 
        /// </summary>
        public byte[] ToByteArray()
        {
            var result = new byte[16];
            ToBytes(result, 0, _key1);
            ToBytes(result, 8, _key2);
            return result;
        }

        /// <summary>
        /// Serilize id value to byte array at specified coordinates
        /// </summary>
        public void ToByteArray(byte[] result, int start)
        {
            ToBytes(result, start, _key1);
            ToBytes(result, start + 8, _key2);
        }

        /// <summary>
        /// Serialize id value to 2 ulong values 
        /// </summary>
        public void ToULongKeys(out ulong key1, out ulong key2)
        {
            key1 = this._key1;
            key2 = this._key2;
        }

        /// <summary>
        /// Serialize id value to 2 long values 
        /// </summary>
        public void ToLongKeys(out long key1, out long key2)
        {
            key1 = (long)this._key1;
            key2 = (long)this._key2;
        }

        // Serilize ulong value to byte array at specified coordinates
        private static void ToBytes(byte[] result, int offset, ulong data)
        {
            result[offset] = (byte)data;
            result[offset + 1] = (byte)(data >> 8);
            result[offset + 2] = (byte)(data >> 16);
            result[offset + 3] = (byte)(data >> 24);
            result[offset + 4] = (byte)(data >> 32);
            result[offset + 5] = (byte)(data >> 40);
            result[offset + 6] = (byte)(data >> 48);
            result[offset + 7] = (byte)(data >> 56);
        }
    }
}