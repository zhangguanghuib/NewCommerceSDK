## This document will show how to config and debug payment terminal (device code) in Store Commerce App.

Steps：
1. Create a POS Hardware Profile like below, set the PIN Pad Device Name as <b>MockPaymentTerminal</b>
    ![image](https://user-images.githubusercontent.com/14832260/213173399-4a1d7b6c-8426-492c-8738-e5ca96c93662.png)
 2. Attach the Hardware Profile to Register:
    ![image](https://user-images.githubusercontent.com/14832260/213174231-77f3d07d-05e7-4544-a8a9-e081dad8ebec.png)
 3. Find the Hardware station project from here 
    https://github.com/zhangguanghuib/NewCommerceSDK/tree/release/v9.36/BarcodeMsrDialogSample/RetailSDK_HardwareStation <br/>
    Make sure the below code is like below:
    ![image](https://user-images.githubusercontent.com/14832260/213175167-9bf00c0a-8cb0-4d1b-85c3-e3325ef79ee2.png)
 4. Build the Store Commerce Installer project to generate the Store Commerce Extension Package Installer,
    ![image](https://user-images.githubusercontent.com/14832260/213175669-de7e3cd6-3812-45ee-bffa-fb30bab68a47.png) 
    Install the package
  5. Install Peripheral Simulator for Retail, cofigure a Credit Card like below:
     ![image](https://user-images.githubusercontent.com/14832260/213176446-c3f99e40-dd08-480f-9a1a-37cc41242597.png)



 

  


    


