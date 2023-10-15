using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
	/// <summary>Indicates the requested status type.</summary>
	public enum StatusType
	{
		/// <summary>Request the EFT status from the PIN pad.</summary>
		Standard = '0',
		/// <summary>Not supported by all PIN pads.</summary>
		TerminalAppInfo = '1',
		/// <summary>Not supported by all PIN pads.</summary>
		AppCPAT = '2',
		/// <summary>Not supported by all PIN pads.</summary>
		AppNameTable = '3',
		/// <summary>Undefined</summary>
		Undefined = '4',
		/// <summary>Not supported by all PIN pads.</summary>
		PreSwipe = '5'
	}

	/// <summary>Indicates the EFT terminal hardware type.</summary>
	public enum EFTTerminalType
	{
		/// <summary>CBA Albert PIN pad terminal</summary>
		Albert,
		/// <summary>Ingenico NPT 710 PIN pad terminal.</summary>
		IngenicoNPT710,
		/// <summary>Ingenico NPT PX328 PIN pad terminal.</summary>
		IngenicoPX328,
		/// <summary>Ingenico NPT i5110 PIN pad terminal.</summary>
		Ingenicoi5110,
		/// <summary>Ingenico NPT i3070 PIN pad terminal.</summary>
		Ingenicoi3070,
		/// <summary>Ingenico Move5000 PIN pad terminal</summary>
		IngenicoMove5000,
		/// <summary>Ingenico IWL or ICT 250 PIN pad terminal</summary>
		IngenicoIxx250,
		/// <summary>Sagem PIN pad terminal.</summary>
		Sagem,
		/// <summary>Verifone PIN pad terminal.</summary>
		Verifone,
		/// <summary>Verifone Vx690 PIN pad terminal</summary>
		VerifoneVx690,
		/// <summary>Verifone Vx820 PIN pad terminal</summary>
		VerifoneVx820,
		/// <summary>Keycorp PIN pad terminal.</summary>
		Keycorp,
		/// <summary>PCEFTPOS development Virtual Pinpad</summary>
		PCEFTPOSVirtualPinpad,
		/// <summary>Unknown PIN pad terminal.</summary>
		Unknown
	}

	/// <summary>PIN pad terminal supported options.</summary>
	[Flags]
	public enum PINPadOptionFlags
	{
		/// <summary>Tipping enabled flag.</summary>
		Tipping = 0x0001,
		/// <summary>Pre-athourization enabled flag.</summary>
		PreAuth = 0x0002,
		/// <summary>Completions enabled flag.</summary>
		Completions = 0x0004,
		/// <summary>Cash-out enabled flag.</summary>
		CashOut = 0x0008,
		/// <summary>Refund enabled flag.</summary>
		Refund = 0x0010,
		/// <summary>Balance enquiry enabled flag.</summary>
		Balance = 0x0020,
		/// <summary>Deposit enabled flag.</summary>
		Deposit = 0x0040,
		/// <summary>Manual voucher enabled flag.</summary>
		Voucher = 0x0080,
		/// <summary>Mail-order/Telephone-order enabled flag.</summary>
		MOTO = 0x0100,
		/// <summary>Auto-completions enabled flag.</summary>
		AutoCompletion = 0x0200,
		/// <summary>Electronic Fallback enabled flag.</summary>
		EFB = 0x0400,
		/// <summary>EMV enabled flag.</summary>
		EMV = 0x0800,
		/// <summary>Training mode enabled flag.</summary>
		Training = 0x1000,
		/// <summary>Withdrawal enabled flag.</summary>
		Withdrawal = 0x2000,
		/// <summary>Funds transfer enabled flag.</summary>
		Transfer = 0x4000,
		/// <summary>Start cash enabled flag.</summary>
		StartCash = 0x8000
	}

	/// <summary>PIN pad terminal key handling scheme.</summary>
	public enum KeyHandlingType
	{
		/// <summary>Single-DES encryption standard.</summary>
		SingleDES = '0',
		/// <summary>Triple-DES encryption standard.</summary>
		TripleDES = '1',
		/// <summary>Unknown encryption standard.</summary>
		Unknown
	}

	/// <summary>PIN pad terminal network option.</summary>
	public enum NetworkType
	{
		/// <summary>Leased line bank connection.</summary>
		Leased = '1',
		/// <summary>Dial-up bank connection.</summary>
		Dialup = '2',
		/// <summary>Unknown bank connection.</summary>
		Unknown
	}

	/// <summary>PIN pad terminal communication option.</summary>
	public enum TerminalCommsType
	{
		/// <summary>Cable link communications.</summary>
		Cable = '0',
		/// <summary>Intrared link communications.</summary>
		Infrared = '1',
		/// <summary>Unknown link communications.</summary>
		Unknown
	}

	/// <summary>A PC-EFTPOS terminal status request object.</summary>
	public class EFTStatusRequest : EFTRequest
	{
		/// <summary>Constructs a default terminal status request object.</summary>
		public EFTStatusRequest() : base(true, typeof(EFTStatusResponse))
		{
		}

		/// <summary>Two digit merchant code</summary>
		/// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
		public string Merchant { get; set; } = "00";

		/// <summary>Type of status to perform.</summary>
		/// <value>Type: <see cref="StatusType"/><para>The default is <see cref="StatusType.Standard" />.</para></value>
		public StatusType StatusType { get; set; } = StatusType.Standard;

		/// <summary>Indicates where the request is to be sent to. Should normally be EFTPOS.</summary>
		/// <value>Type: <see cref="TerminalApplication"/><para>The default is <see cref="TerminalApplication.EFTPOS"/>.</para></value>
		public TerminalApplication Application { get; set; } = TerminalApplication.EFTPOS;
	}

	/// <summary>A PC-EFTPOS terminal status response object.</summary>
	public class EFTStatusResponse : EFTResponse
	{
		/// <summary>Constructs a default terminal status response object.</summary>
		public EFTStatusResponse()
			: base(typeof(EFTStatusRequest))
		{
		}

		/// <summary>Two digit merchant code</summary>
		/// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
		public string Merchant { get; set; } = "00";

		/// <summary>The AIIC that is configured in the terminal.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string AIIC { get; set; }

		/// <summary>The NII that is configured in the terminal.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int NII { get; set; }

		/// <summary>Terminal ID configured in the PIN pad.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string Catid { get; set; } = "";

		/// <summary>Terminal ID configured in the PIN pad.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		[System.Obsolete("Please use Catid instead of TerminalID")]
		public string TerminalID { get { return Catid; } set { Catid = value; } }

		/// <summary>Merchant ID configured in the PIN pad.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string Caid { get; set; } = "";

		/// <summary>Merchant ID configured in the PIN pad.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		[System.Obsolete("Please use Caid instead of MerchantID")]
		public string MerchantID { get { return Caid; } set { Caid = value; } }

		/// <summary>The bank response timeout that is configured in the terminal.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int Timeout { get; set; } = 0;

		/// <summary>Indicates if the PIN pad is currently logged on.</summary>
		/// <value>Type: <see cref="System.Boolean" /></value>
		public bool LoggedOn { get; set; } = false;

		/// <summary>The serial number of the terminal.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string PinPadSerialNumber { get; set; } = "";

		/// <summary>The serial number of the terminal.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		[System.Obsolete("Please use PinPadSerialNumber instead of PINPadSerialNumber")]
		public string PINPadSerialNumber { get { return PinPadSerialNumber; } set { PinPadSerialNumber = value; } }

		/// <summary>PIN pad software version.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string PinPadVersion { get; set; } = "";

		/// <summary>PIN pad software version.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		[System.Obsolete("Please use PinPadVersion instead of PINPadVersion")]
		public string PINPadVersion { get { return PinPadVersion; } set { PinPadVersion = value; } }

		/// <summary>The bank description.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string BankDescription { get; set; } = "";

		/// <summary>Key verification code.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string KVC { get; set; } = "";

		/// <summary>Current number of stored transactions.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int SAFCount { get; set; } = 0;

		/// <summary>The acquirer communications type.</summary>
		/// <value>Type: <see cref="NetworkType" /></value>
		public NetworkType NetworkType { get; set; } = NetworkType.Unknown;

		/// <summary>The hardware serial number.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string HardwareSerial { get; set; } = "";

		/// <summary>The merchant retailer name.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string RetailerName { get; set; } = "";

		/// <summary>PIN pad terminal supported options flags.</summary>
		/// <value>Type: <see cref="PINPadOptionFlags" /></value>
		public PINPadOptionFlags OptionsFlags { get; set; } = 0;

		/// <summary>Store-and forward credit limit.</summary>
		/// <value>Type: <see cref="System.Decimal" /></value>
		public decimal SAFCreditLimit { get; set; } = 0;

		/// <summary>Store-and-forward debit limit.</summary>
		/// <value>Type: <see cref="System.Decimal" /></value>
		public decimal SAFDebitLimit { get; set; } = 0;

		/// <summary>The maximum number of store transactions.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int MaxSAF { get; set; } = 0;

		/// <summary>The terminal key handling scheme.</summary>
		/// <value>Type: <see cref="KeyHandlingType" /></value>
		public KeyHandlingType KeyHandlingScheme { get; set; } = KeyHandlingType.Unknown;

		/// <summary>The maximum cash out limit.</summary>
		/// <value>Type: <see cref="System.Decimal" /></value>
		public decimal CashoutLimit { get; set; } = 0;

		/// <summary>The maximum refund limit.</summary>
		/// <value>Type: <see cref="System.Decimal" /></value>
		public decimal RefundLimit { get; set; } = 0;

		/// <summary>Card prefix table version.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string CPATVersion { get; set; } = "";

		/// <summary>Card name table version.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string NameTableVersion { get; set; } = "";

		/// <summary>The terminal to PC communication type.</summary>
		/// <value>Type: <see cref="TerminalCommsType" /></value>
		public TerminalCommsType TerminalCommsType { get; set; } = TerminalCommsType.Unknown;

		/// <summary>Number of card mis-reads.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int CardMisreadCount { get; set; } = 0;

		/// <summary>Number of memory pages in the PIN pad terminal.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int TotalMemoryInTerminal { get; set; } = 0;

		/// <summary>Number of free memory pages in the PIN pad terminal.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int FreeMemoryInTerminal { get; set; } = 0;

		/// <summary>The type of PIN pad terminal.</summary>
		/// <value>Type: <see cref="EFTTerminalType" /></value>
		public EFTTerminalType EFTTerminalType { get; set; } = EFTTerminalType.Unknown;

		/// <summary>Number of applications in the terminal.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int NumAppsInTerminal { get; set; } = 0;

		/// <summary>Number of available display line on the terminal.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int NumLinesOnDisplay { get; set; } = 0;

		/// <summary>The date the hardware was incepted.</summary>
		/// <value>Type: <see cref="System.DateTime" /></value>
		public DateTime HardwareInceptionDate { get; set; } = DateTime.MinValue;

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