namespace Microsoft.Dynamics
{
    namespace Retail.SampleConnector.Portable
    {
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using System;
        using System.Collections.Generic;

        public class SampleException: Exception
        {
            public SampleException(ErrorCode code, string message)
            {
                this.Errors = new List<PaymentError>();
                this.Errors.Add(new PaymentError(code, message));
            }

            public SampleException(List<PaymentError> errors)
            {
                this.Errors = errors;
            }

            public List<PaymentError> Errors { get; set; }
        }
    }
}
