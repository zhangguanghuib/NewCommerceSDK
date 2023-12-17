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

import { UpdateDeleteAction } from "../Entities/IStoreHours";



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

    public createNewItem(): Promise<void> {

        console.log("Creating a new store hours");

        let dialog: StoreHoursDialogModule = new StoreHoursDialogModule();
        let emptyStoreHours: ClientStoreHours.IStoreHours = {
            id: 200,
            weekDay: ClientStoreHours.WeekDays.Sunday,
            openHour: ClientStoreHours.Hours.eight,
            closeHour: ClientStoreHours.Hours.fourteen,
            channelId: "0"
        };

        return dialog.open(emptyStoreHours)
            .then((result: IStoreHoursDialogResult): Promise<void> => {

                if (ObjectExtensions.isNullOrUndefined(result.updatedStoreHours)) {
                    return Promise.resolve();
                }

                this._customViewControllerBaseState.isProcessing = true;

                let rsStoreDayHours: Entities.StoreDayHours = StoreHourConverter.convertToServerStoreHours(result.updatedStoreHours);
                let storeNumber: string = "";

                return this._context.runtime.executeAsync(new GetDeviceConfigurationClientRequest())
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

    /**
     * Handler for list item selection.
     * @param {any} item
     */
    //public listItemSelected(item: any): Promise<void> {
    //    this._context.logger.logInformational("Item selected on:" + item.weekDay);

    //    // Open update store hours dialog
    //    let dialog: StoreHoursDialogModule = new StoreHoursDialogModule();

    //    return dialog.open(item)
    //        .then((result: IStoreHoursDialogResult): Promise<void> => {
    //            // No action if it is cancel
    //            if (ObjectExtensions.isNullOrUndefined(result.updatedStoreHours)) {
    //                return Promise.resolve();
    //            }

    //            this._customViewControllerBaseState.isProcessing = true;

    //            let rsStoreDayHours: Entities.StoreDayHours = StoreHourConverter.convertToServerStoreHours(result.updatedStoreHours);
    //            return this._context.runtime.executeAsync(
    //                new StoreHours.UpdateStoreDayHoursRequest<StoreHours.UpdateStoreDayHoursResponse>(rsStoreDayHours.Id, rsStoreDayHours)
    //            ).then((response: ClientEntities.ICancelableDataResult<StoreHours.UpdateStoreDayHoursResponse>): Promise<void> => {
    //                if (ObjectExtensions.isNullOrUndefined(response)
    //                    || ObjectExtensions.isNullOrUndefined(response.data)
    //                    || response.canceled) {
    //                    return Promise.resolve();
    //                }

    //                this._context.logger.logInformational("Updated hours is: " + result.updatedStoreHours.openHour.toString());
    //                let returnedStoreHours: ClientStoreHours.IStoreHours = StoreHourConverter.convertToClientStoreHours(response.data.result);
    //                this.currentStoreHours[item.id - 1].openHour = returnedStoreHours.openHour;
    //                this.currentStoreHours[item.id - 1].closeHour = returnedStoreHours.closeHour;
    //                this._customViewControllerBaseState.isProcessing = false; 
    //                return Promise.resolve();
    //            }).catch((reason: any) => {
    //                this._context.logger.logError("StoreHoursView.StoreHoursDialog.UpdateStoreDayHoursRequest: " + JSON.stringify(reason));
    //                this._customViewControllerBaseState.isProcessing = false;
    //                return Promise.reject();
    //            });
    //        }).catch((reason: any) => {
    //            this._context.logger.logError("StoreHoursView.StoreHoursDialog: " + JSON.stringify(reason));
    //            return Promise.reject();
    //        });
    //}

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

                return this.updatetItemSelected(result).then(() => {
                    return this.deleteItemSelected(result);
                }).then(() => {
                    this._customViewControllerBaseState.isProcessing = false;                })
                .catch ((reason: any) => {
                    this._context.logger.logError("StoreHoursView.StoreHoursDialog.UpdateStoreDayHoursRequest: " + JSON.stringify(reason));
                    this._customViewControllerBaseState.isProcessing = false;
                    return Promise.reject();
                });
            }).catch((reason: any) => {
                this._context.logger.logError("StoreHoursView.StoreHoursDialog: " + JSON.stringify(reason));
                return Promise.reject();
            });
    }

    public deleteItemSelected(result: IStoreHoursDialogResult): Promise<void> {
        if (result.updatedStoreHours.action === UpdateDeleteAction.Delete && result.updatedStoreHours.id) {
            return this._context.runtime.executeAsync(
                new StoreHours.DeleteStoreDayHoursRequest<StoreHours.DeleteStoreDayHoursResponse>(result.updatedStoreHours.id)
            ).then((response: ClientEntities.ICancelableDataResult<StoreHours.DeleteStoreDayHoursResponse>): Promise<void> => {
                if (ObjectExtensions.isNullOrUndefined(response)
                    || ObjectExtensions.isNullOrUndefined(response.data)
                    || response.canceled) {
                    return Promise.resolve();
                }

                this.currentStoreHours = this.currentStoreHours.filter((item: ClientStoreHours.IStoreHours) => { item.id !== result.updatedStoreHours.id });

                this._customViewControllerBaseState.isProcessing = false;
                return Promise.resolve();
            })
        }
        return Promise.resolve();
    }

    public updatetItemSelected(result: IStoreHoursDialogResult): Promise<void> {
        if (result.updatedStoreHours.action === UpdateDeleteAction.Update && result.updatedStoreHours.id) {
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
                this.currentStoreHours[result.updatedStoreHours.id - 1].openHour = returnedStoreHours.openHour;
                this.currentStoreHours[result.updatedStoreHours.id - 1].closeHour = returnedStoreHours.closeHour;
                this._customViewControllerBaseState.isProcessing = false;
                return Promise.resolve();
            });
        } else {
            return Promise.resolve();
        }
    }

}
