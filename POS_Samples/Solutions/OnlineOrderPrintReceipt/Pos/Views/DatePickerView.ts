import ko from "knockout";

import * as Views from "PosApi/Create/Views";
import * as Controls from "PosApi/Consume/Controls";
import { ObjectExtensions } from "PosApi/TypeExtensions";

/**
 * The controller for DatePickerView.
 */
export default class DatePickerView extends Views.CustomViewControllerBase {
    public datePicker: Controls.IDatePicker;

    constructor(context: Views.ICustomViewControllerContext) {
        super(context);
        this.state.title = "DatePicker sample";
    }

    /**
     * Bind the html element with view controller.
     *
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {

        ko.applyBindings(this, element);

        let datePickerOptions: Controls.IDatePickerOptions = {
            date: new Date(),
            enabled: true
        };

        let datePickerRootElem: HTMLDivElement = element.querySelector("#datePickerSample") as HTMLDivElement;
        this.datePicker = this.context.controlFactory.create("", "DatePicker", datePickerOptions, datePickerRootElem);
        this.datePicker.addEventListener("DateChanged", (eventData: { date: Date }) => {
            this._dateChangedHandler(eventData.date);
        });
    }

    /**
     * Called when the object is disposed.
     */
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    /**
     * Callback for date picker.
     * @param {Date} newDate
     */
    private _dateChangedHandler(newDate: Date): void {
        this.context.logger.logInformational("DateChanged: " + JSON.stringify(newDate));
    }
}