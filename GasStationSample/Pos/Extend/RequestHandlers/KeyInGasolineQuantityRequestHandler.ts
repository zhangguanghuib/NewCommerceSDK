import { GetCurrentCartClientRequest, GetCurrentCartClientResponse, GetKeyedInQuantityClientRequest, GetKeyedInQuantityClientResponse } from "PosApi/Consume/Cart";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { GetKeyedInQuantityClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";
import { ArrayExtensions, ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";

import { GasStationDataStore } from "../../GasStationDataStore";
import { GASOLINE_QUANTITY_EXTENSION_PROPERTY_NAME } from "../../GlobalConstants";
import { NumberFormattingHelper } from "../../NumberFormattingHelper";

type RequestHandlerResult = ClientEntities.ICancelableDataResult<GetKeyedInQuantityClientResponse>;

export default class AutomatedKeyInGasolineQuantityRequestHandler extends GetKeyedInQuantityClientRequestHandler {

    public executeAsync(request: GetKeyedInQuantityClientRequest<GetKeyedInQuantityClientResponse>): Promise<RequestHandlerResult> {

        if (StringExtensions.compare(request.product.ItemId, GasStationDataStore.instance.gasStationDetails.GasolineItemId, true) !== 0) {
            return this.defaultExecuteAsync(request);
        }

        let getCartRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(request.correlationId);
        return this.context.runtime.executeAsync(getCartRequest)
            .then((result: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>): Promise<RequestHandlerResult> => {
                if (result.canceled) {
                    return Promise.resolve({ canceled: true, data: null });
                }

                let cart: ProxyEntities.Cart = result.data.result;
                let quantityExtensionProperty: ProxyEntities.CommerceProperty
                    = ArrayExtensions.firstOrUndefined(cart?.ExtensionProperties, (p) => p.Key === GASOLINE_QUANTITY_EXTENSION_PROPERTY_NAME);

                if (!StringExtensions.isNullOrWhitespace(cart.Id) && !ObjectExtensions.isNullOrUndefined(quantityExtensionProperty)) {
                    let decimalValue: number = NumberFormattingHelper.roundToNdigits(parseFloat(quantityExtensionProperty.Value.StringValue), 3);
                    return Promise.resolve({ canceled: false, data: new GetKeyedInQuantityClientResponse(decimalValue) });
                } else {
                    return this.defaultExecuteAsync(request);
                }

            });
        
    }

}