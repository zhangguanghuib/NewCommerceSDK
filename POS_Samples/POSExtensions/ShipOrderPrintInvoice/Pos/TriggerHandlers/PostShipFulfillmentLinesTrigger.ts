import { GetHardwareProfileClientRequest, GetHardwareProfileClientResponse, GetDeviceConfigurationClientRequest, GetDeviceConfigurationClientResponse } from "PosApi/Consume/Device";
import { PrinterPrintResponse, PrinterPrintRequest } from "PosApi/Consume/Peripherals";
import { GetReceiptsClientRequest, GetReceiptsClientResponse, GetSalesOrderDetailsBySalesIdServiceRequest, GetSalesOrderDetailsBySalesIdServiceResponse } from "PosApi/Consume/SalesOrders";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import * as Triggers from "PosApi/Extend/Triggers/SalesOrderTriggers";
import { ObjectExtensions } from "PosApi/TypeExtensions";
export default class PostShipFulfillmentLinesTrigger extends Triggers.PostShipFulfillmentLinesTrigger {

    public execute(options: Triggers.IPostShipFulfillmentLinesTriggerOptions): Promise<void> {

        let correlationId: string = this.context.logger.getNewCorrelationId();

        const fulfillmentLine: ProxyEntities.FulfillmentLine = options.fulfillmentLines.length > 0 ? options.fulfillmentLines[0] : null;
        if (!ObjectExtensions.isNullOrUndefined(fulfillmentLine)) {
            let salesId: string = fulfillmentLine.SalesId;

            let req: GetSalesOrderDetailsBySalesIdServiceRequest = new GetSalesOrderDetailsBySalesIdServiceRequest(correlationId, salesId);

            return this.context.runtime.executeAsync(req).then((res: ClientEntities.ICancelableDataResult<GetSalesOrderDetailsBySalesIdServiceResponse>)
                : Promise<ProxyEntities.Receipt[]> => {

                let salesOrder: ProxyEntities.SalesOrder = res.data.salesOrder;

                return Promise.all([
                    this.context.runtime.executeAsync(new GetHardwareProfileClientRequest())
                        .then((response: ClientEntities.ICancelableDataResult<GetHardwareProfileClientResponse>): ProxyEntities.HardwareProfile => {
                            return response.data.result;
                        }),
                    this.context.runtime.executeAsync(new GetDeviceConfigurationClientRequest())
                        .then((response: ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>): ProxyEntities.DeviceConfiguration => {
                            return response.data.result;
                        })])
                    .then((results: any[]): Promise<ClientEntities.ICancelableDataResult<GetReceiptsClientResponse>> => {
                        let hardwareProfile: ProxyEntities.HardwareProfile = results[0];
                        let deviceConfiguration: ProxyEntities.DeviceConfiguration = results[1];

                        let criteria: ProxyEntities.ReceiptRetrievalCriteria = {
                            IsCopy: true,
                            IsRemoteTransaction: salesOrder.StoreId !== deviceConfiguration.StoreNumber,
                            IsPreview: false,
                            QueryBySalesId: true,
                            ReceiptTypeValue: ProxyEntities.ReceiptType.CustomReceipt6,
                            HardwareProfileId: hardwareProfile.ProfileId
                        };

                        let getReceiptsRequest: GetReceiptsClientRequest<GetReceiptsClientResponse> = new GetReceiptsClientRequest(salesOrder.SalesId ? salesOrder.SalesId : salesOrder.Id, criteria);
                        return this.context.runtime.executeAsync(getReceiptsRequest);
                    })
                    .then((response: ClientEntities.ICancelableDataResult<GetReceiptsClientResponse>): ProxyEntities.Receipt[] => {
                        return response.data.result;
                    });
            }).then((recreatedReceipts: ProxyEntities.Receipt[]): Promise<ClientEntities.ICancelableDataResult<PrinterPrintResponse>> => {
                let printRequest: PrinterPrintRequest<PrinterPrintResponse> = new PrinterPrintRequest(recreatedReceipts);
                return this.context.runtime.executeAsync(printRequest);
            }).then((): Promise<void> => {
                return Promise.resolve();
            }).catch((reason: any) => {
                console.error(reason);
                return Promise.reject(reason);
            });

        } else {
            return Promise.resolve();
        }
    }
}