import * as ManageShiftsView from "PosApi/Extend/Views/ManageShiftsView";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { Icons } from "PosApi/Create/Views";

export default class ManageShiftsUpdCommand extends ManageShiftsView.ManageShiftsExtensionCommandBase {

    constructor(context: IExtensionCommandContext<ManageShiftsView.IManageShiftsToExtensionCommandMessageTypeMap>) {
        super(context);
        this.label = "Numeric Dialog";
        this.id = "NumericDialog";
        this.extraClass = Icons.NumberSymbol;
    }

    protected init(state: ManageShiftsView.IManageShiftsExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }

    protected execute(): void {
        this.context.navigator.navigate("NumericInputDialogView");
    }

}