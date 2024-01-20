import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class JDIndexView extends Views.CustomViewControllerBase {

    public onReady(element: HTMLElement): void {
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

}