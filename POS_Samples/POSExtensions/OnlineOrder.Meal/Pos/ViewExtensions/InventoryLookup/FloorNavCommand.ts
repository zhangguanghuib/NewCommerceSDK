import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as InventoryLookupView from "PosApi/Extend/Views/InventoryLookupView";

export default class FloorNavCommand extends InventoryLookupView.InventoryLookupExtensionCommandBase {

 constructor(context: IExtensionCommandContext<InventoryLookupView.IInventoryLookupToExtensionCommandMessageTypeMap>) {
    super(context);
     this.id = "FloorNavCommand";
     this.label = "Floor NAV Index";
     this.extraClass = "iconInvoice";
}

    protected init(state: InventoryLookupView.IInventoryLookupExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }

    protected execute(): void {
        this.context.navigator.navigate("FloorNavIndex");
    }

}