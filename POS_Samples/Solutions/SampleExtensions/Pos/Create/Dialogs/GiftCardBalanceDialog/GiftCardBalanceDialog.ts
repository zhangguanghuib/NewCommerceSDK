import * as Dialogs from "PosApi/Create/Dialogs";
import ko from "knockout";
import { ProxyEntities } from "PosApi/Entities";
import { IGiftCardBalanceDialogResult } from "./GiftCardBalanceDialogTypes";
import { DateExtensions } from "PosApi/TypeExtensions";


export default class GiftCardBalanceDialog extends Dialogs.ExtensionTemplatedDialogBase {
    public balance: ko.Observable<string>;
    public expirationDate: ko.Observable<string>;
    public number: ko.Observable<string>;

    constructor() {
        super();
        this.balance = ko.observable("");
        this.expirationDate = ko.observable("");
        this.number = ko.observable("");
    }


    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }


    public open(giftCard: ProxyEntities.GiftCard): Promise<IGiftCardBalanceDialogResult> {
        this.balance(giftCard.BalanceCurrencyCode.concat(" ", giftCard.Balance.toString()));
        this.expirationDate(DateExtensions.now.toDateString());
        this.number(giftCard.Id);

        let promise: Promise<IGiftCardBalanceDialogResult> =
            new Promise((resolve: (value: IGiftCardBalanceDialogResult) => void,
                reject: (reason: any) => void) => {

                let option: Dialogs.ITemplatedDialogOptions = {
                    title: "Gift card balance",
                    onCloseX: () => {
                        resolve({});
                        return true;
                    },
                    button1: {
                        id: "CloseButton",
                        label: "Close",
                        isPrimary: true,
                        onClick: () => {
                            resolve({});
                            return true;
                        }
                    }
                }

                this.openDialog(option);

            });

        return promise;
    }
}