using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceRuntime.Triggers
{
    using Contoso.CommerceRuntime.Messages;
    //using Bucherer.StoreCommerce.CommerceRuntime.Entities.DataModel;
    //using Bucherer.StoreCommerce.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Framework.Exceptions;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    // using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Request = Microsoft.Dynamics.Commerce.Runtime.Messages.Request;
    using Response = Microsoft.Dynamics.Commerce.Runtime.Messages.Response;
    public class SaveTenderLineRequestTrigger : IRequestTriggerAsync
    {

        private const string BankTransferTenderTypeId = "13";
        private const string BankTransfer = "BankTransfer";

        /// <summary>
        /// Gets the supported requests for this trigger.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new Type[]
                {
                    typeof(SaveTenderLineRequest)
                };
            }
        }

        /// <summary>
        /// Pre trigger code.
        /// </summary>
        /// <param name="request">The request.</param>
        public async Task OnExecuting(Request request)
        {
            ThrowIf.Null(request, "request");

            SaveTenderLineRequest saveTenderLineRequest = request as SaveTenderLineRequest;

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Post trigger code to retrieve extension properties.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public async Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");

            SaveTenderLineResponse saveTenderLineResponse = response as SaveTenderLineResponse;
            SaveTenderLineRequest saveTenderLineRequest = request as SaveTenderLineRequest;

            if (saveTenderLineRequest.TenderLine != null && saveTenderLineRequest.TenderLine.TenderTypeId.Equals(BankTransferTenderTypeId))
            {
                Cart cart = saveTenderLineResponse.Cart;
                if (cart != null && cart.ExtensionProperties != null )
                {
                     CommerceProperty commerceProperty =  
                        cart.ExtensionProperties.FirstOrDefault<CommerceProperty>(p => p.Key.Equals(BankTransfer));

                    if(commerceProperty != null && !String.IsNullOrEmpty(commerceProperty.Value.StringValue))
                    {
                        string bankTransferComment = commerceProperty.Value.StringValue;
                        SetBankTransferCommentToTenderLineRequest setBankTransferCommentToTenderLineRequest = new SetBankTransferCommentToTenderLineRequest(saveTenderLineRequest, bankTransferComment);
                        SetBankTransferCommentToTenderLineResponse setBankTransferCommentToTenderLineResponse = await request.RequestContext.ExecuteAsync<SetBankTransferCommentToTenderLineResponse>(setBankTransferCommentToTenderLineRequest).ConfigureAwait(false);
                    }
                }
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

    }
}
