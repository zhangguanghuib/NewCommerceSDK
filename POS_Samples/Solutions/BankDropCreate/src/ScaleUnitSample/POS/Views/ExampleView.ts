
import * as Views from "PosApi/Create/Views";
import { Entities } from "../DataService/DataServiceEntities.g";
import ExampleViewModel from "./ExampleViewModel";
import { IDataList, IDataListOptions, DataListInteractionMode } from "PosApi/Consume/Controls";
import { ObjectExtensions, ArrayExtensions } from "PosApi/TypeExtensions";
import { ClientEntities } from "PosApi/Entities";

export default class StoreHoursView extends Views.CustomViewControllerBase {
    public readonly viewModel: ExampleViewModel;
    public dataList: IDataList<Entities.ExampleEntity>;

    constructor(context: Views.ICustomViewControllerContext) {
        let config: Views.ICustomViewControllerConfiguration = {
            title: context.resources.getString("string_0001"),
            commandBar: {
                commands: [
                    {
                        name: "Create",
                        label: context.resources.getString("string_2001"),
                        icon: Views.Icons.Add,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.viewModel.createExampleEntity().then((entityCreated) => {
                                if (entityCreated) {
                                    // Re-load the list, since the underlying data was amended
                                    this.dataList.data = this.viewModel.loadedData;
                                }
                            });
                        }
                    },
                    {
                        name: "Edit",
                        label: context.resources.getString("string_2002"),
                        icon: Views.Icons.Edit,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.state.isProcessing = true;
                            this.viewModel.editExampleEntity().then((editsMade) => {
                                if (editsMade) {
                                    // Re-load the list since the underlying data changed
                                    this.dataList.data = this.viewModel.loadedData;
                                }
                                this.state.isProcessing = false;
                            });
                        }
                    },
                    {
                        name: "Delete",
                        label: context.resources.getString("string_1006"),
                        icon: Views.Icons.Delete,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.state.isProcessing = true;
                            this.viewModel.deleteExampleEntity().then(() => {
                                // Re-load the list, since the data has changed
                                this.dataList.data = this.viewModel.loadedData;
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
                    },
                    {
                        name: "Last",
                        label: "Last",
                        icon: Views.Icons.Down,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.state.isProcessing = true;
                            if (this.viewModel.loadedData.length > 0) {
                                let item: Entities.ExampleEntity = this.viewModel.loadedData[this.viewModel.loadedData.length - 1];
                                    if(!ObjectExtensions.isNullOrUndefined(item)) {
                                        this.dataList.selectItems([item]);
                                    }
                            }                          
                            this.state.isProcessing = false; 
                        }
                    },
                    {
                        name: "First",
                        label: "First",
                        icon: Views.Icons.Up,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.state.isProcessing = true;
                            if (this.viewModel.loadedData.length > 0) {
                                let item: Entities.ExampleEntity = this.viewModel.loadedData[0];
                                if (!ObjectExtensions.isNullOrUndefined(item)) {
                                    this.dataList.selectItems([item]);
                                }
                            }
                            this.state.isProcessing = false;
                        }
                    }
                ]
            }
        };

        super(context, config);

        // Initialize the view model.
        this.viewModel = new ExampleViewModel(context);

    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {
        // DataList

        let dataListOptions: IDataListOptions<Entities.ExampleEntity> = {
            interactionMode: DataListInteractionMode.SingleSelect,
            data: this.viewModel.loadedData,
            columns: [
                {
                    title: this.context.resources.getString("string_1001"), // Int data
                    ratio: 40, collapseOrder: 1, minWidth: 100,
                    computeValue: (data: Entities.ExampleEntity): string => data.IntData.toString()
                },
                {
                    title: this.context.resources.getString("string_1002"), // String data
                    ratio: 30, collapseOrder: 2, minWidth: 100,
                    computeValue: (data: Entities.ExampleEntity): string => `<button onclick="localStorage.setItem(${data.IntData}+'myKeyghz', 'myValueghz')">Click me</button >`
                },                {
                    title: this.context.resources.getString("string_1003"), // Extension property string data
                    ratio: 30, collapseOrder: 3, minWidth: 100,
                    computeValue: (data: Entities.ExampleEntity): string => {
                        return ArrayExtensions.firstOrUndefined(
                            data.ExtensionProperties.filter(prop => prop.Key == "customExtensionProp").map(prop => prop.Value.StringValue)
                        );
                    }
                }
            ]
        };

        let dataListRootElem: HTMLDivElement = element.querySelector("#exampleListView") as HTMLDivElement;
        this.dataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", dataListOptions, dataListRootElem);

        this.dataList.addEventListener("SelectionChanged", (eventData: { items: Entities.ExampleEntity[] }) => {
            this.viewModel.seletionChanged(eventData.items);

            let firstItem: Entities.ExampleEntity = ArrayExtensions.firstOrUndefined(eventData.items);
            if (firstItem !== undefined) {
                let keyStr: string = firstItem.IntData + "myKeyghz";
                if (localStorage.getItem(keyStr) !== null) {
                    console.log(localStorage.getItem(keyStr));
                    console.log("row " + firstItem.IntData + "is selected");
                    localStorage.removeItem(keyStr);
                    // First line selected
                    if (firstItem.IntData === 1) {
                        this.context.navigator.navigateToPOSView("CartView");
                    } else if (firstItem.IntData <= 10) { 
                        let customerDetailsOptions: ClientEntities.CustomerDetailsNavigationParameters
                            = new ClientEntities.CustomerDetailsNavigationParameters("004007");

                        this.context.navigator.navigateToPOSView("CustomerDetailsView", customerDetailsOptions);
                    }
                    return;
                } 
            }

            // Else open dialog
            alert("Will navigate a current numpad");
        });

        this.dataList.addEventListener("ItemInvoked", (eventData: { item: Entities.ExampleEntity }): any => {
            console.log(eventData.item);
        });
      
        this.state.isProcessing = true;
        this.viewModel.load().then((): void => {
            // Initialize the data list with what the view model loaded
            this.dataList.data = this.viewModel.loadedData;
            this.state.isProcessing = false;
        });
    }
}
