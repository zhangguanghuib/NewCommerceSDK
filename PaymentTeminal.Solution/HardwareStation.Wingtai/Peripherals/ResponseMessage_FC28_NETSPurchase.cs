using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTR.HWExt.Peripherals
{
    class ResponseMessage_FC28_NETSPurchase : ResponseMessage
    {
        public ResponseMessage_FC28_NETSPurchase(NETS _NETSPayment)
            : base(_NETSPayment)
        {
            netsPayment = _NETSPayment;
        }

        protected override ResponseModel SetResponseTransactionFields(string[] ResponseString)
        {
            int counter = 0;
            ResponseModel response = new ResponseModel();

            try
            {
                foreach (string Field in ResponseString)
                {
                    //get the approval code at beginning
                    counter++;
                    if (counter == 1)
                    {
                        response.ResponseCode = Field.Substring(Field.Length - 3, 2);
                    }

                    string sub;
                    if (Field.Length >= 2)
                    {
                        sub = Field.Substring(0, 2);
                    }
                    else
                    {
                        sub = Field;
                    }

                    switch (sub)
                    {
                        //Generic Tranasaction Response Data
                        case "02": (response.ResponseText = Field.Substring(3)).Trim();//Response text;
                            break;
                        case "D0": (response.MerchantNameAddress = Field.Substring(3)).Trim();//Merchant name and address;
                            break;
                        case "03": (response.TransactionDate = Field.Substring(3)).Trim();//Transaction Date
                            break;
                        case "04": (response.TransactionTime = Field.Substring(3)).Trim();//Transaction Time
                            break;
                        case "01": (response.ApprovalCode = Field.Substring(3)).Trim();//Approval code
                            break;
                        case "41": (response.CashbackServiceFee = Field.Substring(3)).Trim();//Cashback service fee
                            break;
                        case "65": (response.Stan = Field.Substring(3)).Trim();//Stan
                            break;
                        case "16": (response.TerminalID = Field.Substring(3)).Trim();//Terminal ID
                            break;                        
                        case "D1": (response.MerchantId = Field.Substring(3)).Trim();//Merchant ID;
                            break;
                        case "D2": (response.CardIssuerName = Field.Substring(3)).Trim();//Card issuer name;
                            break;
                        case "ZT": (response.AdditionalTransInfo = Field.Substring(3)).Trim();//Additional trans info
                            break;                        
                        case "D3": (response.RetrievalRefNumber = Field.Substring(3)).Trim();//RRM Retrieval Reference Number
                            break;
                        case "FA": (response.ForeignAmount = Field.Substring(3)).Trim();//Foreign amount
                            break;
                        case "F0": (response.ForeignMID = Field.Substring(3)).Trim();//Foreign MID
                            break;                        
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                netsPayment.logHelper.saveLog("Error in SetResponseTransactionFields" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error inner exception : " + ex.InnerException);
                throw new System.Exception("Error in SetResponseTransactionFields, please check log for error details");
            }
        }   
    }
}
