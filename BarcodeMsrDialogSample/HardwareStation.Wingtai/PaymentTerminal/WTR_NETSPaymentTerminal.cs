using WTR.HardwareStation.Models;
using WTR.HWExt.Peripherals;
using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;
using System;
using System.ComponentModel.Composition;

namespace WTR.HWExt.PaymentTerminal
{
    [Export(PeripheralType.Windows, typeof(IWTR_NETSPaymentTerminal))]
    public class WTR_NETSPaymentTerminal : IWTR_NETSPaymentTerminal
    {
        private readonly object asyncLock = new object();

        public string PaymentTerminalName { get; set; }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
            }
        }

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
            return this.makePayment(new WTR_NETSPaymentTerminalRequest(_request));
        }

        public WTR_PaymentTerminalResponse makePayment(WTR_NETSPaymentTerminalRequest _request)
        {
            WTR_PaymentTerminalResponse response;

            switch (_request.TenderType)
            {
                case (int)WTR_PaymentTerminalEx.NETS: // NETS
                    _request.CardActionType = "FC28";
                    response = this.NETSPayment(_request);
                    break;
                case (int)WTR_PaymentTerminalEx.FLASHPAY: //"FLASHPAY": //Should be contact less debit
                    _request.CardActionType = "FC24";
                    response = this.NETSPayment(_request);
                    break;
                default:
                    response = new WTR_PaymentTerminalResponse();
                    response.ResponseText = "Invalid Tender type " + _request.TenderType;
                    break;
            }

            return response;
        }

        private WTR_PaymentTerminalResponse NETSPayment(WTR_PaymentTerminalRequest _request)
        {
            NETS nets;
            //float amount = float.Parse(_request.Amount) * 100;

            int amount = Convert.ToInt32(float.Parse(_request.Amount) * 100);

            switch (_request.CardActionType)
            {
                case "FC24":
                    nets = new NETS_FC24_NETSContactlessDebit();
                    nets.PaymentProcess(amount.ToString(), "123456789", _request.COMPort);
                    return this.constructNETSPaymentTerminalResponse(nets);
                case "FC28":
                    nets = new NETS_FC28_NETSPurchase();
                    nets.PaymentProcess(amount.ToString(), "123456789", _request.COMPort);
                    return this.constructNETSPaymentTerminalResponse(nets);
                case "FC80":
                    nets = new NETS_FC80_NETSLogOn();
                    nets.PaymentProcess(amount.ToString(), "123456789", _request.COMPort);
                    return this.constructNETSPaymentTerminalResponse(nets);
                case "FC81":
                    nets = new NETS_FC81_NETSSettlement();
                    nets.PaymentProcess(amount.ToString(), "123456789", _request.COMPort);
                    return this.constructNETSPaymentTerminalResponse(nets);
                case "FC55":
                    nets = new NETS_FC55_NETSTerminalStatus();
                    nets.PaymentProcess(amount.ToString(), "123456789", _request.COMPort);
                    return this.constructNETSPaymentTerminalResponse(nets);
                default:
                    WTR_PaymentTerminalResponse response = new WTR_PaymentTerminalResponse();
                    response.ResponseText = "Invalid card action type " + _request.CardActionType;
                    return response;
            }
        }


        #region construct response

        private WTR_PaymentTerminalResponse constructNETSPaymentTerminalResponse(NETS _nets)
        {
            WTR_PaymentTerminalResponse response = new WTR_PaymentTerminalResponse();
            response.AdditionalTransInfo = _nets.getResponse().AdditionalTransInfo;
            response.ApprovalCode = _nets.getResponse().ApprovalCode;
            response.BatchNumber = _nets.getResponse().BatchNumber;
            //response.CardBalanceAmount = _nets.getResponse().CardBalanceAmount;
            response.CardIssuerName = _nets.getResponse().CardIssuerName;
            //response.CardNumber = _nets.getResponse().CardNumber;
            response.CashbackServiceFee = _nets.getResponse().CashbackServiceFee;
            response.CashCardCAN = _nets.getResponse().CashCardCAN;
            response.CEPASVersion = _nets.getResponse().CEPASVersion;
            //response.DiscountAmount = _nets.getResponse().DiscountAmount;
            response.ExpiryDate = _nets.getResponse().ExpiryDate;
            response.FeeDueFromMerchant = _nets.getResponse().FeeDueFromMerchant;
            response.FeeDueToMerchant = _nets.getResponse().FeeDueToMerchant;
            response.ForeignAmount = _nets.getResponse().ForeignAmount;
            response.ForeignMID = _nets.getResponse().ForeignMID;
            response.MerchantId = _nets.getResponse().MerchantId;
            response.MerchantNameAddress = _nets.getResponse().MerchantNameAddress;
            response.PurchaseFee = _nets.getResponse().PurchaseFee;
            response.ResponseCode = _nets.getResponse().ResponseCode;
            response.ResponseMessage1 = _nets.getResponse().ResponseMessage1;
            response.ResponseMessage2 = _nets.getResponse().ResponseMessage2;
            response.ResponseText = _nets.getResponse().ResponseText;
            response.RetrievalRefNumber = _nets.getResponse().RetrievalRefNumber;
            //response.SettlementTimeStamp = _nets.getResponse().SettlementTimeStamp;
            response.Stan = _nets.getResponse().Stan;
            response.TerminalID = _nets.getResponse().TerminalID;
            response.TotalFee = _nets.getResponse().TotalFee;
            response.TransactionAmount = _nets.getResponse().TransactionAmount;
            response.TransactionDate = _nets.getResponse().TransactionDate;
            response.TransactionTime = _nets.getResponse().TransactionTime;
            response.TransData = _nets.getResponse().TransData;

            return response;
        }



        #endregion construct response
    }
}
