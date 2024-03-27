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
<div style="border:1">
   https://microsoftapc-my.sharepoint.com/personal/guazha_microsoft_com/_layouts/15/embed.aspx?UniqueId=42f1c6d9-3159-43fe-87d0-e3ddf392e589&nav=%7B%22playbackOptions%22%3A%7B%22startTimeInSeconds%22%3A0%7D%7D&embed=%7B%22ust%22%3Atrue%2C%22hv%22%3A%22CopyEmbedCode%22%7D&referrer=StreamWebApp&referrerScenario=EmbedDialog.Create" width="640" height="360" frameborder="0" scrolling="no" allowfullscreen title="Store Commerce Dual Display Demo-20240327_181558-Meeting Recording.mp4"
</div>
<br/>
4. <ins>You can see:<ins><br>
* When a new product added to the cart, the dual dispaly will show a product:
   <img width="1679" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/b52bb1d8-5739-4ed2-8b49-3fd17c3bee10">
* When more and more products added to the cart and the cart line space exceed the Dual Display Cart Space,  it will auto-scroll to the last line when it is added:
   <img width="1677" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ba634e4b-7cf0-41c5-b63e-7e715f4b8081">
* When the last cart line is showing in the view, and main POS  is editing the first cartline(change its Qty), Dual Display will sroll to the first line:<br/>
   <img width="803" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/50cbddb5-8f3e-41be-90a8-c3f7a59e33ba">
   <img width="1676" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/d7ebd8ca-29fc-4ec2-b8c9-b94668827c27">
 
* When the the first cart line is in Dual Display View,  and main POS is editing the last cart line, it will scroll to the last cart line<br/>
<img width="1669" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/eb62dfcf-1395-4c13-83b7-d55f0911dea7">
<img width="1668" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/42e1ec53-2b10-4a99-95bf-8310c5a87246">

5. <ins>In the bottom of the Dual Display, there is Swiper to show the Picture of the company or the city:<ins><br/>
<img width="1891" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ece104ee-e9e5-4805-832a-daf055811cd4">


6. If you still have trouble,  I would suggest you check in the SQL Store Procedure,  what conditions are not met.
   
