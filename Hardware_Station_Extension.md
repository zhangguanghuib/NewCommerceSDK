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
    It is .Net Standard 2.0 Class Library project:<br/>
   ![image](https://user-images.githubusercontent.com/14832260/184473410-2e0f69d1-a79e-4d27-abf3-73c8749ab881.png)
2. The POS project structure as below:<br/>
    ![image](https://user-images.githubusercontent.com/14832260/184473506-7b4b6daa-7be5-4626-af2d-f9e89b967262.png)
    <br/>You need make sure the manifest file is correct.
3.  Some key code to implement this solution:<br/>
     <font style="background: green">How to call hardware station in POS  Typescript code:</font><br/>
    ![image](https://user-images.githubusercontent.com/14832260/184473676-fb37c5d8-6437-4019-bfd2-78d631f31920.png)
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
    <hr/>
   <font color='green'> Add a command button to show journal view to open this new page:</font><br/>
   ```TS
   import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";

export default class CoinDispenserCommand extends ShowJournalView.ShowJournalExtensionCommandBase {

    constructor(context: IExtensionCommandContext<ShowJournalView.IShowJournalToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "coinDispenserCommand";
        this.label = "Coin Dispenser";
        this.extraClass = "iconInvoice";
        this.isVisible = true;
        this.canExecute = true;
    }

    protected init(state: ShowJournalView.IShowJournalExtensionCommandState): void {
    }

    protected execute(): void {
        this.isProcessing = true;
        this.context.navigator.navigate("ExampleView");
        this.isProcessing = false;
    }
}
   ```
   <hr/>
   Hardware station controller and service code like below:<br/>
   
   ```C#
    [RoutePrefix("COINDISPENSER")]
        public class CoinDispenserController : IController
        {
            private const string CoinDispenserTestName = "MockOPOSCoinDispenser";

            [HttpPost]
            public async Task<bool> DispenseChange(CoinDispenseRequest request, IEndpointContext context)
            {
                ThrowIf.Null(request, "request");

                string deviceName = request.DeviceName;

                if (string.IsNullOrWhiteSpace(deviceName))
                {
                    deviceName = CoinDispenserController.CoinDispenserTestName;
                }

                try
                {
                    var openCoinDispenserDeviceRequest = new OpenCoinDispenserDeviceRequest(deviceName, null);
                    await context.ExecuteAsync<NullResponse>(openCoinDispenserDeviceRequest);


                    var dispenseChangeCoinDispenserDeviceRequest = new DispenseChangeCoinDispenserDeviceRequest(request.Amount);
                    await context.ExecuteAsync<NullResponse>(dispenseChangeCoinDispenserDeviceRequest);

                    return true;
                }
                catch (Exception ex)
                {
                    throw new PeripheralException("Microsoft_Dynamics_Commerce_HardwareStation_CoinDispenser_Error", ex.Message, ex);
                }
                finally
                {
                    var closeCoinDispenserDeviceRequest = new CloseCoinDispenserDeviceRequest();
                    await context.ExecuteAsync<NullResponse>(closeCoinDispenserDeviceRequest);
                }
            }
   ```
   <hr/>
   
   ```C#
    public class OposCoinDispenser : INamedRequestHandler, IDisposable
        {
            private int coinAmount = 1000;
            private bool isOpen = false;

            public string HandlerName
            {
                get { return PeripheralType.Opos; }
            }

            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(OpenCoinDispenserDeviceRequest),
                        typeof(DispenseChangeCoinDispenserDeviceRequest),
                        typeof(CloseCoinDispenserDeviceRequest)
                    };
                }
            }

            public Response Execute(Request request)
            {
                ThrowIf.Null(request, "request");

                Type requestType = request.GetType();

                if(requestType == typeof(OpenCoinDispenserDeviceRequest))
                {
                    var openRequest = (OpenCoinDispenserDeviceRequest)request;
                    this.Open(openRequest.DeviceName);

                }
                else if (requestType == typeof(DispenseChangeCoinDispenserDeviceRequest))
                {
                    var dispenseChangeRequest = (DispenseChangeCoinDispenserDeviceRequest)request;
                    this.DispenseChange(dispenseChangeRequest.Amount);

                }
                else if(requestType == typeof(CloseCoinDispenserDeviceRequest))
                {
                    this.Close();
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported", requestType));
                }

                return new NullResponse();
            }
            ...
   ```
   4. Add POS project and Hardware station Project to Store Commerce installer and build it:
      ![image](https://user-images.githubusercontent.com/14832260/184474503-ddc3a665-9874-497f-86be-9effc62b0723.png)
      and then install the POS installer<br/>
      ![image](https://user-images.githubusercontent.com/14832260/184474528-ca71a855-e68c-4033-8eac-da08f3c400d4.png)
      
      Check the Commerce store application installation folder<br/>
      ![image](https://user-images.githubusercontent.com/14832260/184475109-d104cb20-6e27-4dd4-b82c-1305b257f833.png)

   5. Debug it and you can the hardware station code is hit successfully:
      ![image](https://user-images.githubusercontent.com/14832260/184474581-082ec8d5-04ae-4fba-9d53-57dedef7451f.png)

<hr/>
The steps to implement a payment device is like below:<br/>
1. Go through this document: https://docs.microsoft.com/en-us/dynamics365/commerce/dev-itpro/end-to-end-payment-extension
2. Go to hardware profile, change it as this:
   ![image](https://user-images.githubusercontent.com/14832260/184475560-b798dbd5-407f-4e11-892c-0f0d4bcbbfe9.png)
    
3. Clone the code from https://github.com/microsoft/Dynamics365Commerce.InStore/tree/release/9.39/src/HardwareStationSample/PaymentDevice
4. Update the code as below:
    ![image](https://user-images.githubusercontent.com/14832260/184475360-53eb7775-42b0-419f-a347-e0b0e88481b3.png)
    
5. Make a cash and carry transaction, you can see the break point got hit:<br/>
    ![image](https://user-images.githubusercontent.com/14832260/184475434-6b0b3cb0-6858-4677-a1c4-7c56c8aa5592.png)
