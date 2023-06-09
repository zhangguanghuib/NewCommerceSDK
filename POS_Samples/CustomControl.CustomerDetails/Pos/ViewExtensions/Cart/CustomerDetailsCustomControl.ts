import ko from "knockout";

import {
    CartViewCustomControlBase,
    ICartViewCustomControlState,
    ICartViewCustomControlContext,
    CartChangedData
} from "PosApi/Extend/Views/CartView";

import {
    ObjectExtensions,
    StringExtensions
} from "PosApi/TypeExtensions";

import { ProxyEntities } from "PosApi/Entities";

export default class CustomerDetailsCustomControl extends CartViewCustomControlBase {
    public readonly isCustomerSelected: ko.Computed<boolean>;
    public readonly accountHolder: ko.Computed<string>;
    public readonly accountNumber: ko.Computed<string>;
    public readonly phone: ko.Computed<string>;
    public readonly email: ko.Computed<string>;

    public readonly accountHolderLabel: string;
    public readonly accountNumberLabel: string;
    public readonly phoneLabel: string;
    public readonly emailLabel: string;

    private static readonly TEMPLATE_ID: string = "Microsot_Pos_Extensibility_Samples_CustomerDetails";
    private readonly _customer: ko.Observable<ProxyEntities.Customer>;

    constructor(id: string, context: ICartViewCustomControlContext) {
        super(id, context);

        this.accountHolderLabel = "Account holder";
        this.accountNumberLabel = "Account number";
        this.phoneLabel = "Phone";
        this.emailLabel = "Email";

        this._customer = ko.observable(null);

        this.isCustomerSelected = ko.computed(() => !ObjectExtensions.isNullOrUndefined(this._customer()));

        this.accountHolder = ko.computed(() => {
            if (this.isCustomerSelected()) {
                return this._customer().Name;
            }

            return StringExtensions.EMPTY;
        });

        this.accountNumber = ko.computed(() => {
            if (this.isCustomerSelected()) {
                return this._customer().AccountNumber;
            }

            return StringExtensions.EMPTY;
        });

        this.phone = ko.computed(() => {
            if (this.isCustomerSelected()) {
                return this._customer().Phone;
            }

            return StringExtensions.EMPTY;
        });

        this.email = ko.computed(() => {
            if (this.isCustomerSelected()) {
                return this._customer().Email;
            }

            return StringExtensions.EMPTY;
        });

        this.cartChangedHandler = (data: CartChangedData) => {
            this._customer(data.customer);
        };
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindingsToNode(element, {
            template: {
                name: CustomerDetailsCustomControl.TEMPLATE_ID,
                data: this
            }
        });
    }

    public init(state: ICartViewCustomControlState): void {
        this._customer(state.customer);
    }
}