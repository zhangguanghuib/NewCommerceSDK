import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as InventoryLookupView from "PosApi/Extend/Views/InventoryLookupView";

export default class JDIndexCommand extends InventoryLookupView.InventoryLookupExtensionCommandBase {

 constructor(context: IExtensionCommandContext<InventoryLookupView.IInventoryLookupToExtensionCommandMessageTypeMap>) {
    super(context);
     this.id = "JDIndexCommand";
     this.label = "JD Index";
     this.extraClass = "iconInvoice";
}

    protected init(state: InventoryLookupView.IInventoryLookupExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }

    protected execute(): void {
        this.context.navigator.navigate("Flex2ColsFloorNavIndex");
    }

}