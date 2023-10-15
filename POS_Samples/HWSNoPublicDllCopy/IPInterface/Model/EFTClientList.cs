using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>A PC-EFTPOS client list request object.</summary>
    public class EFTClientListRequest : EFTRequest
    {
        /// <summary>Constructs a default client list object.</summary>
        public EFTClientListRequest() : base(false, typeof(EFTClientListResponse))
        {
        }
    }

    /// <summary>A PC-EFTPOS terminal logon response object.</summary>
    public class EFTClientListResponse : EFTResponse
    {
        public enum EFTClientState { Available, Unavailable }

        public class EFTClient
        {
            public string Name { get; set; } = "";
            public string IPAddress { get; set; } = "";
            public int Port { get; set; } = 0;
            public EFTClientState State { get; set; } = EFTClientState.Unavailable;
        }

        /// <summary>Constructs a default EFT-Client list response object.</summary>
        public EFTClientListResponse() : base(typeof(EFTClientListRequest))
        {
        }

		public bool Success { get; set; } = true;

        public List<EFTClient> EFTClients { get; set; } = new List<EFTClient>();
    }

}
