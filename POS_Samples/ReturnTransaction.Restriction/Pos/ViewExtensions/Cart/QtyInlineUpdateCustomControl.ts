import ko from "knockout";

import {
    CartViewCustomControlBase,
    ICartViewCustomControlState,
    ICartViewCustomControlContext,
    CartLineSelectedData
} from "PosApi/Extend/Views/CartView"

import {
    SetCartLineQuantityOperationRequest,
    SetCartLineQuantityOperationResponse,
    PriceOverrideOperationResponse,
    PriceOverrideOperationRequest
} from "PosApi/Consume/Cart";

import {
    ObjectExtensions,
    ArrayExtensions
} from "PosApi/TypeExtensions";

import { ProxyEntities, ClientEntities } from "PosApi/Entities";
import MessageHelpers from "../../Utilities/MessageHelpers";

export default class QtyInlineUpdateCustomControl extends CartViewCustomControlBase {
    private static readonly TEMPLATE_ID: string = "Pos_CustomControl_Transaction_LineDetails";
    public readonly isCartLineSelecteded: ko.Computed<boolean>;
    private readonly _cartLine: ko.Observable<ProxyEntities.CartLine>;
    public currentQty: ko.Observable<number>;
    public currentPrice: ko.Observable<number>;
    public newQty: ko.Observable<number>;
    public newPrice: ko.Observable<number>;

    private _state: ICartViewCustomControlState;

    constructor(id: string, context: ICartViewCustomControlContext) {
        super(id, context);
        this._cartLine = ko.observable(null);
        this.currentQty = ko.observable(1);
        this.currentPrice = ko.observable(0);
        this.newQty = ko.observable(1);
        this.newPrice = ko.observable(0);

        this.isCartLineSelecteded = ko.computed(() => {
            return !ObjectExtensions.isNullOrUndefined(this._cartLine);
        });

        this.cartLineSelectedHandler = (data: CartLineSelectedData) => {
            if (ArrayExtensions.hasElements(data.cartLines)) {
                this._cartLine(data.cartLines[0]);
                this.currentQty(this._cartLine().Quantity);
                this.currentPrice(this._cartLine().Price);

                this.newQty(this._cartLine().Quantity);
                this.newPrice(this._cartLine().Price);
            }
        };

        this.cartLineSelectionClearedHandler = () => {
            this._cartLine(null);
            this.currentQty(0);
            this.currentPrice(0);

            this.newQty(0);
            this.newPrice(0);
        };

        this.newQty.subscribe((newQty2: number) => {
            if (this._cartLine().Quantity != newQty2) {
                let newQty: number = Number(newQty2);
                this.setCartLineQty(newQty);
            }
        });

        this.newPrice.subscribe((newPrice2: number) => {
            if (this._cartLine().Price != newPrice2) {
                let newPrice: number = Number(newPrice2);
                this.setCartLinePrice(newPrice);
            }
        });

        this.isProcessing = false;
    }

    public onInputQtyChanged(): void {
        let newQty: number = Number(this.currentQty());
        this.setCartLineQty(newQty);
    }

    public onInputPriceChanged(): void {
        let newPrice: number = Number(this.currentPrice());
        this.setCartLinePrice(newPrice);
    }

    public increaseCartLineQty(): void {
        this.setCartLineQty(this._cartLine().Quantity + 1);
    }

    public decreaseCartLineQty(): void {
        if (this._cartLine().Quantity >= 1) {
            this.setCartLineQty(this._cartLine().Quantity - 1);
        }
    }

    private async setCartLineQty(newQty: number): Promise<boolean> {
        try {
            this.isProcessing = true;
            let setCartLineQuantityRequest: SetCartLineQuantityOperationRequest<SetCartLineQuantityOperationResponse> =
                new SetCartLineQuantityOperationRequest<SetCartLineQuantityOperationResponse>("", this._cartLine().LineId, newQty);
            let setCartLineQuantityResponse: ClientEntities.ICancelableDataResult<SetCartLineQuantityOperationResponse> =
                await this.context.runtime.executeAsync(setCartLineQuantityRequest);
            this.isProcessing = false;
            return Promise.resolve(setCartLineQuantityResponse.canceled);
        } catch (reason) {
            this.isProcessing = false;
            if (reason && reason.length >= 1) {
                if (reason[0] && reason[0]._validationFailures && reason[0]._validationFailures.length >= 1) {
                    let msg: string = reason[0]._validationFailures[0].ErrorContext;
                    MessageHelpers.ShowErrorMessage(
                        this.context,
                        msg,
                        reason
                    );
                }
            }
            return Promise.reject(new ClientEntities.ExtensionError(reason));
        }
    }

    private async setCartLinePrice(newPrice: number): Promise<boolean> {
        this.isProcessing = true;
        let request: PriceOverrideOperationRequest<PriceOverrideOperationResponse> =
            new PriceOverrideOperationRequest<PriceOverrideOperationResponse>(this._cartLine().LineId, newPrice, "");
        let response: ClientEntities.ICancelableDataResult<PriceOverrideOperationResponse> = await this.context.runtime.executeAsync(request);
        this.isProcessing = false;
        return Promise.resolve(response.canceled);
    }

    public init(state: ICartViewCustomControlState): void {
        this._state = state;
        console.log(this._state);
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindingsToNode(element, {
            template: {
                name: QtyInlineUpdateCustomControl.TEMPLATE_ID,
                data: this
            }
        })
    }
}