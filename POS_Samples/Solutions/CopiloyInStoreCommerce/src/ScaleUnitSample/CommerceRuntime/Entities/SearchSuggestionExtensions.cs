using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommerceRuntime.Entities
{
    public static class SearchSuggestionExtensions
    {
        const string AdjustedPrice = "AdjustedPrice";

        public static CommerceProperty GetAdjustedPriceCommerceProperty(this SearchSuggestion suggestion)
        {
            CommerceProperty commerceProperty = null;

            // Check if any CommerceProperty's key contains the substring "Price"
            if (suggestion.Value2 != null && suggestion.Value2.Key != null && suggestion.Value2.Key.Equals(AdjustedPrice))
            {
                if (suggestion.Value2.Value != null && suggestion.Value2.Value.DecimalValue != null)
                {
                    commerceProperty = suggestion.Value2;
                }
            }

            if (suggestion.Value3 != null && suggestion.Value3.Key != null && suggestion.Value3.Key.Equals(AdjustedPrice))
            {
                if (suggestion.Value3.Value != null && suggestion.Value3.Value.DecimalValue != null)
                {
                    commerceProperty = suggestion.Value3;
                }
            }

            if (suggestion.Value4 != null && suggestion.Value4.Key != null && suggestion.Value4.Key.Equals(AdjustedPrice))
            {
                if (suggestion.Value4.Value != null && suggestion.Value4.Value.DecimalValue != null)
                {
                    commerceProperty = suggestion.Value4;
                }
            }

            return commerceProperty;
        }

        public static CommerceProperty GetAdjustedPriceCommercePropertyV2(this SearchSuggestion suggestion)
        {
            CommerceProperty commerceProperty = null;

            CommerceProperty[] propertyArray = new CommerceProperty[] { suggestion.Value2, suggestion.Value3, suggestion.Value2 };

            commerceProperty = propertyArray.FirstOrDefault<CommerceProperty>(c =>
            {
                if (c != null && c.Key != null && c.Key.Equals(AdjustedPrice))
                {
                    if (c.Value != null && c.Value.DecimalValue != null)
                    {
                        return true;
                    }
                }
                return false;
            });

            return commerceProperty;
        }
    }
}
