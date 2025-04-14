using System;
using System.Collections.Generic;
using System.Text;

namespace Contoso.StoreCommercePackagingSample.HardwareStation.Messages
{
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System.Runtime.Serialization;


    [DataContract]
    public class ExportInventoryDocumentLinesRequest
    {
        /// <summary>
        /// Gets or sets the encoded binary file.
        /// </summary>
        [DataMember]
        public InventoryInboundOutboundSourceDocumentLine[] Lines { get; private set; }
        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        [DataMember]
        public string FileName { get; set; }
    }
}
