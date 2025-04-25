# Block customer pay cash if they have department dimension
## 1. Finally how it works finally:<br/>
![image](https://github.com/user-attachments/assets/71829e4f-1579-40c4-b137-0a861d920f31)
## 2. Configuration:<br/>
![image](https://github.com/user-attachments/assets/43b6f5b1-82fa-472b-ac6c-e8268030d3d5)

## 3. RTS code:<br/>
```cs
[ExtensionOf(classStr(RetailTransactionServiceEx))]
public final  class AvanadeRetailTransactionService_Extension
{
    public static container avanadeGetCustomerFinancialDimension(DataAreaId _dataAreaId, CustAccount _accountNum, Name _dimAttributeName)
    {
        int fromLine;
        CustTable custTable;
        DimensionAttributeValueSetStorage dimStorage;
        DimensionAttribute dimAttribute;
        DimensionValue dimValue = '';

        try
        {
            fromLine = Global::infologLine();

            if (!_dataAreaId || !_accountNum)
            {
                return [false, "searchCriteria is null", ''];
            }

            custTable = CustTable::findByCompany(_dataAreaId, _accountNum);
            if (custTable.RecId && custTable.DefaultDimension)
            {
                dimStorage = DimensionAttributeValueSetStorage::find(custTable.DefaultDimension);
                if (dimStorage)
                {
                    select firstOnly dimAttribute where dimAttribute.Name == _dimAttributeName;
                    if (dimAttribute.RecId)
                    {
                        dimValue = dimStorage.getDisplayValueByDimensionAttribute(dimAttribute.RecId);
                    }
                }
            }
        }
        catch
        {
            str errorMessage = RetailTransactionServiceUtilities::getInfologMessages(fromLine);
            str axCallStack = con2Str(xSession::xppCallStack());
            return [false, errorMessage, ''];
        }
        return [true, '', dimValue];
    }

}
```
## 4. CRT code:<br/>
```cs
namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;

    public class CustomerDimensionService : IRequestHandlerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(GetCustomerDimensionRequest)
                };
            }
        }

        public Task<Response> Execute(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            Type reqType = request.GetType();
            if (reqType == typeof(GetCustomerDimensionRequest))
            {
                return this.GetCustomerDimension((GetCustomerDimensionRequest)request);
            }
            else
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported.", reqType);
                throw new NotSupportedException(message);
            }
        }

        private async Task<Response> GetCustomerDimension(GetCustomerDimensionRequest request)
        {
            InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(
                 "avanadeGetCustomerFinancialDimension",
                 request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId,
                 request.AccountNum,
                 request.DimensionAttributeName
                );

            InvokeExtensionMethodRealtimeResponse response = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);
            var results = response.Result;
            string dimensionValue = (string)results[0];
            return new GetCustomerDimensionResponse(dimensionValue);
        }
    }
}
```

The controller code:
```cs
public class CustomerDimensionController : IController
{
    [HttpPost]
    [Authorization(CommerceRoles.Employee, CommerceRoles.Application)]
    public async Task<String> GetCustomerDimensionByDimensionAttribute(IEndpointContext context, string accountNum, string dimensionAttribute)
    {
        var getCustomerDimensionRequest = new GetCustomerDimensionRequest(accountNum, dimensionAttribute);
        var response = await context.ExecuteAsync<GetCustomerDimensionResponse>(getCustomerDimensionRequest).ConfigureAwait(false);
        return response.DimensionAttributeValue;
    }
}
```
## 5. Store Commerce Extension code:<br/>
```TS
import { AddTenderLineToCartClientResponse, GetCurrentCartClientRequest, GetCurrentCartClientResponse } from "PosApi/Consume/Cart";
import { GetOrgUnitTenderTypesClientRequest, GetOrgUnitTenderTypesClientResponse } from "PosApi/Consume/OrgUnits";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { AddTenderLineToCartClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import MessageDialog from "../Controls/Dialogs/Create/MessageDialog"
import * as Messages from "../DataService/DataServiceRequests.g";

declare var Commerce: any;
export default class AddTenderLineToCartClientRequestHandlerExt extends AddTenderLineToCartClientRequestHandler {
    public executeAsync(request: Commerce.AddTenderLineToCartClientRequest<Commerce.AddTenderLineToCartClientResponse>)
        : Promise<Commerce.Client.Entities.ICancelableDataResult<Commerce.AddTenderLineToCartClientResponse>> {

        return Promise.all([this.isCashTenderType(request.tenderLine.TenderTypeId), this.getCustomerDimensionValue("Department")]).then((result: any[]) => {
            let isCashTenderType: boolean = result[0];
            let dimensionValue: string = result[1];
            if (isCashTenderType && dimensionValue === '022') {
                const msg: string = `Cash tender type is not allowed from this cusomer with Department Dimension Value: ${dimensionValue}`;
                return MessageDialog.show(this.context, msg).then(() => {
                    return Promise.resolve<ClientEntities.ICancelableDataResult<AddTenderLineToCartClientResponse>>(
                        {
                            canceled: true,
                            data: null,
                        }
                    )
                });
            } else {
                return this.defaultExecuteAsync(request);
            }
        });
    }

    private isCashTenderType(tenderTypeId: string): Promise<boolean> {
        let getOrgUnitTenderTypesClientRequest: GetOrgUnitTenderTypesClientRequest<GetOrgUnitTenderTypesClientResponse>
            = new GetOrgUnitTenderTypesClientRequest();
        return this.context.runtime.executeAsync(getOrgUnitTenderTypesClientRequest)
            .then((response: ClientEntities.ICancelableDataResult<GetOrgUnitTenderTypesClientResponse>): Promise<boolean> => {
                let allTenderTypes: ProxyEntities.TenderType[] = response.data.result;
                let cashTenderTypes: ProxyEntities.TenderType[] = allTenderTypes.filter((tenderType: ProxyEntities.TenderType) => {
                    return tenderType.OperationId === Commerce.Operations.RetailOperation.PayCash;
                });

                let isCashTenderType: boolean = false;
                for (let i: number = 0; i < cashTenderTypes.length; i++) {
                    if (cashTenderTypes[i].TenderTypeId == tenderTypeId) {
                        isCashTenderType = true;
                        break;
                    }
                }
                if (isCashTenderType) { return Promise.resolve(true) }
                else { return Promise.resolve(false) }
            }
        );
    }

    private getCustomerDimensionValue(dimensionAttribute: string): Promise<string> {
        let getCurrentCart: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest<GetCurrentCartClientResponse>();
        return this.context.runtime.executeAsync(getCurrentCart).then(
            (response: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>): Promise<ClientEntities.ICancelableDataResult<string>> => {
                if (!response.canceled) {
                    let cart: ProxyEntities.Cart = response.data.result;
                    if (!ObjectExtensions.isNullOrUndefined(cart)) {
                        let customerAccount: string = cart.CustomerId;
                        if (!StringExtensions.isNullOrWhitespace(customerAccount)) {
                            return Promise.resolve(
                                {
                                    canceled: false,
                                    data: customerAccount,
                                });
                        }
                    }
                }
                return Promise.resolve(
                    {
                        canceled: false,
                        data: "",
                    });
            }

        ).then((response: ClientEntities.ICancelableDataResult<string>): Promise<string> => {
            if (StringExtensions.isEmptyOrWhitespace(response.data)) {
                return Promise.resolve("");
            }
            let accountNum: string = response.data;
            let request: Messages.StoreOperations.GetCustomerDimensionByDimensionAttributeRequest<Messages.StoreOperations.GetCustomerDimensionByDimensionAttributeResponse>
                = new Messages.StoreOperations.GetCustomerDimensionByDimensionAttributeRequest<Messages.StoreOperations.GetCustomerDimensionByDimensionAttributeResponse>(accountNum, dimensionAttribute);
            return this.context.runtime.executeAsync(request).then((response: ClientEntities.ICancelableDataResult<Messages.StoreOperations.GetCustomerDimensionByDimensionAttributeResponse>): Promise<string> => {
                if (!response.canceled) {
                    let dimensionValue: string = response.data.result;
                    if (!StringExtensions.isNullOrWhitespace(dimensionValue)) {
                        return Promise.resolve(dimensionValue);
                    }
                }
                return Promise.resolve("");
            })
        });
    }
}
```
## 6. The project is located: <br/>

https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/POSExtensions/BarcodeUnitOnInventoryLineV1
