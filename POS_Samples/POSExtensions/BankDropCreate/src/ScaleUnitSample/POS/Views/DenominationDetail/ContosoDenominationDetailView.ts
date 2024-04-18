import * as Views from "PosApi/Create/Views";
import ko from "knockout";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { IContosoDenominationDetailViewOptions } from "Views/NavigationContracts";
import { ProxyEntities } from "PosApi/Entities";
import { DataListInteractionMode, IDataList, IDataListOptions } from "PosApi/Consume/Controls";
import ContosoDenominationDetailViewModel from "./ContosoDenominationDetailViewModel";
import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import * as Controls from "PosApi/Consume/Controls";

export default class ContosoDenominationDetailView extends Views.CustomViewControllerBase {

    public viewModel: ContosoDenominationDetailViewModel;
    public dataList: IDataList<ProxyEntities.DenominationDetail>;

    public constructor(context: Views.ICustomViewControllerContext, options?: IContosoDenominationDetailViewOptions) {

        let config: Views.ICustomViewControllerConfiguration = {
            title: options.title,
            commandBar: {
                commands: [
                    {
                        name: "performOperationAppBar",
                        label: "Save",
                        icon: Views.Icons.Save,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            console.log("Save Triggered");
                        }
                    },
                ]
            }
        };

        super(context, config);

        this.viewModel = new ContosoDenominationDetailViewModel(context, this.state, options);

        console.log(options);
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        let dataListOptions: IDataListOptions<ProxyEntities.DenominationDetail> = {
            interactionMode: Controls.DataListInteractionMode.Invoke,
            data: this.viewModel.denominationDetailLines(),
            columns: [
                {
                    title: "DENOMINATION",
                    ratio: 40, collapseOrder: 1, minWidth: 100,
                    computeValue: (data: ProxyEntities.DenominationDetail): string => data.DenominationAmount.toFixed(2)
                },
                {
                    title: "QUANTITY",
                    ratio: 30, collapseOrder: 2, minWidth: 100,
                    computeValue: (data: ProxyEntities.DenominationDetail): string => {
                        return data.QuantityDeclared.toFixed(0);
                    }
                }, {
                    title: "TOTAL",
                    ratio: 30, collapseOrder: 3, minWidth: 100,
                    computeValue: (data: ProxyEntities.DenominationDetail): string => {
                        return CurrencyFormatter.toCurrency(data.AmountDeclared);
                    }
                }
            ]
        };


        let dataListRootElem: HTMLDivElement = element.querySelector("#ContosoDenominationDetailView") as HTMLDivElement;
        this.dataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", dataListOptions, dataListRootElem);

        this.dataList.addEventListener("ItemInvoked", (eventData: { item: ProxyEntities.DenominationDetail }) => {
            this.viewModel.listItemSelected(eventData.item).then(() => {
                this.dataList.data = this.viewModel.denominationDetailLines();
            })
        });


        this.state.isProcessing = true;
        this.dataList.data = this.viewModel.denominationDetailLines();
        this.state.isProcessing = false;
    }
}