
namespace CommerceRuntime.InboundOutboundTriggers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Request = Microsoft.Dynamics.Commerce.Runtime.Messages.Request;
    using Response = Microsoft.Dynamics.Commerce.Runtime.Messages.Response;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class InboundOutboundTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(AddInventoryInboundOutboundDocumentLineRequest) };
            }
        }

        public Task OnExecuted(Request request, Response response)
        {
            return Task.CompletedTask;
        }

        public Task OnExecuting(Request request)
        {
            var req = request as AddInventoryInboundOutboundDocumentLineRequest;
            if (req.Line.QuantityToUpdate > 999999)
            {
                req.Line.QuantityToUpdate = 0m;
                throw new CommerceException("InventoryInboundOutboundDocumentLine", "[Inbound Outbound] Quantity can not greater than 999999");
            }
            return Task.CompletedTask;
        }
    }

    public class MKInventoryInboundOutboundDocumentUpdateLineTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(UpdateInventoryInboundOutboundDocumentLineRequest) };
            }
        }
        public Task OnExecuted(Request request, Response response)
        {
            return Task.CompletedTask;
        }
        public Task OnExecuting(Request request)
        {
            var req = request as UpdateInventoryInboundOutboundDocumentLineRequest;
            if (req.Line.QuantityToUpdate > 999999)
            {
                req.Line.QuantityToUpdate = 0;
                throw new CommerceException("InventoryInboundOutboundDocumentLine", "[Inbound Outbound] Quantity can not greater than 999999");
            }
            return Task.CompletedTask;
        }
    }
}
