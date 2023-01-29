using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WTR.HWExt.Peripherals
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("41E85D5D-C57A-4386-B722-4031D0B1E1A7")]
    public interface IDBSPayment
    {
        bool MakeDBSPayment(string amount, string COMPORT, string ECRUniqueTraceReference);
        bool MakeNetsFlashPay(string amount, string COMPORT, string ECRUniqueTraceReference);

        bool VoidDBSPayment(string amount, string COMPORT, string ECRUniqueTraceReference, string retrievalRefNo, string approvalCode, string invoiceNo);

        string ApprovalCode();
        string InvoiceNo();
        string RetrievalReferenceNumber();
        string TransactionDate();
        string TransactionTime();
        string CardHolderName();
        string EFTCardNumber();
        string EFTTerminalID();
        string TransactionInfo();
        string CardLabel();
        string OutputMessage();
        string ExpiryDate();
        string EntryMode();
        string MerchantID();
        string HostLabel();
        string EMVData();
        string Cardtype();
        string Hosttype();
        string ECRUniqueTraceReference();
        string BatchNumber();
        string AdditionalPrintingFlags();
        string ResponseByteMessage();
    }
}
