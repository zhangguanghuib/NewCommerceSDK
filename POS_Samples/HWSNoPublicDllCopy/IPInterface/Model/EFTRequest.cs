using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
	/// <summary> 
	/// Receipt mode (pos, windows or pinpad printer).
	/// Sometimes called the "ReceiptAutoPrint" flag
	/// </summary>
	public enum ReceiptPrintModeType
	{
		/// <summary> Receipts will be passed back to the POS in the PrintReceipt event </summary>
		POSPrinter = '0',
		/// <summary> The EFT-Client will attempt to print using the printer configured in the EFT-Client (Windows only) </summary>
		EFTClientPrinter = '1',
		/// <summary> Receipts will be printed using the pinpad printer </summary>
		PinpadPrinter = '9',
		/// <summary> Merchant receipts print on internal printer, all other print on POS </summary>
		MerchantInternalPOSPrinter = '7',
		/// <summary> Merchant receipts print on internal printer, all other print using the printer configured in the EFT-Client (Windows only) </summary>
		MerchantInternalEFTClientPrinter = '8'
	}

	/// <summary> 
	/// Receipt cut mode (cut or don't cut). Used when the EFT-Client is handling receipts (ReceiptPrintMode = ReceiptPrintModeType.EFTClientPrinter)
	/// Sometimes called the "CutReceipt" flag
	/// </summary>
	public enum ReceiptCutModeType
	{
		/// <summary> Don't cut receipts </summary>
		DontCut = '0',
		/// <summary> Cut receipts </summary>
		Cut = '1'
	}

	/// <summary>Abstract base class for EFT client requests.</summary>
	public abstract class EFTRequest
	{
		protected bool isStartOfTransactionRequest = true;
		protected Type pairedResponseType = null;

		private EFTRequest()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isStartOfTransactionRequest"></param>
		/// <param name="pairedResponseType"></param>
		public EFTRequest(bool isStartOfTransactionRequest, Type pairedResponseType)
		{
			if (pairedResponseType != null && pairedResponseType.IsSubclassOf(typeof(EFTResponse)) != true)
			{
				throw new InvalidOperationException("pairedResponseType must be based on EFTResponse");
			}

			this.isStartOfTransactionRequest = isStartOfTransactionRequest;
			this.pairedResponseType = pairedResponseType;
		}

		/// <summary>
		/// True if this request starts a paired transaction request/response with displays etc (i.e. transaction, logon, settlement etc)
		/// </summary>
		public virtual bool GetIsStartOfTransactionRequest() { return isStartOfTransactionRequest; }

		/// <summary>
		/// Indicates the paired EFTResponse for this EFTRequest if one exists. Null otherwise.
		/// e.g. EFTLogonRequest will have a paired EFTLogonResponse response
		/// </summary>
		public virtual Type GetPairedResponseType() { return pairedResponseType; }
	}

	/// <summary>Abstract base class for EFT client responses.</summary>
	public abstract class EFTResponse
	{
		protected Type pairedRequestType = null;

		/// <summary>
		/// Hidden default constructor
		/// </summary>
		private EFTResponse()
		{

		}

		public EFTResponse(Type pairedRequestType)
		{
			if (pairedRequestType != null && pairedRequestType.IsSubclassOf(typeof(EFTRequest)) != true)
			{
				throw new InvalidOperationException("pairedRequestType must be based on EFTRequest");
			}

			this.pairedRequestType = pairedRequestType;
		}

		/// <summary>
		/// Indicates the paired EFTRequest for this EFTResponse if one exists. Null otherwise.
		/// e.g. EFTLogonResponse will have a paired EFTLogonRequest request
		/// </summary>
		public virtual Type GetPairedRequestType() { return pairedRequestType; }
	}
}
