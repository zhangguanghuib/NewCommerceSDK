/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics
{
    namespace Retail.SampleConnector.Portable
    {
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;

        /// <summary>
        /// Response for <see cref="ValidateMerchantAccountRequest"/>.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.ResponseBase" />
        internal class ValidateMerchantAccountResponse : ResponseBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidateMerchantAccountResponse"/> class.
            /// </summary>
            /// <param name="locale">The locale.</param>
            /// <param name="serviceAccountId">The service account identifier.</param>
            /// <param name="connectorName">Name of the connector.</param>
            internal ValidateMerchantAccountResponse(string locale, string serviceAccountId, string connectorName)
                : base(locale, serviceAccountId, connectorName)
            {
            }

            /// <summary>
            /// Converts <see cref="ValidateMerchantAccountResponse"/> to <see cref="Response"/>.
            /// </summary>
            /// <param name="validateResponse">The validate response.</param>
            /// <returns>An instance of <see cref="Response"/>.</returns>
            internal static Response ConvertTo(ValidateMerchantAccountResponse validateResponse)
            {
                var response = new Response();
                validateResponse.WriteBaseProperties(response);
                return response;
            }
        }
    }
}
