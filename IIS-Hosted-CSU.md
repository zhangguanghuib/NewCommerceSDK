This document is to demonstrate how to configure an IIS-Hosted CSU environment for new commerce SDK development.<br/>
All steps comes from these two link:<br/>
[How to configure CPOS to use your own Azure AD application](https://community.dynamics.com/ax/b/axforretail/posts/how-to-point-cpos-to-use-your-own-azure-ad-application)<br>
[Introduction to Commerce Sealed Installers](https://community.dynamics.com/ax/b/axforretail/posts/introducing-sealed-installers)<br/>

Steps:<br/>
1. Open [Azure portal](https://aad.portal.azure.com/)<br/>
2. Create a new application for retail server:<br/>
    **Click "New Registration"**
   ![image](https://user-images.githubusercontent.com/14832260/189586780-5b6c9fde-01df-4aee-9d02-24bee801b706.png)
   
   **Leave "Redirect URI" as it is, and click "Register" button**
   ![image](https://user-images.githubusercontent.com/14832260/189586955-f2635115-9da7-48c1-b8d5-1aac7eeeb0be.png)
    
    **Click "Application ID URI", and take a note of it for future use.**
    ![image](https://user-images.githubusercontent.com/14832260/189587661-39edb25e-b5ef-411e-922f-317397720f2f.png)
    
    **Click "Add a scope" and provide the necessary information:**
    ![image](https://user-images.githubusercontent.com/14832260/189588259-6d96ece5-1ef4-43a6-be72-729a7b05ec9e.png)
    
    **Finally it looks like this:**
    ![image](https://user-images.githubusercontent.com/14832260/189588545-ed3aa628-e869-4803-b9e9-dcbf1bfe302c.png)
    
3. Create an application for Customized CPOS<br/>
   **Create "New Registration" as you did in step 2 for Retail Server:**<br/>
   **In the "Redirect URI(Optional)", input the POS URL in Channel Profile**<br/>
   **Click "Register" button**<br/>
   ![image](https://user-images.githubusercontent.com/14832260/189590165-fce9e669-3946-4de7-a193-afbb0cb4e68e.png)
    
    **Manifest**
    ![image](https://user-images.githubusercontent.com/14832260/189590864-076ba5aa-2f5d-4e86-9628-dac7b4ea56b8.png)

    **Token configuration**
    ![image](https://user-images.githubusercontent.com/14832260/189591174-6f662d91-5141-4fef-8e91-964b16054154.png)
    ![image](https://user-images.githubusercontent.com/14832260/189591218-3ef45953-92fc-4004-aab4-942cfc11c225.png)
    
    **Api Permissions**<br/>
    **In the right search panel, input the Application Id of Retail Server you created in step 2**
     ![image](https://user-images.githubusercontent.com/14832260/189591552-55524601-88c0-46ff-b02f-a6fbcc0f2459.png)

     **Find the scope we created in step 2**<br/>
     ![image](https://user-images.githubusercontent.com/14832260/189592189-856b7eb3-5218-4740-b963-cea36855d843.png)

     **Finally it looks like this**
     ![image](https://user-images.githubusercontent.com/14832260/189592301-6f59ad91-a988-44b4-82be-7e59e19a1094.png)
     
     **Take a note of Customized  CPOS application id**：
     ![image](https://user-images.githubusercontent.com/14832260/189592866-ff84374d-6813-4b6d-be82-efaf5aefaafe.png)
     
4. **For this part, I did not change that because I am not sure why we need do that:**<br/>
   ![image](https://user-images.githubusercontent.com/14832260/189593613-995e88a4-4848-4318-84cc-e2ca6bd8b632.png)
   **But I can find the config file described here**<br/>
   ![image](https://user-images.githubusercontent.com/14832260/189593878-08f7870c-54ac-4d4e-b9e5-ad3638b90938.png)

5. Commerce Shared Parameter->Identity provider:
   ![image](https://user-images.githubusercontent.com/14832260/189651468-6e0a91b2-8f75-426f-9700-8c9cdba01e65.png)




