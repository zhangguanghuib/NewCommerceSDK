using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SAG.HardwareStation.PaymentTerminal.DBS
{
    public class ECREngine
    {
        public static DBS.DataContract.DBSResponse Process(DBS.DataContract.DBSRequest dbsRequest)
        {
            DBS.DataContract.DBSResponse dbsResponse = new DBS.DataContract.DBSResponse();

            bool canProcess = true;

            string ecrMsg = dbsRequest.TransactionType; // TransactionType

            switch(dbsRequest.TransactionType)
            {
                case "0": // EDC Sale
                case "2": // EDC Refund
                case "8": // EDC Auth
                case "L": // Instalment sale
                    ecrMsg += dbsRequest.ECRRefNumber.PadLeft(16, '0'); // ECR Ref number
                    ecrMsg += ConvertAmount(dbsRequest.Amount); // Amount in cents
                    ecrMsg += ConvertAmount(dbsRequest.Tips); // Tips in cents
                    //ecrMsg += ConvertAmount(dbsRequest.Amount);//Validation Amount in cents
                    //ecrMsg += ConvertAmount(dbsRequest.Tips);//Validation Tip in cents
                    break;

                case "3": // VOID
                    ecrMsg += dbsRequest.TraceNo; // Trace number to void
                    break;

                default:
                    canProcess = false;
                    dbsResponse.ErrorMessage = string.Format($"Unable procces this transaction type (Type = {dbsRequest.TransactionType})");
                    break;
            }

            if(canProcess)
            {
                if(Environment.Is64BitProcess)
                {
                    dbsResponse = DBSEngineX64.Run(dbsRequest.COMPort, ecrMsg);
                }
                else
                {
                    dbsResponse = DBSEngineX86.Run(dbsRequest.COMPort, ecrMsg);
                }
            }

            return dbsResponse;
        }

        public static DBS.DataContract.DBSResponse ConvertResponseData(string responseData)
        {
            DBS.DataContract.DBSResponse dBSResponse = new DataContract.DBSResponse();

            dBSResponse.TransactionType = TrySubString(responseData, 0, 1);
            dBSResponse.ECRRefNo = TrySubString(responseData, 1, 16);
            dBSResponse.AmountInCents = TrySubString(responseData, 17, 12);
            dBSResponse.TipsInCents = TrySubString(responseData, 29, 12);
            dBSResponse.ResponseCode = TrySubString(responseData, 41, 2);
            dBSResponse.ResponseText = TrySubString(responseData, 43, 20);
            dBSResponse.TransactionDateTime = TrySubString(responseData, 63, 12);
            dBSResponse.CardType = TrySubString(responseData, 75, 10);
            dBSResponse.CardNo = TrySubString(responseData, 85, 19);
            dBSResponse.ExpirationDate = TrySubString(responseData, 104, 4);
            dBSResponse.CardHolderName = TrySubString(responseData, 108, 23);
            dBSResponse.TerminalNo = TrySubString(responseData, 131, 8);
            dBSResponse.MerchantNo = TrySubString(responseData, 139, 15);
            dBSResponse.TraceNo = TrySubString(responseData, 154, 6);
            dBSResponse.BatchNo = TrySubString(responseData, 160, 6);
            dBSResponse.ApprovalCode = TrySubString(responseData, 166, 6);
            dBSResponse.RetrievalRefNo = TrySubString(responseData, 172, 12);
            dBSResponse.EntryMode = TrySubString(responseData, 184, 1);
            dBSResponse.EMVApplicationId = TrySubString(responseData, 185, 32);
            dBSResponse.EMVTRansactionCryptogram = TrySubString(responseData, 217, 16);
            dBSResponse.EMVApplicationName = TrySubString(responseData, 233, 16);
            dBSResponse.EMVNoSignatureRequired = TrySubString(responseData, 249, 1);
            dBSResponse.InstallmentPlan = TrySubString(responseData, 250, 3);
            dBSResponse.MonthlyAmountDue = TrySubString(responseData, 253, 14);
            dBSResponse.Reserved = TrySubString(responseData, 267, 7);
            dBSResponse.DCCAmountInCents = TrySubString(responseData, 274, 12);
            dBSResponse.DCCTipInCents = TrySubString(responseData, 286, 12);
            dBSResponse.ExchangeRate = TrySubString(responseData, 298, 8);
            dBSResponse.LocalCurrencyName = TrySubString(responseData, 306, 3);
            dBSResponse.ForeignCurrencyName = TrySubString(responseData, 309, 3);
            dBSResponse.DCCPrintText = TrySubString(responseData, 310, 1);
            dBSResponse.ExchangeRateFormat = TrySubString(responseData, 311, 1);
            dBSResponse.DCCMarkUpRateText = TrySubString(responseData, 312, 21);

            return dBSResponse;

        }

        private static string TrySubString(string data, int startIndex, int lentgh)
        {
            string ret;

            try
            {
                ret = data.Substring(startIndex, lentgh);
            }
            catch
            {
                ret = string.Empty;
            }

            return ret;
        }

        public static string ConvertAmount(decimal amountInput)
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            double amount = double.Parse(amountInput.ToString(), culture) * 100;

            string origAmt = amount.ToString();
            origAmt = origAmt.Replace(".", "");
            origAmt = origAmt.Replace(",", "");

            return origAmt.PadLeft(12, '0');
        }
    }
}
