import * as Dialogs from "PosApi/Create/Dialogs";
import ko from "knockout";
import { ProxyEntities } from "PosApi/Entities";
import { DateExtensions, ObjectExtensions } from "PosApi/TypeExtensions";
import * as Controls from "PosApi/Consume/Controls";

type SearchTransactionsDialogResolve = (value: ProxyEntities.TransactionSearchCriteria) => void;
type SearchTransactionsDialogReject = (reason: any) => void;
export default class SearchTransactionsDialog extends Dialogs.ExtensionTemplatedDialogBase {

    private _resolve: SearchTransactionsDialogResolve;
    private startDatePicker: Controls.IDatePicker;
    private endDatePicker: Controls.IDatePicker;

    private startDate: Date;
    private endDate: Date;
    public transactionIds: ko.Observable<string>;

    constructor() {
        super();
        this.transactionIds = ko.observable("");
        this.startDate = DateExtensions.getDate(new Date());
        this.endDate = DateExtensions.getDate(new Date());
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        let datePickerOptions: Controls.IDatePickerOptions = {
            date: new Date(),
            enabled: true
        };

        let startDatePickerRootElem: HTMLDivElement = element.querySelector("#datePickerStart") as HTMLDivElement;
        this.startDatePicker = this.context.controlFactory.create("", "DatePicker", datePickerOptions, startDatePickerRootElem);
        this.startDatePicker.addEventListener("DateChanged", (eventData: { date: Date }) => {
            this._dateChangedHandler(eventData.date);
            this.startDate = eventData.date;
        });

        let endDatePickerRootElem: HTMLDivElement = element.querySelector("#datePickerEnd") as HTMLDivElement;
        this.endDatePicker = this.context.controlFactory.create("", "DatePicker", datePickerOptions, endDatePickerRootElem);
        this.endDatePicker.addEventListener("DateChanged", (eventData: { date: Date }) => {
            this._dateChangedHandler(eventData.date);
            this.endDate = eventData.date;
        });
    }

    public open(): Promise<ProxyEntities.TransactionSearchCriteria> {

        let promise: Promise<ProxyEntities.TransactionSearchCriteria> = new Promise((resolve: SearchTransactionsDialogResolve, reject: SearchTransactionsDialogReject) => {
            this._resolve = resolve;
            let option: Dialogs.ITemplatedDialogOptions = {
                title: "Search Transaction",
                button1: {
                    id: "btnUpdate",
                    label: "OK",
                    isPrimary: true,
                    onClick: this.btnUpdateClickHandler.bind(this)
                },
                button2: {
                    id: "btnCancel",
                    label: "Cancel",
                    onClick: this.btnCancelClickHandler.bind(this)
                },
            };

            this.openDialog(option);
        });

        return promise;
    }

    private btnUpdateClickHandler(): boolean {

        let searchCriteria: ProxyEntities.TransactionSearchCriteria = {
            StartDateTime: this.startDate,
            EndDateTime: this.endDate,
            SearchLocationTypeValue: 1,
        };

        if (this.transactionIds()) {
            searchCriteria.TransactionIds = this.transactionIds().split(',');
        }

        this.resolvePromise(searchCriteria);

        return true;
    }

    private btnCancelClickHandler(): boolean {
        // Cancel will return the original value
        // this.resolvePromise(this._originalStoreHour);
        this.resolvePromise(null);
        return true;
    }

    private resolvePromise(result: ProxyEntities.TransactionSearchCriteria): void {
        if (ObjectExtensions.isFunction(this._resolve)) {
            this._resolve(result);
            this._resolve = null;
        }
    }

    private _dateChangedHandler(newDate: Date): void {
        this.context.logger.logInformational("DateChanged: " + JSON.stringify(newDate));
    }

}