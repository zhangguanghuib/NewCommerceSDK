# This document is to demonstrate Self-Hosted CSU deployment.

- ## The more official document can be found from here https://github.com/microsoft/Dynamics365Commerce.ScaleUnit/tree/release/9.50/src/ScaleUnitSample.

The steps:
1. Clone https://github.com/microsoft/Dynamics365Commerce.ScaleUnit
   ```
   git clone https://github.com/microsoft/Dynamics365Commerce.ScaleUnit.git 
   git checkout release/9.50
   ```
1. Install 64 bit version of VS Code for Windows from https://code.visualstudio.com/download
1. Install *.Net Core SDK 6.0* for Windows x64 from https://dotnet.microsoft.com/download/dotnet/6.0.
1. Install *.NET Framework 4.7.2 Developer pack* from https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net472-developer-pack-offline-installer.
1. Install the *Hosting Bundle* (click literally "Hosting Bundle" link, not "x64" nor "x86") for Windows from the [link](https://dotnet.microsoft.com/download/dotnet/6.0).
   <img width="999" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ccdbcc1d-9373-4f47-ad78-c69523d9326d">
1. Navigate to https://lcs.dynamics.com/V2/SharedAssetLibrary select the section *Retail Self-service package files* and then locate there the file ending with *Commerce Scale Unit (SEALED)*. Make sure to select there the version for the release you need, for instance 10.0.22, 10.0.23 and so on. Download the file and place it in the folder [Download](./Download)
<img width="939" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/98955c0e-017a-4673-a886-266faceea7ea">
<img width="617" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c3989786-2fe4-4bfb-8644-b50040882e1a"><br/>
7. You machine should also install the SQL Server(or SQL express), and make sure it is mixed-authentication mode:
   <img width="829" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9f010d1b-7cbb-4da7-92cd-b9634b7863d9"><br/>
8. Launch Visual Studio Code as Administrator, and then open this folder:<br/>
   <img width="517" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/291adf64-1af9-4f54-9eca-c07248d69255"><br/>
   <img width="1100" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/dba69245-43be-45db-82a9-9cb865fd43d7"><br/>
   If you installed SQL Express, you may need change this:<br/>
   <img width="1081" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/6904244d-f3a1-4a74-a25c-61082b09e9f5"><br/>













        




    
    














