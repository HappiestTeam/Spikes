using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles
{
    internal class ByteEncoderHelper
    {
        private ByteEncoderHelper() { }

        private static UTF8Encoding byteEncoder = new UTF8Encoding();

        internal static Byte[] ToByteArray(string value)
        {
            return byteEncoder.GetBytes(value);
        }

        internal static string FromByteArray(Byte[] value)
        {
            return byteEncoder.GetString(value);
        }
    }
}
