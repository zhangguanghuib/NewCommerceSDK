import {
    ICustomLinesGridColumnContext,
    CustomLinesGridColumnBase
} from "PosApi/Extend/Views/CartView";
import { CustomGridColumnAlignment } from "PosApi/Extend/Views/CustomGridColumns";
import { ProxyEntities } from "PosApi/Entities";

export default class LinesCustomGridColumn1 extends CustomLinesGridColumnBase {
    constructor(context: ICustomLinesGridColumnContext) {
        super(context);
    }

    public title(): string {
        return "Line number";
    }

    public computeValue(cartLine: ProxyEntities.CartLine): string {
        return cartLine.LineNumber.toString();
    }

    public alignment(): CustomGridColumnAlignment {
        return CustomGridColumnAlignment.Right;
    }
}