import ko from "knockout";

import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export interface ISampleItem {
    label: string,
    viewName?: string,
    items?: ISampleItem[];
}

export default class SamplesView extends Views.CustomViewControllerBase {

    public readonly samplesList: ISampleItem[];

    constructor(context: Views.ICustomViewControllerContext) {
        super(context);

        this.state.title = "Samples";

        this.samplesList = [
            {
                label: "Pos Controls",
                items: [
                    { label: "DataList (dynamic)", viewName: "DynamicDataListView" }
                ]
            }
        ];
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

    public sampleClick(sampleItem: ISampleItem) {
        this.context.navigator.navigate(sampleItem.viewName);
    }

}