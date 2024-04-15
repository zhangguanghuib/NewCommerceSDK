## How to pave a Commerce SDK especially Store Commerce development environment?

1.Background<br/>
   * Apparently developer engineer have to prepare a Commerce SDK development environment before you start extension development work like coding,  deploy, debugging.
   * This document to contains the basic steps of Commerce SDK development environment preparation, some corner scenarios like network/certificate/TLS that need open support ticket for trouble shooting,  so we can not cover all the details, but it is approved if you follow this document, most of develper can make commerce SDK  environment ready.
   * The basic steps contains:
       + System preparation
       + Sealed CSU  and Store Commerce Installation
       + Software Installation
       + Github Repro clone
       + Build Sample to get extension installer
       + Deploy installer
       + Verify the extension function and debugging for trouble shooting

2. Steps to pave a Commerce SDK development environment<br/>
   2.1. System preparation
       + This link contains very detailed and useful information https://github.com/microsoft/Dynamics365Commerce.ScaleUnit/tree/release/9.45/src/ScaleUnitSample#shared-across-self-hosted-and-iis-hosted-modes
       + Some point my preferrence are:
           + Prefer Visual Studio 2022  instead of Visual Studio code
           + Install .Net Framework 472 to support Store Commerce Extension  Installer and Scale Unit Extension Installer
           + Install .Net 6.0 insteal of .Net 3.1:  https://dotnet.microsoft.com/en-us/download/dotnet/6.0
           + Install Hosting Bundle from same page<br/>
           <img width="502" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7540e0d1-280c-4875-ba9a-ad0777f57128"><br/>
           + Install SQL Server(or SQL server Express) please do remember mix-authentication and enable full-text search<br/>
               ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f1094e7b-5238-4305-b2ae-72b75112bef3)<br/>
           + IIS, just follow this to enable it:<br/>
             ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/37f38d6e-6ad1-459b-89d0-fc45f79d11cd)
2.2. Install Sealed Version CSU:<br/>
       * Please follow this below link that is veried worked for many people: <br/>
           + https://github.com/zhangguanghuib/NewCommerceSDK/blob/main/Readme_Docs/IIS-Hosted-CSU-SingleApp.md <br/>
       * Official document is here:<br/>
           + https://community.dynamics.com/blogs/?redirectedFrom=https://community.dynamics.com/ax/b/axforretail/posts/introducing-sealed-installers
           + https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/install-csu-dev-env <br/>
2.3. Install Store Commerce Application<br/>
      * Install Microsoft Edge WebView2, download from:
          + https://developer.microsoft.com/en-us/microsoft-edge/webview2/?form=MA13LH
      * Download Store Commerce Installer and run this command to install it:
          + StoreCommerce.Installer.exe install --enablewebviewdevtools --trustsqlservercertificate --installoffline --SqlServerName .\sqlexpress
          + (if for CPOS) StoreCommerce.Installer.exe install --enablewebviewdevtools --useremoteappcontent
      * Mode useful command can be found from here:
          + https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/enhanced-mass-deployment
      Till now the development environment should be ready for your development work.    <br/>

3. Start your development work:<br/>
    3.1. There are two github repros need clone for a very a good starting point:<br/>      
        + https://github.com/microsoft/Dynamics365Commerce.ScaleUnit
        + https://github.com/microsoft/Dynamics365Commerce.InStore
        
