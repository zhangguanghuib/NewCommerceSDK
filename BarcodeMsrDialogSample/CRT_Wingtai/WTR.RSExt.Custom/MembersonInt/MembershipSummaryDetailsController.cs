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
        using CRT.CRMService.Messages;


        /// <summary>
        /// A customized CustomersController.
        /// </summary>
        public class MembershipSummaryDetailsController : CommerceController<CustomizedDataModel.MembershipSummaryDetails, string>
        {
            /// <summary>
            /// Gets the controller name used to load extended controller.
            /// </summary>
            public override string ControllerName
            {
                get { return "GetMembershipSummary"; }
            }


            /// <summary>
            /// The action to get the cross loyalty card discount.
            /// </summary>
            /// <param name="parameters">The OData action parameters.</param>
            /// <returns>The MemberId value.</returns>
            [HttpPost]
            [CommerceAuthorization(CommerceRoles.Customer, CommerceRoles.Employee)]
            public CustomizedDataModel.MembershipSummaryDetails GetMembershipSummaryAction(ODataActionParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                var runtime = CommerceRuntimeManager.CreateRuntime(this.CommercePrincipal);
                string MemberCustomerNumber = (string)parameters["MemberCustomerNumber"];
                string TransactionId = (string)parameters["TransactionId"];

                GetMembershipSummaryResponse resp = runtime.Execute<GetMembershipSummaryResponse>(new GetMembershipSummaryRequest(MemberCustomerNumber, TransactionId), null);

                string logMessage = "GetMembershipSummary successfully handled with MemberCustomerNumber '{0}' .";
                RetailLogger.Log.ExtendedInformationalEvent(logMessage, MemberCustomerNumber, MemberCustomerNumber);
                return resp.MembershipSummaryDetails;
            }
           
        }
    }
}
