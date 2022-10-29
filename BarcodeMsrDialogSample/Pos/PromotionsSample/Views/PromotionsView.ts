import * as Views from "PosApi/Create/Views";
import { IPivot, IPivotItem, IPivotOptions } from "PosApi/Consume/Controls"
import { IPromotionViewModelOptions } from "./NavigationContract";
import PromotionsViewModel from "./PromotionsViewModel";
import * as Controls from "PosApi/Consume/Controls";
import { ProxyEntities } from "PosApi/Entities";

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

    dispose(): void {
        throw new Error("Method not implemented.");
    }
    onReady(element: HTMLElement): void {
        throw new Error("Method not implemented.");
    }

}