import OnlineOrderReceiptPrintClientRequest from "./OnlineOrderReceiptPrintClientRequest";
import OnlineOrderReceiptPrintClientResponse from "./OnlineOrderReceiptPrintClientResponse";
import { ClientEntities, ProxyEntities} from "PosApi/Entities";
import { ExtensionRequestHandlerBase, ExtensionRequestType } from "PosApi/Create/RequestHandlers";
import PrintOnlineOrderReceiptResponse from "../Operations/PrintOnlineOrderReceiptResponse";
import { StringExtensions } from "PosApi/TypeExtensions";
import { GetDeviceConfigurationClientRequest, GetDeviceConfigurationClientResponse, GetHardwareProfileClientRequest, GetHardwareProfileClientResponse } from "PosApi/Consume/Device";
import { GetReceiptsClientRequest, GetReceiptsClientResponse, GetSalesOrderDetailsByTransactionIdClientRequest, GetSalesOrderDetailsByTransactionIdClientResponse } from "PosApi/Consume/SalesOrders";
import { StoreOperations } from "../DataService/DataServiceRequests.g";
import { PrinterPrintRequest, PrinterPrintResponse } from "PosApi/Consume/Peripherals";
import Waiter from "../Utils/Waiter";

export default class OnlineOrderReceiptPrintHandler<TResponse extends PrintOnlineOrderReceiptResponse> extends ExtensionRequestHandlerBase<TResponse>
{
    public supportedRequestType(): ExtensionRequestType<TResponse> {
        return OnlineOrderReceiptPrintClientRequest;
    }

    //public executeAsync(request: OnlineOrderReceiptPrintClientRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<TResponse>> {
    //    // console.log(request.searchCriteria);
    //    return this.processByAsyncAwait(request.searchCriteria).then(() => {
    //        let res: OnlineOrderReceiptPrintClientResponse = new OnlineOrderReceiptPrintClientResponse();
    //        return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{
    //            canceled: false,
    //            data: res
    //        });
    //    });   
    //}

    public executeAsync(request: OnlineOrderReceiptPrintClientRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<TResponse>> {
        // console.log(request.searchCriteria);
        return this.processByPromiseInSequence(request.searchCriteria).then(() => {
            let res: OnlineOrderReceiptPrintClientResponse = new OnlineOrderReceiptPrintClientResponse();
            return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{
                canceled: false,
                data: res
            });
        });
    }

    public recreateSalesReceiptsForSalesOrder(salesOrder: ProxyEntities.SalesOrder): Promise<ProxyEntities.Receipt[]> {
        let salesOrderId: string = StringExtensions.EMPTY;
        let queryBySalesId: boolean = false;

        if (!StringExtensions.isNullOrWhitespace(salesOrder.Id)
            && StringExtensions.compare(salesOrder.Id, salesOrder.SalesId, true) !== 0) {
            salesOrderId = salesOrder.Id;
        } else if (!StringExtensions.isNullOrWhitespace(salesOrder.SalesId)) {
            salesOrderId = salesOrder.SalesId;
            queryBySalesId = true;
        }

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
                    QueryBySalesId: queryBySalesId,
                    ReceiptTypeValue: ProxyEntities.ReceiptType.SalesReceipt,
                    HardwareProfileId: hardwareProfile.ProfileId
                };

                let getReceiptsRequest: GetReceiptsClientRequest<GetReceiptsClientResponse> = new GetReceiptsClientRequest(salesOrderId, criteria);
                return this.context.runtime.executeAsync(getReceiptsRequest);
            })
            .then((response: ClientEntities.ICancelableDataResult<GetReceiptsClientResponse>): ProxyEntities.Receipt[] => {
                return response.data.result;
            });
    }

    public async processByAsyncAwait(searchCriteria: ProxyEntities.TransactionSearchCriteria): Promise<void> {
        let request: StoreOperations.SearchJournalTransactionsWithUnPrintReceiptRequest<StoreOperations.SearchJournalTransactionsWithUnPrintReceiptResponse> =
            new StoreOperations.SearchJournalTransactionsWithUnPrintReceiptRequest(searchCriteria);

        let response: ClientEntities.ICancelableDataResult<StoreOperations.SearchJournalTransactionsWithUnPrintReceiptResponse> = await this.context.runtime.executeAsync(request);
        let transactions: ProxyEntities.Transaction[] = response.data.result;

        transactions.forEach(async (trans: ProxyEntities.Transaction) => {
            try {
                let waiter = new Waiter();
                await waiter.waitOneSecond();

                let req: GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>
                    = new GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>(trans.Id, ProxyEntities.SearchLocation.Local);

                let res: ClientEntities.ICancelableDataResult<GetSalesOrderDetailsByTransactionIdClientResponse> = await this.context.runtime.executeAsync(req);

                let recreatedReceipts: ProxyEntities.Receipt[] = await this.recreateSalesReceiptsForSalesOrder(res.data.result);

                let printRequest: PrinterPrintRequest<PrinterPrintResponse> = new PrinterPrintRequest(recreatedReceipts);
                await this.context.runtime.executeAsync(printRequest);

                let setTransactionPrinted: StoreOperations.SetTransactionPrintedRequest<StoreOperations.SetTransactionPrintedResponse>
                    = new StoreOperations.SetTransactionPrintedRequest(trans);
                await this.context.runtime.executeAsync(setTransactionPrinted);
            }
            finally {
                console.log(`transaction ${trans.Id} receipt printed is done`);
            }
        });
    }

    public processByPromiseInSequence(searchCriteria: ProxyEntities.TransactionSearchCriteria): Promise<void> {
        let request: StoreOperations.SearchJournalTransactionsWithUnPrintReceiptRequest<StoreOperations.SearchJournalTransactionsWithUnPrintReceiptResponse> =
            new StoreOperations.SearchJournalTransactionsWithUnPrintReceiptRequest(searchCriteria);

        return this.context.runtime.executeAsync(request).then(
            (response: ClientEntities.ICancelableDataResult<StoreOperations.SearchJournalTransactionsWithUnPrintReceiptResponse>) => {
                let transactions: ProxyEntities.Transaction[] = response.data.result;

                let arrPromise: Promise<ClientEntities.ICancelable>[] = transactions.map((trans: ProxyEntities.Transaction): Promise<ClientEntities.ICancelable> => {
                    let req: GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>
                        = new GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>(trans.Id, ProxyEntities.SearchLocation.Local);

                    return this.context.runtime.executeAsync(req).then((res: ClientEntities.ICancelableDataResult<GetSalesOrderDetailsByTransactionIdClientResponse>): Promise<ProxyEntities.Receipt[]> => {
                        return this.recreateSalesReceiptsForSalesOrder(res.data.result);
                    }).then((recreatedReceipts: ProxyEntities.Receipt[]): Promise<ClientEntities.ICancelableDataResult<PrinterPrintResponse>> => {
                        let printRequest: PrinterPrintRequest<PrinterPrintResponse> = new PrinterPrintRequest(recreatedReceipts);
                        return this.context.runtime.executeAsync(printRequest);
                    }).then((): Promise<ClientEntities.ICancelable> => {
                        console.log(`transaction ${trans.Id} receipt printed is done`);
                        return Promise.resolve({ canceled: true });
                    });
                });

                arrPromise.reduce((_prev: Promise<ClientEntities.ICancelable>, _cur: Promise<ClientEntities.ICancelable>) => {
                    return _prev.then(() => _cur);
                }, Promise.resolve({ canceled: true }));
            });
    }


}
