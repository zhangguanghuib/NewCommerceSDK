namespace Contoso
{
    namespace Commerce.Runtime.StoreHoursSample.Messages
    {
        using System.Runtime.Serialization;
        using Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// A simple request class to get a list of store hours for a store.
        /// </summary>
        [DataContract]
        public sealed class UpdateStoreDayHoursDataRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UpdateStoreDayHoursDataRequest"/> class.
            /// </summary>
            /// <param name="storeDayHours">The store day and hours.</param>
            public UpdateStoreDayHoursDataRequest(StoreDayHours storeDayHours)
            {
                this.StoreDayHours = storeDayHours;
            }

            /// <summary>
            /// Gets the store day and hours related to the request.
            /// </summary>
            [DataMember]
            public StoreDayHours StoreDayHours { get; private set; }
        }
    }
}