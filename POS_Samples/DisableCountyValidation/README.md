# This sample is going to showcase how to support county validation disabled/enabled

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
