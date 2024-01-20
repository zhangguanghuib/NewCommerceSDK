namespace Contoso
{
    namespace Commerce.Runtime.StoreHoursSample.Messages
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// A simple request class to get a list of store hours for a store.
        /// </summary>
        [DataContract]
        public sealed class GetStoreHoursDataRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetStoreHoursDataRequest"/> class.
            /// </summary>
            /// <param name="storeNumber">The store number.</param>
            public GetStoreHoursDataRequest(string storeNumber)
            {
                this.StoreNumber = storeNumber;
            }

            /// <summary>
            /// Gets the store number related to the request.
            /// </summary>
            [DataMember]
            public string StoreNumber { get; private set; }
        }
    }
}