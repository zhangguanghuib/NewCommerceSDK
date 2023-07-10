This document is to demonstrate how to configure an IIS-Hosted CSU in local environment with <b>One Single Azure Application</b> for：
- Retail Server
- Cloud POS
- Async Client<br/>

More official document can be found from:
- [How to configure CPOS to use your own Azure AD application](https://community.dynamics.com/ax/b/axforretail/posts/how-to-point-cpos-to-use-your-own-azure-ad-application)<br>
- [Introduction to Commerce Sealed Installers](https://community.dynamics.com/ax/b/axforretail/posts/introducing-sealed-installers)<br/>

Steps:<br/>
1. Open [Azure portal](https://aad.portal.azure.com/), and create a new App Registrations, give it a name like CSUServer, and Supported Account Types - My Orgnization Only:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/52e17e84-eab0-4193-bc4a-8fdf9c28e46f)
2. Expose an API->Add a scope:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8ea249b3-ebec-45a5-9441-80a6aee46976)
3. API Permissions:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/fb973385-9be8-4f3f-b60b-c99ab76cf1aa)
   And choose the API exposed in last step<br/>:
   <img width="636" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/e68bd651-c98b-42ea-add9-e15b05cd9b87">
4. Authentication->Add a platform->Single page application:->Redrirect URL->Input CPOS  URL from Channel Profile<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/976af717-f045-4182-a3b2-7b18c58c019f)
5. Upload certification (CER file):<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/50b8e8e9-7c6a-4357-84a5-2386a3523f0b)
6.  Go to HQ, create a Channel profile:<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/00e4e207-7797-4e0f-bc57-fe839fb78b06)
7. Set the store with new channel profile:<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7ed445ba-50e2-4cd0-9e8f-b1f140b4559f)
8. Create a channel database, add the required channel, this steps has nothing special:<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/81461121-6731-41c1-ab07-c9638a88f399)
9. Record the Application Id(Client Id) we created into the below form(previously it is called Microsoft Azure Application):<br>
    ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f761f3bc-059e-49da-b2b0-81f504140af5)
10.  Commerce Shared Parameters form->Identity provider:<br/>
    ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ff80dbe9-f06f-44f2-878c-642827d029f5)
    ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/e5b6c4fe-0ff7-4e00-a60f-a8229c20ae84)
11.  Download the channel database configuration xml file,  nothing needs be channged:<br/>
     ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/2d8bbd46-4fba-450f-acd9-8550d3990bce)<br/>
12.  Run command to install CSU, please make sure the <ThumbprintOfCertificate> and <SingleAppIdCreated> are same as we created:<br/>
    ```
    CommerceStoreScaleUnit.StoreSystemSetup.exe install --port 443 --SslCertFullPath "store:///My/LocalMachine?FindByThumbprint=<ThumbprintOfCertificate>" --AsyncClientCertFullPath "store:///My/LocalMachine?FindByThumbprint=<ThumbprintOfCertificate>" --RetailServerCertFullPath "store:///My/LocalMachine?FindByThumbprint=<ThumbprintOfCertificate>" --RetailServerAadClientId "<SingleAppIdCreated>" --RetailServerAadResourceId "api://<SingleAppIdCreated>" --CposAadClientId "<SingleAppIdCreated>"  --AsyncClientAadClientId "<SingleAppIdCreated>" --config StoreSystemSetup.xml --TrustSqlServerCertificate --SkipScaleUnitHealthCheck  --SqlServerName "<MachineName>\SQLEXPRESS" 
    ```




        




    
    














