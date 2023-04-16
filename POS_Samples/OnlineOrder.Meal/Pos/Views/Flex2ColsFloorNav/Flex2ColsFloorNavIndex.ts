import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class Flex2ColsFloorNavIndex extends Views.CustomViewControllerBase {

    public onReady(element: HTMLElement): void {

    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

}