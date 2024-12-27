## Utilize Extension Properties to Store Custom Data and Serialize it

## Background 
In Some Scenarios, we may need store custom data into the standard data entity,  then we can utilize the ExtensionProperties. All data entities that extends CommerceEntity has this propertity, it is an array of CommereProperty which has a key and Value, we can do a lot of things through ExtensionProperties.

Assume a scenario: client ordered a furniture, then want to set the installation date, but the standard function don't have that. In this scenario, we can use ExtensionProperties to archieve that.

## Finally effect:
. Create a customer order.<br/>
. Choose a cart line<br/>
. Click the button on POS:<br/>
  <img width="1068" alt="image" src="https://github.com/user-attachments/assets/9084793f-4504-447e-a838-c2d94927bf8d" /><br>
. Choose a date as installation date, click "OK":<br/>
  <img width="254" alt="image" src="https://github.com/user-attachments/assets/5c31f3b9-5d84-4044-ae3a-60e11adf91af" /> <br/>
. Go to Customer Order Delivery Grid Tab:<br/>
  <img width="607" alt="image" src="https://github.com/user-attachments/assets/41da8a95-c5dc-487e-b69a-baa2a5750867" /><br/>
. Choose a Shipping Method, and then check out the cart.<br/>
. Go to F&O, you will find the installation date for this order line has been recorded into the system.<br/>
  <img width="368" alt="image" src="https://github.com/user-attachments/assets/7c8d7b0a-f25b-43e7-a128-8c591ede80bd" /><br/>

## Implementation details:
### Date Pick Dialog:
```html
<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title></title>
</head>
<body>
    <div class="SearchTransactionsDialog col grow">
        <!-- HTMLLint Disable LabelExistsValidator -->
        <!-- dialog content -->
        <div>
            <div class="h4">Start Date:</div>
            <div class="col stretch">
                <div id="isStartDateOn" />
            </div>
            <div class="pad8">
                <div id="datePickerStart" data-bind="disabled: isStartDateDisabled">
                </div>
            </div>
            <br />
            <div class="h4">End Date:</div>
            <div class="col stretch">
                <div id="isEndDateOn" />
            </div>
            <div class="pad8">
                <div id="datePickerEnd">
                </div>
            </div>
            <div class="h4">Transaction number</div>
            <div class="pad8">
                <textarea class="height220" data-bind="value: transactionIds"></textarea>
            </div>
            <br />
        </div>
        <!-- HTMLLint Enable LabelExistsValidator -->
        <div class="grow"></div>
        <div class="row"></div>
    </div>
</body>
</html>
```
```TS
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

    public toggleSwitchStartDate: Controls.IToggle;
    public toggleSwitchEndDate: Controls.IToggle;

    public isStartDateDisabled: ko.Observable<boolean>;
    public isEndDateDisabled: ko.Observable<boolean>;

    constructor() {
        super();
        this.transactionIds = ko.observable("");
        this.startDate = DateExtensions.getDate(new Date());
        this.endDate = DateExtensions.getDate(new Date());

        this.isStartDateDisabled = ko.observable(false);
        this.isEndDateDisabled = ko.observable(false);
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


        let toggleOptions: Controls.IToggleOptions = {
            labelOn: "On",
            labelOff: "Off",
            checked: !this.isStartDateDisabled(),
            enabled: true,
            tabIndex: 0
        };

        let toggleRootElemStartDate: HTMLDivElement = element.querySelector("#isStartDateOn") as HTMLDivElement;
        this.toggleSwitchStartDate = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "Toggle", toggleOptions, toggleRootElemStartDate);
        this.toggleSwitchStartDate.addEventListener("CheckedChanged", (eventData: { checked: boolean }) => {
            this.toggleStartDate(eventData.checked);
        });

        let toggleOptionsEndDate: Controls.IToggleOptions = {
            labelOn: "On",
            labelOff: "Off",
            checked: !this.isEndDateDisabled(),
            enabled: true,
            tabIndex: 0
        };

        let toggleRootElemEndDate: HTMLDivElement = element.querySelector("#isEndDateOn") as HTMLDivElement;
        this.toggleSwitchEndDate = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "Toggle", toggleOptionsEndDate, toggleRootElemEndDate);
        this.toggleSwitchEndDate.addEventListener("CheckedChanged", (eventData: { checked: boolean }) => {
            this.toggleEndDate(eventData.checked);
        });
    }

    public toggleStartDate(checked: boolean): void {
        this.isStartDateDisabled(!checked);
    }

    public toggleEndDate(checked: boolean): void {
        this.isEndDateDisabled(!checked);
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
            SearchLocationTypeValue: 1
        };

        if (this.isStartDateDisabled()) {
            searchCriteria.StartDateTime = DateExtensions.addDays(DateExtensions.getDate(new Date()), -3);
        } else {
            searchCriteria.StartDateTime = this.startDate;
        }

        if (this.isEndDateDisabled()) {
            searchCriteria.EndDateTime = DateExtensions.getDate(new Date());
        } else {
            searchCriteria.EndDateTime = this.endDate;
        }

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
```





