using System;

namespace WTR.HWExt.PaymentTerminal
{
    using WTR.HardwareStation.Models;
    using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;
    using System.ComponentModel.Composition;
    using Peripherals;

    [Export(PeripheralType.Windows, typeof(IWTR_CreditCardPaymentTerminal))]
    public class WTR_CreditCardPaymentTerminal :IWTR_CreditCardPaymentTerminal 
    {
        private readonly object asyncLock = new object();

        public string PaymentTerminalName { get; set; }

      
        public void Open(string peripheralName, PeripheralConfiguration peripheralConfig)
        {
            this.PaymentTerminalName = peripheralName;
            this.Open(peripheralName);
        }
        /// <summary>
        /// Opens a peripheral.
        /// </summary>
        /// <param name="peripheralName">Name of the peripheral.</param>
        private void Open(string peripheralName)
        {

        }

        public void Close()
        {

        }

        public WTR_PaymentTerminalResponse makePayment(WTR_PaymentTerminalRequest _request)
        {
            return this.makePayment(new WTR_CreditCardPaymentTerminalRequest(_request));
        }

        public WTR_PaymentTerminalResponse makePayment(WTR_CreditCardPaymentTerminalRequest _request)
        {
            WTR_PaymentTerminalResponse response;

            switch (_request.TenderType)
            {

                case (int) WTR_PaymentTerminalEx.CREDITCARD: // "CREDITCARD":
                    response = this.CREDITCARDPayment(_request);
                    break;
                case (int)WTR_PaymentTerminalEx.CONTACTLESS: //"CONTACTLESS": //ContactLess Credit Card,  Apple, Samsung, Android
                    _request.IsContactLess = true;
                    response = this.CREDITCARDPayment(_request);
                    break;
                default:
                    response = new WTR_PaymentTerminalResponse();
                    response.ResponseText = "Invalid Tender type " + _request.TenderType;
                    break;
            }

            return response;
        }


        private WTR_PaymentTerminalResponse CREDITCARDPayment(WTR_CreditCardPaymentTerminalRequest _request)
        {
            IngenicoPaymentDevice id = new IngenicoPaymentDevice();

            // PROD Change
            //float amount = float.Parse(_request.Amount) * 100; //To convert to right amount to be charged

            int amount = Convert.ToInt32(float.Parse(_request.Amount) * 100);
            if (amount > 0)
                id.SendSaleCommand(_request.COMPort, amount.ToString(), _request.IsContactLess);
            else
                id.SendRefundCommand(_request.COMPort, amount.ToString());

            WTR_PaymentTerminalResponse resp = new WTR_PaymentTerminalResponse();
            resp.ApprovalCode = id.ApprovalCode;
            resp.MerchantId = id.MerchantID;
            resp.TerminalID = id.TerminalID;
            resp.ResponseCode = id.ResponseCode;
            resp.ResponseMessage1 = id.ResponseMessage;
            resp.RetrievalRefNumber = id.RetrievalReferenceNumber;
            resp.BatchNumber = id.BatchNumber;
            resp.CardNumberMasked = (id.CardNumberMasked!=null? id.CardNumberMasked.Replace('X','*') : null);
            resp.TransactionAmount = id.Amount;
            resp.TransactionDate = id.TransactionDate;
            resp.CardType = id.CardType;
            resp.EntryMode = id.EntryMode;
            resp.CardLabel = id.CardLabel;
            resp.TVRTSI = id.TVRTSI;
            resp.AID = id.AID;
            resp.ApplicationLabel = id.ApplicationLabel;
            resp.TC = id.TC;
            resp.InvoiceNumber = id.InvoiceNumber;
            resp.ExpiryDate = id.ExpDate;
            // WTR RAD
            resp.ResponseText = id.ResponseMessage;

            if (id.ResponseCode == null && id.ResponseMessage == null)
            {
                resp.ResponseText = "PLEASE CHECK THE TERMINAL / COM PORT CONNECTION";
            }
            else
            {
                if (id.ResponseCode != null && id.ResponseMessage == ":")
                {
                    resp.ResponseText = "PLEASE USE VALID CARD AND TRY AGAIN";
                }

            }
            //END

            return resp;
        }

        public WTR_PaymentTerminalResponse voidPayment(WTR_CardVoidPaymentTerminalRequest _request)
        {
            var response = new WTR_PaymentTerminalResponse();

            IngenicoPaymentDevice IngenicoPayments = new IngenicoPaymentDevice();
            string reply = IngenicoPayments.SendVoidCommand(_request.COMPort, _request.InvoiceNumber);
            if (reply != null && reply.Trim().ToString() != string.Empty)
            {
                response.ResponseCode = IngenicoPayments.ResponseCode;
            }

            return response;
        }
    }
}

