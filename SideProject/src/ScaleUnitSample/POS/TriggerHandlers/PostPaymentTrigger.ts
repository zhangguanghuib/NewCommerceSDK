import * as Triggers from "PosApi/Extend/Triggers/PaymentTriggers"

export default class PostPaymentTrigger extends Triggers.PostPaymentTrigger {

    public execute(options: Triggers.IPostPaymentTriggerOptions): Promise<void> {
        console.log("PostPaymentTrigger" + JSON.stringify(options));
        return Promise.resolve();
    }


}