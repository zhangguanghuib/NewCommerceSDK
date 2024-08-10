## How to return a transaction by POS API?
1.  Reload Show Journal View
```ts

declare var Commerce: any;

  protected execute(): void {
    let CORRELATION_ID: string = this.context.logger.getNewCorrelationId();

    this.isProcessing = true;
    
    let request: GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse> =
        new GetSalesOrderDetailsByTransactionIdClientRequest(this._selectedJournal.Id, ProxyEntities.SearchLocation.Local, CORRELATION_ID);

    this.context.runtime.executeAsync(request).then((response: ClientEntities.ICancelableDataResult<GetSalesOrderDetailsByTransactionIdClientResponse>): void => {
        this.context.navigator.navigateBack();
        let navigationOptions: any = {
            correlationId: CORRELATION_ID
        };
        Commerce.ViewModelAdapter.navigate("ShowJournalView", navigationOptions);
        Promise.resolve();
        this.isProcessing = false;
    });
}
```

2. 
```ts
declare var Commerce: any;

  let options: IDenominationsViewOptions = {
    operationTitle: this._title,
    tenderName: firstItem.tenderName,
    denominationDetails: firstItem.denominations,
    selectionHandler: new Commerce.CancelableSelectionHandler(() => void 0, () => void 0),
    correlationId: this.context.logger.getNewCorrelationId(),
    shouldShowFilteringIcon: false
};

Commerce.ViewModelAdapter.navigate("DenominationsView", options);
```












