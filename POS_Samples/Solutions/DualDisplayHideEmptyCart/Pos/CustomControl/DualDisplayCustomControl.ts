/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import {
    DualDisplayCustomControlBase,
    IDualDisplayCustomControlState,
    IDualDisplayCustomControlContext,
    CartChangedData,
    CustomerChangedData,
    LogOnStatusChangedData
} from "PosApi/Extend/DualDisplay";
import { IDataList, IDataListOptions, DataListInteractionMode } from "PosApi/Consume/Controls";
import { ArrayExtensions, ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import { ProxyEntities } from "PosApi/Entities";
import ko from "knockout";

export default class DualDisplayCustomControl extends DualDisplayCustomControlBase {
    // The data list to bind against and display with cart line information.
    public cartLinesDataList: IDataList<ProxyEntities.CartLine>;

    // Computed values for binding against.
    public readonly cartTotalAmount: ko.Computed<number>;
    public readonly customerName: ko.Computed<string>;
    public readonly customerAccountNumber: ko.Computed<string>;
    public readonly isLoggedOn: ko.Computed<boolean>;
    public readonly employeeName: ko.Computed<string>;

    // Labels for binding against.
    public readonly cartTotalAmountLabel: string;
    public readonly customerNameLabel: string;
    public readonly customerAccountNumberLabel: string;
    public readonly employeeNameLabel: string;

    private static readonly TEMPLATE_ID: string = "Microsot_Pos_Extensibility_Samples_DualDisplay";

    // Observable values used for keeping track of state.
    private readonly _cart: ko.Observable<ProxyEntities.Cart>;
    private readonly _customer: ko.Observable<ProxyEntities.Customer>;
    private readonly _loggedOn: ko.Observable<boolean>;
    private readonly _employee: ko.Observable<ProxyEntities.Employee>;

    private _cartLines: Array<ProxyEntities.CartLine>;
    private _dataList: IDataList<ProxyEntities.CartLine>;

    // The current adding or updating cartline
    public currentCartLine: ko.Observable<ProxyEntities.CartLine>;

    public readonly itemId: ko.Computed<string>;
    public readonly productName: ko.Computed<string>;
    public readonly productQuantity: ko.Computed<string>;
    public readonly productDiscount: ko.Computed<string>;
    public readonly productCost: ko.Computed<string>;
    public imgUrl: ko.Observable<string>;
    public imgIndex: number;
    public readonly IMGTOTAL: number = 10;
    public readonly isCartEmpty: ko.Computed<boolean>;

    constructor(id: string, context: IDualDisplayCustomControlContext) {
        super(id, context);

        // Initializes labels.
        this.cartTotalAmountLabel = this.context.resources.getString("string_5");
        this.customerNameLabel = this.context.resources.getString("string_6");
        this.customerAccountNumberLabel = this.context.resources.getString("string_7");
        this.employeeNameLabel = this.context.resources.getString("string_8");

        // Initializes observable and computed values.
        this._cart = ko.observable(null);
        this._cartLines = [];
        this._customer = ko.observable(null);
        this._loggedOn = ko.observable(false);
        this._employee = ko.observable(null);
        //this.mediaServerUrl = ko.observable('');

        this.isCartEmpty = ko.computed(() => {
            return !ObjectExtensions.isNullOrUndefined(this._cart()) && (this._cart().CartLines.length > 0) ? false : true;
        });

        this.imgIndex = 0;
        this.imgUrl = ko.observable('');
        this.currentCartLine = ko.observable(null);

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
            return ObjectExtensions.isNullOrUndefined(this._employee()) ? StringExtensions.EMPTY : this._employee().Name;
        });

        this.itemId = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this.currentCartLine()) ? StringExtensions.EMPTY : this.currentCartLine().ItemId;
        });

        this.productName = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this.currentCartLine()) || StringExtensions.isEmptyOrWhitespace(this.currentCartLine().Description) ?
                StringExtensions.EMPTY : this.currentCartLine().Description;
        });

        this.productQuantity = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this.currentCartLine()) ? StringExtensions.EMPTY : this.currentCartLine().Quantity.toString();
        });

        this.productDiscount = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this.currentCartLine()) ? StringExtensions.EMPTY : this.currentCartLine().DiscountAmount.toString();
        });

        this.productCost = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this.currentCartLine()) ? StringExtensions.EMPTY : this.currentCartLine().TotalAmount.toString();
        });

        this.cartChangedHandler = (data: CartChangedData) => {
            this._cart(data.cart);
            let currentCartLineId: string = localStorage.getItem("currentCartLineId");
            let isNewLine: boolean = !StringExtensions.isEmptyOrWhitespace(currentCartLineId) && this._dataList.data.filter(line => line.LineId === currentCartLineId).length > 0 ? false : true;
            this._dataList.data = ObjectExtensions.isNullOrUndefined(data.cart) ? [] : data.cart.CartLines;

            // Find the line height
            let listLine: HTMLDivElement = document.querySelector(".dataListLine") as HTMLDivElement;
            let rowHeight = 39;
            if (listLine?.clientHeight) {
                rowHeight = listLine?.clientHeight;
            } 
            // If a new line is adding
            if (data.cart && data.cart.CartLines.length > 0 && !StringExtensions.isEmptyOrWhitespace(currentCartLineId)) {
                //Find the current cart line:
                this.currentCartLine(this._dataList.data.filter(line => line.LineId === currentCartLineId)[0]);

                if (isNewLine) {
                    if (!StringExtensions.isEmptyOrWhitespace(currentCartLineId)) {
                        if (data.cart.CartLines.length >= 20) {
                            setTimeout(() => {
                                let dualDisplayScrollingContainer: HTMLDivElement = document.querySelector('[aria-label="Scrolling Container"]') as HTMLDivElement;
                                let totalHeight: number = dualDisplayScrollingContainer.scrollHeight;
                                let shouldScrollTop = totalHeight - rowHeight * 19;
                                // dualDisplayScrollingContainer.scrollTop = shouldScrollTop;
                                dualDisplayScrollingContainer.scrollTo({
                                    top: shouldScrollTop, // Replace with the position you want to scroll to
                                    behavior: 'smooth'
                                });
                            }, 700);
                        }
                    }
                }
                else {//Update existing line
                    setTimeout(() => {
                        let selectedIndex = ArrayExtensions.findIndex(data.cart.CartLines, (cartline: ProxyEntities.CartLine) => cartline.LineId === currentCartLineId);
                        let dualDisplayScrollingContainer: HTMLDivElement = document.querySelector('[aria-label="Scrolling Container"]') as HTMLDivElement;
                        let shouldScrollTop = rowHeight * selectedIndex;
                        // dualDisplayScrollingContainer.scrollTop = shouldScrollTop;
                        dualDisplayScrollingContainer.scrollTo({
                            top: shouldScrollTop,
                            behavior: 'smooth'
                        });
                    }, 700);
                }
            } else {
                this.currentCartLine(null);
            }
        };

        this.customerChangedHandler = (data: CustomerChangedData) => {
            this._customer(data.customer);
        };

        this.logOnStatusChangedHandler = (data: LogOnStatusChangedData) => {
            // Displays the busy indicator here, even though it's not necessary, in order to showcase and test the scenario.
            this.isProcessing = true;
            window.setTimeout(() => {
                this.isProcessing = false;
            }, 1000);

            this._loggedOn(data.loggedOn);
            this._employee(data.employee);
        };


        // Logs the completion of constructing the DualDisplayCustomControl.
        this.context.logger.logInformational("DualDisplayCustomControl constructed", this.context.logger.getNewCorrelationId());
    }

    /**
     * Binds the control to the specified element.
     * @param {HTMLElement} element The element to which the control should be bound.
     */
    public onReady(element: HTMLElement): void {
        // Initializes the cart lines data list
        let cartLinesDataListOptions: IDataListOptions<ProxyEntities.CartLine> = {
            interactionMode: DataListInteractionMode.SingleSelect,
            data: this._cartLines,
            columns: [
                {
                    title: this.context.resources.getString("string_0"), // ID
                    ratio: 20,
                    collapseOrder: 2,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.ItemId) ? StringExtensions.EMPTY : cartLine.ItemId;
                    }
                },
                {
                    title: this.context.resources.getString("string_1"), // Name
                    ratio: 50,
                    collapseOrder: 4,
                    minWidth: 100,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Description) ? StringExtensions.EMPTY : cartLine.Description;

                    }
                },
                {
                    title: this.context.resources.getString("string_2"), // Quantity
                    ratio: 10,
                    collapseOrder: 3,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Quantity) ? StringExtensions.EMPTY : cartLine.Quantity.toString();

                    }
                },
                {
                    title: this.context.resources.getString("string_3"), // Discount
                    ratio: 10,
                    collapseOrder: 1,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.DiscountAmount) ? StringExtensions.EMPTY : cartLine.DiscountAmount.toString();
                    }
                },
                {
                    title: this.context.resources.getString("string_4"), // Cost
                    ratio: 10,
                    collapseOrder: 5,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.TotalAmount) ? StringExtensions.EMPTY : cartLine.TotalAmount.toString();
                    }
                }
            ]
        };
        ko.applyBindingsToNode(element, {
            template: {
                name: DualDisplayCustomControl.TEMPLATE_ID,
                data: this
            }
        });

        let dualDisplayRootElem: HTMLDivElement = element.querySelector("#dualDisplayDataListSample") as HTMLDivElement;
        this._dataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", cartLinesDataListOptions, dualDisplayRootElem);
    }

    /**
     * Initializes the control.
     * @param {IDualDisplayCustomControlState} state The initial state of the page used to initialize the control.
     */
    public init(state: IDualDisplayCustomControlState): void {
        this._cart(state.cart);
        this._customer(state.customer);
        this._loggedOn(state.loggedOn);
        this._employee(state.employee);
        //his.imgUrl(state.configuration.imageRotatorPath);
        //this.mediaServerUrl(state.configuration.imageRotatorPath);
        //console.log("imageRotatorPath:" + this.mediaServerUrl());
        this._cartLines = ObjectExtensions.isNullOrUndefined(this._cart()) ? [] : this._cart().CartLines;
    }
}