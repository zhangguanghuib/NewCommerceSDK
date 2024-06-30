namespace Contoso.PricingEngine.TradeAgreementCalculator
{
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
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
