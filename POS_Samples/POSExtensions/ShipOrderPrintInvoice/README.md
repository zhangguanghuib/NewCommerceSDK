# The sample code is to show how to print the invoice receipt when ship a customer order

## The steps on Store Commerce are:

1. Create a customer order, ship it from HOUSTON
2. Pay Deposit and checkout the cart
3. Recall order and find "Orders to Ship"
4. Click Ship button
5. Once ship order is successful,  it will call RTS to print the sales order invoice in F&O

##  The POS Trigger entry point is here:
```TS
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
```

## The CRT Code is 
```CS
namespace Contoso
{
    namespace Commerce.Runtime.ReceiptsSample
    {
        using System.Collections.ObjectModel;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        public class GetCustomReceiptsRequestHandler : SingleAsyncRequestHandler<GetCustomReceiptsRequest>
        {
            /// <summary>
            /// Processes the GetCustomReceiptsRequest to return the set of receipts. The request should not be null.
            /// </summary>
            /// <param name="request">The request parameter.</param>
            /// <returns>The GetReceiptResponse.</returns>
            protected override async Task<Response> Process(GetCustomReceiptsRequest request)
            {
                ThrowIf.Null(request, "request");
                ThrowIf.Null(request.ReceiptRetrievalCriteria, "request.ReceiptRetrievalCriteria");

                // 1. We need to get the sales order that we are print receipts for.
                //var getCustomReceiptsRequest = new GetSalesOrderDetailsByTransactionIdServiceRequest(request.TransactionId, SearchLocation.All);
                //var getSalesOrderDetailsServiceResponse = await request.RequestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(getCustomReceiptsRequest).ConfigureAwait(false);

                GetSalesOrderDetailsBySalesIdServiceRequest getSalesOrderDetailsBySalesIdServiceRequest = new GetSalesOrderDetailsBySalesIdServiceRequest(request.TransactionId, SearchLocation.All);
                GetSalesOrderDetailsServiceResponse getSalesOrderDetailsServiceResponse = await request.RequestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(getSalesOrderDetailsBySalesIdServiceRequest).ConfigureAwait(false);

                if (getSalesOrderDetailsServiceResponse == null ||
                    getSalesOrderDetailsServiceResponse.SalesOrder == null)
                {
                    throw new DataValidationException(
                        DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ObjectNotFound,
                        string.Format("Unable to get the sales order created. ID: {0}", request.TransactionId));
                }

                SalesOrder salesOrder = getSalesOrderDetailsServiceResponse.SalesOrder;

                if(request.ReceiptRetrievalCriteria.ReceiptType == ReceiptType.CustomReceipt6)
                {
                    InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest("printSalesInvoice", salesOrder.SalesId);
                    InvokeExtensionMethodRealtimeResponse response = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);
                    ReadOnlyCollection<object> results = response.Result;

                    string resValue = (string)results[0];
                }

                Collection<Receipt> result = new Collection<Receipt>();

                return new GetReceiptResponse(new ReadOnlyCollection<Receipt>(result));
            }
        }
    }
}
```

##  The Realtime Service in FnO is 
```xpp
[ExtensionOf(classStr(RetailTransactionServiceEx))]
public final class ContosoRetailTransactionService_Extension
{
    public static container printSalesInvoice(SalesId _salesId)
    {
        boolean success = true;
        str message = '';
        int fromLine;
        CustInvoiceJour         custInvoiceJour;
      
        try
        {
            select firstonly custInvoiceJour
                order by custInvoiceJour.InvoiceId desc
                where custInvoiceJour.SalesId == _salesId;

            if (custInvoiceJour)
            {
                str ext = SRSPrintDestinationSettings::findFileNameType(SRSReportFileFormat::PDF, SRSImageFileFormat::BMP);
                PrintMgmtReportFormatName printMgmtReportFormatName = PrintMgmtDocType::construct(PrintMgmtDocumentType::SalesOrderInvoice).getDefaultReportFormat();
                                     
                SalesInvoiceContract salesInvoiceContract = new SalesInvoiceContract();
                salesInvoiceContract.parmRecordId(custInvoiceJour.RecId);

                SrsReportRunController  srsReportRunController = new SrsReportRunController();
                srsReportRunController.parmReportName(printMgmtReportFormatName);
                srsReportRunController.parmExecutionMode(SysOperationExecutionMode::Synchronous);
                srsReportRunController.parmShowDialog(false);
                srsReportRunController.parmReportContract().parmRdpContract(salesInvoiceContract);

                SRSPrintDestinationSettings printerSettings = srsReportRunController.parmReportContract().parmPrintSettings();
                printerSettings.printMediumType(SRSPrintMediumType::File);
                printerSettings.fileFormat(SRSReportFileFormat::PDF);
                printerSettings.parmFileName(custInvoiceJour.InvoiceId + ext);
                printerSettings.overwriteFile(true);
                    
                srsReportRunController.startOperation();

                success = true;
                message = 'Sales Invoice Print Successfully';
            }
        }
        catch (Exception::Error)
        {
            success = false;
            message = RetailTransactionServiceUtilities::getInfologMessages(fromLine);
        }

        return [success, "", message];
    }
}
```
