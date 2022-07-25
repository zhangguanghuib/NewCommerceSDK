import ko from "knockout";
import * as Views from "PosApi/Create/Views";
import ExampleViewModel from "./ExampleViewModel";

import { ObjectExtensions} from "PosApi/TypeExtensions";

export default class ExampleView extends Views.CustomViewControllerBase {

    public readonly viewModel: ExampleViewModel;

    constructor(context: Views.ICustomViewControllerContext) {

        super(context);

        this.viewModel = new ExampleViewModel(context);

    }

    dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    onReady(element: HTMLElement): void {

        ko.applyBindings(this, element);

        let pingCoinDispenserButton: HTMLButtonElement = element.querySelector("#pingCoinDispenserButton");
        pingCoinDispenserButton.addEventListener('click', (event) => {
            console.log("button pingCoinDispenserButton clicked");
            console.log(event);
            console.log(event.target);
        });

        let dispenseTenCoinsButton: HTMLButtonElement = element.querySelector("#dispenseTenCoinsButton");
        dispenseTenCoinsButton.addEventListener('click', (event) => {
            console.log("button dispenseTenCoinsButton clicked");
            console.log(event);
            console.log(event.target);
        });

        let dispenseThousandCoinsButton: HTMLButtonElement = element.querySelector("#dispenseThousandCoinsButton");
        dispenseThousandCoinsButton.addEventListener('click', (event) => {
            console.log("button dispenseThousandCoinsButton clicked");
            console.log(event);
            console.log(event.target);
        });
    }

}