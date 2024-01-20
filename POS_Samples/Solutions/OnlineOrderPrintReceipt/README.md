# Online Order Print via Hardware Station Sample
## Overview
- This sample implement a function to print the receipt via a dedicated POS hardware station for orders from online or 3rd-party solution. The ideal solution is whenever a new order created from online or 3rd-part, hardware station API can be called from CRT to print the receipt in the even-driven mode,  but it is unlucky at this moment CRT did not support call HWS API directly,  this solution is a workaround:  the idea is to call CRT API from POS periodically to check if there are any new orders whose receipy need print.
- Because of technical limitation, so from design perspective this solution is not good,  but from technical perspective it is valuable because it leverage some advanced JS/TS technology, so it deserves to practise.

## Configuring the sample
- In HQ, create a new POS operation:<br/>
  <img width="630" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7f75f1c4-7228-4fab-8575-8903087b85c6">
- Add the operation on the POS screen layout button grid:<br/>
  <img width="285" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/410547ec-14d1-4ae2-a6cf-685ea3d3beb4">

## Running the sample
- Logon POS, find the button, click<br/>
  <img width="484" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9c899c92-fef4-45ae-bcbe-27c037f5b1e4">
- Specify the start date and end date you wish the online orders created in this period:<br/>
  <img width="1489" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/a932d652-3fa7-4bd9-8b9d-00ed72d72d6f">
- In the simutor, you will see the receipt will be printed one by one:<br/>
<img width="1176" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c3cda0f0-b8c6-4943-bb52-891635d3127a">
- From test purpose, you can run these two SQL in Channel DB to reset the status:<br/>

```cs
    delete from ext.CONTOSORETAILTRANSACTIONTABLE
    select * from ext.CONTOSORETAILTRANSACTIONTABLE
```
## Technical Details
### SQL
- table: [ext].[CONTOSORETAILTRANSACTIONTABLE]<br/>
  Once the receipt got printed for the transaction,  one record will be created in this table to avoid it will be printed again
- Store Procedure: [ext].[GETPRINTEDTRANSACTIONS]<br/>
  Among the transaction list,  find all the transactions with receipt printed
- Store Procedure: [ext].[SETTRANSACTIONPRINTED]<br/>
  Once receipt got printed,  set the transaction IsReceiptPrinted in the table [ext].[CONTOSORETAILTRANSACTIONTABLE]
### Retail Server
-  SearchJournalTransactionsWithUnPrintReceipt<br/>
   Based on the SearchCriteria, find all the transactions whose receipt has not been printed yet
-  SetTransactionPrinted<br/>
  Set one single transaction IsReceiptPrinted as YES
### POS
- Operation <br/>
  <img width="238" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8649d5d5-d3a1-4ac3-bc87-b84e2cf8cc8e">
  <br/>
  In the operation, an interval will send OnlineOrderReceiptPrintClientRequest every 20 seconds:<br/>
```js
  let response: PrintOnlineOrderReceiptResponse = new PrintOnlineOrderReceiptResponse();

 if (localStorage.getItem(this.PrintOnlineOrderReceiptTimerId)) {
     this.timerId = Number(localStorage.getItem(this.PrintOnlineOrderReceiptTimerId));
     clearInterval(this.timerId);
 }

 let dialog: SearchTransactionsDialog = new SearchTransactionsDialog();
 return dialog.open().then(async (searchCriteria: ProxyEntities.TransactionSearchCriteria) => {
     if (!ObjectExtensions.isNullOrUndefined(searchCriteria)) {
         this.timerId = setInterval(async (): Promise<void> => {
             let clientReq: OnlineOrderReceiptPrintClientRequest<OnlineOrderReceiptPrintClientResponse> =
                 new OnlineOrderReceiptPrintClientRequest(this.context.logger.getNewCorrelationId(), searchCriteria);

             console.log("Online Order Receipt Print Job is starting, and it will search order every 20 seconds and print them!!!!");
             await this.context.runtime.executeAsync(clientReq);
            // await onlineOrderReceiptService.processByAsyncAwait(searchCriteria);
         }, 20000);

         localStorage.setItem(this.PrintOnlineOrderReceiptTimerId, this.timerId.toString());
     }
     return Promise.resolve();
 }).then((): ClientEntities.ICancelableDataResult<TResponse> => {
     return <ClientEntities.ICancelableDataResult<TResponse>>{
         canceled: false,
         data: response
     };
 });
```
- Request Handler <br/>
<img width="231" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f188f837-3b1a-4366-b345-0f184f48e2d0"><br/>
In the request handler, we have two ways to implement GetReceipt and Print Receipt One by One:<br/>

1. Async & Await:
```js
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
```




