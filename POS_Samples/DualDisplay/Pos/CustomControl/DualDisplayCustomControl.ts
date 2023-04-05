import ko from "knockout";

import { CartChangedData, CustomerChangedData, DualDisplayCustomControlBase, IDualDisplayCustomControlContext, LogOnStatusChangedData } from "PosApi/Extend/DualDisplay";

import * as Controls from "PosApi/Consume/Controls";
import { ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";

//Sign in to the client.
//Go to Retail and Commerce > Channel setup > POS setup > POS profiles > Hardware profiles.
//Select the hardware profile that is linked to your register.
//On the Dual display tab, set the Dual display in use option to Yes.
//Go to Retail and Commerce > Retail and Commerce IT > Distribution schedule.
//Select the Registers(1090) job, and then select Run now.


export default class DualDisplayCustomControl extends DualDisplayCustomControlBase {

    public readonly cartLinesDataList: Controls.IDataList<ProxyEntities.CartLine>;

    public readonly cartTotalAmount: ko.Computed<number>;
    public readonly customerName: ko.Computed<string>;
    public readonly customerAccountNumber: ko.Computed<string>;
    public readonly isLoggedOn: ko.Computed<boolean>;
    public readonly employeeName: ko.Computed<string>;

    public readonly cartTotalAmountLabel: string;
    public readonly customerNameLabel: string;
    public readonly customerAccountNumberLabel: string;
    public readonly employeeNameLabel: string;

    private static readonly TEMPLATE_ID: string = "Microsoft_Pos_Extensibility_Samples_DualDisplay";

    private readonly _cart: ko.Observable<ProxyEntities.Cart>;
    private readonly _cartLinesObservable: ko.ObservableArray<ProxyEntities.CartLine>;
    private readonly _customer: ko.Observable<ProxyEntities.Customer>;
    private readonly _loggedOn: ko.Observable<boolean>;
    private readonly _employee: ko.Observable<ProxyEntities.Employee>;

    public imageRotatorPath: string;
    public webBrowserUrl: string;

    constructor(id: string, context: IDualDisplayCustomControlContext) {

        super(id, context);

        this.cartTotalAmountLabel = "Total Amount:";
        this.customerNameLabel = "Customer Name:";
        this.customerAccountNumberLabel = "Customer Account Number:";
        this.employeeNameLabel = "Employee Name:";
        this.imageRotatorPath = "";

        this._cart = ko.observable(null);
        this._cartLinesObservable = ko.observableArray([]);
        this._customer = ko.observable(null);
        this._loggedOn = ko.observable(false);
        this._employee = ko.observable(null);

        this.cartTotalAmount = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._cart()) ? 0.00 : this._cart().TotalAmount;
        });

        this.customerName = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._customer()) ? StringExtensions.EMPTY : this._customer().Name;
        });

        this.customerAccountNumber = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._customer()) ? StringExtensions.EMPTY : this._customer().AccountNumber;
        });

        this.isLoggedOn = ko.computed(() => {
            return this._loggedOn();
        });

        this.employeeName = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._employee())? StringExtensions.EMPTY: this._employee().Name;
        });

        this.cartChangedHandler = (data: CartChangedData) => {
            this._cart(data.cart);
            this._cartLinesObservable(ObjectExtensions.isNullOrUndefined(data.cart) ? [] : data.cart.CartLines);
        }

        this.customerChangedHandler = (data: CustomerChangedData) => {
            this._customer(data.customer);
        }

        this.logOnStatusChangedHandler = (data: LogOnStatusChangedData) => {
            this.isProcessing = true;

            window.setTimeout(() => {
                this.isProcessing = false;
            }, 1000);

            this._loggedOn(data.loggedOn);
            this._employee(data.employee);
        }

        let cartLinesDataListOptions: Readonly<Controls.IDataListOptions<ProxyEntities.CartLine>> = {
            interactionMode: Controls.DataListInteractionMode.None,
            data: this._cartLinesObservable(),
            columns: [
                {
                    title: "ID",
                    ratio: 20,
                    collapseOrder: 2,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.ItemId) ? StringExtensions.EMPTY : cartLine.ItemId;
                    }
                },
                ,
                {
                    title: "Name", // Name
                    ratio: 50,
                    collapseOrder: 4,
                    minWidth: 100,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Description) ? StringExtensions.EMPTY : cartLine.Description;

                    }
                },
                {
                    title: "Quantity", // Quantity
                    ratio: 10,
                    collapseOrder: 3,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Quantity) ? StringExtensions.EMPTY : cartLine.Quantity.toString();

                    }
                },
                {
                    title: "Discount", // Discount
                    ratio: 10,
                    collapseOrder: 1,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.DiscountAmount) ? StringExtensions.EMPTY : cartLine.DiscountAmount.toString();
                    }
                },
                {
                    title: "Cost", // Cost
                    ratio: 10,
                    collapseOrder: 5,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.TotalAmount) ? StringExtensions.EMPTY : cartLine.TotalAmount.toString();
                    }
                }
            ]
        };

        let dataListRootElem: HTMLDivElement = document.querySelector("#dualDisplayDataListSample") as HTMLDivElement;

        this.cartLinesDataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", cartLinesDataListOptions, dataListRootElem);
    }


    public onReady(element: HTMLElement): void {
        ko.applyBindingsToNode(element, {
            template: {
                name: DualDisplayCustomControl.TEMPLATE_ID,
                data: this
            }
        }, this);
    }

    public init(state: Commerce.Extensibility.DualDisplayExtensionTypes.IDualDisplayCustomControlState): void {
        this._cart(state.cart);
        this._customer(state.customer);
        this._loggedOn(state.loggedOn);
        this._employee(state.employee);

        if (StringExtensions.isEmptyOrWhitespace(state.configuration.imageRotatorPath)) {
            this.imageRotatorPath = localStorage.getItem("DualDisplayWebBrowserUrl");
        } else {
            this.imageRotatorPath = state.configuration.imageRotatorPath;
        }

        if (StringExtensions.isEmptyOrWhitespace(state.configuration.webBrowserUrl)) {
            this.webBrowserUrl = localStorage.getItem("DualDisplayWebBrowserUrl");
        } else {
            this.webBrowserUrl = state.configuration.webBrowserUrl;
        }

        this._cartLinesObservable(ObjectExtensions.isNullOrUndefined(this._cart()) ? [] : this._cart().CartLines);
    }

}