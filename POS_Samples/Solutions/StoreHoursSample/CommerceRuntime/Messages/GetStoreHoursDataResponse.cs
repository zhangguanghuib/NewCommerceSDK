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
        public sealed class GetStoreHoursDataResponse : Response
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetStoreHoursDataResponse"/> class.
            /// </summary>
            /// <param name="dayHours">The collection of store hours.</param>
            public GetStoreHoursDataResponse(PagedResult<StoreDayHours> dayHours)
            {
                this.DayHours = dayHours;
            }

            /// <summary>
            /// Gets the found store hours as a paged result set.
            /// </summary>
            [DataMember]
            public PagedResult<StoreDayHours> DayHours { get; private set; }
        }
    }
}