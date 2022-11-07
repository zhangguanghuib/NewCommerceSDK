import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";
//import * as Controls from "PosApi/Consume/Controls";
import { CnlCACustomerRegistrationViewModel } from "./CnlCACustomerRegistrationViewModel";
import ko from "knockout";
import { CustomViewControllerExecuteCommandArgs, Icons } from "PosApi/Create/Views";

//ExtensionViewControllerBase - Previous approach to hide bars
export default class CnlCACustomerRegistrationView extends Views.CustomViewControllerBase {

    public readonly viewModel: CnlCACustomerRegistrationViewModel

    //public toggleSwitchEmail: Controls.IToggle;
    //public toggleSwitchPhone: Controls.IToggle;
    //public toggleSwitchAddress: Controls.IToggle;

    constructor(context: Views.ICustomViewControllerContext, state: Views.CustomViewControllerBase) {
        let config: Views.ICustomViewControllerConfiguration = {
            title: "",
            commandBar: {
                commands: [
                    {
                        name: "Confirm",
                        label: "Confirm\nConfirmer",
                        icon: Icons.Accept,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: CustomViewControllerExecuteCommandArgs): void => {
                            this.viewModel.confirmAndAddCustomerRegistration();
                        }
                    }
                ]
                //MM
            },
            header: {
                isVisible: false
            },
            navigationPane: {
                isVisible: false
            }
        };

        super(context, config);

        //state.isBackNavigationEnabled = false;
        //var test = state.getViewContainer();

        // Initialize the view model.
        this.viewModel = new CnlCACustomerRegistrationViewModel(context, state);


        //this.viewModel.selectedCustomerType(state[0]);
        //this.viewModel.selectedCustomerGroup(state[1]);
        //this.viewModel.channelConfiguration = state[2];
    }

    public onHidden(): void {
    }


    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

    public onfullscreenchange(): void {

    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}