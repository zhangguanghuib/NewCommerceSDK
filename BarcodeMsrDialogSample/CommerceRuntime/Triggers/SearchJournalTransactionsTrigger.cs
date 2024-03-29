﻿using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SearchJournalTransactions : IRequestTriggerAsync
{
    /// <summary>
    /// Gets the supported requests for this trigger.
    /// </summary>
    public IEnumerable<Type> SupportedRequestTypes
    {
        get
        {
            return new[] { typeof(SearchJournalTransactionsServiceRequest) };
        }
    }

    /// <summary>
    /// Post trigger code.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    public async Task OnExecuted(Request request, Response response)
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Pre trigger code
    /// </summary>
    /// <param name="request">The request.</param>
    public async Task OnExecuting(Request request)
    {   
        SearchJournalTransactionsServiceRequest searchJournalTransactionsServiceRequest = request as SearchJournalTransactionsServiceRequest;

        Channel currentChannel = request.RequestContext.GetChannel();
        var searchOrgUnitDataRequest = new SearchOrgUnitDataRequest(currentChannel.RecordId);
        var searchOrgUnitDataResponse = await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<OrgUnit>>(searchOrgUnitDataRequest).ConfigureAwait(false);
        string currentStoreNumber = searchOrgUnitDataResponse.FirstOrDefault()?.OrgUnitNumber ?? string.Empty;

        // If search other store,  throw an error message
        if (!string.IsNullOrEmpty(searchJournalTransactionsServiceRequest.Criteria.StoreId) && searchJournalTransactionsServiceRequest.Criteria.StoreId != currentStoreNumber)
        {
            //throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidStoreNumber, "对不起，暂不支持搜索其他Store的交易记录");

            throw new CommerceException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidStoreNumber.ToString(), "对不起，暂不支持搜索其他Store的交易记录")
            {
                LocalizedMessage = "对不起，暂不支持搜索其他Store的交易记录"
            };
        }

        // If no store is specified,  search in current store
        if (string.IsNullOrEmpty(searchJournalTransactionsServiceRequest.Criteria.StoreId))
        {
            searchJournalTransactionsServiceRequest.Criteria.StoreId = currentStoreNumber;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }
}