using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.OData;
//using Commerce.Runtime.StoreHoursSample.Messages;
using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Retail.RetailServerLibrary;
using Microsoft.Dynamics.Retail.RetailServerLibrary.ODataControllers;
// using SampleDataModel = Commerce.Runtime.DataModel;

using WTR.CRT.DataModel;
using System.Linq;
using WTR.CRTExt.CRMService.Messages;
using WTR.CRT.CRMService.Messages;

namespace WTR
{
    namespace RSExt.Custom
    {


        /// <summary>
        /// The controller to retrieve a new entity.
        /// </summary>
        [ComVisible(false)]
        public class WTR_CancelDiscountController : CommerceController<WTR_CancelDiscount, string>
        {

            public override string ControllerName
            {
                get { return "WTR_CancelDiscount"; }
            }

            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
            public System.Web.OData.PageResult<WTR_CancelDiscount> VoidTransactionAction(ODataActionParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);

                QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
                queryResultSettings.Paging = new PagingInfo(10);

                IEnumerable<UtilizeVoucherCriteria> Vouchers = parameters["UtilizeVouchers"] as IEnumerable<UtilizeVoucherCriteria>;
                WTR_InstantRedemption InstantRedemption = (WTR_InstantRedemption)parameters["InstantRedemption"];


                var request = new VoidTransactionRequest((string)parameters["StoreId"], (string)parameters["TerminalId"],
                    (string)parameters["TransactionId"], (long)parameters["ChannelId"], (string)parameters["DataAreaId"],
                    (string)parameters["WTR_MEMCUSTOMERNUMBER"], (string)parameters["WTR_MEMMEMBERID"], Vouchers.ToArray(), InstantRedemption)
                { QueryResultSettings = queryResultSettings };

                var Response = runtime.Execute<VoidTransactionResponse>(request, null);

                return this.ProcessPagedResults(Response.wtr_CancelDiscount);
            }

         
        }
    }
}
