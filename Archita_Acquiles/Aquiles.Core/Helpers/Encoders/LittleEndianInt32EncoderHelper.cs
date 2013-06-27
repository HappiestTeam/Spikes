using System;
using System.Collections.Generic;

using System.Text;

namespace Aquiles.Helpers.Encoders
{
    /// <summary>
    /// Little Endian Encoder Helper for Int32
    /// </summary>
    public class LittleEndianInt32EncoderHelper : IByteEncoderHelper<Int32>
    {
        private bool isLittleEndian = BitConverter.IsLittleEndian;

        #region IByteEncoderHelper<long> Members

        /// <summary>
        /// Transform a value into a Byte Array
        /// </summary>
        /// <param name="value">value to be transformed</param>
        /// <returns>a byte[]</returns>
        public byte[] ToByteArray(Int32 value)
        {
            byte[] byteArray = BitConverter.GetBytes(value);
            if (!isLittleEndian)
            {
                Array.Reverse(byteArray);
            }
            return byteArray;
        }
        /// <summary>
        /// get an instance with the value from the byte[]
        /// </summary>
        /// <param name="value">the byte[] with data</param>
        /// <returns>a new object</returns>
        public Int32 FromByteArray(byte[] value)
        {
            byte[] copiedValue = null;
            if (!isLittleEndian)
            {
                copiedValue = new byte[value.Length];
                Array.Copy(value, copiedValue, value.Length);
                Array.Reverse(copiedValue);
            }
            else
            {
                copiedValue = value;
            }
            return BitConverter.ToInt32(copiedValue, 0);
        }

        #endregion
    }
}
