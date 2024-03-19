System.InvalidCastException: '[A]Contoso.Commerce.Runtime.StoreHoursSample.Messages.GetStoreHoursDataRequest cannot be cast to [B]Contoso.Commerce.Runtime.StoreHoursSample.Messages.GetStoreHoursDataRequest. 
Type A originates from 'GHZ.StoreHoursSample.DataModel, Version=9.42.0.0, Culture=neutral, PublicKeyToken=null' in the context 'Default' at location 
'C:\Program Files\Microsoft Dynamics 365\10.0\Commerce Scale Unit\Extensions\ScaleUnit.Sample.Installer\GHZ.StoreHoursSample.DataModel.dll'. 
Type B originates from 'GHZ.StoreHoursSample.DataModel, Version=9.42.0.0, Culture=neutral, PublicKeyToken=null' in the context 'Default' at location 
'C:\Program Files\Microsoft Dynamics 365\10.0\Commerce Scale Unit\Extensions\GHZ.StoreHoursSample.ScaleUnit.Installer\GHZ.StoreHoursSample.DataModel.dll'.'


                        // var requestHandler = new UserAuthenticationTransactionService();
                        var response = await request.RequestContext.Runtime.ExecuteAsync<RS.GetEmployeeIdentityByExternalIdentityRealtimeResponse>(request, request.RequestContext/*, requestHandler*/).ConfigureAwait(false);