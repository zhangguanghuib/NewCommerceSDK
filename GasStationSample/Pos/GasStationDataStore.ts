import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { ArrayExtensions, ObjectExtensions } from "PosApi/TypeExtensions";
import { ProxyEntities, ClientEntities } from "PosApi/Entities";
import { GetDeviceConfigurationClientRequest, GetDeviceConfigurationClientResponse } from "PosApi/Consume/Device";
import { GasPumps } from "./DataService/DataServiceRequests.g";
import { Entities } from "DataService/DataServiceEntities.g";

import GasPump = Entities.GasPump;
import GasPumpState = Entities.GasPumpState;

export type PumpStatusChangedHandler = (updatedPumps: GasPump[]) => void;
type StatusChangedHandlerWithId = { readonly handler: PumpStatusChangedHandler, readonly id: number };


export class GasStationDataStore {

    private static _instance: GasStationDataStore;

    private _pumps: GasPump[];

    private _gasStationDetails: Entities.GasStationDetails;
    private _storeNumber: string;
    private readonly _pumpStatusChangedHandlers: StatusChangedHandlerWithId[];

    private constructor() {
        this._pumps = [];
        this._pumpStatusChangedHandlers = [];
    }

    public static get instance(): GasStationDataStore {
        if (ObjectExtensions.isNullOrUndefined(GasStationDataStore._instance)) {
            GasStationDataStore._instance = new GasStationDataStore();
        }

        return GasStationDataStore._instance;
    }

    public get pumps(): GasPump[] {
        return ObjectExtensions.clone(this._pumps);
    }

    public get gasStationDetails(): Readonly<Entities.GasStationDetails> {
        return this._gasStationDetails;
    }

    public get storeNumber(): string {
        return this._storeNumber;
    }


    public initAsync(context: IExtensionContext): Promise<void> {

        return context.runtime.executeAsync(new GetDeviceConfigurationClientRequest())
            .then((response: ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>): ProxyEntities.DeviceConfiguration => {
                return response.data.result;
            })
            .then((deviceConfiguration: ProxyEntities.DeviceConfiguration) => {
                if (ObjectExtensions.isNullOrUndefined(deviceConfiguration)) {
                    return Promise.resolve();
                }
                this._storeNumber = deviceConfiguration.StoreNumber;
                let request = new GasPumps.GetGasPumpsByStoreRequest(this._storeNumber);
                return context.runtime.executeAsync(request).then((result): void => {
                    if (!result.canceled) {
                        this._pumps = result.data.result;
                    }
                })
            }).then((): Promise<void> => {

                let stationDetailsRequest: GasPumps.GetGasStationDetailsByStoreRequest<GasPumps.GetGasStationDetailsByStoreResponse>
                    = new GasPumps.GetGasStationDetailsByStoreRequest(this._storeNumber);

                return context.runtime.executeAsync(stationDetailsRequest)
                    .then((result: ClientEntities.ICancelableDataResult<GasPumps.GetGasStationDetailsByStoreResponse>): void => {
                        if (!result.canceled) {
                            this._gasStationDetails = result.data.result;
                        }
                    });
            }).then((): void => {
                setInterval((): void => {
                    let request = new GasPumps.GetGasPumpsByStoreRequest<GasPumps.GetGasPumpsByStoreResponse>(this._storeNumber);
                    context.runtime.executeAsync(request).then((result): void => {
                        if (!result.canceled) {
                            let pumpsById: { [id: number]: Entities.GasPump } = Object.create(null);
                            let statusChanged: boolean = false;
                            this._pumps.forEach((pump) => {
                                pumpsById[pump.Id] = pump;
                            });

                            result.data.result.forEach((pump) => {
                                let previousPump: Entities.GasPump = pumpsById[pump.Id];
                                if (ObjectExtensions.isNullOrUndefined(previousPump)) {
                                    statusChanged = true;
                                } else if (JSON.stringify(previousPump) !== JSON.stringify(pump)) {
                                    statusChanged = true;
                                }
                            });

                            if (this._pumps.length !== result.data.result.length || statusChanged) {
                                this._pumps = result.data.result;
                                this._pumpStatusChangedHandlers.forEach((pair: StatusChangedHandlerWithId): void => {
                                    pair.handler(this.pumps);
                                });
                            }

                        }
                    })
                }, 4000)
            });
    }

    public stopAllPumpsAsync(context: IExtensionContext): Promise<ReadonlyArray<GasPump>> {
        let stopRequest: GasPumps.StopAllPumpsRequest<GasPumps.StopAllPumpsResponse> = new GasPumps.StopAllPumpsRequest(this._storeNumber);

        return context.runtime.executeAsync(stopRequest).then(
            (result: ClientEntities.ICancelableDataResult<GasPumps.StopAllPumpsResponse>): ReadonlyArray<GasPump> => {
                if (!result.canceled) {
                    this._pumps = result.data.result;
                    this._pumpStatusChangedHandlers.forEach((pair: StatusChangedHandlerWithId): void => {
                        pair.handler(this.pumps);
                    });
                }
                return this.pumps;
            });
    }

    public startAllPumpsAsync(context: IExtensionContext): Promise<ReadonlyArray<GasPump>> {
        let startRequest: GasPumps.StartAllPumpsRequest<GasPumps.StartAllPumpsResponse> = new GasPumps.StartAllPumpsRequest(this._storeNumber);
        return context.runtime.executeAsync(startRequest).then(
            (result: ClientEntities.ICancelableDataResult<GasPumps.StartAllPumpsResponse>): ReadonlyArray<GasPump> => {
                if (!result.canceled) {
                    this._pumps = result.data.result;
                    this._pumpStatusChangedHandlers.forEach((pair: StatusChangedHandlerWithId): void => {
                        pair.handler(this.pumps);
                    });
                }
                return this.pumps;
            }
        );
    }

    public updatePumpStatusAsync(context: IExtensionContext, id: number, state: GasPumpState): Promise<GasPump> {
        let pumpIndex: number = this._pumps.map(p => p.Id).indexOf(id);
        if (pumpIndex < 0) {
            return Promise.reject(new Error("Invalid pump identifier provided"));
        }

        let updateRequest: GasPumps.UpdatePumpStateRequest<GasPumps.UpdatePumpStateResponse> = new GasPumps.UpdatePumpStateRequest(this._storeNumber, id, state);
        return context.runtime.executeAsync(updateRequest).then(
            (result: ClientEntities.ICancelableDataResult<GasPumps.UpdatePumpStateResponse>): GasPump => {
                if (result.canceled) {
                    return null;
                }

                this._pumps.splice(pumpIndex, 1, result.data.result);
                this._pumpStatusChangedHandlers.forEach((pair): void => {
                    pair.handler(this.pumps);
                });

                return result.data.result;
            }
        );

    }


    public addPumpStatusChangedHandler(handler: (updatedPumps: GasPump[]) => void): number {
        if (!ObjectExtensions.isFunction(handler)) {
            throw new Error("GasStationDataStore.addPumpStaysChangedHandler: the provided handler must be a function");
        }

        let handlerId: number = new Date().getTime();
        this._pumpStatusChangedHandlers.push({ handler: handler, id: handlerId });

        return handlerId;
    }

    public removePumpStatusChangedHandler(id: number) {
        let handlerIndex: number = ArrayExtensions.findIndex(this._pumpStatusChangedHandlers, (pair) => pair.id === id);
        if (handlerIndex > 0) {
            this._pumpStatusChangedHandlers.splice(handlerIndex, 1);
        }
    }
}