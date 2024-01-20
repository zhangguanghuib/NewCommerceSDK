using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>
    /// Defines a basket that can be sent as a part of the basket data command
    /// </summary>
    [DataContract]
    public class EFTBasket
    {
        /// <summary>
        /// A unique ID for the basket. Max 32 chars
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// A list of basket items 
        /// </summary>
        [DataMember(Name = "items", EmitDefaultValue = false)]
        public List<EFTBasketItem> Items { get; set; }

        /// <summary>
        /// Total price for the basket, in cents, including tax and discount, but excluding surcharge
        /// </summary>
        [DataMember(Name = "amt", EmitDefaultValue = false)]
        public uint Amount { get; set; }

        /// <summary>
        /// Total tax for the basket, in cents
        /// </summary>
        [DataMember(Name = "tax", EmitDefaultValue = false)]
        public uint Tax { get; set; }

        /// <summary>
        /// Total discount for the basket, in cents
        /// </summary>
        [DataMember(Name = "dis", EmitDefaultValue = false)]
        public uint Discount { get; set; }

        /// <summary>
        /// Surcharge (e.g. for Credit Card), in cents
        /// </summary>
        [DataMember(Name = "sur", EmitDefaultValue = false)]
        public uint Surcharge { get; set; }
    }

    /// <summary>
    /// Defines a basket item that can be sent as a part of the basket data command
    /// </summary>
    [DataContract]
    public class EFTBasketItem
    {
        /// <summary>
        /// A unique ID for the item. Max 32 chars
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Merchant assigned SKU for the item
        /// </summary>
        [DataMember(Name = "sku", EmitDefaultValue = false)]
        public string SKU { get; set; }

        /// <summary>
        /// Quantity (number of units)
        /// </summary>
        [DataMember(Name = "qty", EmitDefaultValue = false)]
        public uint Quantity { get; set; }

        /// <summary>
        /// Individual unit price for the item, in cents, including tax and discount
        /// </summary>
        [DataMember(Name = "amt", EmitDefaultValue = false)]
        public uint Amount { get; set; }

        /// <summary>
        /// Individual unit tax for the item, in cents
        /// </summary>
        [DataMember(Name = "tax", EmitDefaultValue = false)]
        public uint Tax { get; set; }

        /// <summary>
        /// Individual unit discount for the item, in cents
        /// </summary>
        [DataMember(Name = "dis", EmitDefaultValue = false)]
        public uint Discount { get; set; }

        /// <summary>
        /// The EAN (European Article Number) for the item
        /// </summary>
        [DataMember(Name = "ean", EmitDefaultValue = false)]
        public string EAN { get; set; }

        /// <summary>
        /// The UPC (Universal Product Code) for the item
        /// </summary>
        [DataMember(Name = "upc", EmitDefaultValue = false)]
        public string UPC { get; set; }

        /// <summary>
        /// The GTIN (Global Trade Item Number) for the item
        /// </summary>
        [DataMember(Name = "gtin", EmitDefaultValue = false)]
        public string GTIN { get; set; }

        /// <summary>
        /// A short name for the item. Max length 24
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                // Trim to length of 24
                _name = (value?.Length > 24) ? value.Substring(0, 24) : value;
            }
        }

        private string _name;

        /// <summary>
        /// A longer description for the item. Max length 255
        /// </summary>
        [DataMember(Name = "desc", EmitDefaultValue = false)]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                // Trim to length of 255
                _description = (value?.Length > 255) ? value.Substring(0, 255) : value;
            }
        }

        private string _description;

        /// <summary>
        /// Serial code, or other identifying code, for the item 
        /// </summary>
        [DataMember(Name = "srl", EmitDefaultValue = false)]
        public string SerialCode { get; set; }

        /// <summary>
        /// URI to an image for the item
        /// </summary>
        [DataMember(Name = "img", EmitDefaultValue = false)]
        public string ImageUri { get; set; }

        /// <summary>
        /// URI to a product page for the item
        /// </summary>
        [DataMember(Name = "link", EmitDefaultValue = false)]
        public string ProductUri { get; set; }

        /// <summary>
        /// List of category or type information for the item (e.g. “food, confectionary”)
        /// </summary>
        [IgnoreDataMember]
        public List<string> TagList { get; set; }

        /// <summary>
        /// Comma separated category or type information for the item (e.g. “food, confectionary”)
        /// </summary>
        /// <remarks>
        /// Use <see cref="TagList"/> to set the tags.
        /// </remarks>
        [DataMember(Name = "tag", EmitDefaultValue = false)]
        public string Tag
        {
            get
            {
                // Return TagList as comma list
                return (TagList?.Count > 0) ? String.Join(",", TagList) : null;
            }
        }

    }


    /// <summary>
    /// Represents a default basket request
    /// </summary>
    public class EFTBasketDataRequest : EFTRequest
    {
        public EFTBasketDataRequest() : base(false, typeof(EFTBasketDataResponse))
        {
        }

        /// <summary>
        /// The command to perform (create, update, delete)
        /// <see cref="EFTBasketDataCommandCreate"/>
        /// <see cref="EFTBasketDataCommandAdd"/>
        /// <see cref="EFTBasketDataCommandDelete"/>
        /// <see cref="EFTBasketDataCommandRaw"/>
        /// </summary>
        public IEFTBasketDataCommand Command { get; set; }
    }


    /// <summary>
    /// Response to a EFTBasketDataRequest
    /// </summary>
    public class EFTBasketDataResponse : EFTResponse
    {

        public EFTBasketDataResponse() : base(typeof(EFTBasketDataRequest))
        {
        }


        /// <summary>
        /// True if the request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 2 character response code. "00" indicates success.
        /// </summary>
        public string ResponseCode { get; set; }


        /// <summary>
        /// 20 character description of the ResponseCode field.
        /// </summary>
        public string ResponseText { get; set; }
    }



    /// <summary>
    /// Defines a command to perform on a basket (create, update, delete)
    /// </summary>
    public interface IEFTBasketDataCommand
    {

    }

    /// <summary>
    /// Command to create a new basket.
    /// </summary>
    public class EFTBasketDataCommandCreate : IEFTBasketDataCommand
    {
        /// <summary>
        /// Full basket details
        /// </summary>
        public EFTBasket Basket { get; set; }
    }

    /// <summary>
    /// Command to create add an item to a basket
    /// </summary>
    public class EFTBasketDataCommandAdd : IEFTBasketDataCommand
    {
        /// <summary>
        /// Full basket details
        /// </summary>
        public EFTBasket Basket { get; set; }
    }

    /// <summary>
    /// Command to remove an item from a basket
    /// </summary>
    public class EFTBasketDataCommandDelete : IEFTBasketDataCommand
    {
        /// <summary>
        /// The id of the basket to remove the item from 
        /// </summary>
        public string BasketId { get; set; }

        /// <summary>
        /// The id of the item to remove
        /// </summary>
        public string BasketItemId { get; set; }
    }

    /// <summary>
    /// Send a raw json string to the basket data API
    /// </summary>
    public class EFTBasketDataCommandRaw : IEFTBasketDataCommand
    {
        /// <summary>
        /// The basket data content, formatted as json
        /// </summary>
        public string BasketContent { get; set; }
    }
}

