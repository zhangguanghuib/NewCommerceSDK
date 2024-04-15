import * as Views from "PosApi/Create/Views";
import ko from "knockout";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { IContosoDenominationDetailViewOptions } from "Views/NavigationContracts";
import { ProxyEntities } from "PosApi/Entities";


export default class ContosoDenominationDetailView extends Views.CustomViewControllerBase {

    private denominationDetails: ProxyEntities.DenominationDetail[];

    public constructor(context: Views.ICustomViewControllerContext, options?: IContosoDenominationDetailViewOptions) {

        let config: Views.ICustomViewControllerConfiguration = {
            title: options.title,
            commandBar: {
                commands: []
            }
        };

        super(context, config);

        this.denominationDetails = [...options.denominations];
        console.log(options);
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }
}