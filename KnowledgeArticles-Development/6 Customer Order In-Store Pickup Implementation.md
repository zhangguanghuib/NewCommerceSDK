## Customer Order(Call Center Order) In-Store Pickup Notification

1. <ins>Background:</ins><br/>
As you know D365 Commerce Solution Support Customer order (Call Center Order) In-Store Pick-up, some Store Manager want to get notification to show new order created and thatneed pickup from the current store =<br/>
The final function is like this 
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4148ff19-0324-4160-9cf6-2846852eb428)

2. Implementation details
- Enable Dual Display from Hardware profile:
  <img width="689" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/08cd5a67-deff-4f30-ac7d-23d6595c30dc"><br/>
- Run 1070 or 9999 job.
- Log on POS

3. <ins>Below is the recording to show how it is working:</ins><br/>
[![Alternate Text](http://img.youtube.com/vi/L1SN7kqq9lQ/0.jpg)](https://youtu.be/L1SN7kqq9lQ)
<br/>
4. <ins>You can see:</ins><br>
- When a new product added to the cart, the dual dispaly will show a product:
   <img width="1679" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/b52bb1d8-5739-4ed2-8b49-3fd17c3bee10">
- When more and more products added to the cart and the cart line space exceed the Dual Display Cart Space,  it will auto-scroll to the last line when it is added:
   <img width="1677" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ba634e4b-7cf0-41c5-b63e-7e715f4b8081">
- When the last cart line is showing in the view, and main POS  is editing the first cartline(change its Qty), Dual Display will sroll to the first line:<br/>
   <img width="803" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/50cbddb5-8f3e-41be-90a8-c3f7a59e33ba">
   <img width="1676" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/d7ebd8ca-29fc-4ec2-b8c9-b94668827c27">
 
- When the the first cart line is in Dual Display View,  and main POS is editing the last cart line, it will scroll to the last cart line<br/>
<img width="1669" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/eb62dfcf-1395-4c13-83b7-d55f0911dea7">
<img width="1668" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/42e1ec53-2b10-4a99-95bf-8310c5a87246">

5. <ins>In the bottom of the Dual Display, there is Swiper to show the Picture of the company or the city:</ins><br/>
<img width="1891" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ece104ee-e9e5-4805-832a-daf055811cd4">


6. Technical point.
   - Whenever cart changed, like adding a new cart line or update existing cart line, the line id will be recorded and stored in the local storage
   ```ts
   import { ProxyEntities } from "PosApi/Entities";
   import * as CartView from "PosApi/Extend/Views/CartView";
   import { ArrayExtensions, StringExtensions } from "PosApi/TypeExtensions";
   
   export default class CartViewController extends CartView.CartExtensionViewControllerBase {
       public static selectedCartLineId: string = StringExtensions.EMPTY;
       public static selectedCartLines: ProxyEntities.CartLine[];
       public _selectedTenderLines: ProxyEntities.TenderLine[];
       public _isProcessingAddItemOrCustomer: boolean;
   
       constructor(context: CartView.IExtensionCartViewControllerContext) {
           super(context);
           CartViewController.selectedCartLines = [];
           CartViewController.selectedCartLineId = null;
           this.cartLineSelectedHandler = (data: CartView.CartLineSelectedData): void => {
               CartViewController.selectedCartLines = data.cartLines;
               if (ArrayExtensions.hasElements(CartViewController.selectedCartLines)) {
                   CartViewController.selectedCartLineId = CartViewController.selectedCartLines[0].LineId;
                   localStorage.setItem("currentCartLineId", CartViewController.selectedCartLineId);
               }
           }
   
           this.cartLineSelectionClearedHandler = (): void => {
               CartViewController.selectedCartLines = [];
               CartViewController.selectedCartLineId = null;
               localStorage.setItem("currentCartLineId", StringExtensions.EMPTY);
           }
   
           this.tenderLineSelectedHandler = (data: CartView.TenderLineSelectedData): void => {
               this._selectedTenderLines = data.tenderLines;
           }
   
           this.tenderLineSelectionClearedHandler = (): void => {
               this._selectedTenderLines = [];
           }
   
           this.processingAddItemOrCustomerChangedHandler = (processing: boolean): void => {
               this._isProcessingAddItemOrCustomer = processing;
           }
   
           this.cartChangedHandler = (data: CartView.CartChangedData): void => {
               console.log(data);
           }
       }
   }
   ```
7. How to scroll? <br/>

   - Calculate the line height of each cart line:
   ```ts
    // Find the line height
    let listLine: HTMLDivElement = document.querySelector(".dataListLine") as HTMLDivElement;
    let rowHeight = 39;
    if (listLine?.clientHeight) {
        rowHeight = listLine?.clientHeight;
    } 
   ```
   - When a new line is added and the cart line is already over 15 lines, calculate the scrolltop,  the formula is the totalHeight of the scroll container  minus the first 15 lines total height, that is<br/>
     let shouldScrollTop = totalHeight - rowHeight * 15;
    ```ts
     if (isNewLine) {
     if (!StringExtensions.isEmptyOrWhitespace(currentCartLineId)) {
         if (data.cart.CartLines.length >= 16) {
             setTimeout(() => {
                 let dualDisplayScrollingContainer: HTMLDivElement = document.querySelector('[aria-label="Scrolling Container"]') as HTMLDivElement;
                 let totalHeight: number = dualDisplayScrollingContainer.scrollHeight;
                 let shouldScrollTop = totalHeight - rowHeight * 15;
                 dualDisplayScrollingContainer.scrollTop = shouldScrollTop;
             }, 700);
          }
      }
    }
    ```
   - When editing the existing line, the scrolltop is line number * line hight, the code is
   ```ts
   else {//Update existing line
    setTimeout(() => {
        let selectedIndex = ArrayExtensions.findIndex(data.cart.CartLines, (cartline: ProxyEntities.CartLine) => cartline.LineId === currentCartLineId);
        let dualDisplayScrollingContainer: HTMLDivElement = document.querySelector('[aria-label="Scrolling Container"]') as HTMLDivElement;
        let shouldScrollTop = rowHeight * selectedIndex;
        dualDisplayScrollingContainer.scrollTop = shouldScrollTop;
     }, 700);
   }
   ```
   
