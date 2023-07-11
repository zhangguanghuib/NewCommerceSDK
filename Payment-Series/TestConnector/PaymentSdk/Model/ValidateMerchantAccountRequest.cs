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
        /// Request to validate merchant account.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.RequestBase" />
        internal class ValidateMerchantAccountRequest : RequestBase
        {
            /// <summary>
            /// Converts <see cref="Request"/> to <see cref="ValidateMerchantAccountRequest"/>.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>An instance of  <see cref="ValidateMerchantAccountRequest"/>.</returns>
            internal static ValidateMerchantAccountRequest ConvertFrom(Request request)
            {
                var validateRequest = new ValidateMerchantAccountRequest();
                var errors = new List<PaymentError>();
                validateRequest.ReadBaseProperties(request, errors);

                foreach (PaymentProperty property in request.Properties)
                {
                    if (property.Namespace != GenericNamespace.MerchantAccount)
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format("Invalid namespace {0} for property {1}", property.Namespace, property.Name)));
                    }
                    else if (!SampleMerchantAccountProperty.ArrayList.Contains(property.Name))
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format("Invalid property {0}", property.Name)));
                    }
                }

                if (errors.Count > 0)
                {
                    throw new SampleException(errors);
                }

                return validateRequest;
            }
        }
    }
}
