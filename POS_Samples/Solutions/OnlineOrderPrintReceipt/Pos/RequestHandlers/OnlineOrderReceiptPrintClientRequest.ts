import OnlineOrderReceiptPrintClientResponse from "./OnlineOrderReceiptPrintClientResponse";
import { ExtensionRequestBase} from "PosApi/Create/RequestHandlers";
import { ProxyEntities } from "PosApi/Entities";

export default class OnlineOrderReceiptPrintClientRequest<TResponse extends OnlineOrderReceiptPrintClientResponse> extends ExtensionRequestBase<TResponse> {
    constructor(correlationId: string, _searchCriteria: ProxyEntities.TransactionSearchCriteria) {
        super(correlationId);
        this.searchCriteria = _searchCriteria;
    }

    public searchCriteria: ProxyEntities.TransactionSearchCriteria
}

