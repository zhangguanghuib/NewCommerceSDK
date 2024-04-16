import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as InventoryListView from "PosApi/Extend/Views/InventoryDocumentListView";
import { ObjectExtensions } from "PosApi/TypeExtensions";


export default class DisableCreateNewBtn extends InventoryListView.InventoryDocumentListExtensionCommandBase {

    constructor(context: IExtensionCommandContext<InventoryListView.IInventoryDocumentListToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "customerCrossLoyaltyCommand";
        this.label = "Cross Loyalty Discount";
        this.extraClass = "iconLightningBolt";
    }

    protected init(state: InventoryListView.IInventoryDocumentListExtensionCommandState): void {
        this.isVisible = false;
        this.canExecute = true;

        var signInInterval = setInterval(function () {
            let createNewBtn = document.querySelector('#HomeView_InventoryDocumentListCreateHeaderCommandCommand') as HTMLButtonElement;
            if (!ObjectExtensions.isNullOrUndefined(createNewBtn)) {
                createNewBtn.style.display = "none";
                clearInterval(signInInterval);
            }
        }, 100);
    }

    protected execute(): void {

    }

}