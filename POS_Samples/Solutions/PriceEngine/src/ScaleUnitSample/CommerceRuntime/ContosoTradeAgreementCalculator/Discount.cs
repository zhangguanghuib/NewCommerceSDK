using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PricingEngine.ContosoTradeAgreementCalculator
{
    internal class Discount
    {
        internal static string GetUnitOfMeasure(SalesLine salesLine)
        {
            string unitOfMeasure = salesLine.SalesOrderUnitOfMeasure;
            if (string.IsNullOrWhiteSpace(unitOfMeasure))
            {
                unitOfMeasure = salesLine.UnitOfMeasureSymbol ?? string.Empty;
            }

            return unitOfMeasure;
        }
    }
}
