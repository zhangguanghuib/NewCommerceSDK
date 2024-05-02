## How to call CreateTenderDeclarationTransactionClientRequest/Response?
```typescript

import {CreateTenderDeclarationTransactionClientRequest, CreateTenderDeclarationTransactionClientResponse } from "PosApi/Consume/StoreOperations";

commands: [
      {
          name: "TenderDeclaration",
          label: "Tender Declaration",
          icon: Views.Icons.Count,
          isVisible: true,
          canExecute: true,
          execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
              this.viewModel.tenderDeclarationSave().then(() => {
                  this.context.navigator.navigateToPOSView("HomeView");
              });
          }
      }
  ]

    public tenderDeclarationSave(): Promise<void> {
        let correlationId: string = this._context.logger.getNewCorrelationId();
        this._context.runtime.executeAsync(new GetCurrentShiftClientRequest<GetCurrentShiftClientResponse>(correlationId))
            .then((response: ClientEntities.ICancelableDataResult<GetCurrentShiftClientResponse>): ProxyEntities.Shift => {
                return response.data.result;
            }).then((currentShift: ProxyEntities.Shift) => {
                let tenderDetails: ProxyEntities.TenderDetail[] = this._convertCountLinesToDetailLines(this.tenderCountingLines());
                let reasonCodeLines: ProxyEntities.ReasonCodeLine[] = [];
                let createTenderDeclarationTransactionClientRequest: CreateTenderDeclarationTransactionClientRequest<CreateTenderDeclarationTransactionClientResponse> =
                    new CreateTenderDeclarationTransactionClientRequest<CreateTenderDeclarationTransactionClientResponse>(correlationId, false, currentShift, tenderDetails, reasonCodeLines);

                this._context.runtime.executeAsync(createTenderDeclarationTransactionClientRequest).then((response: ClientEntities.ICancelableDataResult<CreateTenderDeclarationTransactionClientResponse>): Promise<void> => {
                if (response.canceled) {
                    let errorMessage: string = `Tender Declaration is cancelled, please retry!`;
                    return Promise.reject(new ClientEntities.ExtensionError(errorMessage));
                } else {
                    return Promise.resolve();
                }
            }).catch((reason: any) => {
                let errorMessage: string = `Tender Declaration Failed for the reason ${reason}`;
                return Promise.reject(new ClientEntities.ExtensionError(errorMessage));
            });
        });
        return Promise.resolve();
    }

 private _convertCountLinesToDetailLines(contosoTenderCountingLines: ContosoTenderCountingLine[]): ProxyEntities.TenderDetail[] {
     let tenderDetailLines: ProxyEntities.TenderDetail[] = [];
     if (ArrayExtensions.hasElements(contosoTenderCountingLines)) {
         contosoTenderCountingLines.forEach((contosoTenderCountingLine: ContosoTenderCountingLine) => {
             if (!ObjectExtensions.isNullOrUndefined(contosoTenderCountingLine)) {
                 let tenderDetailLine: ProxyEntities.TenderDetail = new ProxyEntities.TenderDetailClass();
                 tenderDetailLine.Amount = contosoTenderCountingLine.totalAmount;
                 tenderDetailLine.ForeignCurrency = contosoTenderCountingLine.currencyCode;
                 tenderDetailLine.ForeignCurrencyExchangeRate = contosoTenderCountingLine.exchangeRate;
                 tenderDetailLine.AmountInForeignCurrency = contosoTenderCountingLine.totalAmountInCurrency;
                 tenderDetailLine.TenderTypeId = contosoTenderCountingLine.tenderType.TenderTypeId;
                 tenderDetailLine.TenderRecount = contosoTenderCountingLine.numberOfTenderDeclarationRecount;

                 tenderDetailLine.DenominationDetails = [];

                 contosoTenderCountingLine.denominations.forEach((denominationLine: ProxyEntities.DenominationDetail): void => {
                     if (denominationLine.QuantityDeclared > 0) {
                         tenderDetailLine.DenominationDetails.push(denominationLine);
                     }
                 });
                 tenderDetailLines.push(tenderDetailLine);
             }
         });
     }
     return tenderDetailLines;
 }
```


