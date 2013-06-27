using System;
using System.Collections.Generic;

using System.Text;

namespace Aquiles.Helpers.Encoders
{
    /// <summary>
    /// Encoder Helper for Int32 (when you don't care about Endianness)
    /// </summary>
    public class Int32EncoderHelper : IByteEncoderHelper<Int32>
    {
        #region IByteEncoderHelper<Int32> Members

        /// <summary>
        /// Transform a value into a Byte Array
        /// </summary>
        /// <param name="value">value to be transformed</param>
        /// <returns>a byte[]</returns>
        public byte[] ToByteArray(Int32 value)
        {
            return BitConverter.GetBytes(value);
        }
        /// <summary>
        /// get an instance with the value from the byte[]
        /// </summary>
        /// <param name="value">the byte[] with data</param>
        /// <returns>a new object</returns>
        public Int32 FromByteArray(byte[] value)
        {
            return BitConverter.ToInt32(value, 0);
        }

        #endregion
    }
}
