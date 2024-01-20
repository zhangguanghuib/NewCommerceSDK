namespace Contoso
{
    namespace Commerce.Runtime.StoreHoursSample.Messages
    {
        using System.Collections.ObjectModel;
        using System.Runtime.Serialization;
        using Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Defines a simple response class that holds a list of StoreHour instances.
        /// </summary>
        [DataContract]
        public sealed class DeleteStoreDayHoursDataResponse : Response
        {
            public DeleteStoreDayHoursDataResponse(long id)
            {
                this.Id = id;
            }

            /// <summary>
            /// Gets the store day hours.
            /// </summary>
            [DataMember]
            public long Id { get; private set; }
        }
    }
}