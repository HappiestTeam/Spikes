using System;
using System.Collections.Generic;

using System.Text;

namespace Aquiles.Helpers.Encoders
{
    /// <summary>
    /// Encoder Helper for Long (when you don't care about Endianness)
    /// </summary>
    public class LongEncoderHelper : IByteEncoderHelper<long>
    {
        #region IByteEncoderHelper<long> Members

        /// <summary>
        /// Transform a value into a Byte Array
        /// </summary>
        /// <param name="value">value to be transformed</param>
        /// <returns>a byte[]</returns>
        public byte[] ToByteArray(long value)
        {
            return BitConverter.GetBytes(value);
        }
        /// <summary>
        /// get an instance with the value from the byte[]
        /// </summary>
        /// <param name="value">the byte[] with data</param>
        /// <returns>a new object</returns>
        public long FromByteArray(byte[] value)
        {
            return BitConverter.ToInt64(value, 0);
        }

        #endregion
    }
}
