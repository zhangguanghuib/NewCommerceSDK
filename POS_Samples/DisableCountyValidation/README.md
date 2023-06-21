# This sample is going to showcase how to support county validation disabled/enabled

## Precondition:

Check the dbo.LOGISTICSADDRESSPARAMETERS, it has a field DISABLECOUNTYVALIDATION,<br/> 
but channel database ax.LOGISTICSADDRESSPARAMETERS does not have this field
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/5cdca57a-fc66-485d-ba37-e3cb40eb6627)

Our purpose is to push the data in dbo.LOGISTICSADDRESSPARAMETERS.DISABLECOUNTYVALIDATION to an extension table since the standard table does not support it
## Step 1:  Create a table 
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

## Step 2:  Config CDX to push the column/field in existing table(dbo.LOGISTICSADDRESSPARAMETERS) to extension table([ext].[LOGISTICSADDRESSPARAMETERSExt])
1. Create a new channel table in this form:
<img width="789" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/a22d6e6d-0988-4670-88cc-60027b7f67c6"><br/>
2. Create a scheduler subjob:
   <img width="1708" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/2bf6ff5d-b53c-4256-b94d-1c5dce08abf8">
3. Create a scheduler job
   <img width="1254" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/6114cdc0-f0aa-4767-9d10-1ad378dfca68">
4. Create distribution scheduler:
   <img width="1133" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/a8acdd53-1da6-40d5-89fe-19e6399c7f3d">

## Step 3, run the new job, and you can see the download session can be applied:
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/13623bf9-a202-4853-9048-1789900fa338)

Finally you see the data have been pushed to channel database:
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/25594c36-972a-43f9-bfef-a9b7757f77e6)

## Step 4, in the C# code,  we will call own own store procedure to support county validation enabled or not
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/b3b981d2-1c79-4ddd-a1e5-0918dbec1b6b)

## Step 5， In HQ X++ code,  we made this customization to support that as well:
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9c549327-3af4-4ca0-b10a-d0ee06463555)

## All the code list here:
SQL:<br/>
https://github.com/zhangguanghuib/NewCommerceSDK/blob/main/POS_Samples/DisableCountyValidation/ChannelDatabase/LOGISTICSADDRESSPARAMETERSExt.sql<br/>
https://github.com/zhangguanghuib/NewCommerceSDK/blob/main/POS_Samples/DisableCountyValidation/ChannelDatabase/VALIDATEADDRESSExt.sql<br/>
CRT:<br/>
https://github.com/zhangguanghuib/NewCommerceSDK/blob/main/POS_Samples/DisableCountyValidation/CommerceRuntime/ValidateAddressRequestHandler.cs<br/>
X++<br/>
https://github.com/zhangguanghuib/NewCommerceSDK/blob/main/POS_Samples/DisableCountyValidation/Xpp_RTS/LogisticsPostalAddressEntity1_Extension.xml<br/>
