import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchView from "PosApi/Extend/Views/SearchView";

export default class NavigateToSimpleExtensionViewCommand extends SearchView.ProductSearchExtensionCommandBase {

    constructor(context: IExtensionCommandContext<SearchView.IProductSearchToExtensionCommandMessageTypeMap>) {
        super(context);
    }

    protected init(state: SearchView.IProductSearchExtensionCommandState): void {
        throw new Error("Method not implemented.");
    }

    protected execute(): void {
        throw new Error("Method not implemented.");
    }

}