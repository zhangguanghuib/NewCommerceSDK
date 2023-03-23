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
        using System.Collections.Generic;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        /// <summary>
        /// Response for <see cref="GetPaymentAcceptPointRequest"/>.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.ResponseBase" />
        internal class GetPaymentAcceptPointResponse : ResponseBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetPaymentAcceptPointResponse"/> class.
            /// </summary>
            /// <param name="locale">The locale.</param>
            /// <param name="serviceAccountId">The service account identifier.</param>
            /// <param name="connectorName">Name of the connector.</param>
            internal GetPaymentAcceptPointResponse(string locale, string serviceAccountId, string connectorName)
                : base(locale, serviceAccountId, connectorName)
            {
            }

            /// <summary>
            /// Gets or sets the payment accept URL.
            /// </summary>
            /// <value>
            /// The payment accept URL.
            /// </value>
            internal string PaymentAcceptUrl { get; set; }

            /// <summary>
            /// Gets or sets the payment accept page contents.
            /// </summary>
            /// <value>
            /// The payment accept submit URL.
            /// </value>
            internal string PaymentAcceptPageContents { get; set; }

            /// <summary>
            /// Gets or sets the payment accept submit URL.
            /// </summary>
            /// <value>
            /// The payment accept submit URL.
            /// </value>
            internal string PaymentAcceptSubmitUrl { get; set; }

            /// <summary>
            /// Gets or sets the payment accept message origin.
            /// </summary>
            /// <value>
            /// The payment accept message origin.
            /// </value>
            internal string PaymentAcceptMessageOrigin { get; set; }

            /// <summary>
            /// Converts to <see cref="Response"/>.
            /// </summary>
            /// <param name="acceptPointResponse">The accept point response.</param>
            /// <returns>An instance of <see cref="Response"/>.</returns>
            internal static Response ConvertTo(GetPaymentAcceptPointResponse acceptPointResponse)
            {
                var response = new Response();
                acceptPointResponse.WriteBaseProperties(response);

                var properties = new List<PaymentProperty>();
                if (response.Properties != null)
                {
                    properties.AddRange(response.Properties);
                }

                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.TransactionData, TransactionDataProperties.PaymentAcceptUrl, acceptPointResponse.PaymentAcceptUrl);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.TransactionData, TransactionDataProperties.PaymentAcceptSubmitUrl, acceptPointResponse.PaymentAcceptSubmitUrl);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.TransactionData, TransactionDataProperties.PaymentAcceptMessageOrigin, acceptPointResponse.PaymentAcceptMessageOrigin);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.TransactionData, TransactionDataProperties.PaymentAcceptContent, acceptPointResponse.PaymentAcceptPageContents);

                response.Properties = properties.ToArray();
                return response;
            }
        }
    }
}
