# How to debug SQL Server Store Procedure by Visual Studio 2022 : take "Customer Search on POS" as example
## Bakground,  debug SQL Script in SQL Server Management Studio is not availble 
   See in SSMS, there are only <mark>"Execute"</mark> button but no <mark>"Debug"</mark> button:<br/>
   ![image](https://github.com/user-attachments/assets/a7d35d5a-057e-4b9b-843f-f02a476b3b9e)

## Steps to configure to debug SQL Server Store Procedure by Visual Studio 2022
   1. Open Visual Studio 2022 "Continue Without Code"<br/>
       ![image](https://github.com/user-attachments/assets/db474031-cd7f-42e5-bd56-b9659e6422b0)<br/>
    2. Tools->Connect to Database...<br/>
       ![image](https://github.com/user-attachments/assets/e2bcc613-38dd-46f0-bfb9-4db88a2101e6)<br/>
       ![image](https://github.com/user-attachments/assets/fa7d2c63-5fc1-4161-bf10-6b225c5e481e)<br/>
     3. Tools->SQL Server->New QUery<br/>
        ![image](https://github.com/user-attachments/assets/e98b4c56-e1c1-405e-8855-d3f6ffcfc8ec)<br/>
        ![image](https://github.com/user-attachments/assets/23013a64-4325-4074-bb66-03add99cceb7)<br/>
        Click "Connect"<br/>
      4.You can write a simple SQL query to verify if it works<br/>
         ![image](https://github.com/user-attachments/assets/3de1abac-3615-41af-a097-4fe18d9835db)<br/>
      5. How to debug a SQL Store Procedure in Visual Studio?<br/>
  
