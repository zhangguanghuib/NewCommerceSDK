import { IExtensionViewControllerContext } from "PosApi/Create/Views";
import { HardwareStationDeviceActionRequest, HardwareStationDeviceActionResponse } from "PosApi/Consume/Peripherals";

export default class ExampleViewModel {
    public title: string;
    private _context: IExtensionViewControllerContext;

    constructor(context: IExtensionViewControllerContext) {
        this._context = context;
        this.title = this._context.resources.getString("string_0001");
    }

    public pingCoinDispenser(): void {
        let hardwareStationDeviceActionRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
            new HardwareStationDeviceActionRequest("CUSTOMPINGS",
                "CustomPing",
                { Message: "Knock, knock!" }
            );

        this._context.runtime.executeAsync(hardwareStationDeviceActionRequest).then((result) => {
            this._context.logger.logInformational("Message from HWS: " + result.data.response);
        }).catch((err) => {
            this._context.logger.logInformational("Failure in executing Hardware Station request");
        });
    }

    public dispenseThousandCoins(): void {
        this._dispenseCoins(1000);
    }

    public dispenseTenCoins(): void {
        this._dispenseCoins(10);
    }

    private _dispenseCoins(amount: number): void {
        let hardwareStatationDeviceActionRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
            new HardwareStationDeviceActionRequest("COINDISPENSER",
                "DispenseChange", {
                Amount: amount,
                DeviceName: "MyCoinDispenser"
            });
        this._context.runtime.executeAsync(hardwareStatationDeviceActionRequest).then(() => {
            this._context.logger.logInformational("Hardware Station request executed successfully");
        }).catch((err) => {
            this._context.logger.logInformational("Failure in executing Hardware Station request");
            throw err;
        });

    }
}