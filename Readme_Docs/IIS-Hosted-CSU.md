This document is to demonstrate how to configure an IIS-Hosted CSU environment for new commerce SDK development.<br/>
All steps comes from these two link, the author of these both are Sergey Pikhulya from Microcrosoft Commerce Product Group<br/>
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
   
6. **Create application for async client:**
   ![image](https://user-images.githubusercontent.com/14832260/189651808-ea108b19-a3e9-4374-904e-091da445f828.png)
   **Then click Application ID URI:**
   ![image](https://user-images.githubusercontent.com/14832260/189652042-acd98a6d-fda3-45be-a35d-e866b8c9903a.png)
   **Click add a scope:**
   ![image](https://user-images.githubusercontent.com/14832260/189652319-5d47dca2-6e77-4997-82f4-98823f0d6bef.png)
   **Give a scope name:**
   ![image](https://user-images.githubusercontent.com/14832260/189652571-93eb1770-a9c5-4fff-ad97-931c15c15e73.png)
   **Finally it looks like that:**
   ![image](https://user-images.githubusercontent.com/14832260/189652817-da612d22-ea2f-4fca-b042-631165542ffd.png)
   
7. **Register customized retail server and customized async client id to D365 HQ:<br/>**
   **Go to System administration > Setup > Azure Active Directory applications.<br/>**
   **Enter the application ID (client ID) in the Client ID column,<br/>**
   **Enter descriptive text in the Name column, <br/>**
   **And enter RetailServiceAccount in the User ID column<br/>**
   ![image](https://user-images.githubusercontent.com/14832260/189659229-70e75964-29e3-48c1-b05a-ab9d9427c5dd.png)
   
 8. Create certificate:
    ![image](https://user-images.githubusercontent.com/14832260/189659654-5eaf0d8e-a70d-49f7-a266-82d1a6e8e465.png)
    ![image](https://user-images.githubusercontent.com/14832260/189659870-30ace21f-ce50-4a03-ba11-b298b01eb435.png)
    
 9. Export the certificate:<br/>
    **The steps are as below:**
    ![image](https://user-images.githubusercontent.com/14832260/189660500-505c826d-87c7-4c24-acfe-62b30a594778.png)
    
    ![image](https://user-images.githubusercontent.com/14832260/189660535-e06d2c3a-a692-453f-8b57-fb987826197d.png)
    
    ![image](https://user-images.githubusercontent.com/14832260/189660653-fa08f500-6b74-4ba1-9ff5-93f596d29fe7.png)
    
    ![image](https://user-images.githubusercontent.com/14832260/189660725-7f4d3d07-9a27-49cd-972c-631ca302fbcd.png)
    
    ![image](https://user-images.githubusercontent.com/14832260/189660856-00807575-4fd0-4346-bfe6-2c0e644ba5f1.png)
    
    ![image](https://user-images.githubusercontent.com/14832260/189660939-a8f9cd2d-270e-424c-8b57-38b98ffa3324.png)
    
    **Double click "Personal"**
    ![image](https://user-images.githubusercontent.com/14832260/189662784-81b572ed-a7dd-43fd-857a-26ebf11ffc82.png)
    
    Find the self-signed certificate you created in the step 8, and choose All Tasks->Export
    ![image](https://user-images.githubusercontent.com/14832260/189673757-a769d446-bcb5-4c3d-8d94-a87028327172.png)
    
    ![image](https://user-images.githubusercontent.com/14832260/189664591-f7f85247-ff94-4bef-a5e7-dec17b669842.png)
    
    ![image](https://user-images.githubusercontent.com/14832260/189664682-52ab6196-ad12-44f9-b535-603f06c9e3c6.png)
    
    Choose a location to save the certificate:
    ![image](https://user-images.githubusercontent.com/14832260/189664837-3e5a22aa-bdf3-4470-aeef-d99e413bc555.png)
    
    Click OK, it should shows Export Successfully.
    
 10. Upload the certificate to the Azure Portal for Customized Cloud Retail Server and Customized Async Client:<br/>
        **Steps**<br/>
        **Go to Azure portal, find Customized Retail  Server->Certificates and upload the certificates we created before:**
        ![image](https://user-images.githubusercontent.com/14832260/189666028-4d09e58a-47fc-4cc4-9248-dfa6351b3cd7.png)
        
        Finally it looks this this:
        ![image](https://user-images.githubusercontent.com/14832260/189666806-712d7081-c2c1-4f0a-b3e6-c272305a6e8f.png)
        
        Do the same thing for customized async client:
        ![image](https://user-images.githubusercontent.com/14832260/189667020-b5ecb3d6-abd9-4c31-b2bb-087d6907f8a6.png)
        
 11. Create channel database in D365 HQ:<br/>
        Please follow the document [Create Channel Database](https://docs.microsoft.com/en-us/dynamics365/commerce/dev-itpro/retail-store-scale-unit-configuration-installation#download-the-commerce-scale-unit-installer)<br/>
        
        ![image](https://user-images.githubusercontent.com/14832260/189668114-580c1eff-6055-4215-9401-6dadd55b411b.png)
        
        **Create Channel profile**
        ![image](https://user-images.githubusercontent.com/14832260/189672458-e8e41537-41ad-4dd8-8f57-816f23742675.png)
        
        **Download channel database config file**
        ![image](https://user-images.githubusercontent.com/14832260/189668539-67cb8794-5758-4fe9-9968-d0770b4fae05.png)
        
12. Download sealed version scale unit from LCS Shared Asset Library, and put it under some foldder in your local dev box
    [Shared Asset Library](https://lcs.dynamics.com/V2/SharedAssetLibrary)
    ![image](https://user-images.githubusercontent.com/14832260/189669934-3db1a3c7-6d36-4551-994b-cdea323e0195.png)
    
14. Run the below command to install the sealed CSU in your local dev box:
    **CommerceStoreScaleUnitSetup.exe install --port 446 --SslCertFullPath "store:///My/LocalMachine?FindByThumbprint=<%CerticateThumbPrint%>" --AsyncClientCertFullPath "store:///My/LocalMachine?FindByThumbprint=<%CerticateThumbPrint%>" --RetailServerCertFullPath "store:///My/LocalMachine?FindByThumbprint=<%CerticateThumbPrint%>" --RetailServerAadClientId "<Customized Retail Server ClientId>" --RetailServerAadResourceId "<Application ID URI of Customized Retail Server>" --CposAadClientId "<Customized CPOS ClientId>"  --AsyncClientAadClientId "<Customized Async ClientId>" --config "D:\IISHosted_CSU\StoreSystemSetup.xml" --SqlServerName "(localhost)\SQLEXPRESS" --TrustSqlServerCertificate true**<br/>
   
  **If installation can be done without any error, you should be able to see the retail server and CPOS under IIS**<br/>
   ![image](https://user-images.githubusercontent.com/14832260/189671430-74cc8033-b039-4086-b23b-7993e758db60.png)
   
15. Run 9999 full sync on the channel database,  when all download sessions are applied, you can go ahead to activate CPOS, and make your Commerce SDK development in your local box and without depending on the Retail SDK and the DEV box from LCS.

  


        




    
    














