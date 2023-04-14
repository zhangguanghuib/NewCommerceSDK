/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace WTR.HardwareStation.Models
{
    using Microsoft.Dynamics.Commerce.HardwareStation.Models;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using System.Runtime.Serialization;

    /// <summary>
    /// The payment terminal controller request class.
    /// </summary>
    [DataContract]
    public class WTR_PaymentTerminalRequest : Request
    {

        [DataMember]
        public string COMPort { get; set; }

        [DataMember]
        public string Amount { get; set; }

        [DataMember]
        public int TenderType { get; set; }

        public bool IsContactLess { get; set; }

        public string CardActionType { get; internal set; }
    }

    [DataContract]
    public sealed class WTR_CreditCardPaymentTerminalRequest : WTR_PaymentTerminalRequest
    {
        public WTR_CreditCardPaymentTerminalRequest(WTR_PaymentTerminalRequest _request)
        {
            this.Amount = _request.Amount;
            this.CardActionType = _request.CardActionType;
            this.COMPort = _request.COMPort;
            this.IsContactLess = _request.IsContactLess;
            this.TenderType = _request.TenderType;
        }
    }

    [DataContract]
    public sealed class WTR_NETSPaymentTerminalRequest : WTR_PaymentTerminalRequest
    {
        public WTR_NETSPaymentTerminalRequest(WTR_PaymentTerminalRequest _request)
        {
            this.Amount = _request.Amount;
            this.CardActionType = _request.CardActionType;
            this.COMPort = _request.COMPort;
            this.IsContactLess = _request.IsContactLess;
            this.TenderType = _request.TenderType;
        }
    }

    [DataContract]
    public sealed class WTR_CardVoidPaymentTerminalRequest: WTR_PaymentTerminalRequest
    {
        [DataMember]
        public string InvoiceNumber { get; set; }
    }
}