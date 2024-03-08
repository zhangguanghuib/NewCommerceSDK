namespace RSL
{
    namespace Commerce.Runtime.Extensions
    {
        using System;
        using System.Collections.Generic;
        using System.Collections.ObjectModel;
        using System.Globalization;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Data;
        //using Microsoft.Dynamics.Commerce.Runtime.DataServices.SqlServer;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        //using Microsoft.Dynamics.Commerce.Runtime.TransactionService;
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Retail.Diagnostics;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Messages;
        using System.Linq;
        //using Microsoft.Dynamics.Commerce.Runtime.Services.CustomerOrder;
        using System.Reflection;
        using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
        using System.Threading.Tasks;
       // using Microsoft.Dynamics.Commerce.Runtime.TransactionService.Serialization;
       using  Contoso.CommerceRuntime.Entities.DataModel;

        /// <summary>
        /// Sample service to demonstrate returning an array of a new entity.
        /// </summary>
        public class LotteryTicketDataService : IRequestHandlerAsync
        {
            
            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(SendEmailRequest)
                    };
                }
            }

            /// <summary>
            /// Entry point to Lottery ticket data service.
            /// </summary>
            /// <param name="request">The request to execute.</param>
            /// <returns>Result of executing request, or null object for void operations.</returns>
            public async Task<Response> Execute(Request request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                Type reqType = request.GetType();
                if (reqType == typeof(SendEmailRequest))
                {
                    return  await SendEmail((SendEmailRequest)request).ConfigureAwait(false);
                }
                else
                {
                    string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported.", reqType);
                    RetailLogger.Log.AxGenericWarningEvent(message);
                    throw new NotSupportedException(message);
                }
            }
                 
            private  async Task<SendEmailResponse> SendEmail(SendEmailRequest request)
            {
                // Create customer info object to pass
                CustomerOrderInfo customerOrderInfo = new CustomerOrderInfo
                {
                    TransactionId = request.SalesId,
                    CustomerAccount = request.CustomerId,
                    CreationDateString = request.salestransaction.BeginDateTime.Date.ToString("dd/MM/yyyy"),
                    Id = request.salestransaction.ReceiptId
                };

                foreach (var salesLine in request.salestransaction.SalesLines)
                {
                    // Add the Sales lines
                    var itemInfor = new ItemInfo()
                    {
                        LineNumber = salesLine.LineNumber,
                        ItemId = salesLine.ItemId,
                        Quantity = salesLine.Quantity,
                        NetAmount = salesLine.NetAmount,
                    };
                    // Add the Item attributes
                    foreach (var values in salesLine.AttributeValues)
                    {
                        itemInfor.AttributeValues.Add(new AttributeValueInfo()
                        {
                            Name = values.Name,
                            TextValue = values.GetProperty("TextValue").ToString()
                        }
                        );
                    }
                    customerOrderInfo.Items.Add(itemInfor);
                }

                InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(
                     "sendEmail",
                   customerOrderInfo.ToXml());

                InvokeExtensionMethodRealtimeResponse response = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);

                ReadOnlyCollection<object> results = response.Result;
                
                return new SendEmailResponse(true);
            }
        }
    }
}
