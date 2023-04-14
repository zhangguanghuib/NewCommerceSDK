namespace WTR
{
    namespace HWExt.PaymentTerminal
    {
        using System;
        using System.Composition;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
        using Microsoft.Dynamics.Retail.Diagnostics;
        using WTR.HardwareStation.Models;

        [RoutePrefix("PAYMENTTERMINALS")]
        public class PaymentTerminalsController : IController
        {
            [HttpPost]
            //public WTR_PaymentTerminalResponse makeCreditCardPayment(WTR_PaymentTerminalRequest request, IEndpointContext context)
            public async Task<WTR_PaymentTerminalResponse> makeCreditCardPayment(WTR_PaymentTerminalRequest request, IEndpointContext context)
            {
                ThrowIf.Null(request, "request");
                try
                {
                    if (request.TenderType == (int)WTR_PaymentTerminalEx.CREDITCARD)
                    {
                        WTR_CreditCardPaymentTerminal terminal = new WTR_CreditCardPaymentTerminal();
                        WTR_PaymentTerminalResponse res = terminal.makePayment(request);
                        return res;
                        //return res;
                    }
                    else
                    {
                        throw new Exception("Invalid Tender Type");
                    }
                }
                catch (Exception ex)
                {
                    // Rethrow exception setting of errorResourceId to raise localized general error and logging unlocalized details.
                    RetailLogger.Log.HardwareStationActionFailure("Hardware station an exception occurred when opening a fiscal register.", ex);
                }   
                return null;
            }

            [HttpPost]
            public async Task<WTR_PaymentTerminalResponse> makeNETSPayment(WTR_PaymentTerminalRequest request, IEndpointContext context)
            {
                ThrowIf.Null(request, "request");

                try
                {
                    if (request.TenderType == (int)WTR_PaymentTerminalEx.NETS)
                    {
                        WTR_NETSPaymentTerminal terminal = new WTR_NETSPaymentTerminal();
                        WTR_PaymentTerminalResponse res = terminal.makePayment(request);
                        return res;
                        //WTR_PaymentTerminalResponse res = this.CommerceRuntime.Execute<WTR_PaymentTerminalResponse>(_request, null);
                        //return res;
                    }
                    else
                    {
                        throw new Exception("Invalid Tender Type");
                    }
                }
                catch (Exception ex)
                {
                    RetailLogger.Log.HardwareStationActionFailure("Hardware station an exception occurred when making a payment.", ex);

                    throw new Exception(ex.Message, ex);
                }
                finally
                {
                    //var closeCoinDispenserDeviceRequest = new CloseCoinDispenserDeviceRequest();
                    //this.CommerceRuntime.Execute<NullResponse>(closeCoinDispenserDeviceRequest, null);
                }
            }
        }
    }
}
