import ko from "knockout";

import {
    CartViewCustomControlBase,
    ICartViewCustomControlState,
    ICartViewCustomControlContext,
    CartLineSelectedData
} from "PosApi/Extend/Views/CartView";

import {
    ObjectExtensions,
    StringExtensions,
    ArrayExtensions
} from "PosApi/TypeExtensions";

import { ProxyEntities } from "PosApi/Entities";

export default class LineDetailsCustomControl extends CartViewCustomControlBase {
    private static readonly TEMPLATE_ID: string = "Microsoft_Pos_Extensibility_Samples_LineDetails";
    public readonly cartLineItemId: ko.Computed<string>;
    public readonly cartLineDescription: ko.Computed<string>;
    public readonly isCartLineSelected: ko.Computed<boolean>;
    private readonly _cartLine: ko.Observable<ProxyEntities.CartLine>;
    public _state: ICartViewCustomControlState;

    public constructor(id: string, context: ICartViewCustomControlContext) {
        super(id, context);
        this._cartLine = ko.observable(null);

        this.cartLineItemId = ko.computed(() => {
            let cartLine: ProxyEntities.CartLine = this._cartLine();
            if (!ObjectExtensions.isNullOrUndefined(cartLine)) {
                return cartLine.ItemId;
            }
            return StringExtensions.EMPTY;
        });

        this.cartLineDescription = ko.computed(() => {
            let cartLine: ProxyEntities.CartLine = this._cartLine();
            if (!ObjectExtensions.isNullOrUndefined(cartLine)) {
                return cartLine.Description;
            }
            return StringExtensions.EMPTY;
        });

        this.isCartLineSelected = ko.computed(() => !ObjectExtensions.isNullOrUndefined(this._cartLine()));
        this.cartLineSelectedHandler = (data: CartLineSelectedData) => {
            if (ArrayExtensions.hasElements(data.cartLines)) {
                this._cartLine(data.cartLines[0]);
            }
        };

        this.cartLineSelectionClearedHandler = () => {
            this._cartLine(null);
        };
    }

    /**
    * Binds the control to the specified element.
    * @param {HTMLElement} element The element to which the control should be bound.
    */
    public onReady(element: HTMLElement): void {
        ko.applyBindingsToNode(element, {
            template: {
                name: LineDetailsCustomControl.TEMPLATE_ID,
                data: this
            }
        }, this);
    }

    /**
    * Initializes the control.
    * @param {ICartViewCustomControlState} state The initial state of the page used to initialize the control.
    */
    public init(state: ICartViewCustomControlState): void {
        this._state = state;
    }
}