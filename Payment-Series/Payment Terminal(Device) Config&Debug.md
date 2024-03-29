## This document will show how to config and debug payment terminal (device code) in Store Commerce App.

Steps：
1. Create a POS Hardware Profile like below, set the PIN Pad Device Name as <b>MockPaymentTerminal</b>
    ![image](https://user-images.githubusercontent.com/14832260/213173399-4a1d7b6c-8426-492c-8738-e5ca96c93662.png)
 2. Attach the Hardware Profile to Register:
    ![image](https://user-images.githubusercontent.com/14832260/213174231-77f3d07d-05e7-4544-a8a9-e081dad8ebec.png)
 3. Find the Hardware station project from here, the code is totally from Retail SDK, I did not any changes.
    https://github.com/zhangguanghuib/NewCommerceSDK/tree/release/v9.36/BarcodeMsrDialogSample/RetailSDK_HardwareStation <br/>
    Make sure the below code is like below:
    ![image](https://user-images.githubusercontent.com/14832260/213175167-9bf00c0a-8cb0-4d1b-85c3-e3325ef79ee2.png)
 4. Build the Store Commerce Installer project to generate the Store Commerce Extension Package Installer,
    ![image](https://user-images.githubusercontent.com/14832260/213175669-de7e3cd6-3812-45ee-bffa-fb30bab68a47.png) 
    Install the package
  5. Install Peripheral Simulator for Retail, cofigure a Credit Card like below:
     ![image](https://user-images.githubusercontent.com/14832260/213176446-c3f99e40-dd08-480f-9a1a-37cc41242597.png)
  6. Log into Store Commerce App and paired the dedicated hardware station
     ![image](https://user-images.githubusercontent.com/14832260/213176847-29d8e7ed-2f68-4a2b-affd-8042409f71c2.png)
  7.  Add some product into card, and click pay card, and go to payment view:
      ![image](https://user-images.githubusercontent.com/14832260/213177583-b602487f-f263-497b-b37b-b3bbb3cbc28c.png)
      
      Please make sure it is Swipe Card
   8. CLick "Tender Payment" button, and it will show "Waiting for customer input":
      ![image](https://user-images.githubusercontent.com/14832260/213178063-bc240488-dc62-46ee-ae03-2c8509b3d80f.png)
   9. Go to simulator, and swipe card:
      ![image](https://user-images.githubusercontent.com/14832260/213178518-87de6808-df5f-48aa-9c62-a873fbb9945c.png)
      
   10. Sign your name and click "Enter":
      ![image](https://user-images.githubusercontent.com/14832260/213178755-0b8f9200-ce20-4835-a57f-99063df6a62e.png)
   11. Go back to Store Commerce, you will see the check out successfully.
      ![image](https://user-images.githubusercontent.com/14832260/213178915-1d845a28-a521-4160-a622-813b39ede0d4.png)
    
   12.  During this process, you can attach Visual Studio to Store Commerce Process to debug the payment terminal code:
        ![image](https://user-images.githubusercontent.com/14832260/213179716-367ccba7-574b-4ac4-a3c6-091ba5ccdbcf.png)
        ![image](https://user-images.githubusercontent.com/14832260/213180049-82a7b12c-ff26-4702-b748-dc590a37dd05.png)
        
        You can see authorize payment and capture payment methods are both called


   





 

  


    


