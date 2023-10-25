# POS operation demonstration
   Steps:
   <ol>
      <li>Create a POS  operation in HQ
      <img width="864" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/3eff290a-9f47-4a0c-bedc-38a39a084f6b">
      </li>
      <li>
         Add it to button POS  screen layout and button grid:
         <img width="920" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/30a37816-14f9-498d-b75b-1da0353b336a">
      </li>
      <li>
         Run 1090 job
      </li>
      <li>
         Make POS development for the new POS operation, those contains Respose/Request/Handler/Factor<br/>
         <img width="1482" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/2ca0d1b0-3a14-4f73-ae53-d4f81e770094">
         Source code can be found from
         https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/Return.Restriction/Pos/Operations/SetReasonCodeToCartLine
      </li>
      <li>
         Log on to POS,  choose one cart line and click the "Set Reason Code" button<br/>
         <img width="1905" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/68b0aa04-067b-4be4-b9bf-3109fab22bb2">
      </li>
      <li>
         Finally,  you will be able to see the reason code is already set on the selected cart:<br/>
         <img width="563" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/d5714c59-e2f6-4d7d-98cf-1768fa70a29f">
      </li>
      <li>
         You can also put the operation on other screen:<br/>
         <img width="367" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c9fe9dab-9b23-4dce-9653-9c4c64a43e7b">
      </li>
      <li>
         When click it, you are able to choose one single line to be set reason code:
         <img width="577" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/bc8d502d-b792-4e51-b96d-b5e946ad755e">
      </li>
      <li>
         There is an important thing that is how we can know which cart line is selected,  the way is to make CartViewController to record which line will be selected:<br/>
         <img width="772" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/2d9a1590-b910-40a8-b0d1-d37c364dfa13"><br/>
         Anytime you can call this code to get the current selected cart line:<br/>
         <img width="987" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7b8d5d5b-c320-4fa5-a7d2-9b24816e6ff4">
      </li>
   </ol>   

