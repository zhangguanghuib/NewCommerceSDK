
import ko from "knockout";

import * as Dialogs from "PosApi/Create/Dialogs";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { IConfirmDialogResult } from "./IConfirmDialogResult";
import { ConfirmOption } from "Views/MemberSignUpView/CustomEntities/CustomEntities"
type ConfirmMessageDialogResolve = (value: IConfirmDialogResult) => void;
type ConfirmMessageDialogReject = (reason: any) => void;

export default class ConfirmDialogModule extends Dialogs.ExtensionTemplatedDialogBase {
    public messagePassedToDialog: ko.Observable<string>;
    public userEnteredValue: ko.Observable<string>;

    private resolve: ConfirmMessageDialogResolve;

    constructor(text: string) {
        super();
        this.userEnteredValue = ko.observable(text);
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

    public open(message: string, subTitleMsg: string): Promise<IConfirmDialogResult> {
        this.userEnteredValue(message);

        let promise: Promise<IConfirmDialogResult> = new Promise((resolve: ConfirmMessageDialogResolve, reject: ConfirmMessageDialogReject) => {
            this.resolve = resolve;
            let option: Dialogs.ITemplatedDialogOptions = {
                title: "Confirm",
                subTitle: subTitleMsg,
                onCloseX: this.onCloseX.bind(this),
                button1: {
                    id: "Button1",
                    label: "Yes",
                    isPrimary: true,
                    onClick: this.button1ClickHandler.bind(this)
                },
                button2: {
                    id: "Button2",
                    label: "No",
                    onClick: this.button2ClickHandler.bind(this)
                }
            };

            this.openDialog(option);
        });


        return promise;
    }


    private onCloseX(): boolean {
        this.resolvePromise("1");
        return true;
    }

    private button1ClickHandler(): boolean {
        this.resolvePromise("0");
        return true;
    }

    private button2ClickHandler(): boolean {
        this.resolvePromise("1"); //"1" is No
        return true;
    }

    private resolvePromise(result: string): void {
        if (ObjectExtensions.isFunction(this.resolve)) {
            this.resolve(<IConfirmDialogResult>{
                selectedValue: ConfirmOption[Number(result)].toString()
            });

            this.resolve = null;
        }
    }
}