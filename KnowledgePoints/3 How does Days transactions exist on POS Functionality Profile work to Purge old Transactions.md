##  How does Days transactions exist on POS Functionality Profile work to Purge old Transactions from CSU channel database?

1. <ins>Background:</ins><br/>
Recently some support engineers asks they set the "Days transactions exist" but it seems the old transaction still there and never been deleted automatically<br/>
<img width="1013" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ede5dbb3-deb4-4187-87bf-582f2e85f7ac">
<br/>
This article is going to help introduce the underlying logic and help you understand how it does work.<br/>

2. <ins>Precoditions of this feature will work<ins>
* set the "Days transactions exist" on POS  functionality profile
* Run 1070 or 9999 jon
* Close shift from POS

3. <ins>Why only when close shift from POS, Purge old transactions will happen?   Please see the below process:<ins><br/>
<img width="1107" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7f614090-eef2-4c1b-a2fd-700ad2d506a6">
<img width="1117" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f50442d4-f456-4a79-93db-33527db3ff59">

