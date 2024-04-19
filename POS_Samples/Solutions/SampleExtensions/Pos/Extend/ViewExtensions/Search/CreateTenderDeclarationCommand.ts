import { CreateTenderDeclarationTransactionClientRequest, CreateTenderDeclarationTransactionClientResponse } from "PosApi/Consume/StoreOperations";
import { Icons } from "PosApi/Create/Views";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchView from "PosApi/Extend/Views/SearchView";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { GetCurrentShiftClientRequest, GetCurrentShiftClientResponse } from "PosApi/Consume/Shifts";


export default class CreateTenderDeclarationCommand extends SearchView.ProductSearchExtensionCommandBase {
    /**
     * Creates a new instance of the NavigateToSimpleExtensionViewCommand class.
     * @param {IExtensionCommandContext<ProductDetailsView.IProductSearchToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<SearchView.IProductSearchToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "CreateTenderDeclarationCommand";
        this.label = "Create Tender Declration";
        this.extraClass = Icons.Calculator;
    }

    /**
     * Initializes the command.
     * @param {ProductDetailsView.IProductDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: SearchView.IProductSearchExtensionCommandState): void {
        this.canExecute = true;
        this.isVisible = true;
    }

    /**
     * Executes the command.
     */
    protected execute(): void {

        let correlationId: string = this.context.logger.getNewCorrelationId();

        let tenderDetail: ProxyEntities.TenderDetail = {
            LineNumber: 1,
            TenderTypeId: '1',
            Amount: 10,
            BankBagNumber: '210'
        };

        //let reasonCodeLine1: ProxyEntities.ReasonCodeLine = new ProxyEntities.ReasonCodeLineClass();

        let getCurrentShiftClientRequest: GetCurrentShiftClientRequest<GetCurrentShiftClientResponse> = new GetCurrentShiftClientRequest(correlationId);
        this.context.runtime.executeAsync(getCurrentShiftClientRequest)
            .then((response: ClientEntities.ICancelableDataResult<GetCurrentShiftClientResponse>) => {
                let request: CreateTenderDeclarationTransactionClientRequest<CreateTenderDeclarationTransactionClientResponse>
                    = new CreateTenderDeclarationTransactionClientRequest(
                        correlationId,
                        false,
                        response.data.result,
                        [tenderDetail],
                        []);
                return this.context.runtime.executeAsync(request);
            }).then((response: ClientEntities.ICancelableDataResult<CreateTenderDeclarationTransactionClientResponse>) => {
                console.log(response.data.result);
            }).catch((reason: any) => {
                this.context.logger.logError("NumericInputDialog: " + JSON.stringify(reason));
                return Promise.reject(reason);
            });
    }
}