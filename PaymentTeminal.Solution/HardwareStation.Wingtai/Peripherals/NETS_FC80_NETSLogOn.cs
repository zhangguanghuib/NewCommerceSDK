using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTR.HWExt.Peripherals
{
    public class NETS_FC80_NETSLogOn : NETS
    {
        protected override StringBuilder constructAmountRequest(string _amount)
        {
            try
            {
                StringBuilder amountFormat = new StringBuilder("000000000000");
                _amount = _amount.Replace(".", "");
                _amount = _amount.Replace(",", "");
                StringBuilder AmountSB = new StringBuilder(_amount);

                int amountcount = amountFormat.Length - _amount.Length;
                for (int x = 0; x < _amount.Length; x++)
                {
                    amountFormat[amountcount] = AmountSB[x];
                    amountcount++;
                }

                return amountFormat;
            }
            catch (Exception ex)
            {
                logHelper.saveLog("Error in construct amount format" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error stack : " + ex.StackTrace);
                throw new System.Exception("Error in construct amount format, please check log for error details");
            }
        }

        //construct ECR format
        protected override StringBuilder constructECRRequest(string _amount, string _ECR)
        {
            try
            {
                StringBuilder ECRFormat = new StringBuilder("0000000000000");
                _ECR = _amount.Replace(".", "");
                _ECR = _amount.Replace(",", "");
                StringBuilder ECRSB = new StringBuilder(_ECR);

                int j = ECRFormat.Length - _ECR.Length;
                for (int x = 0; x < _ECR.Length; x++)
                {
                    ECRFormat[j] = ECRSB[x];
                    j++;
                }
                return ECRFormat;
            }
            catch (Exception ex)
            {
                logHelper.saveLog("Error in construct ECR format" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error stack : " + ex.StackTrace);
                throw new System.Exception("Error in construct amount format, please check log for error details");
            }
        }

        //construct request message to terminal
        protected override byte[] constructRequestMessageToTerminal(string _amount, string _ECR)
        {
            try
            {
                logHelper.saveLog("Start construct request message to terminal with amount : " + _amount + " and ECR : " + _ECR);

                byte[] MessageHeader = GetMessageHeader("80");                
                
                List<byte> lst = MessageHeader.OfType<byte>().ToList();//To compute LENGTH
                lst.Add(0x1C);

                byte[] message = lst.ToArray();
                byte[] messageLength = IntToBCD(message.Length);

                List<byte> messageToLRC = messageLength.OfType<byte>().ToList();
                messageToLRC.AddRange(message);
                messageToLRC.Add(0x03);


                byte[] messageToLRCArray = messageToLRC.ToArray();
                var testLRC = BitConverter.ToString(messageToLRCArray);
                byte LRC = calculateLRCRequest(messageToLRCArray);

                messageToLRC.Add(LRC);

                List<byte> FullCommand = new List<byte>();
                FullCommand.Add(0x02);//STX
                FullCommand.AddRange(messageToLRC);

                byte[] messageToLRCArrayFull = FullCommand.ToArray();
                var test = BitConverter.ToString(messageToLRCArrayFull);

                return messageToLRCArrayFull;
            }
            catch (Exception ex)
            {
                logHelper.saveLog("Error in construct constructRequestMessageToTerminal" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error stack : " + ex.StackTrace);
                throw new System.Exception("Error in construct constructRequestMessageToTerminal, please check log for error details");
            }
        }
    }
}
