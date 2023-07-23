namespace Contoso.GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class AddCartLinesAsyncTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(AddCartLinesRequest) };
            }
        }

        public Task OnExecuted(Request request, Response response)
        {
            return Task.CompletedTask;
        }

        public Task OnExecuting(Request request)
        {
            //if (request is AddCartLinesRequest)
            //{
            //    foreach (CartLine cartLine in ((AddCartLinesRequest)request).CartLines)
            //    {
            //        cartLine.Description = "Unified_ReceiptId";
            //    }
            //}

            //if (request is AddCartLinesRequest)
            //{
            //    AddCartLinesRequest oldRequest = (AddCartLinesRequest) request;
            //    List<CartLine> cartLines = new List<CartLine>();
            //    foreach (CartLine cartLine in ((AddCartLinesRequest)request).CartLines)
            //    {
            //        cartLine.Description = "Unified_ReceiptId";
            //        cartLines.Add(cartLine);
            //    }

            //    request = new AddCartLinesRequest(oldRequest.CartId, cartLines, oldRequest.CalculationMode, oldRequest.CartVersion);
            //}



            return Task.CompletedTask;
        }
    }
}
