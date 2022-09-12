This document is to demonstrate how to configure an IIS-Hosted CSU environment for new commerce SDK development.<br/>
All steps comes from these two link:<br/>
[How to configure CPOS to use your own Azure AD application](https://community.dynamics.com/ax/b/axforretail/posts/how-to-point-cpos-to-use-your-own-azure-ad-application)<br>
[Introduction to Commerce Sealed Installers](https://community.dynamics.com/ax/b/axforretail/posts/introducing-sealed-installers)<br/>

Steps:<br/>
1. Open [Azure portal](https://aad.portal.azure.com/)<br/>
2. Create a new application for retail server:<br/>
   <u>Click "New Registration"</u>
   ![image](https://user-images.githubusercontent.com/14832260/189586780-5b6c9fde-01df-4aee-9d02-24bee801b706.png)
   
   Leave "Redirect URI" as it is, and click "Register" button
   ![image](https://user-images.githubusercontent.com/14832260/189586955-f2635115-9da7-48c1-b8d5-1aac7eeeb0be.png)
    
    Click "Application ID URI", and take a note of it for future use.
    ![image](https://user-images.githubusercontent.com/14832260/189587661-39edb25e-b5ef-411e-922f-317397720f2f.png)
    
    Click "Add a scope" and provide the necessary information:
    ![image](https://user-images.githubusercontent.com/14832260/189588259-6d96ece5-1ef4-43a6-be72-729a7b05ec9e.png)
    
    Finally it looks like this:
    ![image](https://user-images.githubusercontent.com/14832260/189588545-ed3aa628-e869-4803-b9e9-dcbf1bfe302c.png)


