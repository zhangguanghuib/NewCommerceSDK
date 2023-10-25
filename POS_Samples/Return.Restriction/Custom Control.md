# Custom Control Demonstration<br/>
<ol>
   <li>
      The first custom control is what I developed called Inline Quantity Update:<br/>
      <img width="1095" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/fa52b187-8b82-450b-aa2b-c725542094cf">
      By this control you can do:
      <ul>
         <li>Click '+' to increase the selected cart line quantity</li>
         <li>Click '-' to increase the selected cart line quantity</li>
         <li>Directly input a new quantity for a cart line</li>
         and
         <li>Input a new price then click "Enter" to update price of the current selected cart line  </li>
      </ul>
   </li>
   <li>
      When there is not cart line selected,  the Qty Inline Update Control will not be showing:
      <img width="1058" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4ef8e553-671b-4054-a160-591a36a92f83">
   <li>
      When you need do is to make these development:<br/>
      <img width="276" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/017eac5d-e1aa-4909-a4dc-9da78ddc8cf5">
   </li>
   <li>
      In manifest file, you should put this kind of section:
      <img width="708" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4180094a-b237-4278-b816-c28a95b8f807">
   </li>
   <li>
      In Screen layout,  add a custom control to the cart screen:<br/>
      <img width="1472" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/0b7f051d-a87f-4d08-855e-2d081fe8eb2b">
      Please make sure the value set on the screenlayout does make the values in manifest.
   </li>
   <li>
      All source code can be found from:<br/>
      https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/Return.Restriction/Pos/ViewExtensions/Cart
   </li>
</ol>
   

