import * as SearchView from "PosApi/Extend/Views/SearchView";
//import { ProxyEntities/*, ClientEntities */} from "PosApi/Entities";

export default class ShowPromotionsCommand extends SearchView.ProductSearchExtensionCommandBase {

    //private _productSearchResultsSelectedData: ProxyEntities.ProductSearchResult[];

    protected init(state: SearchView.IProductSearchExtensionCommandState): void {
        this.isVisible = true;
       // this._productSearchResultsSelectedData = [];
    }


    protected execute(): void {
        this.isProcessing = true;

        this.isProcessing = false;
    }

}