## Utilize Extension Properties to Store Custom Data and Serialize it

## Background 
In Some Scenarios, we may need store custom data into the standard data entity,  then we can utilize the ExtensionProperties. All data entities that extends CommerceEntity has this propertity, it is an array of CommereProperty which has a key and Value, we can do a lot of things through ExtensionProperties.

Assume a scenario: client ordered a furniture, then want to set the installation date, but the standard function don't have that. In this scenario, we can use ExtensionProperties to archieve that.

## Finally effect:
. Create a customer order.<br/>
. Choose a cart line<br/>
. Click the button on POS:<br/>
  <img width="1068" alt="image" src="https://github.com/user-attachments/assets/9084793f-4504-447e-a838-c2d94927bf8d" /><br>
. Choose a date as installation date, click "OK":<br/>
  <img width="254" alt="image" src="https://github.com/user-attachments/assets/5c31f3b9-5d84-4044-ae3a-60e11adf91af" /> <br/>
. Go to Customer Order Delivery Grid Tab:<br/>
  <img width="607" alt="image" src="https://github.com/user-attachments/assets/41da8a95-c5dc-487e-b69a-baa2a5750867" /><br/>
. Choose a Shipping Method, and then check out the cart.<br/>
. Go to F&O, you will find the installation date for this order line has been recorded into the system.<br/>
  <img width="368" alt="image" src="https://github.com/user-attachments/assets/7c8d7b0a-f25b-43e7-a128-8c591ede80bd" /><br/>





