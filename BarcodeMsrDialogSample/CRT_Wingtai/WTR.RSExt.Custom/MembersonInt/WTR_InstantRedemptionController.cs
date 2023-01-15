namespace WTR
{
    namespace RSExt.Custom
    {
        using System;
        using System.Web.Http;
        using System.Web.OData;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Retail.Diagnostics;
        using Microsoft.Dynamics.Retail.RetailServerLibrary;
        using Microsoft.Dynamics.Retail.RetailServerLibrary.ODataControllers;
        using CustomizedDataModel = WTR.CRT.DataModel;
        using System.Collections.Generic;
        using WTR.CRT.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime;
        using System.Web.Script.Serialization;
        using WTR.CRTExt.CRMService.Messages;
        using CRT.CRMService.Messages;

        public class WTR_InstantRedemptionController : CommerceController<CustomizedDataModel.WTR_InstantRedemption, string>
        {
            public override string ControllerName
            {
                get { return "WTR_InstantRedemption"; }
            }

            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Customer, CommerceRoles.Employee)]
            public IEnumerable<CustomizedDataModel.WTR_InstantRedemption> RedeemPointsAction(ODataActionParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);
                string ReqContent = string.Empty;
                ReqContent = new JavaScriptSerializer().Serialize(parameters);

                QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
                queryResultSettings.Paging = new PagingInfo(10);

                var request = new RedeemPointsRequest((WTR_InstantRedemption)parameters["InstantRedemptions"], (string)parameters["StoreId"], (string)parameters["TerminalId"], (string)parameters["TransactionId"], (long)parameters["ChannelId"], (string)parameters["DataAreaId"]) { QueryResultSettings = queryResultSettings };

                RedeemPointsResponse resp = runtime.Execute<RedeemPointsResponse>(request, null);

                string logMessage = "RedeemPoints successfully handled for following Request -'{0}' .";
                RetailLogger.Log.ExtendedInformationalEvent(logMessage, ReqContent);
                return resp.InstantRedemptions;
            }


            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Customer, CommerceRoles.Employee)]
            public IEnumerable<CustomizedDataModel.WTR_InstantRedemption> RefundPointsAction(ODataActionParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);
                string ReqContent = string.Empty;
                ReqContent = new JavaScriptSerializer().Serialize(parameters);

                QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
                queryResultSettings.Paging = new PagingInfo(10);

                var request = new RefundPointsRequest((WTR_InstantRedemption)parameters["InstantRedemptions"], (string)parameters["StoreId"], (string)parameters["TerminalId"], (string)parameters["TransactionId"], (long)parameters["ChannelId"], (string)parameters["DataAreaId"]) { QueryResultSettings = queryResultSettings };

                RefundPointsResponse resp = runtime.Execute<RefundPointsResponse>(request, null);

                string logMessage = "RefundPoints successfully handled for following Request -'{0}' .";
                RetailLogger.Log.ExtendedInformationalEvent(logMessage, ReqContent);
                return resp.InstantRedemptions;
            }
        }
    }
}
