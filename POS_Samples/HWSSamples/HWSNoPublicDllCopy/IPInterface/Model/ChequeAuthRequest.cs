using System; 

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>Indicates if the type of cheque authorization to perform.</summary>
	public enum ChequeType
	{
		/// <summary>Business guarantee authorization.</summary>
		BusinessGuarantee = '0',
		/// <summary>Personal guarantee authorization.</summary>
		PersonalGuarantee = '1',
		/// <summary>Personal guarantee authorization.</summary>
		PersonalAppraisal = '2',
	}

	/// <summary>A PC-EFTPOS cheque authorization request object.</summary>
	public class EFTChequeAuthRequest : EFTRequest
	{
		/// <summary>Constructs a default ChequeAuthRequest object.</summary>
		public EFTChequeAuthRequest() : base(true, typeof(EFTChequeAuthResponse))
		{
		}

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        /// <summary>The branch code of the cheque.</summary>
        /// <value>Type: <see cref="System.String"/><para>A 6-digit branch code (BSB). This is a required property for a cheque authorization request.</para></value>
        public string BranchCode { get; set; } = "";

        /// <summary>The account number of the cheque.</summary>
        /// <value>Type: <see cref="System.String"/><para>A 14-digit account number. This is a required property for a cheque authorization request.</para></value>
        public string AccountNumber { get; set; } = "";

        /// <summary>The serial number on the cheque.</summary>
        /// <value>Type: <see cref="System.String"/><para>A 14-digit serial number. This is a required property for a cheque authorization request.</para></value>
        public string SerialNumber { get; set; } = "";

        /// <summary>The cheque amount to authorize.</summary>
        /// <value>Type: <see cref="System.Decimal"/><para>This is a required property for a cheque authorization request.</para></value>
        public decimal Amount { get; set; } = 0m;

        /// <summary>Indicates if the type of cheque authorization to perform.</summary>
        /// <value>Type: <see cref="ChequeType"/><para>The default is <see cref="ChequeType.PersonalGuarantee"/>.</para></value>
        public ChequeType ChequeType { get; set; } = ChequeType.BusinessGuarantee;

        /// <summary>The reference number attached to the authorization.</summary>
        /// <value>Type: <see cref="System.String"/><para>Maximum 12 digit reference number.</para></value>
        public string ReferenceNumber { get; set; } = "";

        /// <summary>Indicates where the request is to be sent to. Should normally be EFTPOS.</summary>
        /// <value>Type: <see cref="TerminalApplication"/><para>The default is <see cref="TerminalApplication.EFTPOS"/>.</para></value>
        public TerminalApplication Application { get; set; } = TerminalApplication.EFTPOS;
    }

	/// <summary>A PC-EFTPOS cheque authorization response object.</summary>
	public class EFTChequeAuthResponse : EFTResponse
	{
		decimal amount;
		int authNumber;
		string txnRefNum;

		/// <summary>Constructs a default cheque authorization response object.</summary>
		public EFTChequeAuthResponse()
			: base(typeof(EFTChequeAuthRequest))
		{
			amount = 0;
			authNumber = 0;
			txnRefNum = "";
		}

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        /// <summary>The authorization amount for the cheque.</summary>
        /// <value>Type: <see cref="System.Decimal"/><para>Echoed from the request.</para></value>
        public decimal Amount
		{
			get { return amount; }
			set { amount = value; }
		}

		/// <summary>The authorization number for this authorization.</summary>
		/// <value>Type: <see cref="System.Int32"/><para>The authorization number returned from the cheque authorization host.</para></value>
		public int AuthNumber
		{
			get { return authNumber; }
			set { authNumber = value; }
		}

		/// <summary>The reference number attached to the authorization.</summary>
		/// <value>Type: <see cref="System.String"/><para>Echoed from the request.</para></value>
		public string ReferenceNumber
		{
			get { return txnRefNum; }
			set { txnRefNum = value; }
		}

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