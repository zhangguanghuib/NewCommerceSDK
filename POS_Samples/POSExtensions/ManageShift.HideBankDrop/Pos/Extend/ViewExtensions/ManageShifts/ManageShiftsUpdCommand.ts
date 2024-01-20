import * as ManageShiftsView from "PosApi/Extend/Views/ManageShiftsView";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class ManageShiftsUpdCommand extends ManageShiftsView.ManageShiftsExtensionCommandBase {

    constructor(context: IExtensionCommandContext<ManageShiftsView.IManageShiftsToExtensionCommandMessageTypeMap>) {
        super(context);
        this.label = "";
        this.id = "";
        this.extraClass = "";
    }

    protected init(state: ManageShiftsView.IManageShiftsExtensionCommandState): void {
        console.log("command executed");
        let bankDropBtn: HTMLButtonElement = document.getElementById("bankDrop") as HTMLButtonElement;
        if (!ObjectExtensions.isNullOrUndefined(bankDropBtn)) {
            bankDropBtn.style.display = "none";
        }

        this.isVisible = false;
        this.canExecute = true;
    }

    protected execute(): void {
    }

}