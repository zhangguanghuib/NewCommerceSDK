## This article will show loyalty related configurations including earn and redem loyalty points for basic bug reporting from scratch.
1. Go to D365 FO and then this section, most of the settings came from these area.<br/>
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/2c32a5d1-0af0-4c36-a3c5-177e84002df7)
2. Create a  Loyalty reward points, like below, some highlight field should get attention:<br/>          
<img width="1345" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/08470036-b093-4307-81f0-61690e65e174">
3. Create loyalty program:<br/>

![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/2c079295-d199-41aa-8c4e-f8bed3490d0d)
4. Create Loyalty Scheme:<br/>
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/a27e8dc7-bec6-45b0-a690-2f59e115a397)

5.  On the loyalty Scheme form, please don't forget add the channel, otherwise the loyalty point will not be earned:<br/>
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/361a17e6-eaf6-4753-9bc6-35c16c980403)

6.  Log on POS,  create a new customer, when customer creation is done,  click "Add to Sale"<br/>
7.  In the customer details form, click "Issue loyalty Card" <br/>
<img width="266" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/0e49be60-9510-4da1-a95e-eaac7e52564d"><br/>

8.  And then Issue Loyalty card， click  "Issue and Add" button, finally it looks like:<br/>
<img width="236" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/d7c65e1f-1c5b-4c5b-9e90-ad00396e591d"><br/>

9. Go back to HQ,  and find the newly-created loyalty card,  and remove all the pre-defined configuration and only leave our own previous configurations:<br/>
<img width="1203" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/947b7639-7923-4488-9670-5f443cc69c8d">

10. Please run batch job :<br/>
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/e49e4749-20e9-4c72-9e4f-283a3cb71f0f)

12.  If you want, you can create two different points,  those itemid are:<br/>
     TestPoint1<br/>
     TestPoint2<br/>
     The category choose Fashion that is from " Loyalty Scheme"<br/>
     Run product assortment, and run process delivery mode<br/>
13. Run 9999 job and make sure all applied.<br/>

14.  Log on POS, create customer order with two lines,  pay deposit and pickup one line<br/>
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/a0d2c129-a492-474b-973f-f250efb59068)

15.  You will see the loyalty card already get some points<br/>
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/dff96211-aa92-4fed-ab99-30a3a82cb9b0)

16.  When pickup another product,  you can pay the reminder amount due by loyalty card, of course if it is not enough, you can pay cash<br/>
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/d4e396e2-b20b-4f71-8123-cb8e405754d9)

17.  Finally you will see the loyalty card transactions<br/>

![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/1f44cb39-4b6d-499d-bfc0-8793dba2e644)

18.   The Commerce Shared parameters maybe has related configurations:<br/>
<img width="1650" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/81858197-b0d6-49b0-a4dd-14abf5a5d3c6">

<br/>
<br/>
<hr/>
Finally,  the above steps you be should to work for the basic loyalty scenario,  if you face any issue that the loyalty point will not be earned,  please double check and run 9999 job.
