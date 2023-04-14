import * as Triggers from "PosApi/Extend/Triggers/PaymentTriggers";
import { ProxyEntities, ClientEntities } from "PosApi/Entities";
import ICreditCardPaymentTerminal from "./../Peripherals/HardwareStation/CreditCardPaymentTerminal";
import CreditCardPaymentTerminal from "./../Peripherals/HardwareStation/CreditCardPaymentTerminal";
import { IWTR_PaymentTerminalEntity } from "WTRPayment/Peripherals/IPaymentTerminal";

export default class PreVoidPaymentTrigger extends Triggers.PreVoidPaymentTrigger {

    /**
     * Executes the trigger functionality.
     * @param {Triggers.IPreVoidPaymentTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IPreVoidPaymentTriggerOptions): Promise<ClientEntities.ICancelable> {
        this.context.logger.logVerbose("Executing PreVoidPaymentTrigger with options " + JSON.stringify(options) + " at " + new Date().getTime() + ".");
        let tenderLine: ProxyEntities.TenderLine = options.tenderLines[0];

        let comPort: string = "COM2"; //TODO need to pull from hardware profile
      let isManualProperty: ProxyEntities.CommerceProperty =
            Commerce.ArrayExtensions.firstOrUndefined(tenderLine.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                return property.Key === "ISMANUAL";
            });

        let tenderTypeProperty: ProxyEntities.CommerceProperty =
            Commerce.ArrayExtensions.firstOrUndefined(tenderLine.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                return property.Key === "SELECTEDTENDERTYPE";
            });

        let selectedTenderType: string = Commerce.StringExtensions.EMPTY;//
        let isManual: string = Commerce.StringExtensions.EMPTY;
        if (!Commerce.ObjectExtensions.isNullOrUndefined(tenderTypeProperty)) {
            selectedTenderType = tenderTypeProperty.Value.StringValue;
            isManual = isManualProperty.Value.StringValue;

            if (selectedTenderType === "10002" && isManual === "0") //if card was captured via auto integration, try to void card payment through payment terminal
            {
                let invoiceNumberProperty: ProxyEntities.CommerceProperty =
                    Commerce.ArrayExtensions.firstOrUndefined(tenderLine.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                        return property.Key === "INVOICENUMBER";
                    });

                let invoiceNumber: string = invoiceNumberProperty.Value.StringValue;
                let creditCardPaymentTerminal: ICreditCardPaymentTerminal = new CreditCardPaymentTerminal(this.context.runtime);
                return creditCardPaymentTerminal.voidPayment(comPort, tenderLine.AmountInTenderedCurrency, Number(selectedTenderType), invoiceNumber, this.context)
                    .then((result: IWTR_PaymentTerminalEntity): Promise<ClientEntities.ICancelable> => {
                    if (result.ResponseCode != "1")
                        return Promise.resolve({ canceled: true, data: result });
                    else
                        return Promise.resolve({ canceled: false, data: null });
                });
            }
            else
                return Promise.resolve({ canceled: false });
        }
        else
            return Promise.resolve({ canceled: false });
    }
}