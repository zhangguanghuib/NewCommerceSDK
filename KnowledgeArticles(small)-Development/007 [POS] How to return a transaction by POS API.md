## How to return a transaction by POS API?

```ts
    public ReturnTransaction(): Promise<void> {
        let transactionId: string = "HOUSTON-HOUSTON-39-1719972930013";
        let correlationId: string = this._context.logger.getNewCorrelationId();
        this._context.logger.logInformational("ReturnTransaction called with transactionId: " + transactionId + " and correlationId: " + correlationId);
        return this._context.runtime
            .executeAsync(new GetSalesOrderDetailsByTransactionIdClientRequest(transactionId, ProxyEntities.SearchLocation.Local, correlationId))
            .then((response: ClientEntities.ICancelableDataResult<GetSalesOrderDetailsByTransactionIdClientResponse>): Promise<ClientEntities.ICancelableDataResult<ReturnTransactionOperationResponse>> => {
                if (!response.canceled) {
                    let salesOrder: ProxyEntities.SalesOrder = response.data.result;
                    let returnTransactionOperationRequest: ReturnTransactionOperationRequest<ReturnTransactionOperationResponse> = new ReturnTransactionOperationRequest(correlationId, salesOrder);
                    return this._context.runtime
                        .executeAsync(returnTransactionOperationRequest);
                } else {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<ReturnTransactionOperationResponse>>{ canceled: true, data: null });
                }
            })
            .then((response: ClientEntities.ICancelableDataResult<ReturnTransactionOperationResponse>) => {
                this._context.navigator.navigateToPOSView("CartView");
                return Promise.resolve();
            }).catch((reason: any) => {
                this._context.logger.logError("Return Transaction failed: " + JSON.stringify(reason));
            });
    }
```
```ts
  {
      name: "ReturnTransaction",
      label: "Return Transaction",
      icon: Views.Icons.ProductReturn,
      isVisible: true,
      canExecute: true,
      execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
          this.state.isProcessing = true;
          this.viewModel.ReturnTransaction().then(() => {
              this.state.isProcessing = false;
          });
      }
  }
```






