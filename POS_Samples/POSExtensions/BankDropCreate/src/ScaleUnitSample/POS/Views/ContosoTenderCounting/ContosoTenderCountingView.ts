import { IDataList } from "PosApi/Consume/Controls";
import * as Views from "PosApi/Create/Views";
import { ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import ContosoTenderCountingViewModel from "./ContosoTenderCountingViewModel";

export default class ContosoTenderCountingView extends Views.CustomViewControllerBase {

    public readonly viewModel: ContosoTenderCountingViewModel;
    public dataList: IDataList<ProxyEntities.TenderDetail>;

    public constructor(context: Views.ICustomViewControllerContext) {
        super(context);
        this.viewModel = new ContosoTenderCountingViewModel(context);
    }


    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {

        this.viewModel.loadAsync().then((): void => {
            console.log("View Model is loaded successfully");
        });
    }

}