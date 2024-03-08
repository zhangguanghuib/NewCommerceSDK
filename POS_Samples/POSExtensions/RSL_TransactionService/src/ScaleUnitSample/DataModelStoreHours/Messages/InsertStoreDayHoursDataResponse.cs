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
        public sealed class InsertStoreDayHoursDataResponse : Response
        {
            public InsertStoreDayHoursDataResponse(StoreDayHours storeDayHours)
            {
                this.StoreDayHours = storeDayHours;
            }

            /// <summary>
            /// Gets the store day hours.
            /// </summary>
            [DataMember]
            public StoreDayHours StoreDayHours { get; private set; }
        }
    }
}