import * as Triggers from "PosApi/Extend/Triggers/ProductTriggers";
import { ClientEntities } from "PosApi/Entities";

export default class DXC_PrePriceOverrideTrigger extends Triggers.PrePriceOverrideTrigger {

    public execute(options: Triggers.IPrePriceOverrideTriggerOptions): Promise<ClientEntities.ICancelable> {
        //return Promise.resolve({ canceled: true });
        if (options.cartLinePrices.length > 0) {
            return Promise.reject(new ClientEntities.ExtensionError("Not allow price overide"));
        }
        else
        {
            return Promise.resolve({ canceled: false });
        }
    }
}