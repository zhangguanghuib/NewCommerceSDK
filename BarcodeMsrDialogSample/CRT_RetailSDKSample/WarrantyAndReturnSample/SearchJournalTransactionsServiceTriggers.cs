namespace CRT_RetailSDKSample.WarrantyAndReturnSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;



    public class SearchJournalTransactionsServiceTriggers : IRequestTriggerAsync
    {
        private const string ReturnMaxDaysParameterName = "ReturnMaxDays";
        private const string ReturnRemainingDaysExtensionPropertyName = "ReturnReminingDays";
        private const string ServiceChargePercentageParameterName = "ServiceChargePercentage";
        private const string ServiceChargeAmountExtensionPropertyName = "ServiceChargeAmount";

        private const string CustomerNameExtensionPropertyName = "CustomerNameExtension";

        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(SearchJournalTransactionsServiceRequest) };
            }
        }

        public async Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");

            PagedResult<Transaction> transactions = ((SearchJournalTransactionsServiceResponse)response).Transactions;
            if (transactions.IsNullOrEmpty())
            {
                return;
            }

            var config = await this.GetConfigurationParametersAsync(request).ConfigureAwait(false);
            var returnMaxDays = config.Item1;
            var serviceChargePercentage = config.Item2;

            await this.AddExtensionPropertiesToTransactionsAsync(request, transactions, returnMaxDays, serviceChargePercentage).ConfigureAwait(false);

        }

        public async Task OnExecuting(Request request)
        {
            await Task.CompletedTask;
        }

        private async Task<(int?, decimal?)>  GetConfigurationParametersAsync(Request request)
        {
            int? returnMaxDays = null;
            decimal? serviceChargePercentage = null;

            var configurationRequest = new GetConfigurationParametersDataRequest(request.RequestContext.GetPrincipal().ChannelId);
            var configurationResponse = await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<RetailConfigurationParameter>>(configurationRequest).ConfigureAwait(false);

            foreach (RetailConfigurationParameter configuration in configurationResponse)
            {
                if (string.Equals(ReturnMaxDaysParameterName, configuration.Name, StringComparison.OrdinalIgnoreCase))
                {
                    int intValue;
                    if (int.TryParse(configuration.Value, out intValue))
                    {
                        returnMaxDays = intValue;
                    }
                }
                else if (string.Equals(ServiceChargePercentageParameterName, configuration.Name, StringComparison.OrdinalIgnoreCase))
                {
                    decimal decimalValue;
                    if (decimal.TryParse(configuration.Value, out decimalValue))
                    {
                        serviceChargePercentage = decimalValue;
                    }
                }
            }

            return (returnMaxDays, serviceChargePercentage);
        }


        public async Task<SalesOrder> GetOriginSalesOrderAsync(RequestContext requestContext, Transaction transaction)
        {
            ThrowIf.Null(requestContext, nameof(requestContext));

            GetSalesOrderDetailsByTransactionIdServiceRequest salesOrderRequest = new GetSalesOrderDetailsByTransactionIdServiceRequest(
                transaction.Id,
                SearchLocation.All);

            GetSalesOrderDetailsServiceResponse salesOrderResponse = await requestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(salesOrderRequest).ConfigureAwait(false);

            return salesOrderResponse.SalesOrder;
        }

        private async Task AddExtensionPropertiesToTransactionsAsync(Request request, PagedResult<Transaction> transactions, int? returnMaxDays, decimal? serviceChargePercentage)
        {
            foreach (Transaction transaction in transactions)
            {
                SalesOrder salesOrder = await this.GetOriginSalesOrderAsync(request.RequestContext, transaction);
                transaction.SetProperty(CustomerNameExtensionPropertyName, salesOrder.CustomerName);

                if (returnMaxDays != null)
                {
                    // Calculate and append "return remaining days"
                    int transactionLifeInDays = (int)Math.Floor(DateTimeOffset.Now.Subtract(transaction.CreatedDateTime).TotalDays);
                    int remainingDays = Math.Max(0, (int)returnMaxDays - transactionLifeInDays);
                    transaction.SetProperty(ReturnRemainingDaysExtensionPropertyName, remainingDays);
                }

                if (serviceChargePercentage != null)
                {
                    // Calculate and append "service charge amount"
                    decimal serviceChargeAmount = transaction.TotalAmount * (decimal)serviceChargePercentage / 100;
                    var roundingRequest = new GetRoundedValueServiceRequest(serviceChargeAmount, request.RequestContext.GetOrgUnit().Currency);
                    var roundingResponse = await request.RequestContext.ExecuteAsync<GetRoundedValueServiceResponse>(roundingRequest).ConfigureAwait(false);
                    if (roundingResponse != null)
                    {
                        transaction.SetProperty(ServiceChargeAmountExtensionPropertyName, roundingResponse.RoundedValue);
                    }
                }
            }
        }
    }
}
