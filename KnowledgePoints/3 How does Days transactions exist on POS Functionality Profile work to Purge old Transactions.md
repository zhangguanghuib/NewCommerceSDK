##  How does Days transactions exist on POS Functionality Profile work to Purge old Transactions from CSU channel database?

1. <ins>Background:</ins><br/>
Recently some support engineers asks they set the "Days transactions exist" but it seems the old transaction still there and never been deleted automatically<br/>
<img width="1013" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ede5dbb3-deb4-4187-87bf-582f2e85f7ac">
<br/>
This article is going to help introduce the underlying logic and help you understand how it does work.<br/>

2. <ins>Precoditions of this feature will work<ins>
* set the "Days transactions exist" on POS  functionality profile
* Run 1070 or 9999 job
* Close shift from POS

3. <ins>Why only when close shift from POS, Purge old transactions will happen?   Please see the below process:<ins><br/>
From these steps,  you can see the first step is to close Shift, and then the request Chain will call PurgeSalesTransactionsDataRequest API will be called finally<br>
<img width="1107" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7f614090-eef2-4c1b-a2fd-700ad2d506a6">
<img width="1117" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f50442d4-f456-4a79-93db-33527db3ff59">
<img width="1090" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/5a40a9a9-1e8b-41a8-9419-a39dbc291b50">
<img width="1103" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ae01d8b9-1f44-4004-ba03-4e4193057357">
<img width="1095" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/d04d5e7b-01fa-4894-9072-07a8bba7ea9a">

4. <ins>Finally these 4 SQL  procesure will be called to delete the old transactions:<ins><br>
* PURGESALESONTERMINAL
* PURGESALESONTERMINAL
* PURGEASYNCCUSTOMERS
* PURGERETAILTRANSACTIONFISCALCUSTOMERS

5. <ins>Finally if you analyze the below store Procedure, you will find "i_RetentionDays" will be considered to delete old transactions:<ins><br/>
<img width="480" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c20090fa-3128-437b-b96c-e16be66e2388">

6. If you still have trouble,  I would suggest you check in the SQL Store Procedure,  what conditions are not met.
   
