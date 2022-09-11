import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ObjectExtensions } from "PosApi/TypeExtensions";

import {
    FulfillmentLineExtensionCommandBase,
    FulfillmentLinesSelectedData,
    FulfillmentLinePackingSlipSelectedData,
    FulfillmentLinesLoadedData,
    IFulfillmentLineExtensionCommandState,
    IFulfillmentLineToExtensionCommandMessageTypeMap
} from "PosApi/Extend/Views/FulfillmentLineView";

import { AddItemToCartOperationRequest, AddItemToCartOperationResponse, SaveExtensionPropertiesOnCartLinesClientResponse, SaveExtensionPropertiesOnCartLinesClientRequest } from "PosApi/Consume/Cart";
import { GetProductsByIdsClientRequest, GetProductsByIdsClientResponse } from "PosApi/Consume/Products";

export default class FulfillmentLineCommandSellNow extends FulfillmentLineExtensionCommandBase {

    constructor(context: IExtensionCommandContext<IFulfillmentLineToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "sampleFufillmentLineCommandSellNow";
        this.label = "Sell Now";
        this.extraClass = "iconBuy";

        this.fulfillmentLinesSelectionHandler = (data: FulfillmentLinesSelectedData): void => {
        }

        this.fulfillmentLinesSelectionClearedHandler = (): void => {
        }

        this.packingSlipSelectedHandler = (data: FulfillmentLinePackingSlipSelectedData): void => {
            this.isVisible = false;
        }

        this.packingSlipSelectionClearedHandler = (): void => {
            this.isVisible = false;
        }

        this.fulfillmentLinesLoadedHandler = (data: FulfillmentLinesLoadedData): void => {
        }
    }

    protected init(state: IFulfillmentLineExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }

    protected execute(): Promise<void> {
        if (this.isProcessing) {
            return Promise.resolve();
        }

        this.isProcessing = true;
        let serailNumber: string = "1234";
        let cart: ProxyEntities.Cart = null;
        let correlationId: string = this.context.logger.getNewCorrelationId();
        let getProductsByIdsClientRequest: GetProductsByIdsClientRequest<GetProductsByIdsClientResponse> = new GetProductsByIdsClientRequest([68719500371], correlationId);

        return this.context.runtime.executeAsync(getProductsByIdsClientRequest).then
            ((getProductsByIdsClientResponse: ClientEntities.ICancelableDataResult<GetProductsByIdsClientResponse>): ClientEntities.IProductSaleReturnDetails => {
                if (!getProductsByIdsClientResponse.canceled
                    && !ObjectExtensions.isNullOrUndefined(getProductsByIdsClientResponse.data)
                    && !ObjectExtensions.isNullOrUndefined(getProductsByIdsClientResponse.data.products)
                    && getProductsByIdsClientResponse.data.products.length >= 1) {
                    let simpleProduct: ProxyEntities.SimpleProduct = getProductsByIdsClientResponse.data.products[0];

                    if (simpleProduct.Behavior.HasSerialNumber && serailNumber) {
                        simpleProduct.Behavior.HasSerialNumber = false;
                    }

                    let productSaleDetails: ClientEntities.IProductSaleReturnDetails =
                    {
                        product: simpleProduct,
                        quantity: 1,
                        unitOfMeasureSymbol: 'ea'
                    };
                    return productSaleDetails;
                } else {
                    return null;
                }
            }).then((productSaleDetails: ClientEntities.IProductSaleReturnDetails): Promise<void> => {
                let request: AddItemToCartOperationRequest<AddItemToCartOperationResponse>
                    = new AddItemToCartOperationRequest<AddItemToCartOperationResponse>([productSaleDetails], correlationId);

                return this.context.runtime.executeAsync(request).then((result: ClientEntities.ICancelableDataResult<AddItemToCartOperationResponse>) => {
                    if (!result.canceled) {
                        cart = result.data.cart;
                    }

                    let cartLine: ProxyEntities.CartLine = cart.CartLines[cart.CartLines.length - 1];
                    cartLine.SerialNumber = serailNumber;

                    this.saveDataToCartLineByCartLineId(cartLine.LineId, "TestServicingId", correlationId, cart);

                    let cartViewParameters: ClientEntities.CartViewNavigationParameters = new ClientEntities.CartViewNavigationParameters(correlationId);
                    this.context.navigator.navigateToPOSView("CartView", cartViewParameters);
                    this.isProcessing = false;
                })
            }).catch((reason: any) => {
                this.isProcessing = false;
                this.context.logger.logError("Addinf item to cart failed for" + JSON.stringify(reason));
                return Promise.reject(reason);
            });
    }

    private saveDataToCartLineByCartLineId(cartLineId: string, servicingNoteId: string, correlationId: string, cart: ProxyEntities.Cart):
        Promise<ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>> {

        let cartLineExtensionProperty_servicingNoteId: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        cartLineExtensionProperty_servicingNoteId.Key = "servicingNoteId";
        cartLineExtensionProperty_servicingNoteId.Value = new ProxyEntities.CommercePropertyValueClass();
        cartLineExtensionProperty_servicingNoteId.Value.StringValue = servicingNoteId;

        let cartLineExtensionProperty_SerialNumber: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        cartLineExtensionProperty_SerialNumber.Key = "THK_SerialNumber";
        cartLineExtensionProperty_SerialNumber.Value = new ProxyEntities.CommercePropertyValueClass();
        cartLineExtensionProperty_SerialNumber.Value.StringValue = servicingNoteId;

        let extensionPropertiesOnCartLine: ClientEntities.IExtensionPropertiesOnCartLine = {
            cartLineId: cartLineId,
            extensionProperties: [cartLineExtensionProperty_servicingNoteId, cartLineExtensionProperty_SerialNumber]
        };

        let saveExtensionPropertiesOnCartLinesClientRequest: SaveExtensionPropertiesOnCartLinesClientRequest<SaveExtensionPropertiesOnCartLinesClientResponse> =
            new SaveExtensionPropertiesOnCartLinesClientRequest([extensionPropertiesOnCartLine], correlationId);

        return this.context.runtime.executeAsync(saveExtensionPropertiesOnCartLinesClientRequest)
            .then((response: ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>) => {
                if (response.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>>{ canceled: true, data: null });
                }

                return Promise.resolve(<ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>>
                    {
                        canceled: false,
                        data: new SaveExtensionPropertiesOnCartLinesClientResponse(cart)
                    });
            })
    }

}