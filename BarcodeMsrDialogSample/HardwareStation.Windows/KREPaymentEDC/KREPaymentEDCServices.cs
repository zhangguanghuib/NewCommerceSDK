using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KREPaymentEDC.HardwareStation
{
    public class KREPaymentEDCServices
    {
        internal string finalMessage;
        public string createSendDataCimb(string amount, bool qris)
        {
            string aValue;
            string aMsgValue;
            int aXor;
            string Stx;
            string Etx;
            string uniqId;

            // start message 
            aMsgValue = "";
            finalMessage = "";

            //this.createLogFile(plogName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"), "Session initilized");

            aValue = Convert.ToInt64(Math.Round(Convert.ToDouble(amount.Trim()))).ToString();

            /* Amount not round, but at EDC machine still automatic rounded
            aValue = Convert.ToDouble(amount.Trim()).ToString();
            */

            aValue = aValue.PadLeft(10, '0') + "00";

            uniqId = GenerateRandomNumber(6).ToString();

            aMsgValue = "A12" + uniqId + "B10" + aValue;
            string HexValue = "";

            HexValue = String2Hex(aMsgValue);

            //count LRC
            string aLRC = "";
            aXor = 0;

            aXor = CalculateLRC(HexValue);
            aLRC = Convert.ToInt32(aXor).ToString("X2");
            //end count

            //join message
            Stx = "";
            string[] stxNonQris = new string[] { "02", "31" };
            string[] stxQris = new string[] { "02", "3C" };
            string[] stx = qris ? stxQris : stxNonQris;
            for (int i = 0; i <= stx.Length - 1; i++)
            {
                Stx += HexAsciiConvert(stx[i]);
            }

            Etx = "";
            string[] etx = new string[] { "03", aLRC };
            for (int i = 0; i <= etx.Length - 1; i++)
            {
                Etx += HexAsciiConvert(etx[i]);
            }

            finalMessage = Stx + aMsgValue + Etx;

            return finalMessage;
        }

        private string String2Hex(string str)
        {
            string decString = str;
            byte[] bytes = System.Text.Encoding.Default.GetBytes(decString);
            string hexString = BitConverter.ToString(bytes);
            hexString = hexString.Replace("-", "");

            return hexString;

        }

        private string HexAsciiConvert(string hex)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i <= hex.Length - 2; i += 2)
            {
                sb.Append(Convert.ToChar(Int32.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber)));
            }

            return sb.ToString();
        }

        private int CalculateLRC(string ToCalculate)
        {
            int tmp = 0;
            //  Get sum of ToCalculate
            for (int i = 0; (i <= (ToCalculate.Length - 1)); i = (i + 2))
            {
                tmp ^= Convert.ToInt32(Convert.ToChar(Int32.Parse(ToCalculate.Substring(i, 2), System.Globalization.NumberStyles.HexNumber)));
            }

            return tmp;
        }

        public Int64 GenerateRandomNumber(int size)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            string s;
            for (int i = 0; i < size; i++)
            {
                s = Convert.ToString(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(s);
            }

            return Convert.ToInt64((builder.ToString()));
        }
    }
}
