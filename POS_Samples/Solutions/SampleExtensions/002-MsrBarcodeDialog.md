# MsrBarcode Dialog
## Run the Sample
- Go to POS, Order to fulfill, click the command button<br/>
<img width="908" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/61fe3fb4-8f44-4524-9d9e-46a88d660165"><br/>
- Lauch Simulator, go to scan barcode:<br/>
  <img width="711" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/79cfe664-9bb6-4d78-ba75-64ae2db5445b"><br/>
- You will the item number will be showing in the dialog input box <br/>
<img width="912" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/fc474e7c-e97b-45d4-8107-519910708db1"><br/>

## Technical details:
- The below two handlers need be implemented<br/>

```ts
  this.onBarcodeScanned = (data: string): void => {
      this._automatedEntryInProgress = true;
      this.numPad.value = data;
      this._inputType = "Barcode";
      this._automatedEntryInProgress = false;
  }

  this.onMsrSwiped = (data: ClientEntities.ICardInfo): void => {
      this._automatedEntryInProgress = true;
      this.numPad.value = data.CardNumber;
      this._inputType = "MSR";
      this._automatedEntryInProgress = false;

  }
```
- How to add the numpad on the dialog:<br/>

  ```ts
  import { IAlphanumericNumPad, IAlphanumericNumPadOptions } from "PosApi/Consume/Controls";
  public numPad: IAlphanumericNumPad;

    public onReady(element: HTMLElement): void {
      let numPadOptions: IAlphanumericNumPadOptions = {
          globalInputBroker: this.numPadInputBroker,
          label: "Please enter a value, scan or swipe",
          value: ""
      };

      let numPadRootElem: HTMLDivElement = element.querySelector("#barcodeMsrDialogAlphanumericNumpad") as HTMLDivElement;
      this.numPad = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "AlphanumericNumPad", numPadOptions, numPadRootElem);
      this.numPad.addEventListener("EnterPressed", (eventData: { value: Commerce.Extensibility.NumPadValue }) => {
          this._resolvePromise({ canceled: false, inputType: this._inputType, value: eventData.value.toString() });
      });

      this.numPad.addEventListener("ValueChanged", (eventData: { value: string }) => {
          if (!this._automatedEntryInProgress) {
              this._inputType = "Manual";
          }
      });
  }
  ```
- Html for Numpad<br/>
```
  <div class="col">
      <div id="barcodeMsrDialogAlphanumericNumpad"></div>
  </div>
```



