# Enhanced Store Hours Sample to support CRUD
## Overview
This sample is a showcase an improved Store Hours Sample which exists in Retail SDK  for a long time, this sample improved the existing sample so that the new sample fixed some bugs to make the POS view can refresh automatically, and this page also support the traditional CRUD functions, that means this sample can support 
- Create a store hour entry
- Update an existing store hours sample
- Delete an exisiting store hours sample
- Reload the page to get the latest data
- Refresh UI to reflect the change
## Configuring the sample
- Create a POS  Operation in HQ <br/>
  <img width="880" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/b377aabb-0eb3-4707-a4d8-62523302401b">

- Add the POS Operation on the POS button grid designer:HQ <br/>
  <img width="300" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/bec31cf6-4dcc-4227-9ffb-cfb7ec292db3">

- Run job 9999
## Deploy the sample locally
- Firstly make sure your project contains Scale Unit Installer and Store Commerce Installer <br/>
  <img width="310" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/00f0229f-1697-45da-9d58-130c6a01a528">
- Nomrally when you build it, you will see the store commerce extension and Scale unit extension deploy automatically
- Alternatively, you can make run this command to deploy Scale Unit package:
  <img width="845" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/6d3fddd5-952c-4478-9773-6580d5ff01e2"><br/>
- And run this command to deploy Store Commerce Package:
  <img width="1090" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/798aa40f-745d-400e-aa96-8e5a053574e3">

- Finally you can go to control panel to verify the package deployed successfully:
  <img width="857" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ec0af66a-86ed-4c25-a61b-def22312181c">
  <img width="841" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/0a8f5d9a-a6a3-4263-8d5a-be39cbafd994">
## Running the sample
- Go to Cart Screen, find the button "Store Hours", click it HQ <br/>
  <img width="481" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/3527cfc1-d8c9-4aad-80a5-84826f2c4b25">

- Create:<br/>
  Find the command bars and then click '+' button: <br/>
  <img width="116" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/578b2e08-33c7-411b-a8f7-0bdb4ced7bc1">
  On the opened dialog, input the necessary information and the click "Update" button:HQ <br/>
  <img width="827" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/654b6b48-44c9-417f-922f-adacf1210679"><br/>
  you will see a new record created in the view:<br/>
  <img width="1181" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/85acc934-5166-44fb-b70e-9ed642807c99">

- Update:<br/>
  Click one existing record,  one dialog is opened automatically, make the changes you want, and then click "Update" button<br/>
  <img width="1473" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/0176ca03-4607-451a-8528-dfafa6042862">
  You will see the changes reflected on the existing grid
  <img width="1244" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8da3dd82-ded2-4de7-932b-4ddaa03a25fe">

- Delete:<br/>
   Click one existing record,  one dialog is opened automatically, make sure the delete is "Delete"<br/>
  <img width="1462" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4ad682eb-522e-483e-a9c3-91f963d87c4c">
   Click "Update" button, you will see the record got deleted from UI and database
   <img width="1251" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ef80c08e-a260-408a-9c06-08df668c0908">

- Retrieve:<Br/>
  Click "Reload" button on then command bar, and the it will refresh the view with the latest data from database:<br/>
  <img width="137" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/23453879-74d2-4b32-b863-1ba333ad6232"><br/>
  <img width="1364" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/0e0467f1-437d-4a7e-b022-f26864a73d48">
## Implemenation details
# SQL
- Create table [ext].[CONTOSORETAILSTOREHOURSTABLE]
- Create View [ext].[CONTOSORETAILSTOREHOURSVIEW]
- Store Procedure: [ext].[UPDATESTOREDAYHOURS]
- Store Procedure: [ext].DELETESTOREDAYHOURS
- Store Procedure: [ext].INSERTSTOREDAYHOURS
# Retail Server
- API: GetStoreDaysByStore
- API: UpdateStoreDayHours
- API: InsertStoreDayHours
- API: DeleteStoreDayHours
# Commerce Runtime:
Commerce Runtime support these request:
- typeof(GetStoreHoursDataRequest),
- typeof(UpdateStoreDayHoursDataRequest),
- typeof(InsertStoreDayHoursDataRequest),
- typeof(DeleteStoreDayHoursDataRequest)

# POS:
- Load the data from database
```ts:
response.data.result.forEach((storeHour: Entities.StoreDayHours): void => {
    storeDayHours.push(StoreHourConverter.convertToClientStoreHours(storeHour));
});
this.currentStoreHours = storeDayHours;
```
- Update existing store hours data:
```ts
let rsStoreDayHours: Entities.StoreDayHours = StoreHourConverter.convertToServerStoreHours(result.updatedStoreHours);
return this._context.runtime.executeAsync(
    new StoreHours.UpdateStoreDayHoursRequest<StoreHours.UpdateStoreDayHoursResponse>(rsStoreDayHours.Id, rsStoreDayHours)
).then((response: ClientEntities.ICancelableDataResult<StoreHours.UpdateStoreDayHoursResponse>): Promise<void> => {
    if (ObjectExtensions.isNullOrUndefined(response)
        || ObjectExtensions.isNullOrUndefined(response.data)
        || response.canceled) {
        return Promise.resolve();
    }

    this._context.logger.logInformational("Updated hours is: " + result.updatedStoreHours.openHour.toString());
    let returnedStoreHours: ClientStoreHours.IStoreHours = StoreHourConverter.convertToClientStoreHours(response.data.result);

    let filteredArr: ClientStoreHours.IStoreHours[] = this.currentStoreHours.filter(
        (item: ClientStoreHours.IStoreHours) => item.id === result.updatedStoreHours.id);

    if (ArrayExtensions.hasElements(filteredArr)) {
        filteredArr[0].openHour = returnedStoreHours.openHour;
        filteredArr[0].closeHour = returnedStoreHours.closeHour;
    }

    this._customViewControllerBaseState.isProcessing = false;
```

- Delete existing store hours data:
```ts
  return this._context.runtime.executeAsync(
      new StoreHours.DeleteStoreDayHoursRequest<StoreHours.DeleteStoreDayHoursResponse>(result.updatedStoreHours.id)
  ).then((response: ClientEntities.ICancelableDataResult<StoreHours.DeleteStoreDayHoursResponse>): Promise<void> => {
      if (ObjectExtensions.isNullOrUndefined(response)
          || ObjectExtensions.isNullOrUndefined(response.data)
          || response.canceled) {
          return Promise.resolve();
      }

  let filteredArr: ClientStoreHours.IStoreHours[] = this.currentStoreHours.filter(
      (item: ClientStoreHours.IStoreHours) => item.id === result.updatedStoreHours.id);

      if (ArrayExtensions.hasElements(filteredArr)) {
          let index: number = this.currentStoreHours.indexOf(filteredArr[0]);
          this.currentStoreHours.splice(index, 1);
      }

      this._customViewControllerBaseState.isProcessing = false;
```

- Create a new item:
  ```ts
  this._context.runtime.executeAsync(new GetDeviceConfigurationClientRequest())
                    .then((response: ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>): ProxyEntities.DeviceConfiguration => {
                        return response.data.result;
                    })
                    // get store hours
                    .then((deviceConfiguration: ProxyEntities.DeviceConfiguration) => {
                        storeNumber = deviceConfiguration.StoreNumber;
                        rsStoreDayHours.ChannelId = deviceConfiguration.ChannelId+"";
                        return this._context.runtime.executeAsync(
                            new StoreHours.InsertStoreDayHoursRequest<StoreHours.InsertStoreDayHoursResponse>(rsStoreDayHours.Id, rsStoreDayHours)
                        );
                    }).then((response: ClientEntities.ICancelableDataResult<StoreHours.InsertStoreDayHoursResponse>):
                        Promise<ClientEntities.ICancelableDataResult<StoreHours.GetStoreDaysByStoreResponse>> => {
                        if (ObjectExtensions.isNullOrUndefined(response)
                            || ObjectExtensions.isNullOrUndefined(response.data)
                            || response.canceled) {

                            return Promise.resolve({
                                canceled: true,
                                data: null
                            });
                        }

                        return this._context.runtime.executeAsync(
                            new StoreHours.GetStoreDaysByStoreRequest<StoreHours.GetStoreDaysByStoreResponse>(storeNumber));
                    }).then((response: ClientEntities.ICancelableDataResult<StoreHours.GetStoreDaysByStoreResponse>): Promise<void> => {
                        if (ObjectExtensions.isNullOrUndefined(response)
                            || ObjectExtensions.isNullOrUndefined(response.data)
                            || response.canceled) {
                            return Promise.resolve();
                        }

                        let storeDayHours: ClientStoreHours.IStoreHours[] = [];
                        response.data.result.forEach((storeHour: Entities.StoreDayHours): void => {
                            storeDayHours.push(StoreHourConverter.convertToClientStoreHours(storeHour));
                        });
                        this.currentStoreHours = storeDayHours;
                        this._customViewControllerBaseState.isProcessing = false;
  ```





