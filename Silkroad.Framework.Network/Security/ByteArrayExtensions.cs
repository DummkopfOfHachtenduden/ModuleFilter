using System;
using System.Collections.Generic;
using System.Text;

namespace Silkroad.Framework.Common.Security
{
    public static class ByteArrayExtensions
    {
        public static string HexDump(this byte[] buffer)
        {
            return HexDump(buffer, 0, buffer.Length);
        }

        public static string HexDump(this byte[] buffer, int offset, int count)
        {
            const int bytesPerLine = 16;
            StringBuilder output = new StringBuilder();
            StringBuilder ascii_output = new StringBuilder();
            int length = count;
            if (length % bytesPerLine != 0)
            {
                length += bytesPerLine - length % bytesPerLine;
            }
            for (int x = 0; x <= length; ++x)
            {
                if (x % bytesPerLine == 0)
                {
                    if (x > 0)
                    {
                        output.AppendFormat("  {0}{1}", ascii_output.ToString(), Environment.NewLine);
                        ascii_output.Clear();
                    }
                    if (x != length)
                    {
                        output.AppendFormat("{0:d10}   ", x);
                    }
                }
                if (x < count)
                {
                    output.AppendFormat("{0:X2} ", buffer[offset + x]);
                    char ch = (char)buffer[offset + x];
                    if (!Char.IsControl(ch))
                    {
                        ascii_output.AppendFormat("{0}", ch);
                    }
                    else
                    {
                        ascii_output.Append(".");
                    }
                }
                else
                {
                    output.Append("   ");
                    ascii_output.Append(".");
                }
            }
            return output.ToString();
        }

        public static byte[] Replace(this byte[] input, byte[] pattern, byte[] replacement)
        {
            //TODO: CLEAN

            if (pattern.Length == 0)
            {
                return input;
            }

            List<byte> result = new List<byte>();

            int i;

            for (i = 0; i <= input.Length - pattern.Length; i++)
            {
                bool foundMatch = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (input[i + j] != pattern[j])
                    {
                        foundMatch = false;
                        break;
                    }
                }

                if (foundMatch)
                {
                    result.AddRange(replacement);
                    i += pattern.Length - 1;
                }
                else
                {
                    result.Add(input[i]);
                }
            }

            for (; i < input.Length; i++)
            {
                result.Add(input[i]);
            }

            return result.ToArray();
        }
    }
}