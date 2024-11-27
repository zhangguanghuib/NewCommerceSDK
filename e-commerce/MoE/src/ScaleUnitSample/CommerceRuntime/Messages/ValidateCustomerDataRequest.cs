namespace Moe
{
    namespace Commerce.Runtime.Messages
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// A simple request class to get a list of artifacts for the item.
        /// </summary>
        [DataContract]
        public sealed class ValidateCustomerDataRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CheckValidCustomerDataRequest"/> class.
            /// </summary>
            /// <param name="emailId">The item number.</param>
            public ValidateCustomerDataRequest(string emailId)
            {
                this.EmailId = emailId;
            }

            /// <summary>
            /// Gets the item number related to the request.
            /// </summary>
            [DataMember]
            public string EmailId { get; private set; }
        }
    }
}
