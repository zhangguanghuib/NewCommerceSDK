import * as Triggers from "PosApi/Extend/Triggers/InventoryTriggers";
import { ProxyEntities, ClientEntities } from "PosApi/Entities";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import * as Messages from "../DataService/DataServiceRequests.g";

export default class PostGetInventoryDocumentProductQuantityTrigger extends Triggers.PostGetInventoryDocumentProductQuantityTrigger {
    public execute(options: Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions)
        : Promise<Commerce.Triggers.CancelableTriggerResult<Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions>> {
        let productExtensionProperties: ProxyEntities.CommerceProperty[] = options.product.ExtensionProperties;
        let barcodeUnitId: string = "";
        let itemBarcode: string = "";
        if (!ObjectExtensions.isNullOrUndefined(productExtensionProperties)) {
            let unitExtensionProperty: ProxyEntities.CommerceProperty =
                Commerce.ArrayExtensions.firstOrUndefined(productExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                    return property.Key === "unitId";
                });

            if (!ObjectExtensions.isNullOrUndefined(unitExtensionProperty)) {
                barcodeUnitId = unitExtensionProperty.Value.StringValue;
            };

            let itemBarcodeExtensionProperty: ProxyEntities.CommerceProperty =
                Commerce.ArrayExtensions.firstOrUndefined(productExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                    return property.Key === "itemBarcode";
                });
            if (!ObjectExtensions.isNullOrUndefined(itemBarcodeExtensionProperty)) {
                itemBarcode = itemBarcodeExtensionProperty.Value.StringValue;
            }
        }
        console.log(itemBarcode);

        let fromUoM: string = barcodeUnitId;
        let fromQty: number = options.quantity;
        let itemId: string = options.product.ItemId;
        let productId: number = options.product.RecordId;
        let convertedQty: number = 0;

        if (StringExtensions.isEmptyOrWhitespace(itemBarcode) || StringExtensions.isEmptyOrWhitespace(fromUoM)) {
            return Promise.resolve(new Commerce.Triggers.CancelableTriggerResult<Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions>(false, options));
        }

        let request: Messages.StoreOperations.GetInventoryUnitOfMeasureByItemIdRequest<Messages.StoreOperations.GetInventoryUnitOfMeasureByItemIdResponse>
            = new Messages.StoreOperations.GetInventoryUnitOfMeasureByItemIdRequest<Messages.StoreOperations.GetInventoryUnitOfMeasureByItemIdResponse>(productId, itemId);

        return this.context.runtime.executeAsync(request).then((response: ClientEntities.ICancelableDataResult<Messages.StoreOperations.GetInventoryUnitOfMeasureByItemIdResponse>) => {
            if (!response.canceled) {
                let unitOfMeasure: ProxyEntities.UnitOfMeasure = response.data.result;
                if (!ObjectExtensions.isNullOrUndefined(unitOfMeasure)) {
                    let toUoM: string = unitOfMeasure.Symbol;
                    if (toUoM === fromUoM) {
                        return Promise.resolve(new Commerce.Triggers.CancelableTriggerResult<Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions>(false, options));
                    }
                    let conversionRequest: Messages.StoreOperations.ConvertUnitOfMeasureRequest<Messages.StoreOperations.ConvertUnitOfMeasureResponse>
                        = new Messages.StoreOperations.ConvertUnitOfMeasureRequest<Messages.StoreOperations.ConvertUnitOfMeasureResponse>(productId, itemId, fromUoM, toUoM, fromQty);
                    return this.context.runtime.executeAsync(conversionRequest).then((response: ClientEntities.ICancelableDataResult<Messages.StoreOperations.ConvertUnitOfMeasureResponse>) => {
                        if (!response.canceled) {
                            console.log(response.data.result);
                            convertedQty = response.data.result;
                            let newOptions: Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions = {
                                ...options, // Spread operator to copy all properties from options
                                ...options, // Ensures inherited properties from ITriggerOptions are also copied
                                quantity: convertedQty
                            };
                            return Promise.resolve(new Commerce.Triggers.CancelableTriggerResult<Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions>(false, newOptions));
                        } else {
                            return Promise.resolve(new Commerce.Triggers.CancelableTriggerResult<Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions>(false, options));
                        }
                    });
                } else {
                    return Promise.resolve(new Commerce.Triggers.CancelableTriggerResult<Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions>(false, options));
                }
            } else {
                return Promise.resolve(new Commerce.Triggers.CancelableTriggerResult<Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions>(false, options));
            }
        })
           

        //let userInputQty = options.quantity;
        //if (barcodeUnitId === 'pr') {
        //    convertedQty = userInputQty * 2;
        //} else if (barcodeUnitId == 'dz') {
        //    convertedQty = userInputQty * 12;
        //}

        //let newOptions: Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions = {
        //    ...options, // Spread operator to copy all properties from options
        //    ...options, // Ensures inherited properties from ITriggerOptions are also copied
        //    quantity: convertedQty
        //};

        // return Promise.resolve(new Commerce.Triggers.CancelableTriggerResult<Triggers.IPostGetInventoryDocumentProductQuantityTriggerOptions>(false, options));
    }

}