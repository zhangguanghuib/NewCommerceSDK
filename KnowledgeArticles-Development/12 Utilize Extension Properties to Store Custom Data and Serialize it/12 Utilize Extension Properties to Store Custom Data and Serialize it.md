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

### POS Operation to set the the "Installation Date" to Cartline's extension properties:<br/>
SaveDataToSelectedCartLineFactory.ts<br/>
```TS
import { ExtensionOperationRequestFactoryFunctionType } from "PosApi/Create/Operations";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { DateExtensions, ObjectExtensions } from "PosApi/TypeExtensions";
import SaveDataToSelectedCartLineRequest from "./SaveDataToSelectedCartLineRequest";
import SaveDataToSelectedCartLineResponse from "./SaveDataToSelectedCartLineResponse";
import SearchTransactionsDialog from "./../../Dialogs/SearchTransactionsDialog";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<SaveDataToSelectedCartLineResponse> =
    function (
        context: IExtensionContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<SaveDataToSelectedCartLineRequest<SaveDataToSelectedCartLineResponse>>> {

        let installationDate: string = DateExtensions.getDate().toDateString();

        let dialog: SearchTransactionsDialog = new SearchTransactionsDialog();
        return dialog.open().then((criteria: ProxyEntities.TransactionSearchCriteria) => {
            if (!ObjectExtensions.isNullOrUndefined(criteria)) {
                installationDate = criteria.StartDateTime.toDateString();
            }
        }).then(() => {
            let operationRequest: SaveDataToSelectedCartLineRequest<SaveDataToSelectedCartLineResponse> =
                new SaveDataToSelectedCartLineRequest(correlationId, installationDate);

            return Promise.resolve(<ClientEntities.ICancelableDataResult<SaveDataToSelectedCartLineRequest<SaveDataToSelectedCartLineResponse>>>{
                canceled: false,
                data: operationRequest
            });
        });
    };

export default getOperationRequest;
```

SaveDataToSelectedCartLineHandler<br/>
```TS
import { GetCurrentCartClientRequest, GetCurrentCartClientResponse, SaveExtensionPropertiesOnCartLinesClientRequest, SaveExtensionPropertiesOnCartLinesClientResponse } from "PosApi/Consume/Cart";
import { IListInputDialogItem, IListInputDialogOptions, ShowListInputDialogClientRequest, ShowListInputDialogClientResponse } from "PosApi/Consume/Dialogs";
import { ExtensionOperationRequestHandlerBase, ExtensionOperationRequestType } from "PosApi/Create/Operations";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { StringExtensions } from "PosApi/TypeExtensions";
import CartViewController from "../../../Extend/ViewExtensions/Cart/CartViewController";
import SaveDataToSelectedCartLineRequest from "./SaveDataToSelectedCartLineRequest";
import SaveDataToSelectedCartLineResponse from "./SaveDataToSelectedCartLineResponse";

export default class SaveDataToSelectedCartLineHandler<TResponse extends SaveDataToSelectedCartLineResponse>
    extends ExtensionOperationRequestHandlerBase<TResponse>{

    public supportedRequestType(): ExtensionOperationRequestType<TResponse> {
        return SaveDataToSelectedCartLineRequest;
    }

    public executeAsync(request: SaveDataToSelectedCartLineRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<TResponse>> {
        this.context.logger.logInformational("Log message from SaveDataToSelectedCartLineHandler executeAsync().", request.correlationId);
        let getCurrentCartRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(request.correlationId);

        return this.context.runtime.executeAsync(getCurrentCartRequest)
            .then((getCurrentCartClientResponse: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>) => {
                if (getCurrentCartClientResponse.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{
                        canceled: true,
                        data: null
                    });
                }

                let selectedCartLineId: string = CartViewController.selectedCartLineId;
                if (StringExtensions.isNullOrWhitespace(selectedCartLineId)) {
                    return this._showDialog(this.context, getCurrentCartClientResponse.data.result)
                        .then((dialogResult: ClientEntities.ICancelableDataResult<string>) => {
                            if (dialogResult.canceled) {
                                return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{ canceled: true, data: null });
                            }

                            return this._saveDataToCartLineByCartLineId(
                                dialogResult.data,
                                request.installationDate,
                                request.correlationId,
                                getCurrentCartClientResponse.data.result
                            );
                        });
                } else {
                    return this._saveDataToCartLineByCartLineId(
                        selectedCartLineId,
                        request.installationDate,
                        request.correlationId,
                        getCurrentCartClientResponse.data.result
                    );
                }
            });
    }

    private _saveDataToCartLineByCartLineId(cartLineId: string, value: string, correlationId: string, cart: ProxyEntities.Cart):
        Promise<ClientEntities.ICancelableDataResult<TResponse>> {

        let cartLineExtensionProperty: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        cartLineExtensionProperty.Key = "installationDate";
        cartLineExtensionProperty.Value = new ProxyEntities.CommercePropertyValueClass();
        cartLineExtensionProperty.Value.StringValue = value;

        let extensionPropertiesOnCartLine: ClientEntities.IExtensionPropertiesOnCartLine = {
            cartLineId: cartLineId,
            extensionProperties: [cartLineExtensionProperty]
        };

        let saveExtensionPropertiesOnCartLineRequest: SaveExtensionPropertiesOnCartLinesClientRequest<SaveExtensionPropertiesOnCartLinesClientResponse> =
            new SaveExtensionPropertiesOnCartLinesClientRequest([extensionPropertiesOnCartLine], correlationId);

        return this.context.runtime.executeAsync(saveExtensionPropertiesOnCartLineRequest)
            .then((saveExtensionPropertiesOnCartLinesClientResponse: ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>) => {
                if (saveExtensionPropertiesOnCartLinesClientResponse.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{
                        canceled: true,
                        data: null
                    });
                }
                return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{
                    canceled: false,
                    data: new SaveDataToSelectedCartLineResponse(cart)
                });
            });
    }

    private _showDialog(context: IExtensionContext, cart: ProxyEntities.Cart): Promise<ClientEntities.ICancelableDataResult<string>> {
        let convertedListItems: IListInputDialogItem[] = cart.CartLines.map((cartline: ProxyEntities.CartLine): IListInputDialogItem => {
            return {
                label: cartline.Description,
                value: cartline.LineId
            };
        });

        let listInputDialogOptions: IListInputDialogOptions = {
            title: "Select a cart line",
            subTitle: "Cart lines",
            items: convertedListItems
        };

        let dialogRequest: ShowListInputDialogClientRequest<ShowListInputDialogClientResponse> =
            new ShowListInputDialogClientRequest<ShowListInputDialogClientResponse>(listInputDialogOptions);

        return context.runtime.executeAsync(dialogRequest)
            .then((result: ClientEntities.ICancelableDataResult<ShowListInputDialogClientResponse>) => {
                if (result.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<string>>{
                        canceled: true,
                        data: null
                    });
                }

                return Promise.resolve(<ClientEntities.ICancelableDataResult<string>>{
                    canceled: false,
                    data: result.data.result.value.value
                });
            });
    }
}
```

The response and request as below:
```TS
import { Response } from "PosApi/Create/RequestHandlers";
import { ProxyEntities } from "PosApi/Entities";

export default class SaveDataToSelectedCartLineResponse extends Response {
    public cart: ProxyEntities.Cart;
    constructor(cart: ProxyEntities.Cart) {
        super();
        this.cart = cart;
    }
}
```

```TS
import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import SaveDataToSelectedCartLineResponse from "./SaveDataToSelectedCartLineResponse";

export default class SaveDataToSelectedCartLineRequest<TResponse extends SaveDataToSelectedCartLineResponse> extends ExtensionOperationRequestBase<TResponse> {
    public readonly installationDate: string;
    constructor(correlationId: string, installationDate: string) {
        super(60004, correlationId);
        this.installationDate = installationDate;
    }
}
```

###  The API via CartViewController to get which cartline are selected:
```TS
import * as CartView from "PosApi/Extend/Views/CartView";
import { ProxyEntities } from "PosApi/Entities";
import { IExtensionCartViewControllerContext } from "PosApi/Extend/Views/CartView";
import { ArrayExtensions, StringExtensions } from "PosApi/TypeExtensions";

export default class CartViewController extends CartView.CartExtensionViewControllerBase {

    public static selectedCartLineId: string = StringExtensions.EMPTY;
    private _selectedCartLines: ProxyEntities.CartLine[];
    public _selectedTenderLines: ProxyEntities.TenderLine[];
    public _isProcessingAddItemOrCustomer: boolean;

    /**
     * Creates a new instance of the CartViewController class.
     * @param {IExtensionCartViewControllerContext} context The events Handler context.
     * @remarks The events handler context contains APIs through which a handler can communicate with POS.
     */
    constructor(context: IExtensionCartViewControllerContext) {
        super(context);

        this.cartLineSelectedHandler = (data: CartView.CartLineSelectedData): void => {
            this._selectedCartLines = data.cartLines;

            if (ArrayExtensions.hasElements(this._selectedCartLines)) {
                CartViewController.selectedCartLineId = this._selectedCartLines[0].LineId;
            }
        };

        this.cartLineSelectionClearedHandler = (): void => {
            this._selectedCartLines = undefined;
            CartViewController.selectedCartLineId = null;
        };

        this.tenderLineSelectedHandler = (data: CartView.TenderLineSelectedData): void => {
            this._selectedTenderLines = data.tenderLines;
        };

        this.tenderLineSelectionClearedHandler = (): void => {
            this._selectedCartLines = undefined;
        };

        this.processingAddItemOrCustomerChangedHandler = (processing: boolean): void => {
            this._isProcessingAddItemOrCustomer = processing;
        };
    }
}
```

### The new column of "Installation Date" on the Cart Line, Delivery Grid<br/>
```TS
import {
    ICustomDeliveryGridColumnContext,
    CustomDeliveryGridColumnBase
} from "PosApi/Extend/Views/CartView";
import { CustomGridColumnAlignment } from "PosApi/Extend/Views/CustomGridColumns";
import { ProxyEntities } from "PosApi/Entities";

<mark>/**
 * HOW TO ENABLE THIS SAMPLE
 *
 * 1) In HQ, go to Retail > Channel setup > POS > Screen layouts
 * 2) Filter results to "Fabrikam Manager"
 * 3) Under Layout sizes, select the resolution of your MPOS, and click on Layout designer
 * 4) Download, run and sign in to the designer.
 * 5) Right click on Delivery, and click on customize.
 * 6) Find CUSTOM COLUMN 1 in Available columns and move it to Selected columns.
 * 7) Click OK and close the designer.
 * 8) Back in HQ, Go to Retail > Retail IT > Distribution schedule.
 * 9) Select job "9999" and click on Run now.
 */</mark>

export default class DeliveryCustomGridColumn1 extends CustomDeliveryGridColumnBase {

    /**
     * Creates a new instance of the DeliveryCustomGridColumn1 class.
     * @param {ICustomDeliveryGridColumnContext} context The extension context.
     */
    constructor(context: ICustomDeliveryGridColumnContext) {
        super(context);
    }

    /**
     * Gets the custom column title.
     * @return {string} The column title.
     */
    public title(): string {
        return "Installation Date"; 
    }

    /**
     * The custom column cell compute value.
     * @param {ProxyEntities.CartLine} tenderLine The tender line.
     * @return {string} The cell value.
     */
    public computeValue(cartLine: ProxyEntities.CartLine): string {

        let installationDate: string = "";

        cartLine.ExtensionProperties.forEach((extensionProperty: ProxyEntities.CommerceProperty) => {
            if (extensionProperty.Key === "installationDate") {
                installationDate = extensionProperty.Value.StringValue;
            }
        });

        return installationDate;
    }

    /**
     * Gets the custom column alignment.
     * @return {CustomGridColumnAlignment} The alignment.
     */
    public alignment(): CustomGridColumnAlignment {
        return CustomGridColumnAlignment.Right;
    }
}
```

### Finally in F&O,  we fine a right place to insert our custom data that is on extension property to FO System: <br/>
```CS
using System.Collections.Specialized;
using System.Reflection;
using Microsoft.Dynamics.Commerce.Runtime.Services.CustomerOrder;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;

[ExtensionOf(classstr(RetailCreateCustomerOrderExtensions))]
final class RetailCreateCustomerOrderExtensions_Extension
{
    public static void preSalesLineCreate(RetailCustomerOrderLineParameters retailCustomerOrderLineParameters, SalesLineCreateLineParameters salesLineCreateLineParameters)
    {
        SalesTable salesTable;
        OrderLineInstallation orderLineInstallation;

        if (retailCustomerOrderLineParameters.orderHeader.TableId == tableNum(SalesTable))
        {
            salesTable = retailCustomerOrderLineParameters.orderHeader;
        }

        ItemId itemId = retailCustomerOrderLineParameters.itemInfo.ItemId;
        var extensionProps = retailCustomerOrderLineParameters.itemInfo.ExtensionProperties;
        LineNum lineNum = retailCustomerOrderLineParameters.itemInfo.LineNumber;
        str installationDate = '';

        for (int i = 0; i < extensionProps.get_Count(); i++)
        {
            var commerceProperty = extensionProps.get_Item(i);
            
            if (commerceProperty.get_Key() == 'installationDate')
            {
                installationDate = commerceProperty.get_Value().get_StringValue();
                break;
            }
        }

        OrderLineInstallation.SalesId = salesTable.SalesId;
        OrderLineInstallation.LineNum = lineNum;
        OrderLineInstallation.InstallationDate = installationDate;
        OrderLineInstallation.ItemId = itemId;
       
        if(OrderLineInstallation.validateWrite())
        {
            OrderLineInstallation.insert();
        } 
        else
        {
            Global::info("OrderLineInstallation insersion failed.");
        }
    }

}
```

The inspiration is from :<br/>

<img width="913" alt="image" src="https://github.com/user-attachments/assets/2ac27f00-178c-45e7-b8df-d13a9e1ae9c6" />

<br/>

## Recall Order Customer Scenario
# Scenario
1. Click "Recall Order" -> "Order to Ship": <br/>
<img width="390" alt="image" src="https://github.com/user-attachments/assets/fa64d3d7-af46-437d-8014-70dd9b59640b" /><br/>
2.  Highlight the order to be shipped, and click "Edit": <br/>
<img width="917" alt="image" src="https://github.com/user-attachments/assets/c0494ac5-e365-481c-bedc-e846ded0495f" /><br/>
3. Go to "Cart" Screen, you wil the "Installation Date" from "Extension Properties" is still there<br/>
   <img width="830" alt="image" src="https://github.com/user-attachments/assets/57114d22-03c3-4b84-806e-7284384ec612" />
# Implementation Details:
1. When recall order, this RTS API will be called
   ```
   /Classes/RetailTransactionServiceOrders/Methods/getCustomerOrder - (7892, 52)
   ```
   From this method we can find this extension point:<br/>
   <img width="1298" alt="image" src="https://github.com/user-attachments/assets/88077ce2-9b6c-42bd-bed7-5bb39ea397b9" /><br/>

   Hence we implement this extension point:<br/>
   ```cs
    [ExtensionOf(classstr(RetailCustomerOrderExtensions))]
    final class RetailCustomerOrderExtensions_Extension
    {
        [Replaceable]
        public static void getCustomerOrderPreAppendLine(RetailGetCustomerOrderLineParameters lineParameters)
        {
            SalesLine salesLine = lineParameters.salesLine;
            OrderLineInstallation orderLineInstallation = OrderLineInstallation::find(salesLine);
    
            if (orderLineInstallation.RecId)
            {
                XmlDocument _xmlDoc = lineParameters.xmlDoc;
                XmlElement xmlRecord = lineParameters.xmlRecord;
                XmlElement xmlExtensionProperties = _xmlDoc.createElement('ExtensionProperties');
                XmlElement xmlInstallationDateEntry = _xmlDoc.createElement('InstallationDate');
                xmlInstallationDateEntry.innerText(orderLineInstallation.InstallationDate);
                xmlExtensionProperties.appendChild(xmlInstallationDateEntry);
                xmlRecord.appendChild(xmlExtensionProperties);
            }
        }
    
    }
   ```
   This code we read the installation date and then build a XmlNode and put the ExtensionProperties XmlNode and Installation Xml into the Xml Document.<br/>

2. In the CSU  C# code side, we read the "Installation Date" and build a CommerceProperty and put it into the salesline's ExtensionProperties.<br/>
```cs
namespace Contoso.GasStationSample.CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using System.Collections.Concurrent;
    using Microsoft.Extensions.Logging;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages.ProductAvailability;
    using System.Collections.ObjectModel;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages.Inventory;
    using Microsoft.Dynamics.Commerce.Runtime.TransactionService;
    using Microsoft.Dynamics.Commerce.Runtime.TransactionService.Serialization;
    using Microsoft.Dynamics.Retail.Diagnostics;
    using Microsoft.Dynamics.Retail.Diagnostics.Extensions;
    using System.Linq;
    using System.Globalization;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using System.Xml;

    public class RecallCustomerOrderRealtimeRequestHandler : SingleAsyncRequestHandler<RecallCustomerOrderRealtimeRequest>
    {
        protected async override Task<Response> Process(RecallCustomerOrderRealtimeRequest request)
        {
            var transactionServiceClient  = new TransactionServiceClient(request.RequestContext);
            ReadOnlyCollection<object> transactionResponse = null;

            if (request.IsQuote)
            {
                transactionResponse = await transactionServiceClient.GetCustomerQuote(request.Id).ConfigureAwait(false);
            }
            else
            {
                transactionResponse = await transactionServiceClient.GetCustomerOrder(request.Id, includeOnlineOrders: true).ConfigureAwait(false);
            }

            string orderXml = transactionResponse[0].ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(orderXml);

            XmlNodeList itemNodes = xmlDoc.SelectNodes("//CustomerOrder/Items/Item");

            Dictionary<Decimal, string> dict = new Dictionary<Decimal, string>();

            foreach (XmlNode itemNode in itemNodes)
            {
                decimal lineNumber = Convert.ToDecimal(itemNode.Attributes["LineNumber"]?.Value);
                XmlNode installationDateNode = itemNode.SelectSingleNode("ExtensionProperties/InstallationDate");

                if (installationDateNode != null)
                {
                    dict.Add(lineNumber, installationDateNode.InnerText);
                }
            }

            var response = await this.ExecuteNextAsync<RecallCustomerOrderRealtimeResponse>(request).ConfigureAwait(false);

            foreach (var line in response.SalesOrder.SalesLines)
            {
                if (dict.TryGetValue(line.LineNumber, out string value))
                {
                    SalesLine salesline = response.SalesOrder.SalesLines.Where(x => x.LineNumber == line.LineNumber).FirstOrDefault();
                    if(salesline != null)
                    {
                        CommerceProperty commerceProperty = new CommerceProperty("InstallationDate", value);
                        salesline.ExtensionProperties.Add(commerceProperty);
                    }
                }
            }

            return response;
        }
    }
}
```


The doc is here:

https://learn.microsoft.com/en-us/dynamics365/fin-ops-core/dev-itpro/extensibility/extensibility-attributes

The whole project is here:<br/>

https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/Solutions/ImageExtensions




