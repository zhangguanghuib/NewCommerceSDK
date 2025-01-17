
 select * from dbo.RETAILDEVICE as T where T.DeviceId ='300801' --RetailDevice
 
 select T.STORERECID, * from dbo.RetailTerminalTable as T where T.terminalId = '300801' -- From RetailDevice to RetailTerminal
 
 select * from dbo.RETAILCHANNELTABLE as T where T.RecId = 5637146251 --from Terminal to RetailChannelTable
 
 select T.CHANNEL, T.CHANNELPROFILE, T.LIVECHANNELDATABASE, * from dbo.RetailChannelTableExt as T where T.Channel   = 5637146251 -- RetailChannelTableExt
 
 select T.Name, * from dbo.RetailConnDatabaseProfile as T where T.RecId = 5637144576 -- from Channale to Channel database
 
 select * from dbo.RetailChannelProfile as T where T.RecId = 5637144576 -- Get Channel Profile
 
 select T.KEY_, T.VALUE, * from dbo.RetailChannelProfileProperty as T where T.ChannelProfile = 5637144576 -- Get Channel profile line

