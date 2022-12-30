import ko from "knockout";

import * as Views from "PosApi/Create/Views";

//import KnockoutExtensionViewControllerBase from "./BaseClasses/KnockoutExtensionViewControllerBase";
import MemberSignUpViewModel from "./MemberSignUpViewModel";
//import { HeaderSplitView, IHeaderSplitViewState } from "PosUISdk/Controls/HeaderSplitView";
//import { Loader, ILoaderState } from "PosUISdk/Controls/Loader";
//import { AppBar, AppBarCommand, IAppBarCommandState } from "PosUISdk/Controls/AppBar";
import { IMemberSignUpExtensionViewModelOptions } from "./NavigationContracts";
import { ObjectExtensions } from "PosApi/TypeExtensions";

//export default class MemberSignUpView extends KnockoutExtensionViewControllerBase<MemberSignUpViewModel> {
export default class MemberSignUpView extends  Views.CustomViewControllerBase {

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

    public isBusy: ko.Observable<boolean>;

    public readonly viewModel: MemberSignUpViewModel;

    //public readonly headerSplitView: HeaderSplitView;
    //public readonly loader: Loader;
    //public readonly appBar: AppBar;
   // public readonly newMemberSignUpCommand: AppBarCommand;

    // Message objects

    /**
     * Create an instance of LoginViewModel
     * @constructor
     */
    constructor(context: Views.ICustomViewControllerContext, options?: IMemberSignUpExtensionViewModelOptions) {

        let config: Views.ICustomViewControllerConfiguration = {
            title: "Member sign up",
            commandBar: {
                commands: [
                    {
                        name: "newMemberSignUpCommand",
                        label: "Save",
                        icon: Views.Icons.Save,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: Views.CustomViewControllerExecuteCommandArgs): void => {
                            this.viewModel.createMember();
                        }
                    }
                ]
            }
        };

        super(context, config);

        this.viewModel = new MemberSignUpViewModel(context, options);

        //// HeaderSplit
        //let headerSplitViewState: IHeaderSplitViewState = {
        //    title: "Member sign up" //this.viewModel.title
        //};
        //this.headerSplitView = new HeaderSplitView(headerSplitViewState);


        //// Initialize the app bar.
        //this.appBar = new AppBar();

        //let commandState: IAppBarCommandState = {
        //    id: "newMemberSignUpCommand",
        //    label: this.context.resources.getString("string_11"),
        //    execute: this.viewModel.createMember.bind(this.viewModel),
        //    extraClass: "iconLightningBolt"
        //};

        //this.newMemberSignUpCommand = new AppBarCommand(commandState);
    }

    public onNavigateBack(): boolean {
        return this.viewModel.onNavigateBack(true);
    }

    public click_cmdSaveCustomerDetails(): void {
        this.viewModel.createMember();
    }

}

