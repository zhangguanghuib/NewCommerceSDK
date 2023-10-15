namespace PCEFTPOS.EFTClient.IPInterface 
{
	/// <summary>Indicates the type of receipt that has been received.</summary>
	public enum ReceiptType
	{
		/// <summary>A logon receipt was received.</summary>
		Logon = 'L',
		/// <summary>A customer transaction receipt was received.</summary>
		Customer = 'C',
		/// <summary>A merchant transaction receipt was received.</summary>
		Merchant = 'M',
		/// <summary>A settlement receipt was received. This receipt usually contains the signature receipt line and should be printed immediately.</summary>
		Settlement = 'S',
		/// <summary>Receipt text was received. Used internally by component. You should never receive this receipt type.</summary>
		ReceiptText = 'R'
	}

	/// <summary>A PC-EFTPOS receipt request object.</summary>
	public class EFTReceiptRequest : EFTRequest
	{
		/// <summary>Constructs a default display response object.</summary>
		public EFTReceiptRequest() : base(false, typeof(EFTReceiptResponse))
		{
		}
	}

	/// <summary>A PC-EFTPOS receipt response object.</summary>
	public class EFTReceiptResponse : EFTResponse
	{
		public EFTReceiptResponse() : base(typeof(EFTReceiptRequest))
		{

		}

		/// <summary>Constructs a default display response object.</summary>
		//public EFTReceiptResponse(): base(false, typeof(EFTReceiptRequest))
		//{
		//}

		/// <summary>The receipt type.</summary>
		/// <value>Type: <see cref="ReceiptType" /></value>
		public ReceiptType Type { get; set; } = ReceiptType.Customer;

		/// <summary>Receipt text to be printed.</summary>
		/// <value>Type: <see cref="System.String">String array</see></value>
		public string[] ReceiptText { get; set; } = new string[] { "" };

		/// <summary>Receipt response is a pre-print.</summary>
		/// <value>Type: <see cref="System.Boolean" /></value>
		public bool IsPrePrint { get; set; } = false;
	}
}