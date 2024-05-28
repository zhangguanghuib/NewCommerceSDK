using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommerceRuntime.Entities;
using System.Runtime;
using System.Linq;

namespace CommerceRuntime.RequestHandlers
{
    public class SearchSuggestionList
    {
        public List<SearchSuggestion> Value { get; set; }
    }

    public class ProductSearchSuggestionExtService : IRequestHandlerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new Type[]
                {
                    typeof(GetProductSearchSuggestionsRequest)
                };
            }
        }

        bool ContainsPriceKey(SearchSuggestion suggestion)
        {
            // Check if any CommerceProperty's key contains the substring "Price"
            if (suggestion.Value2 != null && suggestion.Value2.Key != null && suggestion.Value2.Key.Contains("Price"))
            {
                return true;
            }

            if (suggestion.Value3 != null && suggestion.Value3.Key != null && suggestion.Value3.Key.Contains("Price"))
            {
                return true;
            }

            if (suggestion.Value4 != null && suggestion.Value4.Key != null && suggestion.Value4.Key.Contains("Price"))
            {
                return true;
            }

            return false;
        }

        [Obsolete]
        public async Task<Response> Execute(Request request)
        {
            try
            {
                if (request == null)
                {
                    var exception = new ArgumentNullException("request");
                    throw exception;
                }
                else if (request is GetProductSearchSuggestionsRequest)
                {
                    GetProductSearchSuggestionsRequest getProductSearchSuggestionsRequest = request as GetProductSearchSuggestionsRequest;

                    var requestHandler = request.RequestContext.Runtime.GetNextAsyncRequestHandler(request.GetType(), this);
                    var response = await request.RequestContext.Runtime.ExecuteAsync<EntityDataServiceResponse<SearchSuggestion>>(request, request.RequestContext, requestHandler, false).ConfigureAwait(false);

                    string jsonContent = System.IO.File.ReadAllText("SearchSuggestions.json");
                    List<SearchSuggestion> searchSuggestions = JsonConvert.DeserializeObject<SearchSuggestionList>(jsonContent).Value;

                    //const string AdjustedPrice = "AdjustedPrice";

                    // ? Not Sure if we can update the property
                    //List<SearchSuggestion> searchSuggestions = response.PagedEntityCollection.ToList<SearchSuggestion>();

                    foreach (SearchSuggestion searchSuggestion in searchSuggestions)
                    {
                        if (searchSuggestion.SuggestionType == SearchSuggestionType.Product && long.Parse(searchSuggestion.Id) != 0)
                        {
                            CommerceProperty priceCommerceProperty = null;
                            long productId = long.Parse(searchSuggestion.Id);

                            priceCommerceProperty = searchSuggestion.GetAdjustedPriceCommercePropertyV2();

                            if (priceCommerceProperty != null)
                            {
                                var getActiveProductPriceRequest = new GetActiveProductPriceRequest
                                {
                                    PriceLookupContext = new PriceLookupContext()
                                    {
                                        LineContexts = new PriceLookupLineContext[]
                                        {
                                            new PriceLookupLineContext()
                                            {
                                                ProductRecordId = productId,
                                            },
                                        },
                                    },
                                    CustomerAccountNumber = "004007",
                                    DateWhenActive = request.RequestContext.GetNowInChannelTimeZone(),
                                    Context = new ProjectionDomain(request.RequestContext.GetPrincipal().ChannelId, 0),
                                    IncludeSimpleDiscountsInContextualPrice = false,
                                    IncludeVariantPriceRange = false,
                                    QueryResultSettings = QueryResultSettings.AllRecords,
                                };

                                var getActiveProductPriceResponse = await request.RequestContext.ExecuteAsync<GetActiveProductPriceResponse>(getActiveProductPriceRequest).ConfigureAwait(false);
                                ProductPrice productPrice = getActiveProductPriceResponse.ProductPrices.SingleOrDefault();

                                if (productPrice != null)
                                {
                                    priceCommerceProperty.Value.DecimalValue = productPrice.AdjustedPrice;
                                }
                            }
                        }
                    }

                    return response;
                }
            }
            finally
            {
                Console.WriteLine("Finally");
            }

            // Do no checks.
            return NullResponse.Instance;
        }

    }
}
