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
        /// <summary>
        /// Class holding the constants specific to France X/Z reports.
        /// </summary>
        public static class ReceiptConstants
        {
            /// <summary>
            /// Translation resource id for perpetual grand total value title.
            /// </summary>
            /// 
            public const string PerpetualGrandTotalResourceId = "XZReportFrance_PerpetualGrandTotal";

            /// <summary>
            /// Translation resource id for perpetual grand total (absolute value) value title.
            /// </summary>
            public const string PerpetualGrandTotalAbsoluteValueResourceId = "XZReportFrance_PerpetualGrandTotalAbsoluteValue";

            /// <summary>
            /// Translation resource id for grand total value title.
            /// </summary>
            public const string GrandTotalResourceId = "XZReportFrance_GrandTotal";

            /// <summary>
            /// Translation resource id for shift totals group header.
            /// </summary>
            public const string ReportShiftTotalsGroupHeaderResourceId = "XZReportFrance_ShiftTotals";

            /// <summary>
            /// Translation resource id tax amount per tax code group header.
            /// </summary>
            public const string ReportVATAmountsGroupHeaderResourceId = "XZReportFrance_VATAmounts";

            /// <summary>
            /// Translation resource id for TotalCashReturns value title.
            /// </summary>
            public const string TotalCashReturnsResourceId = "XZReportFrance_TotalCashReturns";

            /// <summary>
            /// Translation resource id for TotalCashSales value title.
            /// </summary>
            public const string TotalCashSalesResourceId = "XZReportFrance_TotalCashSales";
        }
    }
}
