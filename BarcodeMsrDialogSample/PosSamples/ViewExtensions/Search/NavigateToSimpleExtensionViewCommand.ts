import { Icons } from "PosApi/Create/Views";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchView from "PosApi/Extend/Views/SearchView";

export default class NavigateToSimpleExtensionViewCommand extends SearchView.ProductSearchExtensionCommandBase {

    constructor(context: IExtensionCommandContext<SearchView.IProductSearchToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "navigateToSimpleExtensionViewCommand";
        this.label = "Navigate to Sample View";
        this.extraClass = Icons.Go;
    }

    protected init(state: SearchView.IProductSearchExtensionCommandState): void {
        this.canExecute = true;
        this.isVisible = true;
    }

    protected execute(): void {
        this.context.navigator.navigate("SamplesView");
    }

}