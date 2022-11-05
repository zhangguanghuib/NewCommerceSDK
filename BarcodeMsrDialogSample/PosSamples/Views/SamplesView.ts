import * as Views from "PosApi/Create/Views";

export interface ISampleItem {
    label: string,
    viewName?: string,
    items?: ISampleItem[];
}

export default class SamplesView extends Views.CustomViewControllerBase {

    public dispose(): void {
        throw new Error("Method not implemented.");
    }

    public onReady(element: HTMLElement): void {
        throw new Error("Method not implemented.");
    }

}