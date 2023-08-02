import {
    DualDisplayCustomControlBase,
    IDualDisplayCustomControlState,
    IDualDisplayCustomControlContext,
    CartChangedData,
    LogOnStatusChangedData,
    DualDisplayConfigurationChangedData
} from "PosApi/Extend/DualDisplay";
import * as Controls from "PosApi/Consume/Controls";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import { ProxyEntities } from "PosApi/Entities";
import ko from "knockout";



export default class DualDisplayCustomControlTest extends DualDisplayCustomControlBase {
    public cartLinesDataList: Controls.IDataList<ProxyEntities.CartLine>;
    public imageUrl: string;
    public correlationId: string;

    public readonly subTotal: ko.Computed<number>;
    public readonly tax: ko.Computed<number>;
    public readonly discount: ko.Computed<number>;
    public readonly balance: ko.Computed<number>;
    public readonly checkoutItems: ko.Computed<string>;
    
    
    public readonly subTotalLabel: string;
    public readonly taxLabel: string;
    public readonly discountLabel: string;
    public readonly balanceLabel: string;
    
    public checkoutLabel: string;

    private static readonly TEMPLATE_ID: string = "Microsoft_Pos_Extensibility_Samples_DualDisplay";

    // Observable values used for keeping track of state.
    
    private readonly _cart: ko.Observable<ProxyEntities.Cart>;
    private readonly _cartLinesObservable: ko.ObservableArray<ProxyEntities.CartLine>;
    
    private _cartlinesQuantity: ko.Observable<string>;


    constructor(id: string, context: IDualDisplayCustomControlContext) {
        super(id, context);

        // Initializes labels.
        
        //this.subTotalLabel = this.context.resources.getString("MicrosExtensions_5");
        //this.taxLabel = this.context.resources.getString("MicrosExtensions_6");
        //this.discountLabel = this.context.resources.getString("MicrosExtensions_7");
        //this.balanceLabel = this.context.resources.getString("MicrosExtensions_8");
        
        //this.checkoutLabel = this.context.resources.getString("MicrosExtensions_9");

        this.subTotalLabel = "Sub Total";
        this.taxLabel = "Tax";
        this.discountLabel = "Discount";
        this.balanceLabel = "Balance";

        this.checkoutLabel = "Checkout";


        // Initializes observable and computed values.
        
        this._cart = ko.observable(null);
        this._cartLinesObservable = ko.observableArray([]);
        this._cartlinesQuantity = ko.observable("");
        
        this.correlationId = this.context.logger.getNewCorrelationId();
        
        this.subTotal = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._cart()) ? 0.00 : this._cart().SubtotalAmount;
        });
        
        this.tax = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._cart()) ? 0.00 : this._cart().TaxAmount;
        });

        this.discount = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._cart()) ? 0.00 : this._cart().DiscountAmount;
        });

        this.balance = ko.computed(() => {
            return ObjectExtensions.isNullOrUndefined(this._cart()) ? 0.00 : this._cart().TotalAmount;
        });
        
        this.checkoutItems = ko.computed(() => {
            return (ObjectExtensions.isNullOrUndefined(this._cart())) ?
                StringExtensions.EMPTY : this._cart().CartLines.length.toString() + " item(s).";
        });
        
        
        this.cartChangedHandler = (data: CartChangedData) => {
            
            this._cart(data.cart);
            this._cartLinesObservable(ObjectExtensions.isNullOrUndefined(data.cart) ? [] : data.cart.CartLines);
            this._cartlinesQuantity(ObjectExtensions.isNullOrUndefined(this._cart().CartLines) ? "" : this._cart().CartLines.length.toString());
            if (!ObjectExtensions.isNullOrUndefined(this.cartLinesDataList)) {
                this.cartLinesDataList.data = this._cartLinesObservable();
            }
            
            //this.checkoutLabel = this.context.resources.getString("MicrosExtensions_9") + this._cartlinesQuantity();
            this.checkoutLabel = "Checkout" + this._cartlinesQuantity();

        };
        
        this.dualDisplayConfigurationChangedHandler = (data: DualDisplayConfigurationChangedData) => {
            
        }

        this.logOnStatusChangedHandler = (data: LogOnStatusChangedData) => {

            // Displays the busy indicator here, even though it's not necessary, in order to showcase and test the scenario.
            this.isProcessing = true;
            window.setTimeout(() => {
                this.isProcessing = false;
            }, 1000);
            
        };

        
        // Initializes the cart lines data list
        //let cartLinesDataListOptions: IDataListState<ProxyEntities.CartLine> = {//MigrateCommerceSDK-POS, 9/12/2022, Shamgar.O
        
        
        //this.dataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", dataListOptions, dataListRootElem);//MigrateCommerceSDK-POS, 9/12/2022, Shamgar.O
        //this.cartLinesDataList = new DataList(cartLinesDataListOptions);//MigrateCommerceSDK-POS, 9/12/2022, Shamgar.O
        

        // Logs the completion of constructing the DualDisplayCustomControl.
        this.context.logger.logInformational("DualDisplayCustomControl constructed", this.context.logger.getNewCorrelationId());
    }

    /**
     * Binds the control to the specified element.
     * @param {HTMLElement} element The element to which the control should be bound.
     */
    public onReady(element: HTMLElement): void {
        
        ko.applyBindingsToNode(element, {
            template: {
                name: DualDisplayCustomControlTest.TEMPLATE_ID,
                data: this
            }
        });
        
        let cartLinesDataListOptions: Controls.IDataListOptions<ProxyEntities.CartLine> = {
            //selectionMode: SelectionMode.None,//MigrateCommerceSDK-POS, 9/12/2022, Shamgar.O
            interactionMode: Controls.DataListInteractionMode.None,
            //itemDataSource: this._cartLinesObservable,//MigrateCommerceSDK-POS, 9/12/2022, Shamgar.O
            data: this._cartLinesObservable(),

            columns: [
                {
                    //title: this.context.resources.getString("MicrosExtensions_1"), // Name
                    title: "Name", // Name
                    ratio: 60,
                    collapseOrder: 1,
                    minWidth: 100,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Description) ? StringExtensions.EMPTY : cartLine.Description;

                    }
                },
                {
                    //title: this.context.resources.getString("MicrosExtensions_2"), // Quantity
                    title: "Quantity",
                    ratio: 20,
                    collapseOrder: 2,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Quantity) ? StringExtensions.EMPTY : cartLine.Quantity.toString();

                    }
                },

                {
                    //title: this.context.resources.getString("MicrosExtensions_4"), // Cost
                    title: "Cost",
                    ratio: 20,
                    collapseOrder: 3,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.TotalAmount) ? StringExtensions.EMPTY : cartLine.TotalAmount.toString();
                    }
                }
            ]
        };
        let dataListRootElem: HTMLDivElement = document.querySelector("#dualDisplayDataListSample") as HTMLDivElement;
        this.cartLinesDataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", cartLinesDataListOptions, dataListRootElem);
        this.cartLinesDataList.data = this._cartLinesObservable();

    }


    /**
     * Initializes the control.
     * @param {IDualDisplayCustomControlState} state The initial state of the page used to initialize the control.
     */
    public init(state: IDualDisplayCustomControlState): void {
        
        this._cart(state.cart);       
        this._cartLinesObservable(ObjectExtensions.isNullOrUndefined(this._cart()) ? [] : this._cart().CartLines);
        this._cartlinesQuantity(ObjectExtensions.isNullOrUndefined(this._cart().CartLines) ? "" : this._cart().CartLines.length.toString());

        //this.imageUrl = state.configuration.imageRotatorPath;
        this.imageUrl = "https://picsum.photos/500/700";
        
    }
}
