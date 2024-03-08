namespace RSL
{
    namespace Commerce.Runtime.Extensions.Messages
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// A simple request class to send email.
        /// </summary>
        [DataContract]
        public sealed class SendEmailRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SendEmailRequest"/> class.
            /// </summary>
            /// <param name="SalesId">The Sales number.</param>
            public SendEmailRequest(string SalesId, string CustomerId, SalesOrder salesorder)
            {
                this.SalesId = SalesId;
                this.CustomerId = CustomerId;
                this.salesorder = salesorder;
            }

            public SendEmailRequest(string SalesId, string CustomerId, SalesTransaction salesorder)
            {

                this.SalesId = SalesId;
                this.CustomerId = CustomerId;
                this.salestransaction = salesorder;
            }

            /// <summary>
            /// Gets the email related to the request.
            /// </summary>
            [DataMember]
            public string SalesId { get; private set; }

            [DataMember]
            public string CustomerId { get; private set; }

            [DataMember]
            public SalesOrder salesorder { get; private set; }

            [DataMember]
            public SalesTransaction salestransaction { get; private set; }

        }
    }
}