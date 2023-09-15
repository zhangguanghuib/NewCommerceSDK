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
        using Commerce.Runtime.XZReportsFrance.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;

        /// <summary>
        /// Class holding France-specific shift data calculation logic.
        /// </summary>
        internal static class ShiftFranceCalculator
        {
            /// <summary>
            /// Calculates and fills the shift totals and tax lines happened in the specified shift.
            /// </summary>
            /// <param name="context">Request context.</param>
            /// <param name="shift">The shift to update.</param>
            /// <param name="shiftTerminalId">Shift terminal Identifier.</param>
            /// <param name="shiftId">Shift identifier.</param>
            /// <remark>Assume there is no totals and tax lines are calculated before. Overrides shift totals and tax lines if they exist.</remark>
            public static async Task FillShiftFranceDetailsAsync(RequestContext context, Shift shift, string shiftTerminalId, long shiftId)
            {
                var calculateRequest = new GetShiftDetailsFranceDataRequest(shiftTerminalId, shiftId);
                var shiftDetailsResponse = await context.ExecuteAsync<SingleEntityDataServiceResponse<Shift>>(calculateRequest).ConfigureAwait(false);
                var shiftDetails = shiftDetailsResponse.Entity;

                if (shiftDetails != null)
                {
                    shift.ShiftReturnsTotal = shiftDetails.ShiftReturnsTotal;
                    shift.ShiftSalesTotal = shiftDetails.ShiftSalesTotal;
                    shift.ReturnsGrandTotal = shiftDetails.ReturnsGrandTotal + shiftDetails.ShiftReturnsTotal;
                    shift.SalesGrandTotal = shiftDetails.SalesGrandTotal + shiftDetails.ShiftSalesTotal;

                    shift.TaxLines.Clear();
                    shift.TaxLines.AddRange(shiftDetails.TaxLines);
                }
            }
        }
    }
}
