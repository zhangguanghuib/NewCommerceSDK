import { DataListInteractionMode, IDataList, IDataListOptions } from "PosApi/Consume/Controls";
import * as Views from "PosApi/Create/Views";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ArrayExtensions, ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import ContosoTenderCountingViewModel from "./ContosoTenderCountingViewModel";
import { ContosoTenderCountingLine } from "./ContosoTenderCountingLine";
import ko from "knockout";
import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import { IContosoDenominationDetailViewOptions, IDenominationsViewOptions } from "Views/NavigationContracts";


declare var Commerce: any;

export default class ContosoTenderCountingView extends Views.CustomViewControllerBase {

    public readonly viewModel: ContosoTenderCountingViewModel;
    public dataList: IDataList<ContosoTenderCountingLine>;
    private _title: string;

    public constructor(context: Views.ICustomViewControllerContext) {

        let config: Views.ICustomViewControllerConfiguration = {
            title: "Bank Drop",
            commandBar: {
                commands: [
                    {
                        name: "Save",
                        label: "Save",
                        icon: Views.Icons.Save,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.viewModel.onSave().then(() => { console.log("Save done") });
                        }
                    }
                ]
            }
        };

        super(context, config);

        this._title = config.title;
        this.viewModel = new ContosoTenderCountingViewModel(context);
    }


    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {

        ko.applyBindings(this, element);

        let dataListOptions: IDataListOptions<ContosoTenderCountingLine> = {
            interactionMode: DataListInteractionMode.SingleSelect,
            data: this.viewModel.tenderCountingLines(),
            columns: [
                {
                    title: "Payment methods", 
                    ratio: 40, collapseOrder: 1, minWidth: 100,
                    computeValue: (data: ContosoTenderCountingLine): string => data.tenderName
                },
                {
                    title: "COUNT",
                    ratio: 30, collapseOrder: 2, minWidth: 100,
                    computeValue: (data: ContosoTenderCountingLine): string => {
                        if (data.tenderName === 'Cash' || data.tenderName === 'Other') {
                            let btnKey = `${data.tenderName}_msPOSKey`;
                            return `<button class='btnDenominationCount' onclick="localStorage.setItem('${data.tenderName}_myKeyghz', 'msPOSKey')">Click me</button>`;
                        } else {
                            return "";
                        }
                    }
                }, {
                    title: "TOTAL",
                    ratio: 30, collapseOrder: 3, minWidth: 100,
                    computeValue: (data: ContosoTenderCountingLine): string => {
                            return CurrencyFormatter.toCurrency(data.totalAmount);
                        }
                    }
            ]
        };


        let dataListRootElem: HTMLDivElement = element.querySelector("#ContosoTenderCountingView") as HTMLDivElement;
        this.dataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", dataListOptions, dataListRootElem);

        this.dataList.addEventListener("SelectionChanged", (eventData: { items: ContosoTenderCountingLine[] }) => {
            //this.viewModel.seletionChanged(eventData.items);

            let firstItem: ContosoTenderCountingLine = ArrayExtensions.firstOrUndefined(eventData.items);
            if (firstItem !== undefined) {
                let keyStr: string = `${firstItem.tenderName}_myKeyghz`;
                if (localStorage.getItem(keyStr) !== null) {
                    console.log(localStorage.getItem(keyStr));
                    //console.log("row " + firstItem.IntData + "is selected");
                    localStorage.removeItem(keyStr);
                    // First line selected
                    if (firstItem.tenderName === "Cash") {

                        let options: IContosoDenominationDetailViewOptions = {
                            title: firstItem.tenderName,
                            denominations: firstItem.denominations
                        }

                        this.context.navigator.navigate("ContosoDenominationDetailView", options);

                        /*
                        let options: IDenominationsViewOptions = {
                            operationTitle: this._title,
                            tenderName: firstItem.tenderName,
                            denominationDetails: firstItem.denominations,
                            selectionHandler: new Commerce.CancelableSelectionHandler(() => void 0, () => void 0),
                            correlationId: this.context.logger.getNewCorrelationId(),
                            shouldShowFilteringIcon: false
                        };

                        Commerce.ViewModelAdapter.navigate("DenominationsView", options);*/


                        //this.context.navigator.navigateToPOSView("CartView");

                    } else if (firstItem.tenderName === "Other") {
                        //let customerDetailsOptions: ClientEntities.CustomerDetailsNavigationParameters
                        //    = new ClientEntities.CustomerDetailsNavigationParameters("004007");

                        //this.context.navigator.navigateToPOSView("CustomerDetailsView", customerDetailsOptions);

                        let options: IContosoDenominationDetailViewOptions = {
                            title: firstItem.tenderName,
                            denominations: firstItem.denominations
                        }

                        this.context.navigator.navigate("ContosoDenominationDetailView", options);
                    } 
                } else {
                    this.viewModel.listItemSelected(firstItem).then(() => {
                        this.dataList.data = this.viewModel.tenderCountingLines();
                    })
                };
            }
        });

        this.state.isProcessing = true;
        this.viewModel.loadAsync().then((): void => {
            this.dataList.data = this.viewModel.tenderCountingLines();
            this.state.isProcessing = false;
        });
    }

}