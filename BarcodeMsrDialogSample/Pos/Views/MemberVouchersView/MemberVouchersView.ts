import ko from "knockout";

import * as NewView from "PosApi/Create/Views";
//import KnockoutExtensionViewControllerBase from "./BaseClasses/KnockoutExtensionViewControllerBase";
import MemberVouchersViewModel from "./MemberVouchersViewModel";
import { IMemberVouchersExtensionViewModelOptions } from "./NavigationContracts";
//import { HeaderSplitView, IHeaderSplitViewState } from "PosUISdk/Controls/HeaderSplitView";
//import { DataList, IDataListState, SelectionMode } from "PosUISdk/Controls/DataList";
//import { Loader, ILoaderState } from "PosUISdk/Controls/Loader";
//import { IMemberVoucher } from "../CustomEntities/CustomEntities";
//import { AppBar, AppBarCommand, IAppBarCommandState } from "PosUISdk/Controls/AppBar";
import { ObjectExtensions } from "PosApi/TypeExtensions";
//import { Entities } from "../DataService/DataServiceEntities.g";
import { Entities } from "./DataService/DataServiceEntities.g";

import { IMemberVoucher } from "./CustomEntities/CustomEntities";
import * as Controls from "PosApi/Consume/Controls";
import { DateFormatter } from "PosApi/Consume/Formatters";

/**
 * The controller for MemberVouchersView.
 */
// export default class MemberVouchersView extends KnockoutExtensionViewControllerBase<MemberVouchersViewModel> {
export default class MemberVouchersView extends NewView.CustomViewControllerBase {

    public readonly viewModel: MemberVouchersViewModel;
    //public readonly headerSplitView: HeaderSplitView;
    //public readonly dataList: DataList<IMemberVoucher>;
    //public readonly loader: Loader;
    //public readonly appBar: AppBar;
    //public readonly applyVoucherCommand: AppBarCommand;
    //public readonly cancelVoucherCommand: AppBarCommand;
    public _isNoItemSelected: ko.Observable<boolean>;
    public dataList: Controls.IDataList<IMemberVoucher>;

    constructor(context: NewView.ICustomViewControllerContext, state: NewView.ICustomViewControllerBaseState,
        options?: IMemberVouchersExtensionViewModelOptions) {

        let config: NewView.ICustomViewControllerConfiguration = {
            title: "Member Vouchers",
            commandBar: {
                commands: [
                    {
                        name: "applyVoucherCommand",
                        label: "continue",
                        icon: NewView.Icons.Buy,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: NewView.CustomViewControllerExecuteCommandArgs): void => {
                            this.viewModel.applyVoucher();
                        }
                    },
                    {
                        name: "Cancel",
                        label: "continue",
                        icon: NewView.Icons.Cancel,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: NewView.CustomViewControllerExecuteCommandArgs): void => {
                            this.viewModel.cancelVoucher();
                        }
                    }
                ]
            }
        };

        super(context, config);

        //super(context);

        // Initialize the view model.
        //this.viewModel = new MemberVouchersViewModel(context, options);
        this.viewModel = new MemberVouchersViewModel(context, this.state, options);

        this.state.title = this.viewModel.title();
    }

    /*
    constructor(context: NewView.IExtensionViewControllerContext, options?: IMemberVouchersExtensionViewModelOptions) {
        // Do not save in history
        super(context, false);

        // Initialize the view model.
        this.viewModel = new MemberVouchersViewModel(context, options);

        // Initialize the POS SDK Controls.
        // Loader
        //let loaderState: ILoaderState = {
        //    visible: this.viewModel.isBusy
        //};

        //this.loader = new Loader(loaderState);
        //// HeaderSplit
        //let headerSplitViewState: IHeaderSplitViewState = {
        //    title: this.viewModel.title
        //};
        //this.headerSplitView = new HeaderSplitView(headerSplitViewState);

        this._isNoItemSelected = ko.observable(true);
        // DataList
        let dataListOptions: IDataListState<IMemberVoucher> = {
            selectionMode: SelectionMode.MultiSelect,
            selectionChanged: this.voucherSelectionChangedEventHandler,
            itemDataSource: this.viewModel.currentMemberVouchers,
            columns: [
                {
                    title: context.resources.getString("string_50001"), // Voucher Number,
                    ratio: 20, collapseOrder: 1, minWidth: 100,
                    computeValue: (event: IMemberVoucher): string => { return event.VoucherNumber; }
                },
                {
                    title: context.resources.getString("string_50000"), // Voucher Code
                    ratio: 15, collapseOrder: 2, minWidth: 100,
                    computeValue: (event: IMemberVoucher): string => { return event.VoucherCode; }
                },
                {
                    title: context.resources.getString("string_50002"), // Voucher Name
                    ratio: 25, collapseOrder: 3, minWidth: 100,
                    computeValue: (event: IMemberVoucher): string => { return event.VoucherName; }
                },
                {
                    title: context.resources.getString("string_50012"), // Valid From
                    ratio: 20, collapseOrder: 4, minWidth: 100,
                    computeValue: (event: IMemberVoucher): string => { return DateFormatter.toShortDate(event.ValidFrom); }
                },
                {
                    title: context.resources.getString("string_50013"), // Expiry Date
                    ratio: 20, collapseOrder: 5, minWidth: 100,
                    computeValue: (event: IMemberVoucher): string => { return DateFormatter.toShortDate(event.ExpiryDate); }
                }
            ]
        };
        this.dataList = new DataList(dataListOptions);

        // Initialize the app bar.
        this.appBar = new AppBar();

        let commandState: IAppBarCommandState = {
            id: "applyVoucherCommand",
            label: this.context.resources.getString("string_50007"),
            execute: this.viewModel.applyVoucher.bind(this.viewModel),
            extraClass: "iconBuy"
        };

        let cancelCommandState: IAppBarCommandState = {
            id: "cancelVoucherCommand",
            label: this.context.resources.getString("string_50008"),
            execute: this.viewModel.cancelVoucher.bind(this.viewModel),
            extraClass: "iconCancel"
        };

        this.applyVoucherCommand = new AppBarCommand(commandState);
        this.cancelVoucherCommand = new AppBarCommand(cancelCommandState);
    }*/

    public voucherSelectionChangedEventHandler(items: Entities.MemberVoucher[]): void {
        this.viewModel.selectedMemberVouchers(items);
        var numItemsSelected: number = ObjectExtensions.isNullOrUndefined(items) ? 0 : items.length;

        // Enable or disable available commands that are bound to the following members.
        this._isNoItemSelected(numItemsSelected < 1);
    }


    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        // DataList
        let dataListOptions: Readonly<Controls.IDataListOptions<IMemberVoucher>> = {
            interactionMode: Controls.DataListInteractionMode.MultiSelect,
            data: this.viewModel.currentMemberVouchers(),
            columns: [
                {
                    title: "Voucher Number", // Voucher Number,
                    ratio: 20, collapseOrder: 1, minWidth: 100,
                    computeValue: (event: IMemberVoucher): string => { return event.VoucherNumber; }
                },
                {
                    title: "Voucher Code", // Voucher Code
                    ratio: 15, collapseOrder: 2, minWidth: 100,
                    computeValue: (event: IMemberVoucher): string => { return event.VoucherCode; }
                },
                {
                    title: "Voucher Name", // Voucher Name
                    ratio: 25, collapseOrder: 3, minWidth: 100,
                    computeValue: (event: IMemberVoucher): string => { return event.VoucherName; }
                },
                {
                    title: "Valid From", // Valid From
                    ratio: 20, collapseOrder: 4, minWidth: 100,
                    computeValue: (event: IMemberVoucher): string => { return DateFormatter.toShortDate(event.ValidFrom); }
                },
                {
                    title: "Expiry Date", // Expiry Date
                    ratio: 20, collapseOrder: 5, minWidth: 100,
                    computeValue: (event: IMemberVoucher): string => { return DateFormatter.toShortDate(event.ExpiryDate); }
                }
            ]
        };

        let dataListRootElem: HTMLDivElement = element.querySelector("#memberVouchersListView") as HTMLDivElement;
        this.dataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", dataListOptions, dataListRootElem);
        

    }
}