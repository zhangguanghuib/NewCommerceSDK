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
        using System;
        using System.Collections.Generic;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        /// <summary>
        /// Request to retrieve payment accept result.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.RequestBase" />
        internal class RetrievePaymentAcceptResultRequest : RequestBase
        {
            /// <summary>
            /// Converts <see cref="Request"/> to <see cref="RetrievePaymentAcceptResultRequest"/>.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>An instance of <see cref="RetrievePaymentAcceptResultRequest"/>.</returns>
            internal static RetrievePaymentAcceptResultRequest ConvertFrom(Request request)
            {
                var acceptResultRequest = new RetrievePaymentAcceptResultRequest();
                var errors = new List<PaymentError>();
                acceptResultRequest.ReadBaseProperties(request, errors);

                if (errors.Count > 0)
                {
                    throw new SampleException(errors);
                }

                return acceptResultRequest;
            }
        }
    }
}
