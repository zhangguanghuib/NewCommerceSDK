using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.OData;
using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Retail.RetailServerLibrary;
using Microsoft.Dynamics.Retail.RetailServerLibrary.ODataControllers;

using WTR.CRT.DataModel;
using System.Linq;
using WTR.CRT.CRMService.Messages;

namespace WTR
{
    namespace RSExt.Custom
    {


        /// <summary>
        /// The controller to retrieve a new entity.
        /// </summary>
        [ComVisible(false)]
        public class WTR_TransactionController : CommerceController<Wtr_Transaction, string>
        {

            public override string ControllerName
            {
                get { return "Wtr_Transaction"; }
            }

            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
            public WTR_CRMResponse RegisterTransactionAction(ODataActionParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }
                RegisterTransactionRequest request = null;
                var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);
                
                QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
                queryResultSettings.Paging = new PagingInfo(10);
                IEnumerable<UtilizeVoucherCriteria> Vouchers = parameters["UtilizeVouchers"] as IEnumerable<UtilizeVoucherCriteria>;

                var member = parameters["WTR_MEMCUSTOMERNUMBER"]; //Check if request is N-1
                if (member!=null)
                {
                    request = new RegisterTransactionRequest((Wtr_Transaction)parameters["Transaction"], Vouchers.ToArray(), (WTR_InstantRedemption)parameters["InstantRedemptions"], (string)parameters["StoreId"], (string)parameters["TerminalId"], (string)parameters["TransactionId"], (long)parameters["ChannelId"], (string)parameters["DataAreaId"], (string)parameters["WTR_MEMCUSTOMERNUMBER"], (string)parameters["WTR_MEMMEMBERID"], (bool)parameters["ValidateReceipt"]) { QueryResultSettings = queryResultSettings };
                }
                else
                {
                    request = new RegisterTransactionRequest((Wtr_Transaction)parameters["Transaction"], Vouchers.ToArray(), (WTR_InstantRedemption)parameters["InstantRedemptions"], new RequestContext(runtime), (bool)parameters["ValidateReceipt"]) { QueryResultSettings = queryResultSettings };
                }
                var Response = runtime.Execute<RegisterTransactionResponse>(request, null);
                
                return Response.CRMResponse;
            }
        }
    }
}
