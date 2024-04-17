## Customer Order(Call Center Order) In-Store Pickup Notification

1. <ins>Background:</ins><br/>
As you know D365 Commerce Solution Support Customer order (Call Center Order) In-Store Pick-up, some Store Manager want to get notification to show new order created and thatneed pickup from the current store =<br/>
The final function is like this 
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4148ff19-0324-4160-9cf6-2846852eb428)

2. Implementation details
- Add Custom Control on Screen Layout Designer
- <img width="1482" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/68f80dad-79cd-42a3-8ece-1149f8e7e587">
- <img width="1424" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8d92e1cd-2c70-48dc-b279-b0e728afe1a1">

3. <ins>Install Scale Unit Extension Package</ins><br/>
4. <ins>Install Store Commerce Extension Package</ins><br>
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
   
