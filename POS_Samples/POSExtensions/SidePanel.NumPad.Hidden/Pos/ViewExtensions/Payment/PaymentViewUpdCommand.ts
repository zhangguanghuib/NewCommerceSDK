import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as PaymentView from "PosApi/Extend/Views/PaymentView";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class PaymentViewUpdCommand extends PaymentView.PaymentViewExtensionCommandBase {

    constructor(context: IExtensionCommandContext<PaymentView.IPaymentViewToExtensionCommandMessageTypeMap>) {
        super(context);
        this.label = "";
        this.id = "";
        this.extraClass = "";
    }

    protected init(state: PaymentView.IPaymentViewExtensionCommandState): void {

        let numPadDivs: HTMLCollectionOf<Element> = document.getElementsByClassName("numpad-control-buttons");
        if (!ObjectExtensions.isNullOrUndefined(numPadDivs) && numPadDivs.length > 0) {
            let numPadDiv = numPadDivs[0] as any;
            //navDiv.disabled = true;
            numPadDiv.style.visibility = "hidden";
        }

        //let numPadDiv: HTMLDivElement = document.querySelector('[data-ax-bubble="paymentView_totalAmountNumpad"]') as HTMLDivElement;
        //if (!ObjectExtensions.isNullOrUndefined(numPadDiv)) {
        //    //(numPadDiv as any).disabled = true;
        //    numPadDiv.style.visibility = "hidden";
        //}

        let navDivs: HTMLCollectionOf<Element> = document.getElementsByClassName("navDiv");
        if (!ObjectExtensions.isNullOrUndefined(navDivs) && navDivs.length > 0) {
            let navDiv = navDivs[0] as any;
            //navDiv.disabled = true;
            navDiv.style.visibility = "hidden";
        }

        this.isVisible = false;
        this.canExecute = true;
    }

    protected execute(): void {

    }
}