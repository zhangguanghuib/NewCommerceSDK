using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime;
using System;
using System.Threading.Tasks;
using Microsoft.Dynamics.Commerce.Runtime.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Dynamics.Commerce.Runtime.Extensions;

namespace Contoso.GasStationSample.CommerceRuntime
{
    public class GetChannelTenderTypesDataRequestHandler : SingleAsyncRequestHandler<GetChannelTenderTypesDataRequest>
    {
        private const string SqlParamChannelId = "@channelId";
        private const string SqlParamCountingRequired = "@CountingRequired";
        private const string ChannelTenderTypeViewName = "[CHANNELTENDERTYPEVIEW]";
        //private const string RecIdColumn = "RECID";
        private const int MaxCachedCollectionSize = 500;
        private const string ReadChannelDataFromReplica = "ChannelService.ReadFromReplica";

        [SuppressMessage("Security", "PreventSqlInjectionAnalyzer", Justification = "False positive, query parameters are used correctly.")]
        protected override async Task<Response> Process(GetChannelTenderTypesDataRequest request)
        {
            ThrowIf.Null(request, nameof(request));
            ThrowIf.Null(request.QueryResultSettings, "request.QueryResultSettings");

            ChannelL2CacheDataStoreAccessor level2CacheDataAccessor = this.GetChannelL2CacheDataStoreAccessor(request.RequestContext);
            bool found;
            bool updateL2Cache;
            PagedResult<TenderType> result = DataManager.GetDataFromCache(() => level2CacheDataAccessor.GetChannelTenderTypes(request.ChannelId, request.CountingRequired, request.QueryResultSettings), out found, out updateL2Cache);

            if (!found)
            {
                Stopwatch processTimer = Stopwatch.StartNew();

                var query = new SqlPagedQuery(request.QueryResultSettings);
                string whereClause = string.Format("CHANNEL={0}", SqlParamChannelId);

                query.Parameters[SqlParamChannelId] = request.ChannelId;

                if (request.CountingRequired != null)
                {
                    query.Parameters[SqlParamCountingRequired] = request.CountingRequired == null ? null : (int?)Convert.ToInt32(request.CountingRequired.Value);
                    whereClause = string.Format("CHANNEL={0} AND COUNTINGREQUIRED={1}", SqlParamChannelId, SqlParamCountingRequired);
                }

                query.From = ChannelTenderTypeViewName;
                query.DatabaseSchema = "ext";
                query.Where = whereClause;
                query.IsQueryByPrimaryKey = false;
                // query.OrderBy = RecIdColumn;

                SortColumn sortColumnOperationId = new SortColumn("OPERATIONID", false);
                SortColumn sortColumnDisplayOrder = new SortColumn("DISPLAYORDER", false);
                query.OrderBy = new SortingInfo(sortColumnOperationId, sortColumnDisplayOrder).ToString();

                using (DatabaseContext databaseContext = new DatabaseContext(request.RequestContext, DatabaseConnectionMode.IsReadOnly | ReadFromReplicaIfEnabled(request.RequestContext)))
                {
                    result = await databaseContext.ReadEntityAsync<TenderType>(query).ConfigureAwait(false);
                }

                processTimer.Stop();

                updateL2Cache &= result != null
                                 && result.Results.Count < MaxCachedCollectionSize;
            }

            if (updateL2Cache)
            {
                level2CacheDataAccessor.PutChannelTenderTypes(request.ChannelId, request.CountingRequired, request.QueryResultSettings, result);
            }

            return new EntityDataServiceResponse<TenderType>(result);
        }

        private ChannelL2CacheDataStoreAccessor GetChannelL2CacheDataStoreAccessor(RequestContext context)
        {
            DataStoreManager.InstantiateDataStoreManager(context);
            return new ChannelL2CacheDataStoreAccessor(DataStoreManager.DataStores[DataStoreType.L2Cache], context);
        }

        private static DatabaseConnectionMode ReadFromReplicaIfEnabled(RequestContext requestContext)
        {
            var isReadingFromReplicaConfigKeyEnabled = requestContext.Runtime.Configuration.GetSettingValue<bool>(ReadChannelDataFromReplica) ?? false;
            return isReadingFromReplicaConfigKeyEnabled ? DatabaseConnectionMode.AllowReadFromReplica : DatabaseConnectionMode.Default;
        }
    }
}
