using System;
using System.Collections.Generic;
using System.Text;

namespace PCEFTPOS.EFTClient.IPInterface
{
    class Encoding
    {
        public static long BcdToBin(byte[] buf, int start, int length)
        {
            long number = 0;
            int multplier = 1;

            for (int i = start + length - 1; i >= start; i--)
            {
                number += (buf[i] & 0x0f) * multplier;
                multplier *= 10;
                number += ((buf[i] & 0xF0) >> 4) * multplier;
                multplier *= 10;
            }

            return number;
        }

        public static string BcdToAsc(byte[] buf, int start, int bcdDigits)
        {
            string asc = "";
            int length = bcdDigits / 2 + bcdDigits % 2;

            for (int i = start; i < length; i++)
            {
                // First digit.
                int bcd = ((buf[i] & 0xf0) >> 4);
                if (bcd == 0x0d) asc += "=";
                else if (bcd > 0x09) asc += "?";
                else asc += bcd.ToString();

                // Second digit.
                if (asc.Length < bcdDigits)
                {
                    bcd = (buf[i] & 0x0f);
                    if (bcd == 0x0d) asc += "=";
                    else if (bcd > 0x09) asc += "?";
                    else asc += (buf[i] & 0x0f).ToString();
                }
            }

            return asc;
        }

        public static void BinToBcd(long number, byte[] buffer, int offset, int bcd_digits)
        {
            int length = bcd_digits / 2 + bcd_digits % 2;
            int pos = offset + length - 1;
            int divisor = 1;

            for (int i = 0; i < bcd_digits - 1;)
            {
                buffer[pos] = (byte)((number / divisor) % 10);
                divisor *= 10;
                i++;
                buffer[pos--] += (byte)(((number / divisor) % 10) * 16);
                divisor *= 10;
                i++;
            }
        }

        public static ushort GenCRC(string buf, int len)
        {
            ushort i;
            ushort bufIndex;
            ushort data;
            ushort crc = 0xffff;

            if (len == 0)
                return (ushort)(~crc);

            bufIndex = 0;
            do
            {
                for (i = 0, data = (ushort)(0xff & buf[bufIndex++]); i < 8; i++, data >>= 1)
                {
                    if (((crc & 0x0001) ^ (data & 0x0001)) == 1)
                        crc = (ushort)((crc >> 1) ^ 0x8408);//POLY
                    else
                        crc >>= 1;
                }
            }
            while (--len != 0);

            crc = (ushort)~crc;
            data = crc;
            crc = (ushort)((crc << 8) | ((data >> 8) & 0xff));

            return (crc);
        }
        ///// <exclude/>
        public static byte[] BytesToBytes(byte[] bytes, int start, int length)
        {
            byte[] r = new byte[length];
            Buffer.BlockCopy(bytes, start, r, 0, length);
            return r;
        }

        /// <exclude/>
        public static string BytesToString(byte[] bytes)
        {
            return BytesToString(bytes, 0, bytes.Length);
        }

        /// <exclude/>
        public static string BytesToString(byte[] bytes, int start, int length)
        {
            string str = "";

            for (int i = start; i < start + length; i++)
                str += (char)bytes[i];

            return str;
        }

        /// <exclude/>
        public static byte[] StringToBytes(string data)
        {
            byte[] bytes = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
                bytes.SetValue((byte)data[i], i);

            return bytes;
        }
    }
}
