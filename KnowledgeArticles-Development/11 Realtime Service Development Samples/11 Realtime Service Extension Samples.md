## Table of Contents
- [Topic](#topic)
- [Background](#Background)
- [RetailServer read the data from F&O Database table and render the date on POS view ](#RetailServer read the data from F&O Database table and render the data on POS view )

## Topic
Deep Dive D365 Commerce Realtime Service:  from single value communication to array communication.

## Background 
Real-time Service enables clients to interact with Commerce functionality in real time. Finance and Operation databases and classes can’t be accessed directly from Retail server. You should access them through the CDX class extension using the finance and operations and Commerce Runtime extension, see [Commerce Data Exchange - Real-time Service](https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/extend-commerce-data-exchange)

But it is unfortunately, the samples provided in the official document is too simple,  it only demonstrates one single string value communicatation scenario, actually in real world,  the scenario is much more complex, Object Array is a very Commmon Scenario.

In this article,  I will demonstration two Realtime Service Scenario:<br/>
Scenario #1：RetailServer read the data from F&O Database table and render the date on POS view.<br/>
Scenario #2: Create a new record in F&O database table,  and show the record in FO form, grid view.<br/>

## RetailServer read the data from F&O Database table and render the data on POS view 
### Let us see the final effect:
In F&O,  there is a form to show for each shipping method,  each day's max shipping slot and free slot:<Br/>
![image](https://github.com/user-attachments/assets/0556ccab-e899-41dd-944e-28e93a5f02b1)
<br/>
The data will be showing on the POS  Shipping View for different Shipping Method, then user can choose a proper data to ship.
![image](https://github.com/user-attachments/assets/3bf7af9a-cd89-474d-99c0-d28713ea471d)
### How to implement it?
. Step 1, in F&O we should have a table and form for user to input the delivery mode booking slot:
   <img width="275" alt="image" src="https://github.com/user-attachments/assets/ee7579b4-bdaa-4498-a3b4-9d81ec64f7fa" /><br/>
   ![image](https://github.com/user-attachments/assets/c89922d1-df75-4118-9d5a-f3b7238cf352)<br/>
