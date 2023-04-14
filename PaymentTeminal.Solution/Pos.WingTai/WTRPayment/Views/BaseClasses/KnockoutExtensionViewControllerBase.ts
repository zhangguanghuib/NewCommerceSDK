/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */
import ko from "knockout";
import * as NewView from "PosApi/Create/Views";
import KnockoutExtensionViewModelBase from "./KnockoutExtensionViewModelBase";

/**
 * Represents the base class for knockout based extension view controllers.
 */
abstract class KnockoutExtensionViewControllerBase<TViewModel extends KnockoutExtensionViewModelBase> extends NewView.ExtensionViewControllerBase {
    public abstract readonly viewModel: TViewModel;
    
    constructor(context: NewView.IExtensionViewControllerContext, saveInHistory: boolean) {
        super(context, saveInHistory);
    }

    /**
     * Bind the html element with view controller.
     *
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {
        super.onReady(element);

        // Customized binding
        ko.applyBindings(this, element);
    }
}

export default KnockoutExtensionViewControllerBase;