/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */


namespace Contoso.CommerceRuntime.PricingEngine
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine;

    /// <summary>
    /// These parameters indicate which types of AX discounts (aka trade agreement discounts, aka not Retail Periodic Discounts)
    /// are currently activated and should be allowed on the transaction.
    /// </summary>
    internal class DiscountParameters
    {
        /// <summary>
        /// Contains all combinations deemed valid by PriceParameters table in database.
        /// </summary>
        private HashSet<Tuple<PriceDiscountType, PriceDiscountAccountCode, PriceDiscountItemCode>> enabledCombinations;

        /// <summary>
        /// Create a new DiscountParameters object and initialize from database if possible.
        /// </summary>
        /// <param name="pricingDataManager">Data manager to access pricing data.</param>
        /// <returns>Newly fetched and initialized DiscountParameters object.</returns>
        internal static DiscountParameters CreateAndInitialize(IPricingDataAccessor pricingDataManager)
        {
            DiscountParameters parameters = new DiscountParameters();
            parameters.GetDiscountParameters(pricingDataManager);
            return parameters;
        }

        /// <summary>
        /// Create a new DiscountParameters object from the passed in data.
        /// </summary>
        /// <param name="priceParameters">Data model object representing price activation options.</param>
        /// <returns>Newly fetched and initialized DiscountParameters object.</returns>
        internal static DiscountParameters CreateAndInitialize(PriceParameters priceParameters)
        {
            DiscountParameters parameters = new DiscountParameters();
            parameters.GetDiscountParameters(priceParameters);
            return parameters;
        }

        /// <summary>
        /// True if there is a valid relation between the item code and relation.
        /// </summary>
        /// <param name="itemCode">The item code (table,group,all).</param>
        /// <param name="relation">The item relation.</param>
        /// <returns>Returns true if the relation ok, else false.</returns>
        internal static bool ValidRelation(PriceDiscountItemCode itemCode, string relation)
        {
            ThrowIf.Null(relation, nameof(relation));

            bool ok = true;

            if (!string.IsNullOrEmpty(relation) && (itemCode == PriceDiscountItemCode.AllItems))
            {
                ok = false;
            }

            if (string.IsNullOrEmpty(relation) && (itemCode != PriceDiscountItemCode.AllItems))
            {
                ok = false;
            }

            return ok;
        }

        /// <summary>
        /// True if there is a valid relation between the account code and relation.
        /// </summary>
        /// <param name="accountCode">The account code (table,group,all).</param>
        /// <param name="relation">The account relation.</param>
        /// <returns>True if the account code allows the given relation.</returns>
        internal static bool ValidRelation(PriceDiscountAccountCode accountCode, string relation)
        {
            ThrowIf.Null(relation, nameof(relation));

            bool ok = true;

            if (!string.IsNullOrEmpty(relation) && (accountCode == PriceDiscountAccountCode.AllCustomers))
            {
                ok = false;
            }

            if (string.IsNullOrEmpty(relation) && (accountCode != PriceDiscountAccountCode.AllCustomers))
            {
                ok = false;
            }

            return ok;
        }

        /// <summary>
        /// Returns true or false, whether a certain relation is active for a discount search.
        /// </summary>
        /// <param name="relation">The trade agreement relation(price, line discount, multiline discount, total discount).</param>
        /// <param name="accountCode">The account code(table,group,all).</param>
        /// <param name="itemCode">The item code(table,group,all).</param>
        /// <returns>Returns true if the relation is active, else false.</returns>
        internal bool Activation(PriceDiscountType relation, PriceDiscountAccountCode accountCode, PriceDiscountItemCode itemCode)
        {
            // if parameters haven't been fetched, return false for all
            if (this.enabledCombinations == null)
            {
                return false;
            }

            // look up whether the combination is enabled
            return this.enabledCombinations.Contains(CreateCombo(relation, accountCode, itemCode));
        }

        /// <summary>
        /// Helper method to create new 3-tuple of trade agreement combination.
        /// </summary>
        /// <param name="relation">The type of trade agreement (price, line discount, etc.).</param>
        /// <param name="accountCode">The type of customer (single, group, all).</param>
        /// <param name="itemCode">The type of item (single, group, all).</param>
        /// <returns>3-tuple containing the given combination.</returns>
        private static Tuple<PriceDiscountType, PriceDiscountAccountCode, PriceDiscountItemCode> CreateCombo(PriceDiscountType relation, PriceDiscountAccountCode accountCode, PriceDiscountItemCode itemCode)
        {
            return new Tuple<PriceDiscountType, PriceDiscountAccountCode, PriceDiscountItemCode>(relation, accountCode, itemCode);
        }

        /// <summary>
        /// Get discount parameters from the database. These parameters tell what search possibilities are active.
        /// Populate the internal set of enabled trade agreement combinations based on fetched values.
        /// </summary>
        /// <param name="pricingDataManager">The data manager which returns pricing data.</param>
        private void GetDiscountParameters(IPricingDataAccessor pricingDataManager)
        {
            var priceParameters = pricingDataManager.GetPriceParameters();
            this.GetDiscountParameters(priceParameters);
        }

        /// <summary>
        /// Populate the internal set of enabled trade agreement combinations based on values in given PriceParameters object.
        /// </summary>
        /// <param name="priceParameters">Data model object representing price activation options.</param>
        private void GetDiscountParameters(PriceParameters priceParameters)
        {
            var activeCombinations = new HashSet<Tuple<PriceDiscountType, PriceDiscountAccountCode, PriceDiscountItemCode>>();

            if (priceParameters.ApplyPriceForCustomerAndItem)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.PriceSales, PriceDiscountAccountCode.Customer, PriceDiscountItemCode.Item));
            }

            if (priceParameters.ApplyPriceForCustomerGroupAndItem)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.PriceSales, PriceDiscountAccountCode.CustomerGroup, PriceDiscountItemCode.Item));
            }

            if (priceParameters.ApplyPriceForAllCustomersAndItem)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.PriceSales, PriceDiscountAccountCode.AllCustomers, PriceDiscountItemCode.Item));
            }

            if (priceParameters.ApplyLineDiscountForCustomerAndItem)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.LineDiscountSales, PriceDiscountAccountCode.Customer, PriceDiscountItemCode.Item));
            }

            if (priceParameters.ApplyLineDiscountForCustomerAndItemGroup)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.LineDiscountSales, PriceDiscountAccountCode.Customer, PriceDiscountItemCode.ItemGroup));
            }

            if (priceParameters.ApplyLineDiscountForCustomerAndAllItems)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.LineDiscountSales, PriceDiscountAccountCode.Customer, PriceDiscountItemCode.AllItems));
            }

            if (priceParameters.ApplyLineDiscountForCustomerGroupAndItem)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.LineDiscountSales, PriceDiscountAccountCode.CustomerGroup, PriceDiscountItemCode.Item));
            }

            if (priceParameters.ApplyLineDiscountForCustomerGroupAndItemGroup)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.LineDiscountSales, PriceDiscountAccountCode.CustomerGroup, PriceDiscountItemCode.ItemGroup));
            }

            if (priceParameters.ApplyLineDiscountForCustomerGroupAndAllItems)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.LineDiscountSales, PriceDiscountAccountCode.CustomerGroup, PriceDiscountItemCode.AllItems));
            }

            if (priceParameters.ApplyLineDiscountForAllCustomersAndItem)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.LineDiscountSales, PriceDiscountAccountCode.AllCustomers, PriceDiscountItemCode.Item));
            }

            if (priceParameters.ApplyLineDiscountForAllCustomersAndItemGroup)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.LineDiscountSales, PriceDiscountAccountCode.AllCustomers, PriceDiscountItemCode.ItemGroup));
            }

            if (priceParameters.ApplyLineDiscountForAllCustomersAndAllItems)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.LineDiscountSales, PriceDiscountAccountCode.AllCustomers, PriceDiscountItemCode.AllItems));
            }

            if (priceParameters.ApplyMultilineDiscountForCustomerAndItemGroup)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.MultilineDiscountSales, PriceDiscountAccountCode.Customer, PriceDiscountItemCode.ItemGroup));
            }

            if (priceParameters.ApplyMultilineDiscountForCustomerAndAllItems)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.MultilineDiscountSales, PriceDiscountAccountCode.Customer, PriceDiscountItemCode.AllItems));
            }

            if (priceParameters.ApplyMultilineDiscountForCustomerGroupAndItemGroup)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.MultilineDiscountSales, PriceDiscountAccountCode.CustomerGroup, PriceDiscountItemCode.ItemGroup));
            }

            if (priceParameters.ApplyMultilineDiscountForCustomerGroupAndAllItems)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.MultilineDiscountSales, PriceDiscountAccountCode.CustomerGroup, PriceDiscountItemCode.AllItems));
            }

            if (priceParameters.ApplyMultilineDiscountForAllCustomersAndItemGroup)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.MultilineDiscountSales, PriceDiscountAccountCode.AllCustomers, PriceDiscountItemCode.ItemGroup));
            }

            if (priceParameters.ApplyMultilineDiscountForAllCustomersAndAllItems)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.MultilineDiscountSales, PriceDiscountAccountCode.AllCustomers, PriceDiscountItemCode.AllItems));
            }

            if (priceParameters.ApplyTotalDiscountForCustomerAndAllItems)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.EndDiscountSales, PriceDiscountAccountCode.Customer, PriceDiscountItemCode.AllItems));
            }

            if (priceParameters.ApplyTotalDiscountForCustomerGroupAndAllItems)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.EndDiscountSales, PriceDiscountAccountCode.CustomerGroup, PriceDiscountItemCode.AllItems));
            }

            if (priceParameters.ApplyTotalDiscountForAllCustomersAndAllItems)
            {
                activeCombinations.Add(CreateCombo(PriceDiscountType.EndDiscountSales, PriceDiscountAccountCode.AllCustomers, PriceDiscountItemCode.AllItems));
            }

            // save found combinations on the object
            this.enabledCombinations = activeCombinations;
        }
    }
}
