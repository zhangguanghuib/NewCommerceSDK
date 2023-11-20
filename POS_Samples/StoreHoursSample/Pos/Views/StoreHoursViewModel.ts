/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */
import ko from "knockout";

import { ICustomViewControllerContext, ICustomViewControllerBaseState } from "PosApi/Create/Views";
import { GetDeviceConfigurationClientRequest, GetDeviceConfigurationClientResponse } from "PosApi/Consume/Device";
import { IStoreHoursExtensionViewModelOptions } from "./NavigationContracts";
import KnockoutExtensionViewModelBase from "./BaseClasses/KnockoutExtensionViewModelBase";
import * as ClientStoreHours from "../Entities/IStoreHours";
import StoreHoursDialogModule from "../Controls/Dialogs/StoreHoursDialogModule";
import { IStoreHoursDialogResult } from "../Controls/Dialogs/IStoreHoursDialogResult";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import StoreHourConverter from "../Converter/StoreHourConverter";
import { StoreHours } from "../DataService/DataServiceRequests.g";
import { Entities } from "../DataService/DataServiceEntities.g";


/**
 * The ViewModel for SimpleExtensionView.
 */
export default class StoreHoursViewModel extends KnockoutExtensionViewModelBase {
    public title: ko.Observable<string>;
    public currentStoreHours: ClientStoreHours.IStoreHours[];
    private _context: ICustomViewControllerContext;
    private _customViewControllerBaseState: ICustomViewControllerBaseState;

    constructor(context: ICustomViewControllerContext, state: ICustomViewControllerBaseState,
        options?: IStoreHoursExtensionViewModelOptions) {
        super();

        this._context = context;
        this.title = ko.observable(context.resources.getString("string_0"));
        this._customViewControllerBaseState = state;
        this._customViewControllerBaseState.isProcessing = true;
        this.currentStoreHours = [];
    }

    /**
     * Loads the view model asynchronously.
     */
    public loadAsync(): Promise<void> {

        // Get current store number
        return this._context.runtime.executeAsync(new GetDeviceConfigurationClientRequest())
            .then((response: ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>): ProxyEntities.DeviceConfiguration => {
                return response.data.result;
            })
            // get store hours
            .then((deviceConfiguration: ProxyEntities.DeviceConfiguration)
                : Promise<ClientEntities.ICancelableDataResult<StoreHours.GetStoreDaysByStoreResponse>> => {
                return this._context.runtime.executeAsync(
                    new StoreHours.GetStoreDaysByStoreRequest<StoreHours.GetStoreDaysByStoreResponse>(deviceConfiguration.StoreNumber));
            }).then((response: ClientEntities.ICancelableDataResult<StoreHours.GetStoreDaysByStoreResponse>): void => {
                if (ObjectExtensions.isNullOrUndefined(response)
                    || ObjectExtensions.isNullOrUndefined(response.data)
                    || response.canceled) {
                    return;
                }

                let storeDayHours: ClientStoreHours.IStoreHours[] = [];
                response.data.result.forEach((storeHour: Entities.StoreDayHours): void => {
                    storeDayHours.push(StoreHourConverter.convertToClientStoreHours(storeHour));
                });
                this.currentStoreHours = storeDayHours;
                this._customViewControllerBaseState.isProcessing = false;
            }).catch((reason: any) => {
                this._context.logger.logError("StoreHoursView.StoreHoursDialog: " + JSON.stringify(reason));
                this._customViewControllerBaseState.isProcessing = false;
            });
    }

    /**
     * Handler for list item selection.
     * @param {any} item
     */
    public listItemSelected(item: any): Promise<void> {
        this._context.logger.logInformational("Item selected on:" + item.weekDay);

        // Open update store hours dialog
        let dialog: StoreHoursDialogModule = new StoreHoursDialogModule();

        return dialog.open(item)
            .then((result: IStoreHoursDialogResult): Promise<void> => {
                // No action if it is cancel
                if (ObjectExtensions.isNullOrUndefined(result.updatedStoreHours)) {
                    return Promise.resolve();
                }

                this._customViewControllerBaseState.isProcessing = true;

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
                    this.currentStoreHours[item.weekDay - 1].openHour = returnedStoreHours.openHour;
                    this.currentStoreHours[item.weekDay - 1].closeHour = returnedStoreHours.closeHour;
                    this._customViewControllerBaseState.isProcessing = false; 
                    return Promise.resolve();
                }).catch((reason: any) => {
                    this._context.logger.logError("StoreHoursView.StoreHoursDialog.UpdateStoreDayHoursRequest: " + JSON.stringify(reason));
                    this._customViewControllerBaseState.isProcessing = false;
                    return Promise.reject();
                });
            }).catch((reason: any) => {
                this._context.logger.logError("StoreHoursView.StoreHoursDialog: " + JSON.stringify(reason));
                return Promise.reject();
            });
    }
}
