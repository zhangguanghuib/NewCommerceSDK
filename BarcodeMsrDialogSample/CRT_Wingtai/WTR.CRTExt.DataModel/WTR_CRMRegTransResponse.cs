using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WTR.CRT.DataModel
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System;
    using WTR.CRT.DataModel;
    [DataContract]
    public class TransItem
    {

        [DataMember(Name = "Identifier")]
        public string Identifier { get; set; }
    }
}
