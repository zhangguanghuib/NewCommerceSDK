using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>A PC-EFTPOS cloud logon request object.</summary>
    public class EFTCloudLogonRequest : EFTRequest
    {
        /// <summary>Constructs a default cloud logon request object.</summary>
        public EFTCloudLogonRequest() : base(true, typeof(EFTCloudLogonResponse))
        {
        }

        /// <summary>The cloud logon username</summary>
        /// <value>Type: <see cref="System.String" /><para>The default is ""</para></value>
        public string ClientID { get; set; } = "";

        /// <summary>The cloud logon password</summary>
        /// <value>Type: <see cref="System.String" /><para>The default is ""</para></value>
        public string Password { get; set; } = "";

        /// <summary>The cloud logon pairing code created by the pinpad</summary>
        /// <value>Type: <see cref="System.String" /><para>The default is ""</para></value>
        public string PairingCode { get; set; } = "";
    }

    /// <summary>A PC-EFTPOS cloud logon response object.</summary>
    public class EFTCloudLogonResponse : EFTResponse
    {
        /// <summary>Constructs a default terminal logon response object.</summary>
        public EFTCloudLogonResponse() : base(typeof(EFTCloudLogonRequest))
        {
        }

        /// <summary>Success flag for the call. TRUE for successful</summary>
        /// <value>Type: <see cref="System.Boolean" /></value>
        public bool Success { get; set; } = false;

        /// <summary>Response code for the call</summary>
        /// <value>Type: <see cref="System.String" /></value>
        public string ResponseCode { get; set; } = "";

        /// <summary>Response text for the call</summary>
        /// <value>Type: <see cref="System.String" /></value>
        public string ResponseText { get; set; } = "";
    }
}