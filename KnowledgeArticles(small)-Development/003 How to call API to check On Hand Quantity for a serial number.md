## How to call CreateBankDropTransactionClientRequest/Response?

   ```typescript
    import { CreateBankDropTransactionClientRequest, CreateBankDropTransactionClientResponse, GetBankBagNumberClientRequest, GetBankBagNumberClientResponse} from "PosApi/Consume/StoreOperations";
     commands: [
     {
         name: "Save",
         label: "Save",
         icon: Views.Icons.Bank,
         isVisible: true,
         canExecute: true,
         execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
             this.viewModel.onSave().then(() => {
                 this.context.navigator.navigateToPOSView("HomeView");
             });
         }
     }
]

    public onSave(): Promise<void> {

        let correlationId: string = this._context.logger.getNewCorrelationId();

        Promise.all([
            this._context.runtime.executeAsync(new GetCurrentShiftClientRequest<GetCurrentShiftClientResponse>(correlationId))
                .then((response: ClientEntities.ICancelableDataResult<GetCurrentShiftClientResponse>): ProxyEntities.Shift => {
                    return response.data.result;
                }),

            this._context.runtime.executeAsync(new GetBankBagNumberClientRequest<GetBankBagNumberClientResponse>(correlationId))
                .then((response: ClientEntities.ICancelableDataResult<GetBankBagNumberClientResponse>) => {
                    return response.data.result.bagNumber;
                })
        ]).then((results: any[]) => {
            let currentShift: ProxyEntities.Shift = results[0];
            let bagNumber: string = results[1];
            let tenderDetails: ProxyEntities.TenderDetail[] = this._convertCountLinesToDetailLines(this.tenderCountingLines());

            let createBankDropTransactionClientRequest: CreateBankDropTransactionClientRequest<CreateBankDropTransactionClientResponse> =
                new CreateBankDropTransactionClientRequest<CreateBankDropTransactionClientResponse>(false, currentShift, tenderDetails, bagNumber, correlationId);

            this._context.runtime.executeAsync(createBankDropTransactionClientRequest).then((response: ClientEntities.ICancelableDataResult<CreateBankDropTransactionClientResponse>): Promise<void> => {
                if (response.canceled) {
                    let errorMessage: string = `Bank Drop is cancelled, please retry!`;
                    return Promise.reject(new ClientEntities.ExtensionError(errorMessage));
                } else {
                    return Promise.resolve();
                }
            }).catch((reason: any) => {
                let errorMessage: string = `Bank Drop Failed for the reason ${reason}`;
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

