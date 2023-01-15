/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

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
        using System.Linq;
        using CRT.CRMService.Messages;

        /// <summary>
        /// A customized CustomersController.
        /// </summary>
        public class MemberVoucherController : CommerceController<CustomizedDataModel.MemberVoucher, string>
        {
            /// <summary>
            /// Gets the controller name used to load extended controller.
            /// </summary>
            public override string ControllerName
            {
                get { return "MemberVoucher"; }
            }


            /// <summary>
            /// The action to Get Member Vouchers.
            /// </summary>
            /// <param name="parameters">The OData action parameters.</param>
            /// <returns>The MemberId value.</returns>
            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Customer, CommerceRoles.Employee)]
            public System.Web.OData.PageResult<CustomizedDataModel.MemberVoucher> GetMemberVouchersAction(ODataActionParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);
                string MemberId = (string)parameters["MemberId"];

                PagedResult<CustomizedDataModel.MemberVoucher> resp = runtime.Execute<GetMemberVouchersResponse>(new GetMemberVouchersRequest(MemberId, (string)parameters["StoreId"], (string)parameters["TerminalId"], (string)parameters["TransactionId"], (long)parameters["ChannelId"], (string)parameters["DataAreaId"]), null).MemberVouchers;

                string logMessage = "GetMemberVouchersAction successfully handled with MemberId '{0}' .";
                RetailLogger.Log.ExtendedInformationalEvent(logMessage, MemberId, MemberId);
                return this.ProcessPagedResults(resp);
            }

            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Customer, CommerceRoles.Employee)]
            public IEnumerable<CustomizedDataModel.MemberVoucher> UtilizeVoucherAction(ODataActionParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);
                IEnumerable<UtilizeVoucherCriteria> Vouchers = parameters["UtilizeVouchers"] as IEnumerable<UtilizeVoucherCriteria>;

                var UtilizeVoucherArray = Vouchers.ToArray();
                string ReqContent = string.Empty;
                if (UtilizeVoucherArray != null && UtilizeVoucherArray.Length > 0)
                {
                    ReqContent = new JavaScriptSerializer().Serialize(parameters);
                }
                QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
                queryResultSettings.Paging = new PagingInfo(10);

                var request = new UtilizeVoucherRequest(Vouchers.ToArray(),(string)parameters["TransactionId"]) { QueryResultSettings = queryResultSettings };
                UtilizeVoucherResponse resp = runtime.Execute<UtilizeVoucherResponse>(request, null);

                string logMessage = "UtilizeVoucher successfully handled for following Request -'{0}' .";
                RetailLogger.Log.ExtendedInformationalEvent(logMessage, ReqContent);
                return resp.MemberVouchers;
            }



            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Customer, CommerceRoles.Employee)]
            public IEnumerable<CustomizedDataModel.MemberVoucher> UpdateMemberVoucherAction(ODataActionParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);
                IEnumerable<UtilizeVoucherCriteria> Vouchers = parameters["UtilizeVouchers"] as IEnumerable<UtilizeVoucherCriteria>;

                var UtilizeVoucherArray = Vouchers.ToArray();
                string ReqContent = string.Empty;
                if (UtilizeVoucherArray != null && UtilizeVoucherArray.Length > 0)
                {
                    ReqContent = new JavaScriptSerializer().Serialize(parameters);
                }
                QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
                queryResultSettings.Paging = new PagingInfo(10);

                var request = new UpdateMemberVoucherRequest(UtilizeVoucherArray, (string)parameters["StoreId"], (string)parameters["TerminalId"], (string)parameters["TransactionId"], (long)parameters["ChannelId"], (string)parameters["DataAreaId"]) { QueryResultSettings = queryResultSettings };

                UpdateMemberVoucherResponse resp = runtime.Execute<UpdateMemberVoucherResponse>(request, null);

                string logMessage = "UpdateMemberVoucher successfully handled for following Request -'{0}' .";
                RetailLogger.Log.ExtendedInformationalEvent(logMessage, ReqContent);
                return resp.MemberVouchers;
            }

            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Customer, CommerceRoles.Employee)]
            public IEnumerable<CustomizedDataModel.MemberVoucher> CancelVoucherAction(ODataActionParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);
                IEnumerable<UtilizeVoucherCriteria> Vouchers = parameters["UtilizeVouchers"] as IEnumerable<UtilizeVoucherCriteria>;

                var UtilizeVoucherArray = Vouchers.ToArray();
                string ReqContent = string.Empty;
                if (UtilizeVoucherArray != null && UtilizeVoucherArray.Length > 0)
                {
                    ReqContent = new JavaScriptSerializer().Serialize(parameters);
                }
                QueryResultSettings queryResultSettings = QueryResultSettings.SingleRecord;
                queryResultSettings.Paging = new PagingInfo(10);

                var request = new CancelVoucherRequest(UtilizeVoucherArray, (string)parameters["StoreId"], (string)parameters["TerminalId"], (string)parameters["TransactionId"], (long)parameters["ChannelId"], (string)parameters["DataAreaId"]) { QueryResultSettings = queryResultSettings };

                CancelVoucherResponse resp = runtime.Execute<CancelVoucherResponse>(request, null);

                string logMessage = "CancelVoucher successfully handled for following Request -'{0}' .";
                RetailLogger.Log.ExtendedInformationalEvent(logMessage, ReqContent);
                return resp.MemberVouchers;
            }
        }
    }
}
