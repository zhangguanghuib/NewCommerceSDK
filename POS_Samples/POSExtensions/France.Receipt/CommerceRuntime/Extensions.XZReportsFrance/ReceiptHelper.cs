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
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Class holding custom France X/Z report building methods.
        /// </summary>
        public static class ReceiptHelper
        {
            /// <summary>
            /// The format of a section.
            /// </summary>
            public const string SectionFormat = "{0}:{1}";

            /// <summary>
            /// The padding symbol.
            /// </summary>
            public const char Padding = ' ';

            /// <summary>
            /// The paper width.
            /// </summary>
            public const int PaperWidth = 55;

            /// <summary>
            /// Append France-specific shift details.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="shift">The current shift.</param>
            /// <param name="reportLayout">The receipt layout.</param>
            public static async Task AppendFranceShiftDetailsAsync(RequestContext context, Shift shift, StringBuilder reportLayout)
            {
                ThrowIf.Null(context, "context");
                ThrowIf.Null(shift, "shift");
                ThrowIf.Null(reportLayout, "reportLayout");

                await AppendFranceTotalsAsync(context, shift, reportLayout).ConfigureAwait(false);
                await AppendFranceTaxLinesAsync(context, shift, reportLayout).ConfigureAwait(false);
            }

            /// <summary>
            /// Append France totals information.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="shift">The current shift.</param>
            /// <param name="reportLayout">The receipt layout.</param>
            private static async Task AppendFranceTotalsAsync(RequestContext context, Shift shift, StringBuilder reportLayout)
            {
                string currency = context.GetOrgUnit()?.Currency;

                ReceiptHelper.AppendReportLine(context, reportLayout, ReceiptConstants.ReportShiftTotalsGroupHeaderResourceId);
                ReceiptHelper.AppendReportLine(context,
                    reportLayout,
                    ReceiptConstants.TotalCashSalesResourceId,
                    await ReceiptLocalizationHelper.FormatCurrencyAsync(shift.ShiftSalesTotal, currency, context).ConfigureAwait(false));
                ReceiptHelper.AppendReportLine(context,
                    reportLayout,
                    ReceiptConstants.TotalCashReturnsResourceId,
                    await ReceiptLocalizationHelper.FormatCurrencyAsync(shift.ShiftReturnsTotal, currency, context).ConfigureAwait(false));
                ReceiptHelper.AppendReportLine(context,
                    reportLayout,
                    ReceiptConstants.GrandTotalResourceId,
                    await ReceiptLocalizationHelper.FormatCurrencyAsync(shift.ShiftSalesTotal - shift.ShiftReturnsTotal, currency, context).ConfigureAwait(false));
                ReceiptHelper.AppendReportLine(context,
                    reportLayout,
                    ReceiptConstants.PerpetualGrandTotalResourceId,
                    await ReceiptLocalizationHelper.FormatCurrencyAsync(shift.SalesGrandTotal - shift.ReturnsGrandTotal, currency, context).ConfigureAwait(false));
                ReceiptHelper.AppendReportLine(context,
                    reportLayout,
                    ReceiptConstants.PerpetualGrandTotalAbsoluteValueResourceId,
                    await ReceiptLocalizationHelper.FormatCurrencyAsync(shift.SalesGrandTotal + shift.ReturnsGrandTotal, currency, context).ConfigureAwait(false));

                reportLayout.AppendLine();
            }

            /// <summary>
            /// Append France tax information.
            /// </summary>
            /// <param name="context">The request context.</param>
            /// <param name="shift">The current shift.</param>
            /// <param name="reportLayout">The receipt layout.</param>
            private static async Task AppendFranceTaxLinesAsync(RequestContext context, Shift shift, StringBuilder reportLayout)
            {
                string currency = context.GetOrgUnit()?.Currency;
                var taxLines = shift.TaxLines.Where(t => !string.IsNullOrEmpty(t.TaxCode)).OrderBy(t => t.TaxCode).ToList();

                if (taxLines.Any())
                {
                    ReceiptHelper.AppendReportLine(context, reportLayout, ReceiptConstants.ReportVATAmountsGroupHeaderResourceId);
                    foreach (ShiftTaxLine taxLine in taxLines)
                    {
                        string taxTitle = await ReceiptLocalizationHelper.FormatTaxAsync(taxLine, context).ConfigureAwait(false);
                        var value = await ReceiptLocalizationHelper.FormatCurrencyAsync(taxLine.TaxAmount, currency, context).ConfigureAwait(false);
                        ReceiptHelper.AppendReportLine(context, reportLayout, taxTitle, value);
                    }

                    reportLayout.AppendLine();
                }
            }

            /// <summary>
            /// Appends report line with value with title tuple.
            /// </summary>
            /// <param name="context">Request context.</param>
            /// <param name="receiptBuilder">The receipt string builder.</param>
            /// <param name="titleResourceId">Resource identifier.</param>
            /// <param name="value">Value to print.</param>
            private static void AppendReportLine(RequestContext context, StringBuilder receiptBuilder, string titleResourceId, object value)
            {
                var translatedString = ReceiptLocalizationHelper.GetLocalizedString(titleResourceId, context.LanguageId);
                string valueString = value?.ToString() ?? string.Empty;

                receiptBuilder.AppendLine(string.Format(SectionFormat, translatedString, valueString.PadLeft(PaperWidth - translatedString.Length - 2)));
            }

            /// <summary>
            /// Appends report line with title.
            /// </summary>
            /// <param name="context">Request context.</param>
            /// <param name="receiptBuilder">The receipt string builder.</param>
            /// <param name="titleResourceId">Resource identifier.</param>
            private static void AppendReportLine(RequestContext context, StringBuilder receiptBuilder, string titleResourceId)
            {
                titleResourceId = ReceiptLocalizationHelper.GetLocalizedString(titleResourceId, context.LanguageId);

                receiptBuilder.AppendLine(titleResourceId);
                receiptBuilder.AppendLine(string.Empty.PadLeft(PaperWidth, '-'));
            }
        }
    }
}
