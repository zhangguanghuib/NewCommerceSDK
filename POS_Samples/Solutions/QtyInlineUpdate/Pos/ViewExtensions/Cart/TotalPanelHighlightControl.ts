import { CartChangedData, CartViewCustomControlBase, ICartViewCustomControlContext } from "PosApi/Extend/Views/CartView";
import ko from "knockout";
import { ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class TotalPanelHighlightControl extends CartViewCustomControlBase {
    public readonly numOfCartLines: ko.Computed<string>;
    public readonly numOfCartLinesLabel: string;
    private readonly _currentCart: ko.Observable<ProxyEntities.Cart>;

    private static readonly TEMPLATE_ID: string = "Microsot_Pos_Extensibility_Samples_TotalPanelHighlightControl";

    protected init(state: Commerce.Cart.Extensibility.ICartViewCustomControlState): void {
        this._currentCart(state.cart);
    }

    onReady(element: HTMLElement): void {
        ko.applyBindingsToNode(element, {
            template: {
                name: TotalPanelHighlightControl.TEMPLATE_ID,
                data: this
            }
        });
    }

    constructor(id: string, context: ICartViewCustomControlContext) {
        super(id, context);

        this.numOfCartLinesLabel = "LINES:";
        this._currentCart = ko.observable(null);

        this.numOfCartLines = ko.computed(() => {
            if (!ObjectExtensions.isNullOrUndefined(this._currentCart())) {
                let s: string = this._currentCart().CartLines.length + '';
                console.log(s);
                return this._currentCart().CartLines.length+'';
            }
            return '0';
        });

        this.cartChangedHandler = (data: CartChangedData) => {
            this._currentCart(data.cart);
        }
    }

}