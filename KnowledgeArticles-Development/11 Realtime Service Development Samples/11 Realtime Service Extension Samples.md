## Table of Contents
- [Topic](#topic)
- [Background](#Background)
- [RetailServer read the data from F&O Database table and render the date on POS view ](#RetailServer read the data from F&O Database table and render the data on POS view )

## Topic
Deep Dive D365 Commerce Realtime Service:  from single value communication to array communication.

## Background 
Real-time Service enables clients to interact with Commerce functionality in real time. Finance and Operation databases and classes can’t be accessed directly from Retail server. You should access them through the CDX class extension using the finance and operations and Commerce Runtime extension, see [Commerce Data Exchange - Real-time Service](https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/extend-commerce-data-exchange)

But it is unfortunately, the samples provided in the official document is too simple,  it only demonstrates one single string value communicatation scenario, actually in real world,  the scenario is much more complex, Object Array is a very Commmon Scenario.

In this article,  I will demonstration two Realtime Service Scenario:<br/>
Scenario #1：RetailServer read the data from F&O Database table and render the date on POS view.<br/>
Scenario #2: Create a new record in F&O database table,  and show the record in FO form, grid view.<br/>

## RetailServer read the data from F&O Database table and render the data on POS view 
### Let us see the final effect:
In F&O,  there is a form to show for each shipping method,  each day's max shipping slot and free slot:<Br/>
![image](https://github.com/user-attachments/assets/0556ccab-e899-41dd-944e-28e93a5f02b1)
<br/>
The data will be showing on the POS  Shipping View for different Shipping Method, then user can choose a proper data to ship.
![image](https://github.com/user-attachments/assets/3bf7af9a-cd89-474d-99c0-d28713ea471d)
### How to implement it?
. Step 1
   In F&O we should have a table and form for user to input the delivery mode booking slot:<br/>
   table:<br/>
   <img width="275" alt="image" src="https://github.com/user-attachments/assets/ee7579b4-bdaa-4498-a3b4-9d81ec64f7fa" /><br/>
   form(as before) <br/>
   ![image](https://github.com/user-attachments/assets/c89922d1-df75-4118-9d5a-f3b7238cf352)<br/>
. Step 2
  Fetch the booking slot data based on delivery mode passed from retail server to fill an array object, and then serialize it into a string<br/>

  #### Way 1:  JSON format<br/>
  ```CS
    public static container contosoGetDlvModeBookSlotJson(str _searchCriteriaJson)
    {
        int fromLine;
        Contoso.GasStationSample.CommerceRuntime.Entities.DlvModeBookSlot dlvModeBookSlot = new Contoso.GasStationSample.CommerceRuntime.Entities.DlvModeBookSlot();
        System.Collections.ArrayList resultList = new System.Collections.ArrayList();

        try
        {
           fromLine = Global::infologLine();
           DlvModeBookSlotSearchCriteria searchCriteria = RetailTransactionServiceEx::getDlvModeBookSlotSearchCriteriaFromJsonXpp(_searchCriteriaJson);

           if (!searchCriteria)
           {
               return [false, "searchCriteria is null", ''];
           }

            if (searchCriteria.parmDlvModeCode())
            {
                RetailChannelDlvModeBookingSlot retailChannelDlvModeBookingSlot;
                DlvMode  dlvMode;
                while select retailChannelDlvModeBookingSlot
                    where retailChannelDlvModeBookingSlot.DlvModeCode == searchCriteria.parmDlvModeCode()
                    join Txt from dlvMode 
                    where dlvMode.Code == retailChannelDlvModeBookingSlot.DlvModeCode

                {
                    dlvModeBookSlot = new Contoso.GasStationSample.CommerceRuntime.Entities.DlvModeBookSlot();
                    dlvModeBookSlot.DlvModeCode = searchCriteria.parmDlvModeCode();
                    dlvModeBookSlot.DlvModeTxt = dlvMode.Txt;
                    dlvModeBookSlot.ShippingDate =new System.DateTimeOffset(retailChannelDlvModeBookingSlot.ShippingDate);
                    dlvModeBookSlot.MaxSlot = retailChannelDlvModeBookingSlot.MaxSlots;
                    dlvModeBookSlot.FreeSlot = retailChannelDlvModeBookingSlot.FreeSlots;
                    resultList.Add(dlvModeBookSlot);
                }
            }
        }
        catch
        {
            str errorMessage = RetailTransactionServiceUtilities::getInfologMessages(fromLine);
            str axCallStack = con2Str(xSession::xppCallStack());
            return [false, errorMessage, ''];
        }

        // Serialize the data-contract list using the specified type list.
        System.Type[] typeArray = new System.Type[1]();
        typeArray.SetValue(dlvModeBookSlot.GetType(), 0);
        return [true, '', RetailTransactionService::SerializeToJson(resultList, typeArray)];
    }
  ```
  and the helper method to get the search Criteria:
  ```CS
    private static DlvModeBookSlotSearchCriteria getDlvModeBookSlotSearchCriteriaFromJsonXpp(str _searchCriteriaJson)
    {
        System.Exception ex;
        try
        {
            DlvModeBookSlotSearchCriteria searchCriteria = FormJsonSerializer::deserializeObject(classNum(DlvModeBookSlotSearchCriteria), _searchCriteriaJson);
            return searchCriteria;
        }
        catch(ex)
        {
            return null;
        }
    }
  ```
 In order to use FormJsonSerializer::deserializeObject, we need define <br/>
 ```CS
   [DataContract]
   internal final class DlvModeBookSlotSearchCriteria
   {
       DlvModeId dlvModeCode;
       int serializationFormat;
   
       [DataMember('DlvModeCode')]
       public DlvModeId parmDlvModeCode(DlvModeId _dlvModeCode = this.dlvModeCode)
       {
           this.dlvModeCode = _dlvModeCode;
           return this.dlvModeCode;
       }
   
       [DataMember('SerializationFormat')]
       public int parmSerializationFormat(int _serializationFormat = this.serializationFormat)
       {
           this.serializationFormat = _serializationFormat;
           return this.serializationFormat;
       }
   
   }
 ```
 #### Way 2:  XML format<br/>
```CS
 public static container contosoGetDlvModeBookSlotXml(str _searchCriteriaJson)
 {
     int fromLine;
     Contoso.GasStationSample.CommerceRuntime.Entities.DlvModeBookSlot dlvModeBookSlot = new Contoso.GasStationSample.CommerceRuntime.Entities.DlvModeBookSlot();
     System.Collections.ArrayList resultList = new System.Collections.ArrayList();

     try
     {
         fromLine = Global::infologLine();

         ContosoCRT.TransactionService.DlvModeBookSlotSearchCriteria searchCriteria =
             RetailTransactionServiceEx::getDlvModeBookSlotSearchCriteriaFromXml(_searchCriteriaJson);
         if (!searchCriteria)
         {
             return [false, "searchCriteria is null", ''];
         }
        
         if (searchCriteria.DlvModeCode)
         {
             RetailChannelDlvModeBookingSlot retailChannelDlvModeBookingSlot;
             DlvMode  dlvMode;
             while select retailChannelDlvModeBookingSlot
                 where retailChannelDlvModeBookingSlot.DlvModeCode == searchCriteria.DlvModeCode
                 join Txt from dlvMode
                 where dlvMode.Code == retailChannelDlvModeBookingSlot.DlvModeCode
             {
                 dlvModeBookSlot = new Contoso.GasStationSample.CommerceRuntime.Entities.DlvModeBookSlot();
                 dlvModeBookSlot.DlvModeCode = searchCriteria.DlvModeCode;
                 dlvModeBookSlot.DlvModeTxt = dlvMode.Txt;
                 dlvModeBookSlot.ShippingDate =new System.DateTimeOffset(retailChannelDlvModeBookingSlot.ShippingDate);
                 dlvModeBookSlot.MaxSlot = retailChannelDlvModeBookingSlot.MaxSlots;
                 dlvModeBookSlot.FreeSlot = retailChannelDlvModeBookingSlot.FreeSlots;
                 resultList.Add(dlvModeBookSlot);
             }
         }
     }
     catch
     {
         str errorMessage = RetailTransactionServiceUtilities::getInfologMessages(fromLine);
         str axCallStack = con2Str(xSession::xppCallStack());
         return [false, errorMessage, ''];
     }

     // Serialize the data-contract list using the specified type list.
     System.Type[] typeArray = new System.Type[1]();
     typeArray.SetValue(dlvModeBookSlot.GetType(), 0);
     return [true, '', RetailTransactionService::SerializeToJson(resultList, typeArray)];
 }
```
and the helper method to get the search creteria is:<br/>
```CS
 private static ContosoCRT.TransactionService.DlvModeBookSlotSearchCriteria getDlvModeBookSlotSearchCriteriaFromXml(str _xmlArgumentString)
 {
     XmlDocument         argsXml;
     XmlElement          argsRoot;

     // Get the string value of an XML element argument.
     str getArg(str argName)
     {
         XmlElement xmlRoot = argsRoot.getNamedElement(argName);
         if (xmlRoot != null)
         {
             return xmlRoot.text();
         }
         return '';
     }

     System.Exception ex;
     try
     {
         argsXml   = new XmlDocument();
         argsXml.loadXml(_xmlArgumentString);
         argsRoot = argsXml.documentElement();

         str argDlvModeCode = getArg('DlvModeCode');
         ContosoCRT.TransactionService.DlvModeBookSlotSearchCriteria searchCriteria = new ContosoCRT.TransactionService.DlvModeBookSlotSearchCriteria(argDlvModeCode);
         return searchCriteria;
     }
     catch(ex)
     {
         return null;
     }
 }
```
. Step 3,  In retail Server, we can call the Realtime-Service Extension to get the data in F&O and show on POS view:<br/>
  Firstly, we build a Search Creteria Object:
  ```CS
  ReadOnlyCollection<object> results;

  DlvModeBookSlotSearchCriteria searchCriteria = new DlvModeBookSlotSearchCriteria();
  searchCriteria.DlvModeCode = request.DlvModeCode;
  searchCriteria.PagingInfo = request.QueryResultSettings.Paging;
  ```
  And then call the the realtime service extesion API to get the data,  here the returned value is a string, we need deserialize it into an array:
  ```CS
    InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(
     "contosoGetDlvModeBookSlotJson",
     SerializationHelper.SerializeObjectToJson(searchCriteria)
   );
   
   InvokeExtensionMethodRealtimeResponse response = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);
   
   results = response.Result;
   
   string resXmlValue = (string)results[0];
   
   IEnumerable<DlvModeBookSlot> dlvModeBookSlots;
   if (useJson)
   {
     dlvModeBookSlots = SerializationHelper.DeserializeObjectDataContractFromJson<DlvModeBookSlot[]>(resXmlValue);                 
   }
   else
   {
     dlvModeBookSlots =
         SerializationHelper.DeserializeObjectDataContractFromXml<DlvModeBookSlot[]>(resXmlValue);
   }
   return new GetDlvModeBookSlotsResponse(dlvModeBookSlots.AsPagedResult());
  ```
  For xml Search Creteria, we can use this code:
  ```CS
   InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(
     "contosoGetDlvModeBookSlotXml",
     SerializationHelper.SerializeObjectToXml(searchCriteria)
   );
  ```

. Step 4: showing the data from Realtime Service in a html table, and insert the table in the proper place of the POS view:
```TS
import { ClientEntities } from "PosApi/Entities";
import { GetShippingDateClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";
import { GetShippingDateClientRequest, GetShippingDateClientResponse } from "PosApi/Consume/Cart";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { Entities } from "../../DataService/DataServiceEntities.g";
import { DlvModeBookSlot } from "../../DataService/DataServiceRequests.g";

export default class GetShippingDateClientRequestHandlerExt extends GetShippingDateClientRequestHandler {

    private shippingMethodBookingSlotId: string = 'shippingMethodBookingSlot___';
    private bookSLots: Entities.DlvModeBookSlot[];
    /**
     * Executes the request handler asynchronously.
     * @param {GetShippingDateClientRequest<GetShippingDateClientResponse>} The request containing the response.
     * @return {Promise<ICancelableDataResult<GetShippingDateClientResponse>>} The cancelable promise containing the response.
     */
    public executeAsync(request: GetShippingDateClientRequest<GetShippingDateClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<GetShippingDateClientResponse>> {

        let shippingMethodDiv: HTMLDivElement = document.querySelector('div.section[aria-label="Shipping method"][data-bind*="sectionWrapper: { headerResx: \'string_2504\' }"]');
        console.log(shippingMethodDiv);
        let shippingMethodsSection: HTMLDivElement = shippingMethodDiv.querySelector(`div:nth-child(2)`) as HTMLDivElement;
        console.log(shippingMethodsSection);

        return this.context.runtime.executeAsync(new DlvModeBookSlot.GetDlvModeBookSlotsRequest<DlvModeBookSlot.GetDlvModeBookSlotsResponse>(request.deliveryMethod.Code))
            .then((result: ClientEntities.ICancelableDataResult<DlvModeBookSlot.GetDlvModeBookSlotsResponse>): Promise<void> => {
                this.bookSLots = result.data.result;
                console.log(this.bookSLots);
                // Build your table here
                return Promise.resolve();
            }).catch((reason: any) => {
                this.context.logger.logError("ShippingMethods: " + JSON.stringify(reason));
            }).then(() => {
                let testTable = this.generateTable(this.bookSLots);
                let shippingMethodsList = shippingMethodsSection.querySelector(`div:nth-child(2)`) as HTMLDivElement;;
                shippingMethodsList.appendChild(testTable);
                return this.defaultExecuteAsync(request);
            });
    }

    public generateTable(data: Entities.DlvModeBookSlot[]): HTMLDivElement {
        let shippingMethodBookingSlot: HTMLDivElement = document.getElementById(this.shippingMethodBookingSlotId) as HTMLDivElement;
        if (!ObjectExtensions.isNullOrUndefined(shippingMethodBookingSlot)) {
            shippingMethodBookingSlot.remove();
        }
        shippingMethodBookingSlot = document.createElement('div') as HTMLDivElement;
        shippingMethodBookingSlot.id = this.shippingMethodBookingSlotId;

        const table: HTMLTableElement = document.createElement('table') as HTMLTableElement;

        const thead = document.createElement('thead');
        const headerRow = document.createElement('tr');
        headerRow.style.background = "blue";

        const headers = ['DlvModeCode', 'DlvModeTxt', 'ShippingDate', 'MaxSlot', 'FreeSlot'];
        headers.forEach(headerText => {
            const th = document.createElement('th');
            th.textContent = headerText;
            headerRow.appendChild(th);
        });

        thead.appendChild(headerRow);
        table.appendChild(thead);

        const tbody = document.createElement('tbody');
        data.forEach((slot, index) => {
            const row = document.createElement('tr');

            const cells = [
                slot.DlvModeCode,
                slot.DlvModeTxt,
                slot.ShippingDate.toDateString(),
                slot.MaxSlot.toString(),
                slot.FreeSlot.toString()
            ];

            cells.forEach(cellText => {
                const td = document.createElement('td');
                td.textContent = cellText;
                row.appendChild(td);
            });

            // Apply background color for odd and even rows
            if (index % 2 === 0) {
                row.style.backgroundColor = 'green';
            } else {
                row.style.backgroundColor = '#501669';
            }

            tbody.appendChild(row);
        });

        table.appendChild(tbody);

        // Style the table
        table.style.width = '100%';
        table.style.borderCollapse = 'collapse';
        table.style.fontFamily = 'Arial, sans-serif';
        table.style.fontSize = '14px';
        table.style.color = 'white';
        table.style.border = '2px solid #dddddd';
        table.style.textAlign = 'left';
        table.style.marginTop = '5px';

        // Style the table header
        var headersCells = table.getElementsByTagName('th');
        for (let i = 0; i < headersCells.length; i++) {
            headersCells[i].style.border = '2px solid yellow';
            headersCells[i].style.padding = '8px';
        }

        // Style the table cells
        var cells = table.getElementsByTagName('td');
        for (let k = 0; k < cells.length; k++) {
            cells[k].style.border = '1px solid #dddddd';
            cells[k].style.padding = '8px';
        }

        shippingMethodBookingSlot.appendChild(table);

        return shippingMethodBookingSlot;
    }
}
```



