import { GetCurrentCartClientRequest, GetCurrentCartClientResponse } from "PosApi/Consume/Cart";
import { GetSalesOrderDetailsBySalesIdServiceRequest, GetSalesOrderDetailsBySalesIdServiceResponse, PickUpCustomerOrderLinesClientRequest, PickUpCustomerOrderLinesClientResponse } from "PosApi/Consume/SalesOrders";
import { IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchOrdersView from "PosApi/Extend/Views/SearchOrdersView";
import { StringExtensions } from "PosApi/TypeExtensions";
import MessageDialog from "./MessageDialog";

export default class SampleSearchOrdersCommand extends SearchOrdersView.SearchOrdersExtensionCommandBase {

    private _salesOrdertmp: ProxyEntities.SalesOrder;

    constructor(context: IExtensionCommandContext<SearchOrdersView.ISearchOrdersToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "sampleSearchOrdersCommand";
        this.label = "Sample search orders command";
        this.extraClass = "iconLightningBolt";
    }

    protected init(state: SearchOrdersView.ISerchOrdersExtensionCommandState): void {
        this.isVisible = true;

        this.orderSelectionHandler = (data: SearchOrdersView.SearchOrdersSelectedData): void => {
            this.canExecute = true;
            this._salesOrdertmp = data.salesOrder;
        };

        this.orderSelectionClearedHandler = () => {
            this.canExecute = false;
            this._salesOrdertmp = undefined;
        };
    }

    protected execute(): void {
        let message: string = this._salesOrdertmp.Name + " : " + this._salesOrdertmp.CreatedDateTime;
        MessageDialog.show(this.context, message).then(() => {
            this.doPickup(this.context, this._salesOrdertmp.SalesId)
        });
    }

    public doPickup(context: IOperationContext, salesId: string): Promise<void> {
        return context.runtime.executeAsync(new GetCurrentCartClientRequest()).then(
            (getCartResponse: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>):
                Promise<ClientEntities.ICancelableDataResult<GetSalesOrderDetailsBySalesIdServiceResponse>> => {
                let cartId: string = getCartResponse.data.result.Id;
                if (!StringExtensions.isEmptyOrWhitespace(cartId)) {
                    throw new ClientEntities.ExtensionError("You must complete the current transaction");
                }
                let getSalesOrderRequest: GetSalesOrderDetailsBySalesIdServiceRequest =
                    new GetSalesOrderDetailsBySalesIdServiceRequest(this.context.logger.getNewCorrelationId(), this._salesOrdertmp.SalesId);
                return context.runtime.executeAsync(getSalesOrderRequest);
            }).then((response: ClientEntities.ICancelableDataResult<GetSalesOrderDetailsBySalesIdServiceResponse>):
                Promise<ClientEntities.ICancelableDataResult<PickUpCustomerOrderLinesClientResponse>> => {
                let fulfillmentLines: ProxyEntities.FulfillmentLine[] = response.data.salesOrder.SalesLines
                    .map((orderline: ProxyEntities.SalesLine) => {
                        return <ProxyEntities.FulfillmentLine>{
                            SalesId: this._salesOrdertmp.SalesId,
                            SalesLineNumber: orderline.LineNumber,
                            CustomerId: this._salesOrdertmp.CustomerId,
                            QuantityOrdered: orderline.Quantity,
                            QuantityPicked: orderline.QuantityPicked + orderline.QuantityPacked
                        };
                    });
                let pickupOrder: PickUpCustomerOrderLinesClientRequest = new PickUpCustomerOrderLinesClientRequest(this.context.logger.getNewCorrelationId(), fulfillmentLines);
                return context.runtime.executeAsync(pickupOrder);
            }).then((pickupOrderResponse: any) => {
                context.navigator.navigateToPOSView("CartView");
                return Promise.resolve();
            });
    }
}