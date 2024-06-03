import * as PaymentView from "PosApi/Extend/Views/PaymentView";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { UpdatePaymentAmountData } from "PosApi/Extend/Views/PaymentView";
import { HardwareStationDeviceActionRequest, HardwareStationDeviceActionResponse } from "PosApi/Consume/Peripherals";
import { ObjectExtensions } from "PosApi/TypeExtensions";

declare var Commerce: any;
export default class PaymentViewCommandV2 extends PaymentView.PaymentViewExtensionCommandBase {

    public disposalDelay: number;

    constructor(context: IExtensionCommandContext<PaymentView.IPaymentViewToExtensionCommandMessageTypeMap>) {
        super(context);

        this.label = "Detect Payment View";
        this.id = "DetectPaymentView";
        this.extraClass = "iconAccept";
    }

    /**
     * Initializes the command.
     * @param {PaymentView.IPaymentViewExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: PaymentView.IPaymentViewExtensionCommandState): void {

        //this.disposalDelay = Commerce.ApplicationSession.instance.posConfiguration.disposalDelay as number;

        let timer = setInterval(() => {
            let viewName: string = Commerce.ViewModelAdapter.getCurrentViewName();
            console.log("The current view is: " + viewName);
            if (viewName !== 'PaymentView') {
                console.log("The current view is: " + viewName);
                clearInterval(timer);
                //Withdraw the mpnet from the cash change machine
                this.execute();
            }
        }, 200);
       
        this.isVisible = true;
        this.canExecute = true;
    }

    protected execute(): void {

        let amount: number = 30;

        let hardwareStatationDeviceActionRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
            new HardwareStationDeviceActionRequest("COINDISPENSER",
                "DispenseChange", {
                Amount: amount,
                DeviceName: "MyCoinDispenser"
            });

        this.context.runtime.executeAsync(hardwareStatationDeviceActionRequest).then(() => {
            this.context.logger.logInformational("Hardware Station request executed successfully");
            let data: UpdatePaymentAmountData = {
                paymentAmount: 30
            };
            this.updatePaymentAmount(data);
        }).catch((err) => {
            this.context.logger.logInformational("Failure in executing Hardware Station request");
            throw err;
        });
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}