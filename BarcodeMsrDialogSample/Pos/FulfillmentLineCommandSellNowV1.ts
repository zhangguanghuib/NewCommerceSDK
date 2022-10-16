// This code is demonstrating a way to skip serial number dialog and assign serial number to cart line with code
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
import { GetProductsByIdsClientRequest, GetProductsByIdsClientResponse, SelectProductVariantClientRequest, SelectProductVariantClientResponse } from "PosApi/Consume/Products";
import { StoreOperations } from "./DataService/DataServiceRequests.g";

interface IPreDefinedDimension {
    DimensionType: string,
    DimensionValue: string
};

export default class FulfillmentLineCommandSellNowV1 extends FulfillmentLineExtensionCommandBase {

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

        // Product informtion
        let serailNumber: string = "1234";
        let productIds: number[] = [68719504871];//TestColorSize
        let unitEa: string = "ea";

        let predefinedDimensions: Array<IPreDefinedDimension> = [{ DimensionType: '3', DimensionValue: "XSmall" }, { DimensionType: '1', DimensionValue: "Black" }];

        let cart: ProxyEntities.Cart = null;
        let correlationId: string = this.context.logger.getNewCorrelationId();
        let getProductsByIdsClientRequest: GetProductsByIdsClientRequest<GetProductsByIdsClientResponse> = new GetProductsByIdsClientRequest(productIds, correlationId);

        return this.context.runtime.executeAsync(getProductsByIdsClientRequest).then
            ((getProductsByIdsClientResponse: ClientEntities.ICancelableDataResult<GetProductsByIdsClientResponse>): Promise<ProxyEntities.SimpleProduct> => {
                // If no product returned
                if (getProductsByIdsClientResponse.canceled
                    || ObjectExtensions.isNullOrUndefined(getProductsByIdsClientResponse.data)
                    || ObjectExtensions.isNullOrUndefined(getProductsByIdsClientResponse.data.products)
                    || getProductsByIdsClientResponse.data.products.length < 1) {
                    return Promise.reject("productSaleDetails is not correctly created");
                }
                return Promise.resolve(getProductsByIdsClientResponse.data.products[0]);
            }).then((simpleProduct: ProxyEntities.SimpleProduct): Promise<ProxyEntities.SimpleProduct> => {
                return this.retrieveProductDimensionValuev1(simpleProduct, predefinedDimensions);
            }).then((simpleProduct: ProxyEntities.SimpleProduct): Promise<ClientEntities.ICancelableDataResult<ProxyEntities.SimpleProduct>> => {
                if (simpleProduct.ProductTypeValue === ProxyEntities.ProductType.Master) { // this is a product master
                    let selectProductVariantClientRequest: SelectProductVariantClientRequest<SelectProductVariantClientResponse> = new SelectProductVariantClientRequest(simpleProduct, simpleProduct.Dimensions);
                    return this.context.runtime.executeAsync(selectProductVariantClientRequest).then(
                        (selectProductVariantClientResponse: ClientEntities.ICancelableDataResult<SelectProductVariantClientResponse>): Promise<ClientEntities.ICancelableDataResult<ProxyEntities.SimpleProduct>> => {
                            if (!selectProductVariantClientResponse.canceled &&
                                selectProductVariantClientResponse.data.result.ProductTypeValue === ProxyEntities.ProductType.Variant) {
                                return Promise.resolve({ canceled: false, data: selectProductVariantClientResponse.data.result });
                            }
                            else {
                                return Promise.resolve({ canceled: true, data: null });
                            }
                        }
                    )
                }
                else {
                    return Promise.resolve({ canceled: false, data: simpleProduct });
                }
            }).then((productVariantResult: ClientEntities.ICancelableDataResult<ProxyEntities.SimpleProduct>): Promise<ClientEntities.IProductSaleReturnDetails> => {
                if (!productVariantResult.canceled) {
                    let productSaleDetails: ClientEntities.IProductSaleReturnDetails =
                    {
                        product: productVariantResult.data,
                        quantity: 1,
                        unitOfMeasureSymbol: unitEa
                    };
                    return Promise.resolve(productSaleDetails);
                } else {
                    return Promise.reject("productSaleDetails is not correctly created");
                }
            }).then((productSaleDetails: ClientEntities.IProductSaleReturnDetails): Promise<ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>> => {

                let request: AddItemToCartOperationRequest<AddItemToCartOperationResponse>
                    = new AddItemToCartOperationRequest<AddItemToCartOperationResponse>([productSaleDetails], correlationId);

                return this.context.runtime.executeAsync(request).then((result: ClientEntities.ICancelableDataResult<AddItemToCartOperationResponse>):
                    Promise<ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>> => {
                    if (result.canceled) {
                        return Promise.reject("Add item to cart operation is cancelled");
                    }
                    cart = result.data.cart;
                    let cartLine: ProxyEntities.CartLine = cart.CartLines[cart.CartLines.length - 1];
                    cartLine.SerialNumber = serailNumber;
                    return this.saveDataToCartLineByCartLineId(cartLine.LineId, "TestServicingId", correlationId, cart);                 
                });
            }).then((saveExtensionPropertiesOnCartLinesClientResponse: ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>): Promise<void> => {
                 let cartViewParameters: ClientEntities.CartViewNavigationParameters = new ClientEntities.CartViewNavigationParameters(correlationId);
                    this.context.navigator.navigateToPOSView("CartView", cartViewParameters);
                    this.isProcessing = false;
                    return Promise.resolve();
            });
    }

    //https://msazure.visualstudio.com/D365/_git/Retail-Rainier-Samples?path=%2Fsrc%2FPOS%2FExtensions%2FSampleExtensions%2FViewExtensions%2FCustomerDetails%2FCustomerDetailsFriendsPanel.ts&_a=contents&version=GBmaster
    private retrieveProductDimensionValuev1(simpleProduct: ProxyEntities.SimpleProduct, predefinedDimensions: Array<IPreDefinedDimension>):
        Promise<ProxyEntities.SimpleProduct> {

        if (simpleProduct.ProductTypeValue === ProxyEntities.ProductType.Master) { // this is a product master
            let channelId: number = 5637144592;
            let dimensionPromises: Promise<void>[] = [];

            simpleProduct.Dimensions.forEach(
                (dimension: ProxyEntities.ProductDimension): void => {
                    let jewelryGetDimensionValuesRequest: StoreOperations.JewelryGetDimensionValuesRequest<StoreOperations.JewelryGetDimensionValuesResponse>
                        = new StoreOperations.JewelryGetDimensionValuesRequest<StoreOperations.JewelryGetDimensionValuesResponse>(
                            channelId, simpleProduct.RecordId, dimension.DimensionTypeValue.toString());

                    let dimensionPromise: Promise<void>
                        = this.context.runtime.executeAsync(jewelryGetDimensionValuesRequest).then(
                            (jewelryGetDimensionValuesResponse: ClientEntities.ICancelableDataResult<StoreOperations.JewelryGetDimensionValuesResponse>): void => {
                                if (!jewelryGetDimensionValuesResponse.canceled) {
                                    jewelryGetDimensionValuesResponse.data.result.length;
                                    let preDefinedDimensionValues = predefinedDimensions.filter(t => t.DimensionType === dimension.DimensionTypeValue.toString());
                                    if (preDefinedDimensionValues.length === 1) {
                                        let dimensionValue = jewelryGetDimensionValuesResponse.data.result.filter(t => t.DimensionId === preDefinedDimensionValues[0].DimensionValue)
                                        if (dimensionValue.length === 1) {
                                            console.table(dimensionValue[0]);
                                            dimension.DimensionValue = dimensionValue[0];
                                        }
                                    }
                                }
                            });
                    dimensionPromises.push(dimensionPromise);
                });

            // Wait for all promise got resolved
            return Promise.all(dimensionPromises).then(() => {
                return Promise.resolve(simpleProduct);
            }).catch((reason: any) => {
                return Promise.reject("Fail to get dimension values");
                });
        }
        else {
            // Standalone product
            return Promise.resolve(simpleProduct);
        }
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