# Online Order Print via Hardware Station Sample
## Overview
- This sample implement a function to print the receipt via a dedicated POS hardware station for orders from online or 3rd-party solution. The ideal solution is whenever a new order created from online or 3rd-part, hardware station API can be called from CRT to print the receipt in the even-driven mode,  but it is unlucky at this moment CRT did not support call HWS API directly,  this solution is a workaround:  the idea is to call CRT API from POS periodically to check if there are any new orders whose receipy need print.
- Because of technical limitation, so from design perspective this solution is not good,  but from technical perspective it is valuable because it leverage some advanced JS/TS technology, so it deserves to practise.

## Configuring the sample

## Running the sample

## APIs and extension points used
### "PosApi/Create/Dialogs"


### "PosApi/Create/Views"
- CustomViewControllerBase:

