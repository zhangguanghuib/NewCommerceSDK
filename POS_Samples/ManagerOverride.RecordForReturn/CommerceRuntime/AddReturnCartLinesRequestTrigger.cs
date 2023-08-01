using Microsoft.Dynamics.Commerce.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contoso.GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class AddReturnCartLinesRequestTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(AddReturnCartLinesRequest) };
            }
        }

        /// <summary>
        /// Post trigger to retrieve extension package.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public async Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");

            AddReturnCartLinesRequest addReturnCartLinesRequest = (AddReturnCartLinesRequest)request;
            ICommercePrincipal commercePrincipal = addReturnCartLinesRequest.RequestContext.GetPrincipal();

            if (commercePrincipal.OriginalUserId != commercePrincipal.UserId)
            {
               string originTransactionId =  addReturnCartLinesRequest.ReturnCartLines.AsPagedResult().ToList().First().ReturnTransactionId;

                var message = string.Format(
                          "Manager with id '{0}' has approved override for operation with id '{1}' for original transaction {3} to the operator with id '{2}'.",
                          commercePrincipal.StaffId,
                          RetailOperation.ReturnTransaction,
                          commercePrincipal.OriginalUserId,
                          originTransactionId);

                await LogAuditEntry(
                           request.RequestContext,
                           ExtensibleAuditEventType.ManagerOverride,
                           "ElevateUser",
                           message,
                           AuditLogTraceLevel.Trace).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Pre trigger code.
        /// </summary>
        /// <param name="request">The request.</param>
        public Task OnExecuting(Request request)
        {
            return Task.CompletedTask;
        }

        private static async Task LogAuditEntry(RequestContext context, ExtensibleAuditEventType eventType, string source, string value, AuditLogTraceLevel logTraceLevel = AuditLogTraceLevel.Trace)
        {
            var auditEvent = new AuditEvent();
            auditEvent.InitializeEventInfo(source, value, logTraceLevel, eventType);

            var auditLogServiceRequest = new RegisterAuditEventServiceRequest(auditEvent);
            await context.ExecuteAsync<NullResponse>(auditLogServiceRequest).ConfigureAwait(false);
        }
    }
}
