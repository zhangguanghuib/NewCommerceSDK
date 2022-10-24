import {
    GetCurrentCartClientRequest, GetCurrentCartClientResponse, PriceOverrideOperationResponse, SaveAttributesOnCartLinesClientRequest,
    SaveAttributesOnCartLinesClientResponse, SaveExtensionPropertiesOnCartLinesClientRequest,
    SaveExtensionPropertiesOnCartLinesClientResponse,
    SetCartLineQuantityOperationRequest,
    SetCartLineQuantityOperationResponse,
    PriceOverrideOperationRequest
} from "PosApi/Consume/Cart";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ClientEntities, ProxyEntities} from "PosApi/Entities";
import {
    FulfillmentLineExtensionCommandBase,
    FulfillmentLinePackingSlipSelectedData,
    FulfillmentLinesLoadedData,
    FulfillmentLinesSelectedData,
    IFulfillmentLineToExtensionCommandMessageTypeMap
} from "PosApi/Extend/Views/FulfillmentLineView";
import ICancelableDataResult = ClientEntities.ICancelableDataResult;

export default class FulfillmentLineCmdUpdateCartLine extends FulfillmentLineExtensionCommandBase {

    constructor(context: IExtensionCommandContext<IFulfillmentLineToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "FulfillmentLineCmdUpdateCartLine";
        this.label = "Update Cart Line";
        this.extraClass = "iconPrint";

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

    protected init(state: Commerce.Extensibility.IFulfillmentLineExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }

    protected execute(): Promise<void> {

        if (this.isProcessing) {
            return Promise.resolve();
        }

        console.log("Start update cart line");
        let correlationId: string = this.context.logger.getNewCorrelationId();
        let getCurrentCartLineClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(correlationId);

        return this.context.runtime.executeAsync(getCurrentCartLineClientRequest)
            .then((getCurrentCartClientResponse: ICancelableDataResult<GetCurrentCartClientResponse>):
                Promise<ICancelableDataResult<SaveAttributesOnCartLinesClientResponse>> => {
                if (!getCurrentCartClientResponse.canceled && getCurrentCartClientResponse.data.result.CartLines.length) {
                    console.log(getCurrentCartClientResponse.data.result.CartLines[0].LineId);
                    let cartLineId = getCurrentCartClientResponse.data.result.CartLines[0].LineId;
                    return this._saveDataToCartLineByCartLineId(cartLineId, correlationId);
                }
                return Promise.resolve({ canceled: getCurrentCartClientResponse.canceled, data: null });
            }).then((response: ICancelableDataResult<SaveAttributesOnCartLinesClientResponse>):
                Promise<ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>> =>{
                if (!response.canceled) {
                    let cartLineId: string = response.data.result.CartLines[0].LineId;
                    return this._updateAvailableQuantity(cartLineId, "1234", correlationId);
                }
                return Promise.resolve({ canceled: response.canceled, data: null });
            }).then((response: ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>):
                Promise<ClientEntities.ICancelableDataResult<SetCartLineQuantityOperationResponse>> => {
                if (!response.canceled) {
                    let cartLineId: string = response.data.result.CartLines[0].LineId;
                    return this._setQuantityForLine(cartLineId, 10, correlationId);
                }
                return Promise.resolve({ canceled: response.canceled, data: null });
            }).then((response: ClientEntities.ICancelableDataResult<SetCartLineQuantityOperationResponse>):
                Promise<ClientEntities.ICancelableDataResult<PriceOverrideOperationResponse>> => {
                if (!response.canceled) {
                    let cartLineId: string = response.data.cart.CartLines[0].LineId;
                    return this._overridePriceForLine(cartLineId, 100, correlationId);
                }
                return Promise.resolve({ canceled: response.canceled, data: null });
            }).then((response: ClientEntities.ICancelableDataResult<PriceOverrideOperationResponse>): Promise<void> => {
                let cartViewParameters: ClientEntities.CartViewNavigationParameters = new ClientEntities.CartViewNavigationParameters(correlationId);
                this.context.navigator.navigateToPOSView("CartView", cartViewParameters);
                this.isProcessing = false;
                return Promise.resolve();
            }).catch((reason: any) => {
                console.error(reason);
                this.isProcessing = false;
            });
    }

    private _saveDataToCartLineByCartLineId(cartLineId: string, correlationId: string):
        Promise<ClientEntities.ICancelableDataResult<SaveAttributesOnCartLinesClientResponse>> {

        let attributesBatchId: ProxyEntities.AttributeTextValueClass = new ProxyEntities.AttributeTextValueClass();
        attributesBatchId.Name = "BatchId";
        attributesBatchId.TextValue = "BatchId_Value";
        attributesBatchId.ExtensionProperties = new Array<ProxyEntities.CommercePropertyClass>();
        attributesBatchId.TextValueTranslations = new Array<ProxyEntities.TextValueTranslationClass>();

        let attributesOnCartLine: ClientEntities.IAttributesOnCartLine = {
            cartLineId: cartLineId,
            attributes: [attributesBatchId]
        };

        let saveExtensionPropertiesOnClientRequest: SaveAttributesOnCartLinesClientRequest<SaveAttributesOnCartLinesClientResponse>
            = new SaveAttributesOnCartLinesClientRequest([attributesOnCartLine], correlationId);

        return this.context.runtime.executeAsync(saveExtensionPropertiesOnClientRequest);
    }

    private _updateAvailableQuantity(cartLineId: string, value: string, correlationId: string):
        Promise<ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>> {

        let cartLineExtensionProperty:ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        cartLineExtensionProperty.Key = "QuantityAvailable";
        cartLineExtensionProperty.Value = new ProxyEntities.CommercePropertyValueClass();
        cartLineExtensionProperty.Value.StringValue = '1';

        let PriceSensitiveProperty: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        PriceSensitiveProperty.Key = "PriceSensitive";
        PriceSensitiveProperty.Value = new ProxyEntities.CommercePropertyValueClass();
        PriceSensitiveProperty.Value.StringValue = '0';

        let extensionPropertiesOnCartLine: ClientEntities.IExtensionPropertiesOnCartLine = {
            cartLineId: cartLineId,
            extensionProperties: [cartLineExtensionProperty, PriceSensitiveProperty]
        };

        let saveExtensionPropertiesOnCartLinesClientRequest:SaveExtensionPropertiesOnCartLinesClientRequest<SaveExtensionPropertiesOnCartLinesClientResponse> =
            new SaveExtensionPropertiesOnCartLinesClientRequest([extensionPropertiesOnCartLine], correlationId);

        return this.context.runtime.executeAsync(saveExtensionPropertiesOnCartLinesClientRequest)
    }


    private _setQuantityForLine(cartLineId: string, quantity:number, correlationId: string):
        Promise<ClientEntities.ICancelableDataResult<SetCartLineQuantityOperationResponse>> {

        let setCartLineQuantityOperationRequest:SetCartLineQuantityOperationRequest<SetCartLineQuantityOperationResponse> =
            new SetCartLineQuantityOperationRequest<SetCartLineQuantityOperationResponse>(correlationId, cartLineId, quantity);

        return this.context.runtime.executeAsync(setCartLineQuantityOperationRequest); 
    }

    private _overridePriceForLine(cartLineId:string, price:number, correlationId :string):
    Promise<ClientEntities.ICancelableDataResult<PriceOverrideOperationResponse>> {

        let priceString: string = 98 + price.toString();
        price = parseFloat(priceString);
        let priceOverrideOperationRequest:PriceOverrideOperationRequest<PriceOverrideOperationResponse> =
            new PriceOverrideOperationRequest<PriceOverrideOperationResponse>(cartLineId, price, "BatchAndVintageOverride");

        return this.context.runtime.executeAsync(priceOverrideOperationRequest);
    }
}