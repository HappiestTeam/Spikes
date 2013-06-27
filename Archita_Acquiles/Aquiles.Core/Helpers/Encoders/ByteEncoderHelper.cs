using System;
using System.Collections.Generic;

using System.Text;

namespace Aquiles.Helpers.Encoders
{
    /// <summary>
    /// Encoder Helper 
    /// </summary>
    public static class ByteEncoderHelper
    {
        /// <summary>
        /// Little Endian Encoder Helper for Int32
        /// </summary>
        public static readonly IByteEncoderHelper<Int32> LittleEndianInt32Encoder;
        /// <summary>
        /// Big Endian Encoder Helper for Int32
        /// </summary>
        public static readonly IByteEncoderHelper<Int32> BigEndianInt32Encoder;
        /// <summary>
        /// Encoder Helper for Int32 (when you don't care about Endianness)
        /// </summary>
        public static readonly IByteEncoderHelper<Int32> Int32Encoder;
        /// <summary>
        /// Little Endian Encoder Helper for Long
        /// </summary>
        public static readonly IByteEncoderHelper<long> LittleEndianLongEncoder;
        /// <summary>
        /// Big Endian Encoder Helper for Long
        /// </summary>
        public static readonly IByteEncoderHelper<long> BigEndianLongEncoder;
        /// <summary>
        /// Encoder Helper for Long (when you don't care about Endianness)
        /// </summary>
        public static readonly IByteEncoderHelper<long> LongEncoder;
        /// <summary>
        /// Encoder Helper for ASCII
        /// </summary>
        public static readonly IByteEncoderHelper<string> ASCIIEncoder;
        /// <summary>
        /// Encoder Helper for UTF8
        /// </summary>
        public static readonly IByteEncoderHelper<string> UTF8Encoder;
        /// <summary>
        /// Encoder Helper for GUID
        /// </summary>
        public static readonly IByteEncoderHelper<Guid> GuidEncoder;

        /// <summary>
        /// Encoder Helper for UUID
        /// </summary>
        public static readonly IByteEncoderHelper<Guid> UUIDEnconder;

        static ByteEncoderHelper()
        {
            LongEncoder = new LongEncoderHelper();
            Int32Encoder = new Int32EncoderHelper();
            ASCIIEncoder = new ASCIIEncoderHelper();
            UTF8Encoder = new UTF8EncoderHelper();
            GuidEncoder = new GUIDEncoderHelper();
            UUIDEnconder = new UUIDEncoderHelper();
            BigEndianInt32Encoder = new BigEndianInt32EncoderHelper();
            LittleEndianInt32Encoder = new LittleEndianInt32EncoderHelper();
            BigEndianLongEncoder = new BigEndianLongEncoderHelper();
            LittleEndianLongEncoder = new LittleEndianLongEncoderHelper();
        }

    }
}

