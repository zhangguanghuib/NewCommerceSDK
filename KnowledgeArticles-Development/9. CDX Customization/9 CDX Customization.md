## D365 Commerce CDX Customization.

1. <ins>Background:</ins><br/>
During the D365 Commerce Project Implementation, create custom table and push and pull data between HQ and CSU database is very important, this article will show cases how to make CDX customization based on the official samples.
2. How it looks like:<br/>
   . when log on POS and nothing in the cart,  then the dual display only showing the embeded website, for me it is Bing for demo the function<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/06941f0c-7574-431e-b004-f9cca9596cf0)
   . when add a new product into the cart, you can see the shopping cart showing in the left, website in the right:<br/>
    ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9cbf3386-bb5b-4e6a-a3ac-ff97ccbed008)
   . when checkout the cart,  the cart disappear again,  and only show the website<br/>
    ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/e611ed95-aff5-47d7-a293-d5e965576594)

   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/b9b176bc-2014-43b3-8c1d-1c4ccdb23d67)

3.  The source code is :<br/>
   https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/Solutions/DualDisplayHideEmptyCart

 4.  The key code is:<br/>
 ```ts
  this.isCartEmpty = ko.computed(() => {
      return !ObjectExtensions.isNullOrUndefined(this._cart()) && (this._cart().CartLines.length > 0) ? false : true;
  });
 ```

```html
<div data-bind="css: {'cartDiv': true,  'width40Percentage': true}, visible: !isCartEmpty()">
...
</div>
...
<div data-bind="css: {'imgContainer': true, 'width60Percentage':true }, visible: !isCartEmpty()">
    <div style="border:solid">
        <iframe id="webview" src="https://www.bing.com/"  style="height: 1100px; overflow: scroll;"></iframe>
    </div>
</div>
 <div data-bind="css: {'imgContainer': true, 'width100Percentage':true }, visible: isCartEmpty()">
    <div style="border:solid">
        <iframe id="webview" src="https://www.bing.com/"  style="height: 1100px; overflow: scroll;"></iframe>
    </div>
</div>
```
