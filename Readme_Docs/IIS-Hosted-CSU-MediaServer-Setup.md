# This document is to demonstrate IIS-Hosted CSU Media Server Setup.

- ## The background is currently the Sealed-Version CSU installation does not include Media Server Setup, so user need manually set the media server externally or internally.

The steps:
1. Go to IIS, create a new website:<br/>
<img width="243" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/003946de-6221-4636-9578-51eaa2d7fec5">
<br/>
2.  Input the site name and its Physical path and port:<br/>
   <img width="437" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/29de661e-beed-4c8c-a208-39a67e546d8b">
   <br/>
   Click "OK" button.<br/>
3. Copy all the images folders from Legacy Retail Server:<br/>
   <img width="850" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/6d26f970-bc39-4cdd-b922-67be0514349b"><br/>
4. Click Directory Browsing:<br/>
  <img width="810" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/67345982-a3af-4b19-8c87-bbd1e6014d85"><br/>
  <img width="636" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/95479968-bed2-45e8-abe2-4d82e0743d7f"><br/>
5.Add a new binding with type https:<br/>
  <img width="1107" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/3c257247-223c-4ae1-990e-44a4488aa576">

6. Browse the site in Browser and make sure all images are visible:<br/>
   <img width="449" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7ec32b6b-14b6-43a9-ab6c-4620301dce3d"><br/>
    <img width="579" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/0e8940b2-1f8d-4b25-a7a4-7f0ca254b004">
7. Set Channel Profile as below:<br/>
   <img width="595" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8f2ce4bb-b29e-4922-8c07-95eee4d38fdd">

8. Run 9999 job<br/>
9. Logon to POS,  find product 0001, you will find the product images are showing properly:<br/>
   <img width="1475" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c041b502-6921-4830-b389-742e3c314753">












        




    
    














