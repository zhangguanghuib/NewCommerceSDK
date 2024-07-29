
import * as Views from "PosApi/Create/Views";
import AVACartsViewModel from "./AVACartsViewModel";
//import { IDataList, IDataListOptions, DataListInteractionMode } from "PosApi/Consume/Controls";
import { ObjectExtensions } from "PosApi/TypeExtensions";

/**
 * The controller for AVACartsView.
 */
export default class AVACartsView extends Views.CustomViewControllerBase {
    public readonly viewModel: AVACartsViewModel;
   // public dataList: IDataList<Entities.AVACartsEntity>;

    constructor(context: Views.ICustomViewControllerContext) {
        let config: Views.ICustomViewControllerConfiguration = {
            title: context.resources.getString("string_0001"),
            commandBar: {
                commands: [
                    {
                        name: "Delete",
                        label: context.resources.getString("string_1006"),
                        icon: Views.Icons.Delete,
                        isVisible: true,
                        canExecute: false,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.state.isProcessing = true;
                            this.viewModel.deleteAVACartsEntity().then(() => {
                                // Re-load the list, since the data has changed
                               // this.dataList.data = this.viewModel.loadedData;
                                this.state.isProcessing = false;
                            });
                        }
                    },
                    {
                        name: "PingTest",
                        label: context.resources.getString("string_3001"),
                        icon: Views.Icons.LightningBolt,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.state.isProcessing = true;
                            this.viewModel.runPingTest().then(() => {
                                this.state.isProcessing = false;
                            });
                        }
                    }
                ]
            }
        };

        super(context, config);

        // Initialize the view model.
        this.viewModel = new AVACartsViewModel(context);
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {
        // DataList
        //let dataListOptions: IDataListOptions<Entities.AVACartsEntity> = {
        //    interactionMode: DataListInteractionMode.SingleSelect,
        //    data: this.viewModel.loadedData,
        //    columns: [
        //        {
        //            title: this.context.resources.getString("string_1001"), // Int data
        //            ratio: 40, collapseOrder: 1, minWidth: 100,
        //            computeValue: (data: Entities.AVACartsEntity): string => data.IntData.toString()
        //        },
        //        {
        //            title: this.context.resources.getString("string_1002"), // String data
        //            ratio: 30, collapseOrder: 2, minWidth: 100,
        //            computeValue: (data: Entities.AVACartsEntity): string => data.StringData
        //        },
        //        {
        //            title: this.context.resources.getString("string_1003"), // Extension property string data
        //            ratio: 30, collapseOrder: 3, minWidth: 100,
        //            computeValue: (data: Entities.AVACartsEntity): string => {
        //                return ArrayExtensions.firstOrUndefined(
        //                    data.ExtensionProperties.filter(prop => prop.Key == "customExtensionProp").map(prop => prop.Value.StringValue)
        //                );
        //            }
        //        }
        //    ]
        //};

        //let dataListRootElem: HTMLDivElement = element.querySelector("#avaAVACartsListView") as HTMLDivElement;
        //this.dataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", dataListOptions, dataListRootElem);

        //this.dataList.addEventListener("SelectionChanged", (eventData: { items: Entities.AVACartsEntity[] }) => {
        //    this.viewModel.seletionChanged(eventData.items);

        //    // Update the command states to reflect the current selection state.
        //    this.state.commandBar.commands.forEach(
        //        command => command.canExecute = (
        //            ["Create", "PingTest"].some(name => name == command.name) ||
        //            this.viewModel.isItemSelected()
        //        )
        //    );
        //});

        this.state.isProcessing = true;
        this.viewModel.load().then((): void => {
            // Initialize the data list with what the view model loaded
            //this.dataList.data = this.viewModel.loadedData;
            this.state.isProcessing = false;
        });
    }
}
