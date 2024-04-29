## Complex POS UI Development Sample (Like Bank Drop)

1. **Overall Background:**

   <p>As you know in the age of Retail SDK, MPOS or Cloud CPOS, the POS extension is totally integrated with the Out-of-Box CPOS or MPOS, so when develop engineers develop  POS View, POS dialog,  they even can just utilize the OOB POS Control in their customization code, it is working fine.</p>
   <p>But moving forward to Commerce SDK,  when develop Store Commerce Extensions, there are a lot of new limitations, the major one is a lot PosUI Control is no longer usable, one typical sampel the Numpad. In Commerce SDK  we have to implement Numpad in a totally different way instead of just putting the Numpad Control on the POS view. More official document can be found:<br/>
      
   [Use POS controls in extensions](https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/pos-extension/controls-pos-extension)
   
   <p> From the above offical doc, you can see only these POS Controls are supported by PosApi:</p>
     
   | Control | Interfaces | Description |
   |---------|------------|-------------|
   | Data list | IDataList, IPaginatedDataList | A responsive list control that is used throughout POS to show rows of information. |
   | Date picker  | IDatePicker | The date picker control that is used in POS. |
   | Menu | IMenu | The menu control that is used in POS to show contextual information. |
   | Number Pad | IAlphanumericNumPad, ICurrencyNumPad, INumericNumPad, ITransactionNumPad | <p>Number pads that are used throughout POS. Different types of number pads have different behaviors and input formatting:</p><ul><li>**Alphanumeric numpad** – This type of number pad accepts alphanumeric input.</li><li>**Currency numpad** – This type of number pad accepts monetary values.</li><li>**Numeric numpad** – This type of number pad accepts only numeric values.</li><li>**Transaction numpad** – This type of number pad accepts item identifiers or quantities. It's typically used in transaction scenarios.</li></ul> |
   | Time Picker | ITimePicker | The time picker control that is used in POS. |
   | Toggle | IToggle | The toggle switch control that is used in POS. |


2. **Commerce SDK's technical limitations**
   - The first challenge: <br/>
   <p>The big limitation of the datalist control is its columns only support *string*, the normal data list code is like</p>
   
   ```Javascript
    let dataListOptions: IDataListOptions<Entities.ExampleEntity> = {
        interactionMode: DataListInteractionMode.SingleSelect,
        data: this.viewModel.loadedData,
        columns: [
            {
                title: this.context.resources.getString("string_1001"), // Int data
                ratio: 40, collapseOrder: 1, minWidth: 100,
                computeValue: (data: Entities.ExampleEntity): string => data.IntData.toString()
            },
            {
                title: this.context.resources.getString("string_1002"), // String data
                ratio: 60, collapseOrder: 2, minWidth: 100,
                computeValue: (data: Entities.ExampleEntity): string => data.StringData
            }
        ]
   };
   ```

   That means that each column of the data list can only be a simple string, if user want to go to different view or open different dialog, from Commerce Product Group,  that is impossible.<br/>
   - The second challenge: <br/>
      Currently POS api only supports the simple dialog:<br/>
     ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/41b044f5-9b63-4e60-853e-5d37764b5f63)<br/>
     If you want to a little bit complex dialog, even including a Numeric Numpad on the dialog,  you may need extend the Templated Dialog to build the dialog like you build a normal POS View, that is also a challenge.<br/>
     
   - The 3rd challenge: <br/>
     There is another challenge is that in Retail SDK,  developers can user ApplicationContext to get the cached global variables like Store Information,  Store Tender Types, Store Default Customers, some parnters heavily depends on these variables,  but in Commerce SDK,  these unavailable. 
     

4. **This sample's goal is to make Bank-Drop Like Function totally depends on POS  Api instead of copying the OOB POS Controls since that is not supported:**
   - Finally the function looks like this
   - Open the custom "Bank Drop" View,  it looks like:
   - <img width="1443" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/294d4e36-538b-4423-8c54-eef9c8f52c5e">
   - If we click the tender type which are not cash, it will pop-up dialog to ask you input the tender amount:
   - ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ade76e8c-41d3-49ed-b303-0954c50b02ab)
   - If we click the "Button" embeded in the line, it will open another view:
   - ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/a55aef19-d5eb-4d53-87b7-fec1da64710a)
   - If we click the "Quantity Column", it will open the "Templated Dialog" that will have Numpad that only allow input the "Integer" Quantity
   - ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/6cfabcd8-1109-49af-aa3a-5340724234fe)
   - <img width="1490" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/08bee820-ad10-4c27-a000-01f13262d500"><br/>
      You will notice when the input quantity number value changed,  the Demonination Total and Currency Total will be changing to reflect the entered Quntity
   - When click "Enter" on the Numpad,  yoy will see the demonination line's Quantity/Amount will changed as well<br/>
     <img width="1230" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/6432b33d-3190-40e0-8d38-8244b1c5ce0c">
   - When click "Total" column,  you will see the different dialog will open, this time it is a current numpad, so it allow input decimal number:<br/>
      ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/0d4bb86e-dfb0-4a77-9d85-268a6fa6c946)
   - Please "Enter" on the numpad, the same thing happened:<br/>
     <img width="1198" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/b2cff769-ee42-4abe-a012-c28e65eb6a45">
   - The last thing is if we click another "cash" pay tender type:<br/>
   <img width="1427" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/79f9ed28-b3a5-4e7f-b420-ae63c0a6fef4">
   - In the opened denomination lines,  the window total comes from its parent:
   - <img width="1182" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/5fe3bbd6-cd67-4006-93ca-72054305bdda">

5. **How to make the program know which column/row user clicked?**
   - The solution is to embeded the relative information in javascript code in the data list, take the denomination lines page as sample:<br/>
   
     ```typescript
        let dataListOptions: IDataListOptions<ProxyEntities.DenominationDetail> = {
         interactionMode: Controls.DataListInteractionMode.Invoke,
         data: this.viewModel.denominationDetailLines(),
         columns: [
             {
                 title: "DENOMINATION",
                 ratio: 40, collapseOrder: 1, minWidth: 100,
                 computeValue: (data: ProxyEntities.DenominationDetail): string => data.DenominationAmount.toFixed(2)
             },
             {
                 title: "QUANTITY",
                 ratio: 30, collapseOrder: 2, minWidth: 100,
                 computeValue: (data: ProxyEntities.DenominationDetail): string => { 
                     return `<button class='btnDenominationCount' onclick="localStorage.setItem('QuantityDeclared_DenominationDetailView', '')">${data.QuantityDeclared.toFixed(0) }</button>`;                         
                 }
             }, {
                 title: "TOTAL",
                 ratio: 30, collapseOrder: 3, minWidth: 100,
                 computeValue: (data: ProxyEntities.DenominationDetail): string => {
                     //return CurrencyFormatter.toCurrency(data.AmountDeclared);
                     return `<button class='btnDenominationCount' onclick="localStorage.setItem('AmountDeclared_DenominationDetailView', '')">${data.AmountDeclared.toFixed(2)}</button>`;                         
                 }
             }
         ]
       };
    ```
   <br/>
   The idea is to make the column as a button,  and it got clicked,  in the LocalStorage to record this column name,  the code is as  above.
   
6. **Based on the key(column name) stored in localStorage, it will open different dialog**
   ```
       public listItemSelected(item: ProxyEntities.DenominationDetail): Promise<void> {

        let keyQuantityDeclared: string = 'QuantityDeclared_DenominationDetailView';
        let keyAmountDeclared: string = 'AmountDeclared_DenominationDetailView';

        if (localStorage.getItem(keyQuantityDeclared) !== null) {
            // alert(`QUANTITY column cliked, the clicked row ${item.QuantityDeclared}- ${item.DenominationAmount}`);
            localStorage.removeItem(keyQuantityDeclared);
            let contosoDenominationInputDialog: ContosoDenominationInputDialog = new ContosoDenominationInputDialog();
            let inputs: IContosoDenominationInputDialogResult = {
                item: item,
                inputType: "Quantity" 
            }
            return contosoDenominationInputDialog.open(inputs).then((result: IContosoDenominationInputDialogResult) => {
                if (!result.canceled) {
                    item.QuantityDeclared = result.item.QuantityDeclared;
                    item.AmountDeclared = result.item.AmountDeclared;
                }
            });
        } else if (localStorage.getItem(keyAmountDeclared) !== null) {
            // alert(`ToTAL column cliked, the clicked row ${item.AmountDeclared}- ${item.DenominationAmount}`);
            localStorage.removeItem(keyAmountDeclared);
            let contosoDenominationAmountDialog: ContosoDenominationAmountDialog = new ContosoDenominationAmountDialog();
            let inputs: IContosoDenominationInputDialogResult = {
                item: item,
                inputType: "Amount"
            }
            return contosoDenominationAmountDialog.open(inputs).then((result: IContosoDenominationInputDialogResult) => {
                if (!result.canceled) {
                    item.QuantityDeclared = result.item.QuantityDeclared;
                    item.AmountDeclared = result.item.AmountDeclared;
                }
            });
        }

        return Promise.resolve();
    }
   ```


8. **How to make a Templated Dialog instead of the simple dialog <br/>**\
   - It need extends Dialogs.ExtensionTemplatedDialogBase
   - Then you can made the dialog as the normal view, like declare Observable variables and bind to the html page filed
   - It has an open method to open the dialog
   - The open method should return a promise
   ```TS 
   import * as Dialogs from "PosApi/Create/Dialogs";
   import * as Views from "PosApi/Create/Views";
   import * as Controls from "PosApi/Consume/Controls";
   import ko from "knockout";
   import { IContosoDenominationInputDialogResult } from "./ContosoDenominationNumericDialogTypes";
   import { ObjectExtensions } from "PosApi/TypeExtensions";
   import { ProxyEntities } from "PosApi/Entities";
   import { CurrencyFormatter } from "PosApi/Consume/Formatters";
   import ContosoNumberExtensions from "../ContosoTenderCounting/ContosoNumberExtensions";
   
   type ContosoDenominationInputDialogResolveFunction = (value: IContosoDenominationInputDialogResult) => void;
   type ContosoDenominationInputDialogRejectFunction = (reason: any) => void;
   export default class ContosoDenominationInputDialog extends Dialogs.ExtensionTemplatedDialogBase {
       public _item: ProxyEntities.DenominationDetail;
       public numPad: Controls.INumericNumPad;
       public numpadValue: ko.Observable<string>;
       private readonly _numPadOptions: Controls.INumericNumPadOptions;
   
       public currency: ko.Observable<string>;
       public denominationLabel: ko.Observable<string>;
       public denominationValue: ko.Observable<string>;
       public denominationTotalValue: ko.Observable<string>;
   
       public resolve: ContosoDenominationInputDialogResolveFunction;
   
       constructor() {
           super();
           this.numpadValue = ko.observable('');
           this.currency = ko.observable('USD');
           this.denominationLabel = ko.observable('1.00');
           this.denominationValue = ko.observable('2.00');
           this.denominationTotalValue = ko.observable('4.00');
   
           this._numPadOptions = {
               decimalPrecision: 0,
               globalInputBroker: undefined,
               label: "Enter Quantity",
               value: 0
           };
       }
   
   
       public onReady(element: HTMLElement): void {
           ko.applyBindings(this, element);
   
           let numPadRootElem: HTMLDivElement = element.querySelector("#NumericNumPad") as HTMLDivElement;
           this.numPad = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "NumericNumPad", this._numPadOptions, numPadRootElem);
           this.numPad.addEventListener("EnterPressed", (eventData: { value: Commerce.Extensibility.NumPadValue }): Promise<IContosoDenominationInputDialogResult> => {
               return this._resolvePromise({ canceled: false, item: this._item, value: Number(eventData.value.toString())});
           });
   
           this.numPad.addEventListener("ValueChanged", (eventData: { value: Commerce.Extensibility.NumPadValue }) => {
               // Update UI
               let denominationQty: number = Number(eventData.value.toString());
               this.denominationValue(CurrencyFormatter.toCurrency(denominationQty));
               let denominationAmount: number = ContosoNumberExtensions.roundToNDigits(denominationQty * this._item.DenominationAmount, 2);
               this.denominationTotalValue(CurrencyFormatter.toCurrency(denominationAmount));
   
               // Update original item
               this._item.QuantityDeclared = denominationQty;
               this._item.AmountDeclared = denominationAmount;
           });
       }
   
       public open(inputDialogResult: IContosoDenominationInputDialogResult): Promise<IContosoDenominationInputDialogResult> {
           let promise: Promise<IContosoDenominationInputDialogResult> = new Promise((resolve: ContosoDenominationInputDialogResolveFunction, reject: ContosoDenominationInputDialogRejectFunction) => {
               this.resolve = resolve;
               this._item = inputDialogResult.item;
   
               this.currency(inputDialogResult.item.Currency);
               this.denominationLabel = ko.observable(inputDialogResult.item.DenominationAmount);
               this.denominationValue = ko.observable(CurrencyFormatter.toCurrency(inputDialogResult.item.QuantityDeclared));
               this.denominationTotalValue = ko.observable(CurrencyFormatter.toCurrency(inputDialogResult.item.AmountDeclared));
   
               let option: Dialogs.ITemplatedDialogOptions = {
                   title: inputDialogResult.inputType,
                   onCloseX: this._cancelButtonClickHandler.bind(this)
               };
               this.openDialog(option);
           });
   
           return promise;
       }
   
       private _cancelButtonClickHandler(): boolean {
           this._resolvePromise({ canceled: true });
           return false;
       }
   
       private onNumPadEnter(value: Commerce.Extensibility.NumPadValue) {
           this.numpadValue(value.toString());
           this.numPad.value = 0;
       }
   
       private _resolvePromise(result: IContosoDenominationInputDialogResult): Promise<IContosoDenominationInputDialogResult> {
           if (ObjectExtensions.isFunction(this.resolve)) {
               this.resolve(result);
   
               this.resolve = null;
               this.closeDialog();
               return Promise.resolve(result);
           } else {
               return Promise.resolve({ canceled: true });
           }
       }
   }
   ```
and it need has a html to show the customized fields as you want:
<div class="ContosoDenominationNumericDialog col grow maxWidth320">
    <div id="denominationDialogTemplate">
        <div class="grow">
            <div class="h4 secondaryFontColor padTop16">Denomination</div>
            <div class="row padBottom12 denominationLine">
                <div class="grow centerY">
                    <div class="h1" data-bind="text: currency">USD</div>
                </div>
                <div class="grow centerY alignRight">
                    <div class="h1" data-bind="text: denominationLabel">1.00</div>
                </div>
            </div>
            <div class="padTop12">
                <div class="row padBottom12">
                    <div class="grow">
                        <div class="h4 secondaryFontColor">Denomination total</div>
                    </div>
                    <div class="grow textRight">
                        <div class="h4" data-bind="text: denominationValue">0.00</div>
                    </div>
                </div>
            </div>
            <div class="padTop12">
                <div class="row padBottom12">
                    <div class="grow">
                        <div class="h4 secondaryFontColor">Currency total</div>
                    </div>
                    <div class="grow textRight">
                        <div class="h4" data-bind="text: denominationTotalValue">0.00</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="grow"></div>
    <div class="pad20 col maxWidth320">
        <div class="pad8">
            <div class="minWidth260 maxWidth320" id="NumericNumPad"></div>
            <div class="h4" data-bind="text: numpadValue"></div>
        </div>
    </div>
</div>
[Source code](https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/Solutions/Notification-Sample)
