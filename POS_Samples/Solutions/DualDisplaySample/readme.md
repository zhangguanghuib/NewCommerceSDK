# Dual Display Sample
## Overview
This sample showcases a sample where the Store Commerce App is extended to show more information on dual display.

## Running the sample
- Open the Developer Command Prompt for Visual Studio 2022
- Restore the nuget packages for the solution by running "nuget restore DualDisplaySample.sln"
- Initialize Store Commerce development for the solution by running "msbuild DualDisplaySample.sln /t:InitDev"
- Open Visual Studio Code in the solution root directory
- Build the solution using the "Build & Install Store Commerce Extension" task in VSCode
- Open PowerShell as Admin in the ScaleUnit.Installer bin directory & run ".\Contoso.DualDisplaySample.ScaleUnit.Installer.exe install"
- Open the "Run & Debug Tab" in VSCode and use the "Debug Store Commerce" option to launch Store Commerce app with the debugger attached.
- Sign in to Store Commerce.
- Navigate to the transaction page and add a product to the cart.

> For dual screen mode to work, two physical displays must be connected to the device.

## Debugging the sample
To find your custom code in the debugger, you should use Developer tools (F12) in the second Store Commerce window. In the main window, the dual display extensions will not be visible.
![Example of debug window](DebugWindow.png)

## Enhancement:
- Add a new class to track the selected cart lines:
  <img width="847" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/3be99cc0-9775-4637-97d5-7d520e77fee5">

- Whenever cart changes, Dual Display will scroll to the same cart lines to match the main cart screen:
  <img width="1101" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/5fa7c200-93dc-4f52-992a-6d0822bcce9e">


