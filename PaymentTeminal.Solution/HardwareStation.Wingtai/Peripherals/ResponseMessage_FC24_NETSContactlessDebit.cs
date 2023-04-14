using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTR.HWExt.Peripherals
{
    class ResponseMessage_FC24_NETSContactlessDebit : ResponseMessage
    {
        public ResponseMessage_FC24_NETSContactlessDebit(NETS _NETSPayment)
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
                        case "02": (response.ResponseText = Field.Substring(3)).Trim();//Response Text;
                            break;
                        case "D0": (response.MerchantNameAddress = Field.Substring(3)).Trim();//Merchant name;
                            break;
                        case "16": (response.TerminalID = Field.Substring(3)).Trim();//EFT Terminal ID;
                            break;
                        case "D1": (response.MerchantId = Field.Substring(3)).Trim();//Merchant ID;
                            break;
                        case "50": (response.BatchNumber = Field.Substring(3)).Trim();//Batch number
                            break;
                        case "H6": (response.Stan = Field.Substring(3)).Trim();//STAN;
                            break;
                        case "40": (response.TransactionAmount = Field.Substring(3)).Trim();//Transaction amount;
                            break;
                        case "ZA": (response.TotalFee = Field.Substring(3)).Trim();//Total Fee;
                            break;
                        case "ZB": (response.FeeDueToMerchant = Field.Substring(3)).Trim();//Fee due to merchant;
                            break;
                        case "ZC": (response.FeeDueFromMerchant = Field.Substring(3)).Trim();//Fee due from merchant;
                            break;
                        case "30": (response.CashCardCAN = Field.Substring(3)).Trim();//Cash card CAN;
                            break;
                        case "C2": (response.ExpiryDate = Field.Substring(3)).Trim();//Expiry date;
                            break;
                        case "R0": (response.ResponseMessage1 = Field.Substring(3)).Trim();//Response message 1
                            break;
                        case "R1": (response.ResponseMessage2 = Field.Substring(3)).Trim();//Response message 2
                            break;
                        case "ZP": (response.PurchaseFee = Field.Substring(3)).Trim();//Purchase fee
                            break;
                        case "01": (response.ApprovalCode = Field.Substring(3)).Trim();//Approval code
                            break;
                        case "D2": (response.CardIssuerName = Field.Substring(3)).Trim();//Card issuer name
                            break;
                        case "ZT": (response.AdditionalTransInfo = Field.Substring(3)).Trim();//Additional transaction information
                            break;
                        case "D3": (response.RetrievalRefNumber = Field.Substring(3)).Trim();//Retrieval reference number
                            break;
                        case "C1": (response.CEPASVersion = Field.Substring(3)).Trim();//CEPAS version
                            break;
                        case "C0": (response.TransData = Field.Substring(3)).Trim();//Trans data
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
