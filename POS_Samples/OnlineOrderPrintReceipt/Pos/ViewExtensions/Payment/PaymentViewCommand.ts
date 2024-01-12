import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as PaymentView from "PosApi/Extend/Views/PaymentView";
export default class PaymentViewCommand extends PaymentView.PaymentViewExtensionCommandBase {
    constructor(context: IExtensionCommandContext<PaymentView.IPaymentViewToExtensionCommandMessageTypeMap>) {
        super(context);
        this.label = "Payment Command";
        this.id = "PaymentCommand";
        this.extraClass = "iconLightningBolt";
    }

    protected init(state: PaymentView.IPaymentViewExtensionCommandState): void {
        this.isVisible = false;
        this.canExecute = false;

        let selectElement = document.querySelector("#selectGiftCardInput");
        selectElement.addEventListener("change", (event) => {
            console.log(event);
            let cardPanel = document.querySelector('[data-bind="visible: paymentMethodViewModel.isManualEntryAllowedInCardSetup"]') as HTMLDivElement;
            if (event.target["value"] == "0") {
                cardPanel.style.visibility = "hidden";
            } else {
                cardPanel.style.visibility = "visible";
            }
        });
    }

    protected execute(): void {
    }
}