/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace Commerce.Runtime.XZReportsFrance
    {
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        ///  Class holding localization and formatting methods.
        /// </summary>
        internal static class ReceiptLocalizationHelper
        {
            private const int TaxCodeWidth = 10;
            private const int TaxRateWidth = 4;

            /// <summary>
            /// Gets the localized string.
            /// </summary>
            /// <param name="titleResourceId">The resource ID for title text.</param>
            /// <param name="languageId">The language ID.</param>
            /// <returns>The localized string. If cannot find the translated string, then return the original string.</returns>
            internal static string GetLocalizedString(string titleResourceId, string languageId)
            {
                if (string.IsNullOrEmpty(titleResourceId))
                {
                    return string.Empty;
                }

                var result = XZReportsLocalizer.Instance.Translate(languageId, titleResourceId);

                // If cannot find the translated string, then return the original string.
                if (string.IsNullOrEmpty(result))
                {
                    return titleResourceId;
                }
                else
                {
                    return result;
                }
            }

            /// <summary>
            /// Formats the tax line.
            /// </summary>
            /// <param name="taxLine">The tax line to be formatted.</param>
            /// <param name="context">The request context.</param>
            /// <returns>The formatted tax line.</returns>
            internal static async Task<string> FormatTaxAsync(ShiftTaxLine taxLine, RequestContext context)
            {
                string taxRateText = await FormatNumberAsync(taxLine.TaxRate, context).ConfigureAwait(false);
                string taxCodeText = string.Format("{0}{1}%", taxLine.TaxCode.PadRight(TaxCodeWidth), taxRateText.PadLeft(TaxRateWidth));

                return taxCodeText;
            }

            /// <summary>
            /// Formats the number.
            /// </summary>
            /// <param name="number">The number to be formatted.</param>
            /// <param name="context">The request context.</param>
            /// <returns>The formatted number.</returns>
            internal static async Task<string> FormatNumberAsync(decimal number, RequestContext context)
            {
                var request = new GetFormattedNumberServiceRequest(number);
                var resultResponse = await context.ExecuteAsync<GetFormattedContentServiceResponse>(request).ConfigureAwait(false);

                return resultResponse.FormattedValue;
            }

            /// <summary>
            /// Formats the value as currency.
            /// </summary>
            /// <param name="value">The decimal value of the currency to be formatted.</param>
            /// <param name="currencyCode">The currency code.</param>
            /// <param name="context">The request context.</param>
            /// <returns>The formatted value of the currency.</returns>
            internal static async Task<string> FormatCurrencyAsync(decimal value, string currencyCode, RequestContext context)
            {
                string currencySymbol = string.Empty;
                int currencyDecimals = 0;

                if (!string.IsNullOrWhiteSpace(currencyCode))
                {
                    var getCurrenciesDataRequest = new GetCurrenciesDataRequest(currencyCode, QueryResultSettings.SingleRecord);
                    var currencyResponse = await context.Runtime.ExecuteAsync<EntityDataServiceResponse<Currency>>(getCurrenciesDataRequest, context).ConfigureAwait(false);
                    Currency currency = currencyResponse.PagedEntityCollection.FirstOrDefault();

                    if (currency != null)
                    {
                        currencySymbol = currency.CurrencySymbol;
                        currencyDecimals = currency.NumberOfDecimals;
                    }
                }

                var formattingRequest = new GetFormattedCurrencyServiceRequest(value, currencySymbol, currencyDecimals);
                var formattedValueResponse = await context.ExecuteAsync<GetFormattedContentServiceResponse>(formattingRequest).ConfigureAwait(false);

                return formattedValueResponse.FormattedValue;
            }
        }
    }
}
