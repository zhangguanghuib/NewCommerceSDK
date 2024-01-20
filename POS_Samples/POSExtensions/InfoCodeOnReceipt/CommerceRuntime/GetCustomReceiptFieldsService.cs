using System;
using System.Collections.Generic;
using System.Text;

namespace Contoso.GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    /// <summary>
    /// The extended service to get custom receipt field.
    /// </summary>
    /// <remarks>
    /// To print custom receipt fields on a receipt, one must handle <see cref="GetSalesTransactionCustomReceiptFieldServiceRequest"/>
    /// and <see cref="GetCustomReceiptFieldServiceResponse"/>. Here are several points about how to do this.
    /// 1. CommerceRuntime calls this request if and only if it needs to print values for some fields that are not supported by default. So make
    ///    sure your custom receipt fields have different names than existing ones. Adding a prefix in front of custom filed names would
    ///    be a good idea. The value of custom filed name should match the value you defined in AX, on Custom Filed page.
    /// 2. User should handle content-related formatting. This means that if you want to print "$ 10.00" instead of "10" on the receipt, 
    ///    you must generate "$ 10.00" by yourself. You can call <see cref="GetFormattedCurrencyServiceRequest"/> to do this. There are also some
    ///    other requests designed to format other types of values such as numbers and date time. Note, the user DO NOT need to worry about alignment,
    ///    the CommerceRuntime will take care of that.
    /// 3. If any exception happened when getting the value of custom receipt fields, CommerceRuntime will print empty value on the receipt and the
    ///    exceptions will be logged.
    /// 4. So far, only sales-transaction-based custom receipts are supported. This means you can do customization for receipts when checking out
    ///    a normal sales transaction or creating/picking up a customer order.
    /// </remarks>
    public class GetCustomReceiptFieldsService : IRequestHandlerAsync
    {
        /// <summary>
        /// Gets the supported request types.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(GetSalesTransactionCustomReceiptFieldServiceRequest),
                };
            }
        }

        /// <summary>
        /// Executes the requests.
        /// </summary>
        /// <param name="request">The request parameter.</param>
        /// <returns>The GetReceiptServiceResponse that contains the formatted receipts.</returns>
        public async Task<Response> Execute(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            Type requestedType = request.GetType();

            if (requestedType == typeof(GetSalesTransactionCustomReceiptFieldServiceRequest))
            {
                return await this.GetCustomReceiptFieldForSalesTransactionReceiptsAsync((GetSalesTransactionCustomReceiptFieldServiceRequest)request).ConfigureAwait(false);
            }

            throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
        }

        /// <summary>
        /// Gets the custom receipt filed for all transaction-based receipts, such as SalesReceipt, CustomerOrderReceipt, PickupReceipt, CreditCardReceipt, and so on.
        /// </summary>
        /// <param name="request">The service request to get custom receipt filed.</param>
        /// <returns>The value of custom receipt filed.</returns>
        private async Task<Response> GetCustomReceiptFieldForSalesTransactionReceiptsAsync(GetSalesTransactionCustomReceiptFieldServiceRequest request)
        {
            string receiptFieldName = request.CustomReceiptField;

            SalesOrder salesOrder = request.SalesOrder;
            SalesLine salesLine = request.SalesLine;
            TenderLine tenderLine = request.TenderLine;

            // Get the store currency.
            string currency = request.RequestContext.GetOrgUnit().Currency;

            string returnValue = null;
            switch (receiptFieldName)
            {
                case "WARRANTYID":
                    {
                        // FORMAT THE VALUE
                        returnValue = request.SalesLine.ReasonCodeLines.FirstOrDefault().Information;
                    }

                    break;

                case "ITEMNUMBER":
                    {
                        returnValue = salesLine == null ? string.Empty : "Custom_" + salesLine.ItemId;
                    }

                    break;

                case "TENDERID":
                    {
                        returnValue = tenderLine == null ? string.Empty : "Custom_" + tenderLine.TenderTypeId;
                    }

                    break;
            }

            return new GetCustomReceiptFieldServiceResponse(returnValue);
        }

        /// <summary>
        /// Formats the currency to another currency.
        /// </summary>
        /// <param name="value">The digital value of the currency to be formatted.</param>
        /// <param name="currencyCode">The code of the target currency.</param>
        /// <param name="context">The request context.</param>
        /// <returns>The formatted value of the currency.</returns>
        private async Task<string> FormatCurrencyAsync(decimal value, string currencyCode, RequestContext context)
        {
            GetRoundedValueServiceRequest roundingRequest = null;

            string currencySymbol = string.Empty;

            // Get the currency symbol.
            if (!string.IsNullOrWhiteSpace(currencyCode))
            {
                var getCurrenciesDataRequest = new GetCurrenciesDataRequest(currencyCode, QueryResultSettings.SingleRecord);
                var currencyResponse = await context.Runtime.ExecuteAsync<EntityDataServiceResponse<Currency>>(getCurrenciesDataRequest, context).ConfigureAwait(false);
                Currency currency = currencyResponse.PagedEntityCollection.FirstOrDefault();
                currencySymbol = currency.CurrencySymbol;
            }

            roundingRequest = new GetRoundedValueServiceRequest(value, currencyCode, 0, false);

            var roundedValueResponse = await context.ExecuteAsync<GetRoundedValueServiceResponse>(roundingRequest).ConfigureAwait(false);
            decimal roundedValue = roundedValueResponse.RoundedValue;

            var formattingRequest = new GetFormattedCurrencyServiceRequest(roundedValue, currencySymbol);
            var formattedValueResponse = await context.ExecuteAsync<GetFormattedContentServiceResponse>(formattingRequest).ConfigureAwait(false);
            string formattedValue = formattedValueResponse.FormattedValue;
            return formattedValue;
        }
    }
}
