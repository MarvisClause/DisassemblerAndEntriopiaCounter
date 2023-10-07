using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisEn
{
    public class SHA1
    {
        public static string ComputeSHA1(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);

            uint[] h = new uint[]
            {
        0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0xC3D2E1F0
            };

            byte[] paddedData = AddPadding(data);

            for (int offset = 0; offset < paddedData.Length; offset += 64)
            {
                uint[] w = new uint[80];

                for (int i = 0; i < 16; i++)
                {
                    w[i] = BitConverter.ToUInt32(paddedData, offset + i * 4);
                }

                for (int i = 16; i < 80; i++)
                {
                    w[i] = CircularLeftShift(w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16], 1);
                }

                uint a = h[0];
                uint b = h[1];
                uint c = h[2];
                uint d = h[3];
                uint e = h[4];

                for (int i = 0; i < 80; i++)
                {
                    uint f, k;

                    if (i < 20)
                    {
                        f = (b & c) | ((~b) & d);
                        k = 0x5A827999;
                    }
                    else if (i < 40)
                    {
                        f = b ^ c ^ d;
                        k = 0x6ED9EBA1;
                    }
                    else if (i < 60)
                    {
                        f = (b & c) | (b & d) | (c & d);
                        k = 0x8F1BBCDC;
                    }
                    else
                    {
                        f = b ^ c ^ d;
                        k = 0xCA62C1D6;
                    }

                    uint temp = CircularLeftShift(a, 5) + f + e + k + w[i];
                    e = d;
                    d = c;
                    c = CircularLeftShift(b, 30);
                    b = a;
                    a = temp;
                }

                h[0] += a;
                h[1] += b;
                h[2] += c;
                h[3] += d;
                h[4] += e;
            }

            byte[] hashBytes = new byte[20];
            for (int i = 0; i < 5; i++)
            {
                Array.Copy(BitConverter.GetBytes(h[i]), 0, hashBytes, i * 4, 4);
            }

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private static byte[] AddPadding(byte[] data)
        {
            int originalLength = data.Length;
            long bitLength = originalLength * 8;

            // Append a single "1" bit
            Array.Resize(ref data, originalLength + 1);
            data[originalLength] = 0x80;

            // Calculate the number of padding zeros needed
            int paddingZeros = 64 - (originalLength + 1) % 64;

            // Append padding zeros
            Array.Resize(ref data, originalLength + 1 + paddingZeros);

            // Append the original message length in bits
            for (int i = 0; i < 8; i++)
            {
                data[data.Length - 8 + i] = (byte)(bitLength >> (56 - 8 * i));
            }

            return data;
        }

        private static uint CircularLeftShift(uint value, int bits)
        {
            return (value << bits) | (value >> (32 - bits));
        }

    }
}
