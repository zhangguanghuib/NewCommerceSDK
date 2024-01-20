using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>PC-EFTPOS key types.</summary>
    public enum EFTPOSKey
    {
        /// <summary>The OK/CANCEL key.</summary>
        OkCancel = '0',
        /// <summary>The YES/ACCEPT key.</summary>
        YesAccept = '1',
        /// <summary>The NO/DECLINE key.</summary>
        NoDecline = '2',
        /// <summary>The AUTH key.</summary>
        Authorise = '3'
    }

    /// <summary>A PC-EFTPOS client list request object.</summary>
    public class EFTSendKeyRequest : EFTRequest
    {
        /// <summary>Constructs a default client list object.</summary>
        public EFTSendKeyRequest() : base(false, null)
        {
            isStartOfTransactionRequest = false;
        }

        /// <summary> The type of key to send </summary>
        public EFTPOSKey Key { get; set; } = EFTPOSKey.OkCancel;

        /// <summary> Data entered by the POS (e.g. for an 'input entry' dialog type) </summary>
        public string Data { get; set; } = "";
    }
}
