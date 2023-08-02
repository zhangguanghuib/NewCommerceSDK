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
    public correlationId: string;
    private static readonly TEMPLATE_ID: string = "Microsoft_Pos_Extensibility_Samples_DualDisplay";
    private readonly _cart: ko.Observable<ProxyEntities.Cart>;
    private readonly _cartLinesObservable: ko.ObservableArray<ProxyEntities.CartLine>;   

    constructor(id: string, context: IDualDisplayCustomControlContext) {
        super(id, context);
  
        this._cart = ko.observable(null);
        this._cartLinesObservable = ko.observableArray([]);     
        this.correlationId = this.context.logger.getNewCorrelationId();
         
        this.cartChangedHandler = (data: CartChangedData) => {         
            this._cart(data.cart);
            this._cartLinesObservable(ObjectExtensions.isNullOrUndefined(data.cart) ? [] : data.cart.CartLines);
            if (!ObjectExtensions.isNullOrUndefined(this.cartLinesDataList)) {
                this.cartLinesDataList.data = this._cartLinesObservable();
            }       
        };
        
        this.dualDisplayConfigurationChangedHandler = (data: DualDisplayConfigurationChangedData) => {
            
        }

        this.logOnStatusChangedHandler = (data: LogOnStatusChangedData) => {
            this.isProcessing = true;
            window.setTimeout(() => {
                this.isProcessing = false;
            }, 1000);     
        };

        this.context.logger.logInformational("DualDisplayCustomControl constructed", this.context.logger.getNewCorrelationId());
    }

    public onReady(element: HTMLElement): void {
        
        ko.applyBindingsToNode(element, {
            template: {
                name: DualDisplayCustomControlTest.TEMPLATE_ID,
                data: this
            }
        });
        
        let cartLinesDataListOptions: Controls.IDataListOptions<ProxyEntities.CartLine> = {
            interactionMode: Controls.DataListInteractionMode.None,
            data: this._cartLinesObservable(),
            columns: [
                {
                    title: "Name", 
                    ratio: 60,
                    collapseOrder: 1,
                    minWidth: 100,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Description) ? StringExtensions.EMPTY : cartLine.Description;
                    }
                },
                {
                    title: "Quantity",
                    ratio: 20,
                    collapseOrder: 2,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Quantity) ? StringExtensions.EMPTY : cartLine.Quantity.toString();
                    }
                },
                {
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
        //this.cartLinesDataList.data = this._cartLinesObservable();
    }

    public init(state: IDualDisplayCustomControlState): void {     
        this._cart(state.cart);       
        this._cartLinesObservable(ObjectExtensions.isNullOrUndefined(this._cart()) ? [] : this._cart().CartLines);
    }
}
