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

        /// <summary>
        /// Sample exception class for input errors and payment errors.
        /// </summary>
        public class SampleException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SampleException"/> class.
            /// </summary>
            /// <param name="code">The error code.</param>
            /// <param name="message">The error message.</param>
            public SampleException(ErrorCode code, string message)
            {
                this.Errors = new List<PaymentError>();
                this.Errors.Add(new PaymentError(code, message));
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SampleException"/> class.
            /// </summary>
            /// <param name="errors">The errors.</param>
            public SampleException(List<PaymentError> errors)
            {
                this.Errors = errors;
            }

            /// <summary>
            /// Gets or sets the errors.
            /// </summary>
            public List<PaymentError> Errors { get; set; }
        }
    }
}
