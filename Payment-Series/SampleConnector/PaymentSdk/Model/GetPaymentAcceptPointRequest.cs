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
        /// Request to get payment accept point.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.RequestBase" />
        internal class GetPaymentAcceptPointRequest : RequestBase
        {
            /// <summary>
            /// Converts from <see cref="Request"/>.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>An instance of <see cref="GetPaymentAcceptPointRequest"/>.</returns>
            internal static GetPaymentAcceptPointRequest ConvertFrom(Request request)
            {
                var acceptPointRequest = new GetPaymentAcceptPointRequest();
                var errors = new List<PaymentError>();
                acceptPointRequest.ReadBaseProperties(request, errors);

                if (errors.Count > 0)
                {
                    throw new SampleException(errors);
                }

                return acceptPointRequest;
            }
        }
    }
}
