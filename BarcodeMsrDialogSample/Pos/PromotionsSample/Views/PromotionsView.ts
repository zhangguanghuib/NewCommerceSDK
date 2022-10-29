import * as Views from "PosApi/Create/Views";
import { IPivot, IPivotItem, IPivotOptions } from "PosApi/Consume/Controls"
import { IPromotionViewModelOptions } from "./NavigationContract";
import PromotionsViewModel from "./PromotionsViewModel";
import * as Controls from "PosApi/Consume/Controls";
import { ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class PromotionsView extends Views.CustomViewControllerBase {
    public readonly viewModel: PromotionsViewModel;
    public availableDiscountsDataList: Controls.IDataList<ProxyEntities.DiscountCode>;
    public upcomingDiscountsDataList: Controls.IDataList<ProxyEntities.DiscountCode>;
    public readonly promotionsPivot: IPivot;
    public readonly availableDiscountsPivotItem: IPivotItem;
    public readonly upcomingDiscountsPivotItem: IPivotItem;
    public constantPromotionDiscountsMenu: Controls.IMenu;
    private readonly _promotionsViewModelOptions?: IPromotionViewModelOptions;
    private static readonly SHOW_DISCOUNTS_MENU_COMMAND_NAME: string = "showConstantPromotionDiscountsMenunCommand";
    private static readonly ADD_TO_SALE_COMMAND_NAME: string = "addToSaleCommand";
    private static readonly SELL_NOW_COMMAND_NAME: string = "sellNowCommand";

    constructor(context: Views.ICustomViewControllerContext, options?: IPromotionViewModelOptions) {

        let config: Views.ICustomViewControllerConfiguration = {
            title: "",
            commandBar: {
                commands: [
                    {
                        name: PromotionsView.SELL_NOW_COMMAND_NAME,
                        label: context.resources.getString("string_22"),
                        icon: Views.Icons.Buy,
                        isVisible: true,
                        canExecute: false,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.viewModel.sellNowAsync();
                        }
                    },
                    {
                        name: PromotionsView.ADD_TO_SALE_COMMAND_NAME,
                        label: context.resources.getString("string_21"),
                        icon: Views.Icons.Add,
                        isVisible: true,
                        canExecute: false,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.viewModel.addToSaleAsync();
                        }
                    },
                    {
                        name: PromotionsView.SHOW_DISCOUNTS_MENU_COMMAND_NAME,
                        label: context.resources.getString("string_20"),
                        icon: Views.Icons.Money,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.showConstantPromotionsDiscounts(args.commandId);
                        }
                    }]
            }
        };

        super(context, config);

        this._promotionsViewModelOptions = options;
        this.viewModel = new PromotionsViewModel(context, this.state);

        //let availableDiscountsPivotItemState: Ip

        this.availableDiscountsPivotItem = {
            id: "",
            header: context.resources.getString("string_5"), //Available
            onReady: null
        };

        this.upcomingDiscountsPivotItem = {
            id: "",
            header: context.resources.getString("string_6"), //Upcoming
            onReady: null
        }

        this.promotionsPivot = {
            selectedItem: null,
            selectItem: null,
            addEventListener: null,
            removeEventListener: null
        };

        this.state.title = this.viewModel.title();
    }

    public showConstantPromotionsDiscounts(appBarCommandId: string): void {
        let anchor: HTMLElement = document.getElementById(appBarCommandId);
        this.constantPromotionDiscountsMenu.show(anchor);
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
    onReady(element: HTMLElement): void {
        throw new Error("Method not implemented.");
    }

}