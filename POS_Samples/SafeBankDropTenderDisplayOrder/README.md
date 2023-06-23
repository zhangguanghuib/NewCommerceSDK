## This feature is going to provide support display order on tender types for safe drop or bank drop
### 1. How it works
#### 1)In HQ side, set the dislay order for store tender types
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/58fe220f-9291-4752-bc63-9866965ecbb0)
#### 2)Run this download distribution job and make download session applied
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/872c1732-e947-4a49-bdf1-dc43dcc7c489)
#### 3)On POS, go to safe drop or bank drop, you will find the tender type display order will be adjusted
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/efe09b42-5ff5-4739-8609-0de3804ced24)
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/54c232ab-336e-4431-9544-d235508c7033)
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f823f09e-9c91-428b-9720-503bac3547b6)

### 2. How it gets implemented
#### In HQ  side:
##### 1) Create a EDT based in Real type:
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/d16f9d57-4c39-403f-8def-2be5d4d49e21)
##### 2) Add the EDT to the table extension of RetailStoreTenderType
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/712f9264-ac42-4e94-9920-ed3b86099504)
##### 3) Add this field on the form
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/82b729a8-5e9c-4875-a026-301717ab3a39)

#### CDX side:
##### 1) Create all the field for the new table:
<img width="1300" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/58ba5f5e-5424-4270-b98d-2faeaf5c8f2e">
Important:
In case you see the SysPick Dialog in the right,  just skip that, you need manually input the field name and save it.
##### 2) Create scheduler subjob:
<img width="1103" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/2aad63ac-4c21-4419-830c-7da91e8b67c9">
##### 3) Create scheduler job:
<img width="878" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/42c4a9b4-e838-41d6-8c51-6ccc7942ee74">
##### 4) Create distribution scheduler:
<img width="827" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/52cca225-db0b-43d9-9bc0-9967840b070c">



