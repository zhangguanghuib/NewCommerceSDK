using System;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.OData;
using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Retail.RetailServerLibrary;
using Microsoft.Dynamics.Retail.RetailServerLibrary.ODataControllers;
using WTR.CRT.DataModel;
using WTR.CRTExt.CRMService.Messages;
using WTR.CRT.CRMService.Messages;

// WTR_RAD After SIT2

namespace WTR
{
    namespace RSExt.Custom
    {


        /// <summary>
        /// The controller to retrieve a new entity.
        /// </summary>
        [ComVisible(false)]
        public class WTR_TransactionMemberController : CommerceController<WTR_MemberIdentifier, string>
        {

            public override string ControllerName
            {
                get { return "WTR_MemberIdentifier"; }
            }

            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
            public WTR_MemberIdentifier GetMemCustomerIdAction(ODataActionParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);

                QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
                queryResultSettings.Paging = new PagingInfo(10);


                var request = new GetMemCustomerIdRequest((string)parameters["StoreId"], (string)parameters["TerminalId"],
                    (string)parameters["TransactionId"], (long)parameters["ChannelId"], (string)parameters["DataAreaId"])
                { QueryResultSettings = queryResultSettings };

                var Response = runtime.Execute<GetMemCustomerIdResponse>(request, null);

                return Response.WTR_MemberIdentifier;
            }


        }
    }
}
