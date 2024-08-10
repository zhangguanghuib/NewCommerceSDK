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

How it works?<br/>
1. Click Return Transaction button:<br/>
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/71c70491-c675-4f01-99c1-b2d2f3f24d02)<br/>

2. I will jump to the returnable products view?<br/>
   Choose the first line to return:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7ed62d49-7bb9-49c7-a729-0fc30942b91b)

3.  Finally it will go to the cart view like below<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8d0a3215-212e-42c7-9cab-24a2432b109f)










