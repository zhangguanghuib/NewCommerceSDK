import ko from "knockout";

import * as NewView from "PosApi/Create/Views";

import KnockoutExtensionViewModelBase from "./KnockoutExtensionViewModelBase";

abstract class KnockoutExtensionViewControllerBase<TViewModel extends KnockoutExtensionViewModelBase> extends NewView.ExtensionViewControllerBase {
    public abstract readonly viewModel: TViewModel;

    constructor(context: NewView.IExtensionViewControllerContext, saveInHistory: boolean) {
        super(context, saveInHistory);
    }

    public onReady(element: HTMLElement): void {
        super.onReady(element);
        ko.applyBindings(this, element);
    }
}

export default KnockoutExtensionViewControllerBase;