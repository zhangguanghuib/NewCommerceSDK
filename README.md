## New Commerce SDK benifit
   1.   Don't need LCS VM  + Retail SDK,  normal DEV Box(Visual stuido 2017(2019)/Visual Studio code + .Net Core/.Net Frameork) is OK  to develop commerce extension (POS, CRT，Retail Server).
   2.   Any changes in custom logic,  you can nonly build the corresponding extension installer, un-install and re-install the extension installer, not need build the whole retail SDK  and create a Deployable package.   MPOS->MPOS Installer,   RSSU ->RSSU Installer
   3.   The process is simplified.
          Before we need create retail proxy project,  and copy the generated TypeScript Proxy and copy to POS project.
          Now we have one single C# project for CRT and retail server, and add this project as reference to POS  project, the TS  proxy will be generated automatically when build the solution
         
         Before we need manually update commerceruntime.ext.config and web.config to add the extension assembly
         Now we don't need do that.
         .....
         
## Store hours sample migration
    Before doing the below steps, you can refer to this link to understand how we can make store hours sample work on old retail SDK: 
    https://github.com/zhangguanghuib/POS_Extension/tree/main/StoreHours
    1. Create a repo in you github
    2. Create a solution
    3. Add these C# projects, 
            ChannelDatabase
                 Target Framework: .NET Standard 2.0
                 Output Type     : Class Libarary
                 Nuget Package   : Microsoft.Dynamics.Commerce.Sdk.ChannelDatabase
            CommerceRuntime:
                 Target Framework: .NET Standard 2.0
                 Output Type     : Class Libarary
                 Nuget Package   : Microsoft.Dynamics.Commerce.Sdk.Runtime
            POS:
                 Target Framework: .NET Standard 2.0
                 Output Type     : Class Libarary
                 Nuget Package   : Microsoft.Dynamics.Commerce.Sdk.Pos
                                   knockoutjs
                                   Microsoft.TypeScript.MSBuild
                 Reference Project: CommerceRuntime
            ScaleUnit:
                 Target Framework: .NET Standard 2.0
                 Output Type     : Class Libarary
                 Nuget Package:     Microsoft.Dynamics.Commerce.Sdk.ScaleUnit
                 Reference Project: POS, ChannelDatabase and CommerceRuntime.
            StoreCommerceApp.Installer: 
                 Target Framework:  .Net Framework 4.6.1
                 Output Type:        Console Application, that is exe
                 Reference Projects: POS, ChannelDatabase and CommerceRuntime.
                 Nuget Package   :   Microsoft.Dynamics.Commerce.Sdk.Installers.StoreCommerce
            ScaleUnit.Installer :
                 Target Framework:  .Net Framework 4.6.1
                 Output Type:       Console Application, that is exe
                 Reference Projects: POS, ChannelDatabase and CommerceRuntime
                 Nuget Packages:     Microsoft.Dynamics.Commerce.Sdk.Installers.ScaleUnit
     4. The generated package from ScaleUnit project can be uploaded to LCS  for Cloud-Scale unit update
   
## How to debug CPOS if you have difficulty to install Store Commerce App or Sealed MPOS:     
https://docs.microsoft.com/en-us/dynamics365/commerce/dev-itpro/pos-extension/debug-pos-extension#run-and-debug-cloud-pos
### Steps:
1. cd C:\RetailCloudPos\webroot\Extensions
2. set ExtensionPackageName=GHZ.StoreHoursSample
3. set AbsolutePathToExtensionPackageProject=C:\NewCommerceSDKRepro\NewCommerceSDK\StoreHoursSample\Pos
4. mklink /D %ExtensionPackageName% %AbsolutePathToExtensionPackageProject%

### How to load the POS extension packags:
![image](https://user-images.githubusercontent.com/14832260/176575303-fb7fb06a-f822-4ac1-9a98-4215bf05be1e.png)

###  Where is the generated CSU+CPOS packages:
![image](https://user-images.githubusercontent.com/14832260/176575606-2487f0d4-92c3-4472-87ea-fbc0ae72e365.png)

##  Store Commerce App
1. Install: StoreCommerce.Installer.exe install --Verbosity Trace 
2. Useful link:  https://community.dynamics.com/ax/b/axforretail/posts/introducing-sealed-installers
## Self-Hosted CSU development and debugging
1. Clone this repro https://github.com/microsoft/Dynamics365Commerce.ScaleUnit
2. Copy the below folder to your own repro:
    ![image](https://user-images.githubusercontent.com/14832260/176613056-d69c8baf-6d71-43b2-b924-b13d185335e4.png)
3. Update the Install.ps1:
    ![image](https://user-images.githubusercontent.com/14832260/176613281-67a628ba-e1c2-4a4c-81c9-f1e71aa62b72.png)
4. Put the OOB  Scale Unit Installer under the download folder:
5. Press F5 to debug
6. POSTMAN to test the API:
       
       Get Store Hours API
           * url: http://localhost:1802/Commerce/StoreHours/GetStoreDaysByStore?$top=250&$count=true&api-version=7.3
           * header: oun 052
           * Body: {"StoreNumber":"HOUSTON"}
           * Method: POST
   
       Update store hours API:
           * url: http://localhost:1802/Commerce/StoreHours(3)/UpdateStoreDayHours?api-version=7.3
           * header: oun 052
           * Body: {"storeDayHours":{"DayOfWeek":2,"OpenTime":28800,"CloseTime":54800,"Id":2}}
           * Method: Post
   
   You can see the breakpoint is hit:
   ![image](https://user-images.githubusercontent.com/14832260/176614996-53734607-f98e-4b1b-a10e-99cfaa0503cb.png)
