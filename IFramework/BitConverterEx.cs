using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace IFramework
{
    /// <summary>
    /// byte[]转换
    /// </summary>
    public static class BitConverterEx
    {
        private static readonly uint[] ByteToHexCharLookupLowerCase = CreateByteToHexLookup(false);
        private static readonly uint[] ByteToHexCharLookupUpperCase = CreateByteToHexLookup(true);

        // 16x16 table, set up for direct visual correlation to Unicode table with hex coords
        private static readonly byte[] HexToByteLookup = new byte[] {
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
        };

        private static uint[] CreateByteToHexLookup(bool upperCase)
        {
            var result = new uint[256];

            if (upperCase)
            {
                for (int i = 0; i < 256; i++)
                {
                    string s = i.ToString("X2", CultureInfo.InvariantCulture);
                    result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
                }
            }
            else
            {
                for (int i = 0; i < 256; i++)
                {
                    string s = i.ToString("x2", CultureInfo.InvariantCulture);
                    result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
                }
            }

            return result;
        }

        /// <summary>
        /// string
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="lower"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] bytes, bool lower = true)
        {
            var lookup = lower ? ByteToHexCharLookupLowerCase : ByteToHexCharLookupUpperCase;
            var result = new char[bytes.Length * 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                int offset = i * 2;
                var val = lookup[bytes[i]];
                result[offset] = (char)val;
                result[offset + 1] = (char)(val >> 16);
            }

            return new string(result);
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hex)
        {
            int length = hex.Length;
            int rLength = length / 2;

            if (length % 2 != 0)
            {
                throw new ArgumentException("Hex string must have an even length.");
            }

            byte[] result = new byte[rLength];

            for (int i = 0; i < rLength; i++)
            {
                int offset = i * 2;

                byte b1;
                byte b2;

                try
                {
                    b1 = HexToByteLookup[hex[offset]];

                    if (b1 == 0xff)
                    {
                        throw new ArgumentException("Expected a hex character, got '" + hex[offset] + "' at string index '" + offset + "'.");
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    throw new ArgumentException("Expected a hex character, got '" + hex[offset] + "' at string index '" + offset + "'.");
                }

                try
                {
                    b2 = HexToByteLookup[hex[offset + 1]];

                    if (b2 == 0xff)
                    {
                        throw new ArgumentException("Expected a hex character, got '" + hex[offset + 1] + "' at string index '" + (offset + 1) + "'.");
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    throw new ArgumentException("Expected a hex character, got '" + hex[offset + 1] + "' at string index '" + (offset + 1) + "'.");
                }

                result[i] = (byte)(b1 << 4 | b2);
            }

            return result;
        }

        /// <summary>
        /// short
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static short ToInt16(byte[] buffer, int offset)
        {
            short value = default(short);

#pragma warning disable CS0675 // 对进行了带符号扩展的操作数使用了按位或运算符
            value |= buffer[offset + 1];
            value <<= 8;
            value |= buffer[offset];
#pragma warning restore CS0675 // 对进行了带符号扩展的操作数使用了按位或运算符

            return value;
        }
        /// <summary>
        /// int
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int ToInt32(byte[] buffer, int offset)
        {
            int value = default(int);

            value |= buffer[offset + 3];
            value <<= 8;
            value |= buffer[offset + 2];
            value <<= 8;
            value |= buffer[offset + 1];
            value <<= 8;
            value |= buffer[offset];

            return value;
        }
        /// <summary>
        /// long
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static long ToInt64(byte[] buffer, int offset)
        {
            long value = default(long);

            value |= buffer[offset + 7];
            value <<= 8;
            value |= buffer[offset + 6];
            value <<= 8;
            value |= buffer[offset + 5];
            value <<= 8;
            value |= buffer[offset + 4];
            value <<= 8;
            value |= buffer[offset + 3];
            value <<= 8;
            value |= buffer[offset + 2];
            value <<= 8;
            value |= buffer[offset + 1];
            value <<= 8;
            value |= buffer[offset];

            return value;
        }
        /// <summary>
        /// ushort
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ushort ToUInt16(byte[] buffer, int offset)
        {
            ushort value = default(ushort);

            value |= buffer[offset + 1];
            value <<= 8;
            value |= buffer[offset];

            return value;
        }
        /// <summary>
        /// uint
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static uint ToUInt32(byte[] buffer, int offset)
        {
            uint value = default(uint);

            value |= buffer[offset + 3];
            value <<= 8;
            value |= buffer[offset + 2];
            value <<= 8;
            value |= buffer[offset + 1];
            value <<= 8;
            value |= buffer[offset];

            return value;
        }
        /// <summary>
        /// ulong
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ulong ToUInt64(byte[] buffer, int offset)
        {
            ulong value = default(ulong);

            value |= buffer[offset + 7];
            value <<= 8;
            value |= buffer[offset + 6];
            value <<= 8;
            value |= buffer[offset + 5];
            value <<= 8;
            value |= buffer[offset + 4];
            value <<= 8;
            value |= buffer[offset + 3];
            value <<= 8;
            value |= buffer[offset + 2];
            value <<= 8;
            value |= buffer[offset + 1];
            value <<= 8;
            value |= buffer[offset];

            return value;
        }
        /// <summary>
        /// float
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static float ToSingle(byte[] buffer, int offset)
        {
            var union = default(SingleByteUnion);

            if (BitConverter.IsLittleEndian)
            {
                union.Byte0 = buffer[offset];
                union.Byte1 = buffer[offset + 1];
                union.Byte2 = buffer[offset + 2];
                union.Byte3 = buffer[offset + 3];
            }
            else
            {
                union.Byte3 = buffer[offset];
                union.Byte2 = buffer[offset + 1];
                union.Byte1 = buffer[offset + 2];
                union.Byte0 = buffer[offset + 3];
            }

            return union.Value;
        }
        /// <summary>
        /// double
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static double ToDouble(byte[] buffer, int offset)
        {
            var union = default(DoubleByteUnion);

            if (BitConverter.IsLittleEndian)
            {
                union.Byte0 = buffer[offset];
                union.Byte1 = buffer[offset + 1];
                union.Byte2 = buffer[offset + 2];
                union.Byte3 = buffer[offset + 3];
                union.Byte4 = buffer[offset + 4];
                union.Byte5 = buffer[offset + 5];
                union.Byte6 = buffer[offset + 6];
                union.Byte7 = buffer[offset + 7];
            }
            else
            {
                union.Byte7 = buffer[offset];
                union.Byte6 = buffer[offset + 1];
                union.Byte5 = buffer[offset + 2];
                union.Byte4 = buffer[offset + 3];
                union.Byte3 = buffer[offset + 4];
                union.Byte2 = buffer[offset + 5];
                union.Byte1 = buffer[offset + 6];
                union.Byte0 = buffer[offset + 7];
            }

            return union.Value;
        }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static decimal ToDecimal(byte[] buffer, int offset)
        {
            var union = default(DecimalByteUnion);

            if (BitConverter.IsLittleEndian)
            {
                union.Byte0 = buffer[offset];
                union.Byte1 = buffer[offset + 1];
                union.Byte2 = buffer[offset + 2];
                union.Byte3 = buffer[offset + 3];
                union.Byte4 = buffer[offset + 4];
                union.Byte5 = buffer[offset + 5];
                union.Byte6 = buffer[offset + 6];
                union.Byte7 = buffer[offset + 7];
                union.Byte8 = buffer[offset + 8];
                union.Byte9 = buffer[offset + 9];
                union.Byte10 = buffer[offset + 10];
                union.Byte11 = buffer[offset + 11];
                union.Byte12 = buffer[offset + 12];
                union.Byte13 = buffer[offset + 13];
                union.Byte14 = buffer[offset + 14];
                union.Byte15 = buffer[offset + 15];
            }
            else
            {
                union.Byte15 = buffer[offset];
                union.Byte14 = buffer[offset + 1];
                union.Byte13 = buffer[offset + 2];
                union.Byte12 = buffer[offset + 3];
                union.Byte11 = buffer[offset + 4];
                union.Byte10 = buffer[offset + 5];
                union.Byte9 = buffer[offset + 6];
                union.Byte8 = buffer[offset + 7];
                union.Byte7 = buffer[offset + 8];
                union.Byte6 = buffer[offset + 9];
                union.Byte5 = buffer[offset + 10];
                union.Byte4 = buffer[offset + 11];
                union.Byte3 = buffer[offset + 12];
                union.Byte2 = buffer[offset + 13];
                union.Byte1 = buffer[offset + 14];
                union.Byte0 = buffer[offset + 15];
            }

            return union.Value;
        }
        /// <summary>
        /// Guid
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Guid ToGuid(byte[] buffer, int offset)
        {
            var union = default(GuidByteUnion);
            // First 10 bytes of a guid are always little endian
            // Last 6 bytes depend on architecture endianness
            // See http://stackoverflow.com/questions/10190817/guid-byte-order-in-
            union.Byte0 = buffer[offset];
            union.Byte1 = buffer[offset + 1];
            union.Byte2 = buffer[offset + 2];
            union.Byte3 = buffer[offset + 3];
            union.Byte4 = buffer[offset + 4];
            union.Byte5 = buffer[offset + 5];
            union.Byte6 = buffer[offset + 6];
            union.Byte7 = buffer[offset + 7];
            union.Byte8 = buffer[offset + 8];
            union.Byte9 = buffer[offset + 9];

            if (BitConverter.IsLittleEndian)
            {
                union.Byte10 = buffer[offset + 10];
                union.Byte11 = buffer[offset + 11];
                union.Byte12 = buffer[offset + 12];
                union.Byte13 = buffer[offset + 13];
                union.Byte14 = buffer[offset + 14];
                union.Byte15 = buffer[offset + 15];
            }
            else
            {
                union.Byte15 = buffer[offset + 10];
                union.Byte14 = buffer[offset + 11];
                union.Byte13 = buffer[offset + 12];
                union.Byte12 = buffer[offset + 13];
                union.Byte11 = buffer[offset + 14];
                union.Byte10 = buffer[offset + 15];
            }

            return union.Value;
        }
        /// <summary>
        /// sbyte
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static sbyte ToSByte(byte[] bytes, int offset)
        {
            var b = bytes[offset];
            if (b > 127)
                return (sbyte)(b - 256);
            else
                return (sbyte)b;
        }
        /// <summary>
        /// bool
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static bool ToBoolean(byte[] buffer, int offset)
        {
            return buffer[offset] == 1;
        }
        /// <summary>
        /// char
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static char ToChar(byte[] buffer, int offset)
        {
            if (BitConverter.IsLittleEndian)
                return (char)(((buffer[offset] & 0xFF) << 8) | (buffer[offset+1] & 0xFF));
            return (char)(((buffer[offset+1] & 0xFF) << 8) | (buffer[offset ] & 0xFF));
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(sbyte value)
        {
            if (value <0)
                return new byte[] {
                    (byte)(value + 256),
                };
            else
                return new byte[] {
                    (byte)(value),
                };
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(char value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return new byte[] {
                    (byte)((value & 0xFF00) >> 8),
                    (byte)(value & 0xFF)
                };
            }
            else
            {
                return new byte[] {
                    (byte)(value & 0xFF),
                    (byte)((value & 0xFF00) >> 8),
                };
            }

        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(bool value)
        {
            return new byte[] {
                    value?(byte)1:(byte)0,
                };
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(short value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return new byte[] {
                    (byte)value,
                    (byte)(value >> 8)
                };
            }
            else
            {
                return new byte[] {
                    (byte)(value >> 8),
                    (byte)value,
                };
            }
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(int value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return new byte[] {
                    (byte)value,
                    (byte)(value >> 8),
                   (byte)(value >> 16),
                    (byte)(value >> 24),

            };

            }
            else
            {
                return new byte[] {

                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value,

            };

            }
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(long value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return new byte[] {

                     (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                (byte)(value >> 32),
                (byte)(value >> 40),
                (byte)(value >> 48),
                (byte)(value >> 56)
            };
            }
            else
            {
                return new byte[] {

                    (byte)(value >> 56),
                (byte)(value >> 48),
                (byte)(value >> 40),
                (byte)(value >> 32),
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value
                };
            }
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(ushort value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return new byte[] {
                     (byte)value,
                     (byte)(value >> 8)
                };

            }
            else
            {
                return new byte[] {
                     (byte)(value >> 8),
                     (byte)value,
                };

            }
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(uint value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return new byte[] {
                        (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24)
            };

            }
            else
            {
                return new byte[] {

                     (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)value
                };

            }
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(ulong value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return new byte[] {

                    (byte)value,
                    (byte)(value >> 8),
                    (byte)(value >> 16),
                    (byte)(value >> 24),
                    (byte)(value >> 32),
                    (byte)(value >> 40),
                    (byte)(value >> 48),
                    (byte)(value >> 56),
                };
            }
            else
            {
                return new byte[] {
                    (byte)(value >> 56),
                    (byte)(value >> 48),
                    (byte)(value >> 40),
                    (byte)(value >> 32),
                    (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)value
                };
            }
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(float value)
        {
            var union = default(SingleByteUnion);
            union.Value = value;

            if (BitConverter.IsLittleEndian)
            {
                return new byte[] {
                    union.Byte0,
                    union.Byte1,
                    union.Byte2,
                    union.Byte3,

                };
            }
            else
            {
                return new byte[] {
                union.Byte3,
                union.Byte2,
                union.Byte1,
                union.Byte0,
            };

            }
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(double value)
        {
            var union = default(DoubleByteUnion);
            union.Value = value;

            if (BitConverter.IsLittleEndian)
            {
                return new byte[] {
                    union.Byte0,
                    union.Byte1,
                    union.Byte2,
                    union.Byte3,
                    union.Byte4,
                    union.Byte5,
                    union.Byte6,
                    union.Byte7,
                };
            }
            else
            {
                return new byte[] {
                     union.Byte7,
                     union.Byte6,
                     union.Byte5,
                     union.Byte4,
                     union.Byte3,
                     union.Byte2,
                     union.Byte1,
                     union.Byte0,

                };
            }
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(decimal value)
        {
            var union = default(DecimalByteUnion);
            union.Value = value;

            if (BitConverter.IsLittleEndian)
            {
                return new byte[] {

                     union.Byte0,
                     union.Byte1,
                     union.Byte2,
                     union.Byte3,
                     union.Byte4,
                     union.Byte5,
                     union.Byte6,
                     union.Byte7,
                     union.Byte8,
                     union.Byte9,
                     union.Byte10,
                     union.Byte11,
                     union.Byte12,
                     union.Byte13,
                     union.Byte14,
                     union.Byte15,
                };
            }
            else
            {
                return new byte[] {

                    union.Byte15,
                    union.Byte14,
                    union.Byte13,
                    union.Byte12,
                    union.Byte11,
                    union.Byte10,
                    union.Byte9,
                    union.Byte8,
                    union.Byte7,
                    union.Byte6,
                    union.Byte5,
                    union.Byte4,
                    union.Byte3,
                    union.Byte2,
                    union.Byte1,
                    union.Byte0,

                };
            }
        }
        /// <summary>
        /// byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(Guid value)
        {
            var union = default(GuidByteUnion);
            union.Value = value;
            byte[] buffer = new byte[16];
            // First 10 bytes of a guid are always little endian
            // Last 6 bytes depend on architecture endianness
            // See http://stackoverflow.com/questions/10190817/guid-byte-order-in-net

            // TODO: Test if this actually works on big-endian architecture. Where the hell do we find that?

            buffer[0] = union.Byte0;
            buffer[1] = union.Byte1;
            buffer[2] = union.Byte2;
            buffer[3] = union.Byte3;
            buffer[4] = union.Byte4;
            buffer[5] = union.Byte5;
            buffer[6] = union.Byte6;
            buffer[7] = union.Byte7;
            buffer[8] = union.Byte8;
            buffer[9] = union.Byte9;

            if (BitConverter.IsLittleEndian)
            {
                buffer[10] = union.Byte10;
                buffer[11] = union.Byte11;
                buffer[12] = union.Byte12;
                buffer[13] = union.Byte13;
                buffer[14] = union.Byte14;
                buffer[15] = union.Byte15;
            }
            else
            {
                buffer[10] = union.Byte15;
                buffer[11] = union.Byte14;
                buffer[12] = union.Byte13;
                buffer[13] = union.Byte12;
                buffer[14] = union.Byte11;
                buffer[15] = union.Byte10;
            }
            return buffer;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct SingleByteUnion
        {
            [FieldOffset(0)]
            public byte Byte0;

            [FieldOffset(1)]
            public byte Byte1;

            [FieldOffset(2)]
            public byte Byte2;

            [FieldOffset(3)]
            public byte Byte3;

            [FieldOffset(0)]
            public float Value;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct DoubleByteUnion
        {
            [FieldOffset(0)]
            public byte Byte0;

            [FieldOffset(1)]
            public byte Byte1;

            [FieldOffset(2)]
            public byte Byte2;

            [FieldOffset(3)]
            public byte Byte3;

            [FieldOffset(4)]
            public byte Byte4;

            [FieldOffset(5)]
            public byte Byte5;

            [FieldOffset(6)]
            public byte Byte6;

            [FieldOffset(7)]
            public byte Byte7;

            [FieldOffset(0)]
            public double Value;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct DecimalByteUnion
        {
            [FieldOffset(0)]
            public byte Byte0;

            [FieldOffset(1)]
            public byte Byte1;

            [FieldOffset(2)]
            public byte Byte2;

            [FieldOffset(3)]
            public byte Byte3;

            [FieldOffset(4)]
            public byte Byte4;

            [FieldOffset(5)]
            public byte Byte5;

            [FieldOffset(6)]
            public byte Byte6;

            [FieldOffset(7)]
            public byte Byte7;

            [FieldOffset(8)]
            public byte Byte8;

            [FieldOffset(9)]
            public byte Byte9;

            [FieldOffset(10)]
            public byte Byte10;

            [FieldOffset(11)]
            public byte Byte11;

            [FieldOffset(12)]
            public byte Byte12;

            [FieldOffset(13)]
            public byte Byte13;

            [FieldOffset(14)]
            public byte Byte14;

            [FieldOffset(15)]
            public byte Byte15;

            [FieldOffset(0)]
            public decimal Value;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct GuidByteUnion
        {
            [FieldOffset(0)]
            public byte Byte0;

            [FieldOffset(1)]
            public byte Byte1;

            [FieldOffset(2)]
            public byte Byte2;

            [FieldOffset(3)]
            public byte Byte3;

            [FieldOffset(4)]
            public byte Byte4;

            [FieldOffset(5)]
            public byte Byte5;

            [FieldOffset(6)]
            public byte Byte6;

            [FieldOffset(7)]
            public byte Byte7;

            [FieldOffset(8)]
            public byte Byte8;

            [FieldOffset(9)]
            public byte Byte9;

            [FieldOffset(10)]
            public byte Byte10;

            [FieldOffset(11)]
            public byte Byte11;

            [FieldOffset(12)]
            public byte Byte12;

            [FieldOffset(13)]
            public byte Byte13;

            [FieldOffset(14)]
            public byte Byte14;

            [FieldOffset(15)]
            public byte Byte15;

            [FieldOffset(0)]
            public Guid Value;
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
