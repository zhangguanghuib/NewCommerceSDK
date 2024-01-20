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
        public sealed class InsertStoreDayHoursDataRequest : Request
        {
            public InsertStoreDayHoursDataRequest(StoreDayHours storeDayHours)
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