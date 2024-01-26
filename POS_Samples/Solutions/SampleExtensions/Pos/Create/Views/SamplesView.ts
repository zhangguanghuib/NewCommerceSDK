import * as Views from "PosApi/Create/Views";
//import { ObjectExtensions } from "PosApi/TypeExtensions";
//import ko from "knockout";

export interface ISampleItem {
    label: string;
    viewName?: string;
    items?: ISampleItem[];
}

/**
 * The controller for SamplesView.
 */
export default class SamplesView extends Views.CustomViewControllerBase {
    dispose(): void {
        throw new Error("Method not implemented.");
    }
    onReady(element: HTMLElement): void {
        throw new Error("Method not implemented.");
    }
}