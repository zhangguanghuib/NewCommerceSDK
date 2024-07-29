

import * as Views from "PosApi/Create/Views";
import { GetCurrentCartClientRequest, GetCurrentCartClientResponse } from "PosApi/Consume/Cart";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions, ArrayExtensions } from "PosApi/TypeExtensions";
import { VoidCartLineOperationRequest, VoidCartLineOperationResponse } from "PosApi/Consume/Cart";
import ko from "knockout";
type ICancelableDataResult<TResult> = ClientEntities.ICancelableDataResult<TResult>;

ko.bindingHandlers.qrcode = {
    init: function (element, valueAccessor) {
        // Create the QRCode object
        const qrCode = new QRCode(element, {
            text: ko.unwrap(valueAccessor()),
            width: 256,
            height: 256,
            colorDark: "#000000",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.H
        });

        // Update the QRCode when the value changes
        ko.computed(() => {
            const value = ko.unwrap(valueAccessor());
            qrCode.clear();  // Clear the current QR code
            qrCode.makeCode(value);  // Generate a new QR code with the new value
        });

        // Prevent Knockout from updating the DOM element itself
        return { controlsDescendantBindings: true };
    }
};

export default class VoidCartLineView extends Views.CustomViewControllerBase {
    public currentCart: ko.Observable<string>;
    public cartLineId: string;
    public qrText: ko.Observable<string>;

    /**
     * Creates a new instance of the VoidCartLineView class.
     * @param {Views.ICustomViewControllerContext} context The custom view controller context.
     * @param {any} [options] The options used to initialize the view state.
     */
    constructor(context: Views.ICustomViewControllerContext, options?: any) {
        // Do not save in history
        super(context);
        this.state.title = "Void cart line sample";
        this.currentCart = ko.observable("");
        this.cartLineId = "";
        this.qrText = ko.observable("https://www.microsoft.com");
    }

    /**
     * Bind the html element with view controller.
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {
  
        ko.applyBindings(this, element);
    }

    /**
     * Gets the current cart.
     */
    public getCurrentCart(): void {
        let getCartRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest<GetCurrentCartClientResponse>();
        this.context.runtime.executeAsync(getCartRequest).then((value: ICancelableDataResult<GetCurrentCartClientResponse>) => {
            let cart: ProxyEntities.Cart = (<GetCurrentCartClientResponse>value.data).result;
            let nonVoidedCartLines: ProxyEntities.CartLine[] = cart.CartLines.filter((cartLine: ProxyEntities.CartLine) => {
                return !cartLine.IsVoided;
            });
            if (ArrayExtensions.hasElements(nonVoidedCartLines)) {
                this.cartLineId = nonVoidedCartLines[0].LineId;
            }

            this.currentCart(JSON.stringify(cart));
        }).catch((err: any) => {
            this.currentCart(JSON.stringify(err));
        });
    }

    /**
     * Voids a cart line.
     */
    public voidCartLine(): void {
        let voidCartLineOperationRequest: VoidCartLineOperationRequest<VoidCartLineOperationResponse> =
            new VoidCartLineOperationRequest<VoidCartLineOperationResponse>(this.cartLineId, this.context.logger.getNewCorrelationId());

        this.context.runtime.executeAsync(voidCartLineOperationRequest).then((value: ICancelableDataResult<VoidCartLineOperationResponse>) => {
            if (value.canceled) {
                this.currentCart("Void cart line is canceled");
            } else {
                this.currentCart(JSON.stringify(value.data.cart));
            }
        }).catch((err: any) => {
            this.currentCart(JSON.stringify(err));
        });
    }

    /**
     * Called when the object is disposed.
     */
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}