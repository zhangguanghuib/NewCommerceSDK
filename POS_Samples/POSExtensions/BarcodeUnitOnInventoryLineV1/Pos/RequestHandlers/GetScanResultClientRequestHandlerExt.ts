
import { GetScanResultClientRequestHandler } from "PosApi/Extend/RequestHandlers/ScanResultsRequestHandlers";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { GetScanResultClientRequest, GetScanResultClientResponse } from "PosApi/Consume/ScanResults";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class GetScanResultClientRequestHandlerExt extends GetScanResultClientRequestHandler {

    /**
     * Executes the request handler asynchronously.
     * @param {GetScanResultClientRequest<GetScanResultClientResponse>} The request containing the response.
     * @return {Promise<ICancelableDataResult<GetScanResultClientResponse>>} The cancelable promise containing the response.
     */
    public executeAsync(request: GetScanResultClientRequest<GetScanResultClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<GetScanResultClientResponse>> {
        let newBarCode: string = request.scanText;
        // User could implement new business logic here to process the bar code.
        // Following example take the first 13 characters as the new bar code.
        // newBarCode = newBarCode.substr(0, 13);

        let newRequest: GetScanResultClientRequest<GetScanResultClientResponse> =
            new GetScanResultClientRequest<GetScanResultClientResponse>(newBarCode);

        return this.defaultExecuteAsync(newRequest).then(
            (response: ClientEntities.ICancelableDataResult<GetScanResultClientResponse>): Promise<ClientEntities.ICancelableDataResult<GetScanResultClientResponse>> =>
            {
                if (!ObjectExtensions.isNullOrUndefined(response.data.result.Product)
                    && !ObjectExtensions.isNullOrUndefined(response.data.result?.Barcode?.ItemBarcode)) {
                    const itemBarcode: ProxyEntities.ItemBarcode = response.data.result?.Barcode?.ItemBarcode;
                    const barcodeUnitId: string = itemBarcode.UnitId;
                    const barcode: string = itemBarcode.ItemBarcodeValue;

                    let unitIdExtensionProperty: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
                    unitIdExtensionProperty.Key = "unitId";
                    unitIdExtensionProperty.Value = new ProxyEntities.CommercePropertyValueClass();
                    unitIdExtensionProperty.Value.StringValue = barcodeUnitId;

                    let itemBarcodeExtensionProperty: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
                    itemBarcodeExtensionProperty.Key = "itemBarcode";
                    itemBarcodeExtensionProperty.Value = new ProxyEntities.CommercePropertyValueClass();
                    itemBarcodeExtensionProperty.Value.StringValue = barcode;

                    const product: ProxyEntities.SimpleProduct = response.data.result.Product;
                    product.ExtensionProperties.push(unitIdExtensionProperty);
                    product.ExtensionProperties.push(itemBarcodeExtensionProperty);
                }
                return Promise.resolve({
                    canceled: response.canceled,
                    data: response.data,
                } as ClientEntities.ICancelableDataResult<GetScanResultClientResponse>);
            });   
    }
}