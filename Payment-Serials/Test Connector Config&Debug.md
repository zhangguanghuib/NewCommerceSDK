## This document will show how to config and debug the test connector for call center and retail CPOS to help you to understand the how payment process is working, and also help you dive into how to develop your own payment connector.

Steps：
1. Go to RetailSDK, find the PaymentSDK solution:
   ![image](https://user-images.githubusercontent.com/14832260/207480907-64aff128-b7dd-4bfe-a842-b5150fbcec78.png)
2. In order to differentiate the existing test connector and the one here,  I would change some places, changing the "Test Connector" to "Contoso Payment Connector"
   ![image](https://user-images.githubusercontent.com/14832260/207481178-8b2027bd-9850-4fc5-b013-d070a554189f.png)
   ![image](https://user-images.githubusercontent.com/14832260/207481283-0606e33d-09b8-4a0c-92bb-e1e16a37d98c.png)
   ![image](https://user-images.githubusercontent.com/14832260/207481673-68bdea46-561d-4d6e-aaf0-7ae16900341e.png)  
3. Create database for PaymentAcceptWeb
   ![image](https://user-images.githubusercontent.com/14832260/207484524-106049af-c739-424c-afd8-19926a1d3487.png)
    Change the database connection string:
    ![image](https://user-images.githubusercontent.com/14832260/207483006-a5356ae3-f6f0-4401-9f9e-d0dd86b614c4.png)

4. Debug this payment connector for call-center scenario:
   Copy the dll and pdb to C:\AOSService\PackagesLocalDirectory\Bin\Connectors 
   ![image](https://user-images.githubusercontent.com/14832260/207483635-b60587c4-b358-45d9-91cc-b6a086eb6390.png)
   
   Also copy them into the below folder:
   ![image](https://user-images.githubusercontent.com/14832260/207484273-6ab0d147-0523-41a1-a34c-123aeefb8baa.png)

