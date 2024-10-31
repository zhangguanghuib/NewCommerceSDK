using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.GasStationSample.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// A simple request class.
    /// </summary>
    [DataContract]
    public sealed class GetBarCodeImageRequest : Request
    {
        public GetBarCodeImageRequest(string barCodeString)
        {
            this.BarCodeString = barCodeString;
        }

        /// <summary>
        /// Field 
        /// </summary>
        [DataMember]
        public string BarCodeString { get; private set; }
    }
}
