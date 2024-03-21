import * as Triggers from "PosApi/Extend/Triggers/PaymentTriggers";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ClientEntities } from "PosApi/Entities";

export default class PrePaymentTrigger extends Triggers.PrePaymentTrigger {
    public execute(options: Triggers.IPrePaymentTriggerOptions): Promise<ClientEntities.ICancelable> {
        if (ObjectExtensions.isNullOrUndefined(options)) {
            // This will never happen, but is included to demonstrate how to return a rejected promise when validation fails.
            let error: ClientEntities.ExtensionError
                = new ClientEntities.ExtensionError("The options provided to the PrePaymentTrigger were invalid. Please select a product and try again.");

            return Promise.reject(error);
        } else {
            return Promise.resolve({ canceled: false });
        }
    }

}