# Online Order Print via Hardware Station Sample
## Overview
- This sample implement a function to print the receipt via a dedicated POS hardware station for orders from online or 3rd-party solution. The ideal solution is whenever a new order created from online or 3rd-part, hardware station API can be called from CRT to print the receipt in the even-driven mode,  but it is unlucky at this moment CRT did not support call HWS API directly,  this solution is a workaround:  the idea is to call CRT API from POS periodically to check if there are any new orders whose receipy need print.
- Because of technical limitation, so from design perspective this solution is not good,  but from technical perspective it is valuable because it leverage some advanced JS/TS technology, so it deserves to practise.

## Configuring the sample
- In HQ, create a new POS operation:
  <img width="630" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7f75f1c4-7228-4fab-8575-8903087b85c6">
- Add the operation on the POS screen layout button grid:
  <img width="285" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/410547ec-14d1-4ae2-a6cf-685ea3d3beb4">

## Running the sample

## APIs and extension points used
### "PosApi/Create/Dialogs"


### "PosApi/Create/Views"
- CustomViewControllerBase:

