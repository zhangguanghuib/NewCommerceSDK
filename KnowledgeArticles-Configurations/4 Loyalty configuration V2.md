1:  Config the Loyalty reward points<br/>
![image](https://github.com/user-attachments/assets/901bfad8-877d-4319-9b6e-9426ea81daf1)<br/>

2. Create Loyalty Program<br/>
![image](https://github.com/user-attachments/assets/2b1142a6-8983-4558-9d78-251fc9689ac1) <br/>

3. Create loyalty Scheme, it will build loyalty program/Reward Loyalty Point/Channel <br/>
   This setting means:<br/>
   Earning Rules:  1 $ will earn 1.2 loyalty points<br/>
   Redemption Rules:  100 $ can redemed 1 $ discount <br/>
   And these rules will be applied to the products under category "EYSLoyalty"
   <img width="1185" alt="image" src="https://github.com/user-attachments/assets/8946e26f-1b24-4cde-870b-b7712e2cbf2d"><br/>
   
5. Process Loyalty Schemes<br/>
   ![image](https://github.com/user-attachments/assets/0eeb93ac-795f-421b-9fcf-75240d358a20)<br/>

6.  Run job 1050<br/>
   ![image](https://github.com/user-attachments/assets/38c75ea0-3dad-4750-8fb0-e98d00bdb603)<br/>

7. Create 3 products for testing:<br/>
   ![image](https://github.com/user-attachments/assets/9bc2dd2a-a3f6-466f-981b-f4727b0db228)<br/>

   Please make sure no item sales tax group for these 3 products:<br/>
   <mark>EYSLoyaltyProd1:   $124.5</mark><br/>
   <mark>EYSLoyaltyProd2:   $59.9</mark><br/>
   <mark>EYSLoyaltyProd3:   $2.37</mark><br/>

   Then run the traditional like jobs like "process assortment, process delivery mode,  process loyalty schemes" ect.<>br/

6.  Create a new customer from POS and issue loyalty card for the new customer,  the loyalty card number is 5500305<br/>

7. on POS,  Sell the product : EYSLoyaltyProd1<br/>
   <img width="1214" alt="image" src="https://github.com/user-attachments/assets/764adeb9-4ca5-4caa-865c-9df81b26ff95"><br/>
   And earn these loyalty point:<br>
   <mark> That is 124.5 * 1.2 = 149.4</mark>
   <img width="1200" alt="image" src="https://github.com/user-attachments/assets/c4def8be-117d-4177-a7f2-8628e7390df1"><br/>
8. Return the above product and exchange it with another two products,  the transaction is like:<br/>
   <img width="1145" alt="image" src="https://github.com/user-attachments/assets/7cc3e2ad-72cc-407e-98a1-9d5a4e4e6b5e"><br/>
   The journal:<br/>
   ![image](https://github.com/user-attachments/assets/0112108a-ac22-4995-bdbe-2988c063c7c2)<br/>









