using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Retail.RetailServerLibrary;
using Microsoft.Dynamics.Retail.RetailServerLibrary.ODataControllers;
using System;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.OData;
using WTR.CRTExt.CRMService.Messages;
using WTR.CRT.DataModel;
using WTR.CRT.CRMService.Messages;
using WTR.CRT.CRMService.Services;

namespace WTR.RSExt.Custom
{
    [ComVisible(false)]
    public class WTR_MemberController :CommerceController<WTR_CRMResponse, string>
    {
        public override string ControllerName
        {
            get { return "WTR_CRMResponse"; }
        }

        [HttpPost]
        [CommerceAuthorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public WTR_CRMResponse CreateMemberAction(ODataActionParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);

            QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
            queryResultSettings.Paging = new PagingInfo(10);


            var request = new CreateMemberRequest();
            request.memberDetails = (Member)parameters["MemberDetails"];
            request.QueryResultSettings = queryResultSettings;

            var Response = runtime.Execute<CreateMemberResponse>(request, null);
            var res = new WTR_CRMResponse("WTR_CRMResponse");
            res.ReturnStatus = Response.ReturnStatus;
            res.ReturnMessage = Response.ReturnMessage;
            res.Id = 0;
            return res;
        }

        [HttpPost]
        [CommerceAuthorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public WTR_CRMResponse UpdateMemberAction(ODataActionParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);

            QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
            queryResultSettings.Paging = new PagingInfo(10);


            var request = new UpdateMemberRequest();
            request.memberDetails  = (Member)parameters["MemberDetails"];

            request.QueryResultSettings = queryResultSettings;

            var Response = runtime.Execute<UpdateMemberResponse>(request, null);
            var res = new WTR_CRMResponse("WTR_CRMResponse");
            res.ReturnStatus = Response.ReturnStatus;
            res.ReturnMessage = Response.ReturnMessage;
            res.Id = 0;
            return res;
        }


        [HttpPost]
        [CommerceAuthorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public WTR_CRMResponse ReturnTransactionAction(ODataActionParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);

            QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
            queryResultSettings.Paging = new PagingInfo(10);


            var request = new ReturnTransactionRequest((bool)parameters["IsFullReturn"], (bool)parameters["IsReturnByReceipt"], (Wtr_Transaction)parameters["Wtr_Transaction"]) { QueryResultSettings = queryResultSettings };

            var Response = runtime.Execute<ReturnTransactionResponse>(request, null);

            return Response.response;
        }

        [HttpPost]
        [CommerceAuthorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public MembershipSummaryDetails SetMemberAction(ODataActionParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);

            QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
            queryResultSettings.Paging = new PagingInfo(10);


            var request = new SetMemberRequest();
            request.TransactionId = (string)parameters["TransactionId"];
      
            var Response = runtime.Execute<GetMembershipSummaryResponse>(request, null);
           
            return Response.MembershipSummaryDetails;
        }

    }
}
