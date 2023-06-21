# This sample is going to showcase how to support county validation disabled/enabled

Precondition:

Check the dbo.LOGISTICSADDRESSPARAMETERS, it has a field DISABLECOUNTYVALIDATION, but channel database ax.LOGISTICSADDRESSPARAMETERS does not have this field
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/5cdca57a-fc66-485d-ba37-e3cb40eb6627)

Step 1:  Create a table 
```sql
CREATE TABLE [ext].[LOGISTICSADDRESSPARAMETERSExt](
	[RECID] [bigint] NOT NULL,
	[KEY] [int] NULL,
	[DISABLECOUNTYVALIDATION] [int] NOT NULL,
 CONSTRAINT [I_LOGISTICSADDRESSPARAMETERS_RECID] PRIMARY KEY CLUSTERED 
(
	[RECID] ASC
)
```

Step 2:  Config CDX to push the column/field in existing table(dbo.LOGISTICSADDRESSPARAMETERS) to extension table([ext].[LOGISTICSADDRESSPARAMETERSExt])
1. Create a new channel table in this form:
<img width="789" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/a22d6e6d-0988-4670-88cc-60027b7f67c6">
2. Create a scheduler subjob:
   <img width="1708" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/2bf6ff5d-b53c-4256-b94d-1c5dce08abf8">
3. Create a scheduler job
   <img width="1254" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/6114cdc0-f0aa-4767-9d10-1ad378dfca68">
4. Create distribution scheduler:
   <img width="1133" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/a8acdd53-1da6-40d5-89fe-19e6399c7f3d">



