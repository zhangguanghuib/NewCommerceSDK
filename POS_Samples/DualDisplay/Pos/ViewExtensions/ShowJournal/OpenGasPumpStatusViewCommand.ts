import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";

export default class OpenGasPumpStatusViewCommand extends ShowJournalView.ShowJournalExtensionCommandBase {


    constructor(context: IExtensionCommandContext<ShowJournalView.IShowJournalToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "openGasPumpStatusViewCommand";
        this.label = "open GasPumpStatusView";
        this.extraClass = "iconGo";
    }
    protected init(state: ShowJournalView.IShowJournalExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }
    protected execute(): void {
        this.context.navigator.navigate("GasPumpStatusView");
    }
}


    