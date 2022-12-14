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

4. Copy to AOS:
   Copy the dll and pdb to C:\AOSService\PackagesLocalDirectory\Bin\Connectors 
   ![image](https://user-images.githubusercontent.com/14832260/207484722-30858c7c-189e-4d5a-8181-1e9d060f438d.png)
   
   Also copy them into the below folder:
   ![image](https://user-images.githubusercontent.com/14832260/207484690-57a2db62-a3d1-457d-8b63-12e93a67e6be.png)
   
 5. Copy the dll to these below locations:
     C:\RetailServer\webroot\bin
     C:\RetailServer\webroot\bin\Connectors
     C:\RetailServer\webroot\bin\Ext
 6. Go to CPOS,  web.config, add the below:
    ![image](https://user-images.githubusercontent.com/14832260/207490788-59fd1ecc-aa00-4a2c-9bbe-e475ee9d2fac.png)
 7.  Create hardware profile as below:
     ![image](https://user-images.githubusercontent.com/14832260/207491105-72100eb0-7c2b-4f4e-90ed-434e36c8e410.png)
  8. Attach the hardware profile to register, run 1090 job
  9. Go to CPOS, make a transaction and pay card:
     ![image](https://user-images.githubusercontent.com/14832260/207491720-460bbc21-793f-4050-b6ce-988c7b74332c.png)
     ![image](https://user-images.githubusercontent.com/14832260/207491960-c4c18e11-b67d-4ea5-86d0-d671da3cba53.png)
     ![image](https://user-images.githubusercontent.com/14832260/207492077-bcd0ffa5-9270-49a5-988e-8df5da44997c.png)



  


    


