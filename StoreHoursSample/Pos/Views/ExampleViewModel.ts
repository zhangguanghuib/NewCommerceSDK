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

    private _dispenseCoins(amount: number): void {

    }
}