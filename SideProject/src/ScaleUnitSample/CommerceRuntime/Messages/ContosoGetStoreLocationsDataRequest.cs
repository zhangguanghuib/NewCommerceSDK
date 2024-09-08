namespace CommerceRuntime.Messages
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;

    [DataContract]
    public class ContosoGetStoreLocationsDataRequest : DataRequest
    {
        public ContosoGetStoreLocationsDataRequest(QueryResultSettings settings)
        {
            this.QueryResultSettings = settings;
        }
    }
}
