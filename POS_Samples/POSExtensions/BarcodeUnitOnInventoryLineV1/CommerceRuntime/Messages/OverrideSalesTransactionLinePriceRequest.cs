namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public class OverrideSalesTransactionLinePriceRequest : Request
    {
        public OverrideSalesTransactionLinePriceRequest(Cart cart, string lineId, decimal newPrice, CalculationModes? modes)
        {
            this.Cart = cart;
            this.LineId = lineId;
            this.NewPrice = newPrice;
            this.Modes = modes;
        }

        /// <summary>
        /// Gets the cart object.
        /// </summary>
        [DataMember]
        [Required]
        public Cart Cart{ get; private set; }

        /// <summary>
        /// Gets the cart line identifier with new price.
        /// </summary>
        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string LineId { get; private set; }

        /// <summary>
        /// Gets the new price for the cart line.
        /// </summary>
        [DataMember]
        [Required]
        public decimal NewPrice { get; private set; }

        /// <summary>
        /// Gets the calculation modes.
        /// </summary>
        [DataMember]
        public CalculationModes? Modes { get; private set; }

        /// <inheritdoc cref="Request"/>
        public override IEnumerable<DataValidationFailure> Validate()
        {
            CartLine lineToUpdate = null;
            var validationFailures = new Collection<DataValidationFailure>();

            if (this.Cart.CartLines.Any())
            {
                lineToUpdate = this.Cart.CartLines.SingleOrDefault(l => l.LineId.Equals(this.LineId, StringComparison.OrdinalIgnoreCase));
            }

            if (lineToUpdate == null)
            {
                validationFailures.Add(new DataValidationFailure(
                    DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidRequest,
                    $"Cannot override price as cart line with ID {this.LineId} is not found."));
            }

            return validationFailures;
        }
    }
}
