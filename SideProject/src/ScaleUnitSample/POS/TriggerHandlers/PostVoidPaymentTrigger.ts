import * as Triggers from "PosApi/Extend/Triggers/PaymentTriggers"
export default class PostVoidPaymentTrigger extends Triggers.PostVoidPaymentTrigger {

    public execute(options: Triggers.IPostVoidPaymentTriggerOptions): Promise<void> {
        console.log("PostVoidPaymentTrigger: " + JSON.stringify(options));
        return Promise.resolve();
    }
}
