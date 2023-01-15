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
        using Microsoft.Dynamics.Commerce.Runtime;
        using System.Web.Script.Serialization;
        using WTR.CRTExt.CRMService.Messages;
        using CRT.CRMService.Messages;

        public class WTR_ConversionRateController : CommerceController<CustomizedDataModel.WTR_ConversionRate, string>
        {
            public override string ControllerName
            {
                get { return "WTR_ConversionRate"; }
            }

            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Customer, CommerceRoles.Employee)]
            public IEnumerable<CustomizedDataModel.WTR_ConversionRate> GetConversionRateAction(ODataActionParameters parameters)
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

                var request = new GetConversionRateRequest((string)parameters["StoreId"], (string)parameters["TerminalId"], (string)parameters["TransactionId"], (long)parameters["ChannelId"], (string)parameters["DataAreaId"]) { QueryResultSettings = queryResultSettings };

                GetConversionRateResponse resp = runtime.Execute<GetConversionRateResponse>(request, null);

                string logMessage = "GetConversionRate successfully handled for following Request -'{0}' .";
                RetailLogger.Log.ExtendedInformationalEvent(logMessage, ReqContent);
                return resp.ConversionRate;
            }
        }
    }
}
