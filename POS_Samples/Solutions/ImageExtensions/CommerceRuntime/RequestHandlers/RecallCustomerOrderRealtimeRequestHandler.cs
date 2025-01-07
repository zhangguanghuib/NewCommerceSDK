namespace Contoso.GasStationSample.CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using System.Collections.Concurrent;
    using Microsoft.Extensions.Logging;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages.ProductAvailability;
    using System.Collections.ObjectModel;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages.Inventory;
    using Microsoft.Dynamics.Commerce.Runtime.TransactionService;
    using Microsoft.Dynamics.Commerce.Runtime.TransactionService.Serialization;
    using Microsoft.Dynamics.Retail.Diagnostics;
    using Microsoft.Dynamics.Retail.Diagnostics.Extensions;
    using System.Linq;
    using System.Globalization;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using System.Xml;

    public class RecallCustomerOrderRealtimeRequestHandler : SingleAsyncRequestHandler<RecallCustomerOrderRealtimeRequest>
    {
        protected async override Task<Response> Process(RecallCustomerOrderRealtimeRequest request)
        {
            var transactionServiceClient  = new TransactionServiceClient(request.RequestContext);
            ReadOnlyCollection<object> transactionResponse = null;

            if (request.IsQuote)
            {
                transactionResponse = await transactionServiceClient.GetCustomerQuote(request.Id).ConfigureAwait(false);
            }
            else
            {
                transactionResponse = await transactionServiceClient.GetCustomerOrder(request.Id, includeOnlineOrders: true).ConfigureAwait(false);
            }

            string orderXml = transactionResponse[0].ToString();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(orderXml);

            XmlNodeList itemNodes = xmlDoc.SelectNodes("//CustomerOrder/Items/Item");

            Dictionary<Decimal, string> dict = new Dictionary<Decimal, string>();

            foreach (XmlNode itemNode in itemNodes)
            {
                decimal lineNumber = Convert.ToDecimal(itemNode.Attributes["LineNumber"]?.Value);
                XmlNode installationDateNode = itemNode.SelectSingleNode("ExtensionProperties/InstallationDate");

                if (installationDateNode != null)
                {
                    dict.Add(lineNumber, installationDateNode.InnerText);
                }
            }

            var response = await this.ExecuteNextAsync<RecallCustomerOrderRealtimeResponse>(request).ConfigureAwait(false);

            foreach (var line in response.SalesOrder.SalesLines)
            {
                if (dict.TryGetValue(line.LineNumber, out string value))
                {
                    SalesLine salesline = response.SalesOrder.SalesLines.Where(x => x.LineNumber == line.LineNumber).FirstOrDefault();
                    if(salesline != null)
                    {
                        CommerceProperty commerceProperty = new CommerceProperty("InstallationDate", value);
                        salesline.ExtensionProperties.Add(commerceProperty);
                    }
                }
            }

            return response;
        }
    }
}
