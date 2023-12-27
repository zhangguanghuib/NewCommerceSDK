# Improved Store Hours Sample
## Overview
This sample is a showcase an improved Store Hours Sample which exists in Retail SDK  for a long time, this sample improved the existing sample so that the new sample fixed some bugs to make the POS view can refresh automatically, and this page also support the traditional CRUD functions, that means this sample can support 
- Create a store hour entry
- update an existing store hours sample
- Delete an exisiting store hours sample
- Reload the page to get the latest data
- Refresh UI to reflect the change
## Configuring the sample
- Create a POS  Operation in HQ <br/>
  <img width="880" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/b377aabb-0eb3-4707-a4d8-62523302401b">

- Add the POS Operation on the POS button grid designer:HQ <br/>
  <img width="300" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/bec31cf6-4dcc-4227-9ffb-cfb7ec292db3">

- Run job 9999
## Running the sample
- Go to Cart Screen, find the button "Store Hours", click it HQ <br/>
  <img width="481" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/3527cfc1-d8c9-4aad-80a5-84826f2c4b25">
- Find the command bars and then click '+':HQ <br/>
  <img width="116" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/578b2e08-33c7-411b-a8f7-0bdb4ced7bc1">
- On the opened dialog, input the necessary information and the click "Update" button:HQ <br/>
  <img width="827" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/654b6b48-44c9-417f-922f-adacf1210679"><br/>
  you will see a new record created in the view:<br/>
  <img width="1181" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/85acc934-5166-44fb-b70e-9ed642807c99">
- Click one existing record,  one dialog is opened automatically, make the changes you want, and then click "Update" button<br/>
  <img width="1473" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/0176ca03-4607-451a-8528-dfafa6042862">
  You will see the changes reflected on the existing grid
 <img width="1244" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8da3dd82-ded2-4de7-932b-4ddaa03a25fe">

- Click one existing record,  one dialog is opened automatically, make sure the delete is "Delete"<br/>
  <img width="1462" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4ad682eb-522e-483e-a9c3-91f963d87c4c">
   Click "Update" button, you will see the record got deleted from UI and database
   <img width="1251" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ef80c08e-a260-408a-9c06-08df668c0908">

- Click "Reload" button on then command bar, and the it will refresh the view with the latest data from database
  <img width="1364" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/0e0467f1-437d-4a7e-b022-f26864a73d48">





