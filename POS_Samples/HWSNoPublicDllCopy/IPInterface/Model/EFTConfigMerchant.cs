using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>A PC-EFTPOS terminal config merchant request object.</summary>
	public class EFTConfigureMerchantRequest : EFTRequest
	{
		/// <summary>Constructs a default terminal configure request object.</summary>
		public EFTConfigureMerchantRequest() : base(true, typeof(EFTConfigureMerchantResponse))
		{
		}

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        /// <summary>The terminal ID (CatID) to configure the terminal with.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        public string Catid { get; set; } = "";

        /// <summary>The terminal ID (CatID) to configure the terminal with.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        [System.Obsolete("Please use Catid instead of TerminalID")]
        public string TerminalID { get { return Catid; } set { Catid = value; } }

        /// <summary>The merchant ID (CaID) to configure the terminal with.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        public string Caid { get; set; } = "";

        /// <summary>The merchant ID (Caid) to configure the terminal with.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        [System.Obsolete("Please use Caid instead of MerchantID")]
        public string MerchantID { get { return Caid; } set { Caid = value; } }

        /// <summary>The AIIC to configure the terminal with.</summary>
        /// <value>Type: <see cref="System.Int32" /></value>
        /// <remarks>Not support by most PIN pad terminals.</remarks>
        public int AIIC { get; set; } = 0;

		/// <summary>The NII to configure the terminal with.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		/// <remarks>Not support by most PIN pad terminals.</remarks>
		public int NII { get; set; } = 0;

        /// <summary>The bank response timeout specified in seconds.</summary>
        /// <value>Type: <see cref="System.Int32" /></value>
        /// <remarks>Not support by most PIN pad terminals.</remarks>
        public int Timeout { get; set; } = 45;

        /// <summary>Indicates where the request is to be sent to. Should normally be EFTPOS.</summary>
        /// <value>Type: <see cref="TerminalApplication"/><para>The default is <see cref="TerminalApplication.EFTPOS"/>.</para></value>
        public TerminalApplication Application { get; set; } = TerminalApplication.EFTPOS;
    }

	/// <summary>A PC-EFTPOS terminal configure response object.</summary>
	public class EFTConfigureMerchantResponse : EFTResponse
	{
		/// <summary>Constructs a default terminal configure response object.</summary>
		public EFTConfigureMerchantResponse() : base(typeof(EFTConfigureMerchantRequest))
		{
		}

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";


        /// <summary>Indicates if the request was successful.</summary>
        /// <value>Type: <see cref="System.Boolean"/></value>
        public bool Success { get; set; } = false;

        /// <summary>The response code of the request.</summary>
        /// <value>Type: <see cref="System.String"/><para>A 2 character response code. "00" indicates a successful response.</para></value>
        public string ResponseCode { get; set; } = "";

        /// <summary>The response text for the response code.</summary>
        /// <value>Type: <see cref="System.String"/></value>
        public string ResponseText { get; set; } = "";
    }
}