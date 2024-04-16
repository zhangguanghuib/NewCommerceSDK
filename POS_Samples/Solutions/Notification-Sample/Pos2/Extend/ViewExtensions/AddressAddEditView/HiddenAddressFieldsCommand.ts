import * as AddressAddEditView  from "PosApi/Extend/Views/AddressAddEditView";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";


export default class DisableEmailReceiptCommand extends AddressAddEditView.AddressAddEditExtensionCommandBase {

    public constructor(context: IExtensionCommandContext<AddressAddEditView.IAddressAddEditToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "Hide";
        this.label = "Hide";
        this.extraClass = "iconGo";
        this.isVisible = false;
    }
    protected init(state: AddressAddEditView.IAddressAddEditExtensionCommandState): void {
        var signInInterval = setInterval(function () {

            let buildingComplishment: HTMLInputElement = document.querySelector("#bcTabRead") as HTMLInputElement;
            if (buildingComplishment && buildingComplishment.parentElement && buildingComplishment.parentElement.parentElement) {
                buildingComplishment.parentElement.parentElement.parentElement.style.display = "none";
            }

            let district: HTMLInputElement = document.querySelector("#districtTabRead") as HTMLInputElement;

            if (district && district.parentElement && district.parentElement.parentElement) {
                district.parentElement.parentElement.parentElement.style.display = "none";
            }

            if (district && buildingComplishment) {
                clearInterval(signInInterval);
            }

        }, 100);
     
    }

    protected execute(): void {
        
    }

}