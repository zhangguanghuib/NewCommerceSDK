import { PreCustomerAddTrigger, IPreCustomerAddTriggerOptions } from "PosApi/Extend/Triggers/CustomerTriggers";
import { ClientEntities } from "PosApi/Entities";

export default class CnlCAPreCustomerSearchTrigger extends PreCustomerAddTrigger {

    public execute(options: IPreCustomerAddTriggerOptions): Promise<ClientEntities.ICancelable> {
        console.log("before executing Pre Customer add Trigger");

        this.context.navigator.navigate("CnlCACustomerRegistrationView", options);

        return Promise.resolve({ canceled: false });
    }
}