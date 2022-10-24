import PumpGasDialog from "Create/Dialogs/PumpGasDialog";
import { Entities } from "DataService/DataServiceEntities.g";
import { GasStationDataStore } from "GasStationDataStore";
import { GasPumpStatus } from "GasStationTypes";
import { NumberFormattingHelper } from "NumberFormattingHelper";
import { ShowListInputDialogClientRequest, ShowListInputDialogClientResponse } from "PosApi/Consume/Dialogs";
import { Icons } from "PosApi/Create/Views";
import { ClientEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ISimpleProductDetailsToExtensionCommandMessageTypeMap, SimpleProductDetailsExtensionCommandBase, SimpleProductDetailsProductChangedData } from "PosApi/Extend/Views/SimpleProductDetailsView";
import { StringExtensions } from "PosApi/TypeExtensions";

export default class PumpGasExtensionCommand extends SimpleProductDetailsExtensionCommandBase {

    constructor(context: IExtensionCommandContext<ISimpleProductDetailsToExtensionCommandMessageTypeMap>) {
        super(context);
        this.extraClass = Icons.DeliveryTruck;
        this.id = "startStopPumpingGasExtensionCommand";
        this.label = "Start/stop pumping gas";
        this.productChangedHandler = (data: SimpleProductDetailsProductChangedData): void => {
            if (StringExtensions.compare(data.product.ItemId, GasStationDataStore.instance.gasStationDetails.GasolineItemId, true) === 0) {
                this.isVisible = true;
                this.canExecute = true;
            }
        };
    }

    protected init(state: Commerce.Extensibility.ISimpleProductDetailsExtensionCommandState): void {
        if (StringExtensions.compare(state.product?.ItemId, GasStationDataStore.instance.gasStationDetails.GasolineItemId, true) === 0) {
            this.isVisible = true;
            this.canExecute = true;
        }
    }

    protected execute(): void {
        this.isProcessing = true;
        let dialogOptions: ClientEntities.Dialogs.IListInputDialogOptions = {
            title: "Select gas pump",
            items: GasStationDataStore.instance.pumps.filter((pump: Entities.GasPump): boolean => {
                return pump.State.GasPumpStatusValue === GasPumpStatus.Idle
            }).map((pump: Entities.GasPump): ClientEntities.Dialogs.IListInputDialogItem => {
                return { value: pump.Id, label: pump.Name };
            })
        };

        let selectPumpRequest: ShowListInputDialogClientRequest<ShowListInputDialogClientResponse> = new ShowListInputDialogClientRequest(dialogOptions);

        this.context.runtime.executeAsync(selectPumpRequest).then((result: ClientEntities.ICancelableDataResult<ShowListInputDialogClientResponse>) => {
            if (result.canceled) {
                return;
            }

            let dialog = new PumpGasDialog();
            dialog.open({ pumpId: result.data.result.value.value })
                .then((pumpResult: ClientEntities.ICancelableDataResult<number>): Promise<any> => {
                    if (result.canceled) {
                        return Promise.resolve();
                    }

                    return GasStationDataStore.instance.updatePumpStatusAsync(
                        this.context,
                        result.data.result.value.value,
                        {
                            GasPumpStatusValue: GasPumpStatus.PumpingComplete,
                            LastUpdateTime: new Date(),
                            SaleVolume: pumpResult.data,
                            SaleTotal: NumberFormattingHelper.roundToNdigits(GasStationDataStore.instance.gasStationDetails.GasBasePrice * pumpResult.data, 3)
                        }
                    );
                }).catch((reason: any): void => {
                }).then((): void => {
                    this.isProcessing = false;
                });
        });
    }

}