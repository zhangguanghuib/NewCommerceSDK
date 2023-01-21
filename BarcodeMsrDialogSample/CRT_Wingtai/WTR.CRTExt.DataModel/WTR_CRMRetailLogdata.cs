using Microsoft.Dynamics.Commerce.Runtime;
//using Microsoft.Dynamics.Retail.Diagnostics;
using System;

namespace WTR.CRT.DataModel
{
    public class WTR_CRMRetailLogdata
    {
    
        public WTR_CRMRetailLogdata(RequestContext req, string transactionId, string memberNo, string cardNo, string retailPosAction, int statusCode )
        {
            this.RequestContext = req;
            this.TransactionId = transactionId;
            this.Store = this.RequestContext.GetDeviceConfiguration().StoreNumber;
            this.Terminal = this.RequestContext.GetTerminalId();
            this.RetailPOSAction = retailPosAction;
            this.StatusCode = statusCode;
            if (memberNo != null)
                this.MemberNumber = memberNo;
            else
                this.MemberNumber = string.Empty;
            this.CardNo = cardNo;
            this.LogDate = System.DateTime.UtcNow;
        }

        public string TransactionId { get; set; }

        public string Store { get; set; }

        public string Terminal { get; set; }

        public string RetailPOSAction { get; set; }

        public int StatusCode { get; set; }

        public string MemberNumber { get; set; }

        public string CardNo { get; set; }

        public RequestContext RequestContext { get; set; }

        public DateTime LogDate { get; set; }

    }
}
