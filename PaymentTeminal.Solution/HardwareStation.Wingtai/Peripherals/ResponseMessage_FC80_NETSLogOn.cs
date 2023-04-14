using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTR.HWExt.Peripherals
{
    class ResponseMessage_FC80_NETSLogOn : ResponseMessage
    {
        public ResponseMessage_FC80_NETSLogOn(NETS _NETSPayment)
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
                        case "D0": (response.MerchantNameAddress = Field.Substring(3)).Trim();//Merchant Name and address;
                            break;
                        case "03": (response.TransactionDate = Field.Substring(3)).Trim();//Transaction Date
                            break;
                        case "04": (response.TransactionTime = Field.Substring(3)).Trim();//Transaction Time
                            break;
                        case "65": (response.Stan = Field.Substring(3)).Trim();//STAN;
                            break;
                        case "16": (response.TerminalID = Field.Substring(3)).Trim();//Terminal ID;
                            break;
                        case "D1": (response.MerchantId = Field.Substring(3)).Trim();//Merchant ID;
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
