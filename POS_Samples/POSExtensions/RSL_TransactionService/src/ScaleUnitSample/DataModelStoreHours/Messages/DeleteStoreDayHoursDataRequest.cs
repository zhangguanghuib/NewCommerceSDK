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
        public sealed class DeleteStoreDayHoursDataRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UpdateStoreDayHoursDataRequest"/> class.
            /// </summary>
            /// <param name="storeDayHours">The store day and hours.</param>
            public DeleteStoreDayHoursDataRequest(long id)
            {
                this.Id = id;
            }

            /// <summary>
            /// Gets the store day and hours related to the request.
            /// </summary>
            [DataMember]
            public long Id { get; private set; }
        }
    }
}