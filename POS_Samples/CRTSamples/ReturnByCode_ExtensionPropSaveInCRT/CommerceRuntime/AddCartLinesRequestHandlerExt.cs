namespace Contoso.GasStationSample.CommerceRuntime
{
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Dynamics.Commerce.Runtime.Extensions;
    using System.Collections.Generic;
    using Microsoft.Dynamics.Retail.Diagnostics.Extensions;
    using Microsoft.Dynamics.Retail.Diagnostics;
    using System.Linq;

    public class AddCartLinesRequestHandlerExt : SingleAsyncRequestHandler<AddCartLinesRequest>
    {
        private enum AddCartLinesRequestHandlerEvents
        {
            /// <summary>
            /// Event to inform that an updated cart line was included in a AddCartLines request.
            /// </summary>
            UpdatingExistingLinesOnAddCartLines,
        }

        protected async override Task<Response> Process(AddCartLinesRequest request)
        {
            ThrowIf.Null(request, nameof(request));

            AddCartLinesRequest oldRequest = request;
            List<CartLine> cartLines = new List<CartLine>();
            foreach (CartLine cartLine in oldRequest.CartLines)
            {
                cartLine.Description = "Unified_ReceiptId";
                cartLines.Add(cartLine);
            }
            request = new AddCartLinesRequest(oldRequest.CartId, cartLines, oldRequest.CalculationMode, oldRequest.CartVersion);

            // we support editing an existing line during AddCartLines, logging to know if customers are using so we can remove it.
            if (request.CartLines.Any(IsNotNewLine))
            {
                var numberOfLines = request.CartLines.Count(IsNotNewLine).ToString();
                RetailLogger.Log.LogInformation(
                    AddCartLinesRequestHandlerEvents.UpdatingExistingLinesOnAddCartLines,
                    "Updating cart lines in a add cart line call, number of updated lines: {numberOfLines}.",
                    numberOfLines.AsSystemMetadata());
            }

            var cart = new Cart { Id = request.CartId, Version = request.CartVersion };
            cart.CartLines.AddRange(request.CartLines);

            // Allow adding gift card item through AddCart if the gift card is already activated through 3rd party gift card provider.
            bool isGiftCardOperation = cart.CartLines.Where((line) => line.IsGiftCardLine).Any();

            var saveCartRequest = new SaveCartRequest(cart, request.CalculationMode, isGiftCardOperation);
            var response = await oldRequest.RequestContext.ExecuteAsync<SaveCartResponse>(saveCartRequest).ConfigureAwait(false);

            return response;
        }

        private static bool IsNotNewLine(CartLine cartLine) => !string.IsNullOrEmpty(cartLine.LineId);
    }
}
