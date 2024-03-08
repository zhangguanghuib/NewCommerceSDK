namespace RSL
{
    namespace Commerce.Runtime.Extensions.Messages
    {
        using System.Collections.ObjectModel;
        using System.Runtime.Serialization;
        //using Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Defines a simple response class that to send email.
        /// </summary>
        [DataContract]
        public sealed class SendEmailResponse : Response
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SendEmailResponse"/> class.
            /// </summary>
            /// <param name="EmailSend">The collection of tickets.</param>
            public SendEmailResponse(bool EmailSend)
            {
                this.EmailSend = EmailSend;
            }

            /// <summary>
            /// Indication of whether the send Email.
            /// </summary>
            public bool EmailSend { get; private set; }
        }
    }
}