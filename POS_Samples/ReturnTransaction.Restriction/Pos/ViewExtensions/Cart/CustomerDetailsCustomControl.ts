import ko from "knockout";
import { GetCustomerClientRequest, GetCustomerClientResponse } from "PosApi/Consume/Customer";
import { ProxyEntities, ClientEntities } from "PosApi/Entities";
import { CartChangedData } from "PosApi/Extend/Header";
import { CartViewCustomControlBase, ICartViewCustomControlContext } from "PosApi/Extend/Views/CartView";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";

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


    protected init(state: Commerce.Cart.Extensibility.ICartViewCustomControlState): void {
        this._customer(state.customer);
    }


    public onReady(element: HTMLElement): void {
        ko.applyBindingsToNode(element, {
            template: {
                name: CustomerDetailsCustomControl.TEMPLATE_ID,
                data: this
            }
        });
    }

    constructor(id: string, context: ICartViewCustomControlContext) {
        super(id, context);

        this.accountHolderLabel = "";
        this.accountNumberLabel = "";
        this.phoneLabel = "";
        this.emailLabel = "";

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
            if (this._customer() || this._customer().AccountNumber != data.cart.CustomerId) {
                let req: GetCustomerClientRequest<GetCustomerClientResponse> = new GetCustomerClientRequest(data.cart.CustomerId);
                this.context.runtime.executeAsync(req).then((res: ClientEntities.ICancelableDataResult<GetCustomerClientResponse>): void => {
                    this._customer(res.data.result);
                });
            }
        }

    }
}