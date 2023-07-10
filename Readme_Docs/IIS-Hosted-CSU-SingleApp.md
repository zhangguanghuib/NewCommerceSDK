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
4. Authentication->Add a platform->Single page application:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/976af717-f045-4182-a3b2-7b18c58c019f)





        




    
    














