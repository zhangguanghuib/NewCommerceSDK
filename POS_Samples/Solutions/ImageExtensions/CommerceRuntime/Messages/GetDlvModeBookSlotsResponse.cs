using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.GasStationSample.CommerceRuntime.Messages
{
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Contoso.GasStationSample.CommerceRuntime.Entities;

    [DataContract]
    public sealed class GetDlvModeBookSlotsResponse : Response
    {
        public GetDlvModeBookSlotsResponse(PagedResult<DlvModeBookSlot> pagedResult)
        {
            this.DlvModeBookSlots = pagedResult;
        }

        [DataMember]
        public PagedResult<DlvModeBookSlot> DlvModeBookSlots { get; private set; }
    }
 
}
