import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";

export default class ShowMenusViewCommand extends ShowJournalView.ShowJournalExtensionCommandBase {
    constructor(context: IExtensionCommandContext<ShowJournalView.IShowJournalToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "ShowMenusViewCommand";
        this.label = "Open Menus View";
        this.extraClass = "iconPickup";
    }
    protected init(state: ShowJournalView.IShowJournalExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }
    protected execute(): Promise<void> {
        if (this.isProcessing) {
            return Promise.resolve();
        }

        this.isProcessing = true;
        this.context.navigator.navigate("MenusView");
        this.isProcessing = false;
        return Promise.resolve();
    }

}