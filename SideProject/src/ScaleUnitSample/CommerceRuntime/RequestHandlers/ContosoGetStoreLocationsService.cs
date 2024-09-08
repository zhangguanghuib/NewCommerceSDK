

namespace CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Contoso.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Localization.Services.Messages;
    using SixLabors.ImageSharp.Formats.Bmp;
    using System.IO;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp;
    using CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Data.Types;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;

    public class ContosoGetStoreLocationsService : IRequestHandlerAsync
    {
        private const string OrgUnitLocationViewName = "ORGUNITLOCATIONVIEW";
        private const string OrgUnitQueryAlias = "ou";
        private const string ChannelIdColumn = "CHANNELID";
        private const string RecordIdTableTypeParameter = "@tvp_RecId";

        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(ContosoGetStoreLocationsDataRequest),
                };
            }
        }

        public Task<Response> Execute(Request request)
        {
            ThrowIf.Null(request, nameof(request));

            switch (request)
            {
                case ContosoGetStoreLocationsDataRequest getStoreLocationsDataRequest:
                    return this.GetStoreLocations(getStoreLocationsDataRequest);
                default:
                    throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
            }
        }


        private async Task<Response> GetStoreLocations(ContosoGetStoreLocationsDataRequest request)
        {
            ThrowIf.Null(request, nameof(request));
            ThrowIf.Null(request.QueryResultSettings, nameof(request.QueryResultSettings));

            var currentChannelId = request.RequestContext.GetChannel().RecordId;
            IEnumerable<long> channelIds = new List<long> { currentChannelId };
            IEnumerable <OrgUnitLocation> orgUnitLocations = new List<OrgUnitLocation>();

            var query = new SqlPagedQuery(request.QueryResultSettings)
            {
                DatabaseSchema = "ext",
                From = OrgUnitLocationViewName,
                Alias = OrgUnitQueryAlias,
                Select = new ColumnSet("*"),
                OrderBy = ChannelIdColumn,
            };

            using (var recordIdTableType = new RecordIdTableType(channelIds))
            using (DatabaseContext databaseContext = new DatabaseContext(request.RequestContext, DatabaseConnectionMode.IsReadOnly))
            {
                query.Parameters[RecordIdTableTypeParameter] = recordIdTableType;
                orgUnitLocations = (await databaseContext.ReadEntityAsync<OrgUnitLocation>(query).ConfigureAwait(false)).ToList();
            }

            if (orgUnitLocations.Any())
            {
                channelIds = orgUnitLocations.Select(x => x.ChannelId);
                var getOrgUnitAddressDataRequest = new GetOrgUnitAddressDataRequest(channelIds);
                var addresses = await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<OrgUnitAddress>>(getOrgUnitAddressDataRequest).ConfigureAwait(false);

                orgUnitLocations = orgUnitLocations.GroupJoin(
                    addresses,
                    orgUnitLocation => orgUnitLocation.ChannelId,
                    orgUnitAddress => orgUnitAddress.ChannelId,
                    (orgUnitLocation, orgUnitAddresses) =>
                    {
                        var orgUnitAddress = orgUnitAddresses.FirstOrDefault();
                        if (orgUnitAddress != null)
                        {
                            orgUnitLocation.OrgUnitName = orgUnitAddress.OrgUnitName;
                            orgUnitLocation.Address = orgUnitAddress.FullAddress;
                            orgUnitLocation.Country = orgUnitAddress.ThreeLetterISORegionName;
                            orgUnitLocation.Zip = orgUnitAddress.ZipCode;
                            orgUnitLocation.State = orgUnitAddress.State;
                            orgUnitLocation.StateName = orgUnitAddress.StateName;
                            orgUnitLocation.County = orgUnitAddress.County;
                            orgUnitLocation.CountyName = orgUnitAddress.CountyName;
                            orgUnitLocation.City = orgUnitAddress.City;
                            orgUnitLocation.Street = orgUnitAddress.Street;
                            orgUnitLocation.StreetNumber = orgUnitAddress.StreetNumber;
                            orgUnitLocation.Latitude = orgUnitAddress.Latitude;
                            orgUnitLocation.Longitude = orgUnitAddress.Longitude;
                            orgUnitLocation.BuildingCompliment = orgUnitAddress.BuildingCompliment;
                            orgUnitLocation.Postbox = orgUnitAddress.Postbox;
                            orgUnitLocation.DistrictName = orgUnitAddress.DistrictName;
                            orgUnitLocation.PostalAddressId = orgUnitAddress.RecordId;
                        }

                        return orgUnitLocation;
                    });
            }

            return new EntityDataServiceResponse<OrgUnitLocation>(orgUnitLocations.AsPagedResult());
        }
    }
}
