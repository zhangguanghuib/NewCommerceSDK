This document is to demonstrate how to develop hardware station customizatin and debug that for trouble shooting

Through this demon, you will learn how to develop these below things:
There are 3 sample provide:

* In Sample#1, you will learn: <br/>
  1. How to develop a hardware station solution.
  2. How to make a new POS page including html page, view file in Typescript,  View Mode code file in Typescript.
  3. How to open the page you just created through adding a command bar button in the existing page.
  4. How to call hardware station API  in POS TS  code.

* In Sample #2, you will learn: <br/>
  1. How to overide the existing hardware logic.

* In Sample #3, you will learn: <br/>
  1. How to delevelop a payment device.

<hr/>

<font size="4">Sample #1 -  Coin Dispenser</font>

The final effect of this sample is like this:

1. Go to show journal, click the new command button we added:
   ![image](https://user-images.githubusercontent.com/14832260/184473152-d17c3aa2-8718-465c-a965-c65342e4dba0.png)
2. The new page will open:
    ![image](https://user-images.githubusercontent.com/14832260/184473213-68e41a46-7d04-4788-8093-81ec8494f012.png)
3. Click each button to call hardware station to do health check, or dispense 10 coin, or 1000 coin,  if there is no enough coin to dispense, the hardare station will return error:
    ![image](https://user-images.githubusercontent.com/14832260/184473324-1d7a3024-0625-4aeb-aede-ea573519da60.png)


The steps of making this solution is as below:
1. Hardware station project structure are like below:
    It is .Net Standard 2.0 Class Library project:
   ![image](https://user-images.githubusercontent.com/14832260/184473410-2e0f69d1-a79e-4d27-abf3-73c8749ab881.png)
2. The POS project structure as below:
    ![image](https://user-images.githubusercontent.com/14832260/184473506-7b4b6daa-7be5-4626-af2d-f9e89b967262.png)
    You need make sure the manifest file is correct.
3.  Some key code to implement this solution:
    How to call hardware station in POS  Typescript code:
    ```TS
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
                new HardwareStationDeviceActionRequest("CUSTOMPING",
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
    ```

