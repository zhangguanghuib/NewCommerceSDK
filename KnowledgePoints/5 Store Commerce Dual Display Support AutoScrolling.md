## Store Commerce Dual Display Support AutoScrolling.

1. <ins>Background:</ins><br/>
As you know the Store Commerce Out-Of-Box did not support Dual Display,  when enable Dual Display in the Funcationality Profile,  the Dual Display only show a simple amount due likee below<br/>

<br/>
This article is going to develop a customization to support Dual Display to support auto-scroll when more and more product are putting into cart that overflow the  Dual Display cart space.<br/>

2. <ins>Precoditions of this feature will work<ins>
* Enable Dual Display from Hardware profile
* Run 1070 or 9999 job
* Log on POS

3. <ins>Below is the recording to show how it is working:<ins><br/>
<div>
<iframe src="https://microsoftapc-my.sharepoint.com/personal/guazha_microsoft_com/_layouts/15/embed.aspx?UniqueId=42f1c6d9-3159-43fe-87d0-e3ddf392e589&nav=%7B%22playbackOptions%22%3A%7B%22startTimeInSeconds%22%3A0%7D%7D&embed=%7B%22ust%22%3Atrue%2C%22hv%22%3A%22CopyEmbedCode%22%7D&referrer=StreamWebApp&referrerScenario=EmbedDialog.Create" width="640" height="360" frameborder="0" scrolling="no" allowfullscreen title="Store Commerce Dual Display Demo-20240327_181558-Meeting Recording.mp4"></iframe>
</div>
4. <ins>Finally these 4 SQL  procesure will be called to delete the old transactions:<ins><br>
* PURGESALESONTERMINAL
* PURGESALESONTERMINAL
* PURGEASYNCCUSTOMERS
* PURGERETAILTRANSACTIONFISCALCUSTOMERS

5. <ins>Finally if you analyze the below store Procedure, you will find "i_RetentionDays" will be considered to delete old transactions:<ins><br/>
<img width="480" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c20090fa-3128-437b-b96c-e16be66e2388">

6. If you still have trouble,  I would suggest you check in the SQL Store Procedure,  what conditions are not met.
   
