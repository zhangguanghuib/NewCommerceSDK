using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTR.HardwareStation.Models;

namespace WTR.HWExt.PaymentTerminal
{
    public interface IWTR_CreditCardPaymentTerminal : IPeripheral
    {
        /// <summary>
        /// The interface to the payment terminals
        /// </summary>
        WTR_PaymentTerminalResponse makePayment(WTR_PaymentTerminalRequest _request);
        WTR_PaymentTerminalResponse voidPayment(WTR_CardVoidPaymentTerminalRequest _request);
    }

    public interface IWTR_NETSPaymentTerminal : IPeripheral
    {
        /// <summary>
        /// The interface to the payment terminals
        /// </summary>
        WTR_PaymentTerminalResponse makePayment(WTR_PaymentTerminalRequest _request);
    }

    internal enum WTR_PaymentTerminalEx
    {
        CREDITCARD = 10002,
        NETS = 10003,
        FLASHPAY = 10004,
        CONTACTLESS = 10005,
    }
}
