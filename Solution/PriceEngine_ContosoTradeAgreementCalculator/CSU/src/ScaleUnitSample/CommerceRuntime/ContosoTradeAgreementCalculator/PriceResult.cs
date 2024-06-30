namespace Contoso.PricingEngine.TradeAgreementCalculator
{
    /// <summary>
    /// Result from a price lookup.
    /// </summary>
    internal struct PriceResult
    {
        /// <summary>
        /// Price value.
        /// </summary>
        internal readonly decimal Price;

        /// <summary>
        /// Customer Price Group.
        /// </summary>
        internal readonly string CustPriceGroup;

        /// <summary>
        /// Maximum retail price.
        /// </summary>
        internal decimal MaximumRetailPriceIndia;

        /// <summary>
        /// Trade agreement record Id.
        /// </summary>
        internal long TradeAgreementRecordId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceResult"/> struct.
        /// </summary>
        /// <param name="price">Price of the result.</param>
        /// <param name="tradeAgreementRecordId">Trade agreement record Id.</param>
        /// <param name="maximumRetailPriceIndia">Maximum retail price.</param>
        /// <param name="custPriceGroup">Customer price group.</param>
        internal PriceResult(decimal price, long tradeAgreementRecordId, decimal maximumRetailPriceIndia = decimal.Zero, string custPriceGroup = null)
        {
            this.Price = price;
            this.TradeAgreementRecordId = tradeAgreementRecordId;
            this.MaximumRetailPriceIndia = maximumRetailPriceIndia;
            this.CustPriceGroup = custPriceGroup;
        }
    }
}
