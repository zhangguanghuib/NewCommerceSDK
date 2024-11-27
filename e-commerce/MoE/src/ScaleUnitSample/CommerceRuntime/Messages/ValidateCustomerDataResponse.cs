namespace Moe
{
    namespace Commerce.Runtime.Messages
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Moe.Commerce.Runtime.DataModel;

        /// <summary>
        /// Defines a simple response class that holds a list of artifact instances.
        /// </summary>
        [DataContract]
        public sealed class ValidateCustomerDataResponse : Response
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidateCustomerDataResponse"/> class.
            /// </summary>
            /// <param name="customer">The collection of customer information.</param>
            public ValidateCustomerDataResponse(ValidCustomer validCustomer)
            {
                this.ValidCustomer = validCustomer;
            }

            /// <summary>
            /// Gets the found customer as a paged result set.
            /// </summary>
            [DataMember]
            public ValidCustomer ValidCustomer { get; private set; }
        }
    }
}
