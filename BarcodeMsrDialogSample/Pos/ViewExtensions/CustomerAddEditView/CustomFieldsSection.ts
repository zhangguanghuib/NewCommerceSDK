import ko from "knockout";

import {
    CustomerAddEditCustomControlBase
} from "PosApi/Extend/Views/CustomerAddEditView";

export default class CustomFieldsSection extends CustomerAddEditCustomControlBase {

    public ssn: ko.Observable<string>;

    protected init(state: Commerce.Extensibility.ICustomerAddEditCustomControlState): void {

    }


    onReady(element: HTMLElement): void {

    }

}