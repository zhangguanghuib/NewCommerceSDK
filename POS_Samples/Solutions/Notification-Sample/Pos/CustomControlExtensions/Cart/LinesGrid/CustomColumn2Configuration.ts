import {
    ICustomLinesGridColumnContext,
    CustomLinesGridColumnBase
} from "PosApi/Extend/Views/CartView";
import { CustomGridColumnAlignment } from "PosApi/Extend/Views/CustomGridColumns";
import { ProxyEntities } from "PosApi/Entities";

export default class LinesCustomGridColumn2 extends CustomLinesGridColumnBase {
    constructor(context: ICustomLinesGridColumnContext) {
        super(context);
    }

    public title(): string {
        return "DISCOUNTED";
    }

    public computeValue(cartLine: ProxyEntities.CartLine): string {

        if (!cartLine.IsVoided && cartLine.DiscountLines.length > 0) {
            return "DISCOUNTED_YES";
        } else {
            return "DISCOUNTED_NO";
        }
    }

    public alignment(): CustomGridColumnAlignment {
        return CustomGridColumnAlignment.Right;
    }
}