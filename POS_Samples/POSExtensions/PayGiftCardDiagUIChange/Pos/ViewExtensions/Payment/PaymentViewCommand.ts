import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as PaymentView from "PosApi/Extend/Views/PaymentView";
import { PaymentViewPaymentCardChanged } from "PosApi/Extend/Views/PaymentView";
import { StringExtensions } from "PosApi/TypeExtensions";
export default class PaymentViewCommand extends PaymentView.PaymentViewExtensionCommandBase {
    constructor(context: IExtensionCommandContext<PaymentView.IPaymentViewToExtensionCommandMessageTypeMap>) {
        super(context);
        this.label = "Payment Command";
        this.id = "PaymentCommand";
        this.extraClass = "iconLightningBolt";
        const signInInterval = setInterval(() => {
            const numDialog: HTMLDivElement = document.querySelector('div[data-bind="customControlInternal: \'NumpadDialog\'"]') as HTMLDivElement;
            let button: HTMLButtonElement = numDialog && numDialog.querySelector('button[aria-label="Close"]') as HTMLButtonElement;
            let selectElement = <HTMLSelectElement>document.querySelector("#selectGiftCardInput");

            console.log("Found close button: " + button);
            console.log("selectElement found: " + selectElement);

            if (button && selectElement) {
                button.click();
                clearInterval(signInInterval);
            }
        }, 100);

        setTimeout(() => {
            let selectElement = <HTMLSelectElement>document.querySelector("#selectGiftCardInput");
            if (!selectElement) {
                clearInterval(signInInterval);
            }
        }, 3000);
    }


    protected init(state: PaymentView.IPaymentViewExtensionCommandState): void {
        this.isVisible = false;
        this.canExecute = false;

        // Get the select element
        let selectElement = <HTMLSelectElement>document.querySelector("#selectGiftCardInput");

        if (selectElement) {
            // Get the selected value
            let selectedValue = selectElement.value;
            console.log(`Selected value: ${selectedValue}`);

            // Get the selected text
            let selectedText = selectElement.options[selectElement.selectedIndex].text;
            console.log(`Selected text: ${selectedText}`);

            // Get the selected index
            let selectedIndex = selectElement.selectedIndex;
            console.log(`Selected index: ${selectedIndex}`);

            let cardPanel = document.querySelector('[data-bind="visible: paymentMethodViewModel.isManualEntryAllowedInCardSetup"]') as HTMLDivElement;

            if (selectedValue == "0") {
                cardPanel.style.visibility = "hidden";
            } else {
                cardPanel.style.visibility = "visible";
            }
        }

        this.handler();

        this.paymentViewPaymentCardChangedHandler = (data: PaymentViewPaymentCardChanged) => {
            //console.log(data.paymentCard.CardNumber);
            //if (data.paymentCard.CardNumber && data.paymentCard.CardNumber.length > 0) {
            //    let cardPanel = document.querySelector('[data-bind="visible: paymentMethodViewModel.isManualEntryAllowedInCardSetup"]') as HTMLDivElement;
            //    cardPanel.style.visibility = "visible";
            //}
            let cardPanel: HTMLDivElement = document.querySelector('[data-bind="visible: paymentMethodViewModel.isManualEntryAllowedInCardSetup"]') as HTMLDivElement;
            let cardNum: HTMLAnchorElement = cardPanel.querySelector('div>:nth-child(2)');
            if (cardNum && !StringExtensions.isEmptyOrWhitespace(cardNum.innerHTML) && cardNum.innerHTML != 'Enter gift card') {
                cardPanel.style.visibility = "visible";
            }
        }
    }

    protected execute(): void {
    }

    public handler(): void {
        let selectElement: HTMLSelectElement = document.querySelector("#selectGiftCardInput") as HTMLSelectElement;
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
}