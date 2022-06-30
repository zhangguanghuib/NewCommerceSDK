# NewCommerceSDK

##New Commerce SDK benifit
   1.   Don't need LCS VM  + Retail SDK,  normal DEV Box(Visual stuido 2017(2019)/Visual Studio code + .Net Core/.Net Frameork) is OK  to develop commerce extension (POS, CRT，Retail Server).
   2.   Any changes in custom logic,  you can nonly build the corresponding extension installer, un-install and re-install the extension installer, not need build the whole retail SDK  and create a Deployable package.   MPOS->MPOS Installer,   RSSU ->RSSU Installer
   3.   The process is simplified.
          Before we need create retail proxy project,  and copy the generated TypeScript Proxy and copy to POS project.
          Now we have one single C# project for CRT and retail server, and add this project as reference to POS  project, the TS  proxy will be generated automatically when build the solution
         
         Before we need manually update commerceruntime.ext.config and web.config to add the extension assembly
         Now we don't need do that.
         .....
         

cd C:\RetailCloudPos\webroot\Extensions

set ExtensionPackageName=GHZ.StoreHoursSample

set AbsolutePathToExtensionPackageProject=C:\NewCommerceSDKRepro\NewCommerceSDK\StoreHoursSample\Pos

mklink /D %ExtensionPackageName% %AbsolutePathToExtensionPackageProject%
