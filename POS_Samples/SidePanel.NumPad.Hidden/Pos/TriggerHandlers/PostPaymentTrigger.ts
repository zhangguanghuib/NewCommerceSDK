import * as Triggers from "PosApi/Extend/Triggers/PaymentTriggers";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class PostPaymentTrigger extends Triggers.PostPaymentTrigger {
    public execute(options: Triggers.IPostPaymentTriggerOptions): Promise<void> {
        console.log(" PostPaymentTrigger executed");
        let navDivs: HTMLCollectionOf<Element> = document.getElementsByClassName("navDiv");
        if (!ObjectExtensions.isNullOrUndefined(navDivs) && navDivs.length > 0) {
            let navDiv = navDivs[0] as any;
            navDiv.disabled = true;
            navDiv.style.visibility = "visible";
        }
        return Promise.resolve();
    }

}