![image](https://github.com/user-attachments/assets/4c8abd36-7b64-4f3f-b359-3c81c3d0402e)# How to debug SQL Server Store Procedure by Visual Studio 2022 : take "Customer Search on POS" as example
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
      5. Capture the SQL from SQL Profiler<br/>
          Open SQL profiler<br/>
          ![image](https://github.com/user-attachments/assets/a4676add-4836-4911-b38b-3f0f23a279c9)<br/>
          Then choose "Commerce Profiler":<br/>
          ![image](https://github.com/user-attachments/assets/351f04b4-3e35-449d-9d71-e00a819b6c42)<br/>
          Click OK<br/>

          Open Store Commerce, input the Customer Search Key words:<br/>
         ![image](https://github.com/user-attachments/assets/878c11b5-c4b4-4c9e-a542-a762ddf8be31)
      6. When POS Customer Search is done, In Sql Profiler:<br/>
         ![image](https://github.com/user-attachments/assets/3f7997f9-6225-4e9c-9161-636d21e56c86)<br/>

      7.  Stop Trace and then Search "GETCUSTOMERSEARCHRESULTSBYFIELDS": <br/>
           ![image](https://github.com/user-attachments/assets/8005bfcc-b56e-4d25-b7d6-b43f30f222da)<br/>
          That is the SQL we debug as the starting point<br/>
```
declare @p1 crt.CUSTOMERSEARCHBYFIELDCRITERIATABLETYPE
insert into @p1 values(N'Name',N'"Contoso*"',0)

declare @p6 crt.QUERYRESULTSETTINGSTABLETYPE
insert into @p6 values(0,81,0,N'',1)

exec [crt].GETCUSTOMERSEARCHRESULTSBYFIELDS @tvp_CustomerSearchByFieldCriteria=@p1,@bi_ChannelId=5637144592,@nvc_DataAreaId=N'usrt',@i_MaxTop=2147483647,@i_MinCharsForWildcardEmailSearch=7,@TVP_QUERYRESULTSETTINGS=@p6
```
           


  
