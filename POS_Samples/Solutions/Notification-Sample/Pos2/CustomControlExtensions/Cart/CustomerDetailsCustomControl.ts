import ko from "knockout";

import {
    CartViewCustomControlBase,
    ICartViewCustomControlState,
    ICartViewCustomControlContext,
    CartChangedData,
} from "PosApi/Extend/Views/CartView";

import {
    ObjectExtensions,
    StringExtensions
} from "PosApi/TypeExtensions";
import {ClientEntities } from "PosApi/Entities";

export default class CRMCustomerDetailsCustomControl extends CartViewCustomControlBase {

    public isCustomerSelected: ko.Computed<boolean>;
    public _isLoaderVisible: ko.Observable<boolean>;
    public isNonReturnTransaction: ko.Observable<boolean>;
    public name: ko.Observable<string>;


    public _state: ICartViewCustomControlState;
    private static readonly TEMPLATE_ID: string = "WTR_Pos_Extension_CRMCustomerDetails";

    public constructor(id: string, context: ICartViewCustomControlContext) {
        super(id, context);
        this.isNonReturnTransaction = ko.observable(true);
        this.name = ko.observable(StringExtensions.EMPTY);
        this._isLoaderVisible = ko.observable(false);

        this.cartChangedHandler = (data: CartChangedData) => {
            this.isProcessing = true;
            try {
                this.name(StringExtensions.EMPTY);
                this.isCustomerSelected =
                    ko.computed(() => (!ObjectExtensions.isNullOrUndefined(data.cart.CustomerId)) && !StringExtensions.isEmptyOrWhitespace(data.cart.CustomerId));

                if (this.isCustomerSelected()) {
                    this.name(data.customer.Name);
                }
            }
            catch (e) {
                this.context.logger.logInformational(e);
            }
            finally {
                this.isProcessing = false;
            }
        }
    }

    protected init(state: ICartViewCustomControlState): void {
        this._state = state;
    }

    public onReady(element: HTMLElement): void {
        try {
            ko.applyBindingsToNode(element, {
                template: {
                    name: CRMCustomerDetailsCustomControl.TEMPLATE_ID,
                    data: this
                }
            }, this);
        }
        catch (e) {
            this.context.logger.logInformational(e);
        }
    }

    public openSearchDialog(): void {
        try {
            let options: ClientEntities.SearchNavigationParameters = new ClientEntities.SearchNavigationParameters(
                ClientEntities.SearchViewSearchEntity.Customer);

            this.context.navigator.navigateToPOSView("SearchView", options);
        }
        catch (e) {
            this.context.logger.logInformational(e);
        }
    }

    public closeDialogClicked(): void {
        this.name(StringExtensions.EMPTY);
        console.log("closeDialogClicked");
    }
}