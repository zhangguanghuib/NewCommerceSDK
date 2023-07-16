/// <summary>
///   class is helper class for card type
/// </summary>
namespace Contoso
{
    using Commerce.HardwareStation.Extension.UPOS_HardwareStation.DataModel;
    using Microsoft.Dynamics.Retail.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Commerce.HardwareStation.Extension.UPOS_HardwareStation.Helper
    {
        class CardTypeHelper
        {
            public static string getCardNumber(string cardNumber, UPOS_CardTypeInfoHeader cardTypeCollection, string defaultCard)
            {
                bool isCardAssociated = false;
                string cardNo = cardNumber.Replace(" ","");

                try
                {
                    foreach (UPOS_CardTypeInfoDetails cardType in cardTypeCollection.CardTypeInfoDetails)
                    {
                        if (CardTypeHelper.IsAssociatedCardType(cardType, cardNumber))
                        {
                            isCardAssociated = true;
                            return cardNo;
                        }
                    }
                    if (isCardAssociated == false)
                    {
                        if (!string.IsNullOrEmpty(defaultCard))
                        {
                            cardNo = defaultCard;
                        }
                        else
                        {
                            UPOS_CardTypeInfoDetails cardType = cardTypeCollection.CardTypeInfoDetails.FirstOrDefault();
                            if (cardType != null)
                            {
                                cardNo = cardType.NUMBERFROM;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    RetailLogger.Instance.PaymentConnectorLogException("getCardNumber", "Connector Name PCEFTPOS", "platform", ex);
                }
                return cardNo;
            }
            public static string getCardType(string cardNumber, UPOS_CardTypeInfoHeader cardTypeCollection, string defaultCard)
            {
                bool isCardAssociated = false;        
                string cardType = string.Empty;

                try
                {
                    foreach (UPOS_CardTypeInfoDetails cardTypeinfo in cardTypeCollection.CardTypeInfoDetails)
                    {
                        if (CardTypeHelper.IsAssociatedCardType(cardTypeinfo, cardNumber))
                        {
                            isCardAssociated = true;
                            return cardTypeinfo.NAME;
                        }
                    }
                    if (isCardAssociated == false)
                    {                       
                        if (!string.IsNullOrEmpty(defaultCard))
                        {
                            foreach (UPOS_CardTypeInfoDetails cardTypeinfo in cardTypeCollection.CardTypeInfoDetails)
                            {
                                if (CardTypeHelper.IsAssociatedCardType(cardTypeinfo, defaultCard))
                                {
                                    isCardAssociated = true;
                                    return cardTypeinfo.NAME;
                                }
                            }
                        }
                        if (isCardAssociated == false)
                        {
                            UPOS_CardTypeInfoDetails cardTypeinfo = cardTypeCollection.CardTypeInfoDetails.FirstOrDefault();
                            if (cardType != null)
                            {
                                cardType = cardTypeinfo.NAME;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    RetailLogger.Instance.PaymentConnectorLogException("getCardNumber", "Connector Name PCEFTPOS", "platform", ex);
                }
                return cardType;
            }
            private static bool IsAssociatedCardType(UPOS_CardTypeInfoDetails cardType, string cardNumber)
            {
                if (!string.IsNullOrWhiteSpace(cardNumber))
                {
                    int? nullable1 = CardTypeHelper.ParseInt(cardType.NUMBERFROM);
                    int? nullable2 = CardTypeHelper.ParseInt(cardType.NUMBERTO);
                    int length = cardType.NUMBERFROM.Length;
                    int? nullable3 = cardNumber.Length > length ? CardTypeHelper.ParseInt(cardNumber.Substring(0, length)) : CardTypeHelper.ParseInt(cardNumber);
                    int? nullable4 = nullable1;
                    int? nullable5 = nullable3;
                    if ((nullable4.GetValueOrDefault() <= nullable5.GetValueOrDefault() ? (nullable4.HasValue & nullable5.HasValue ? 1 : 0) : 0) != 0)
                    {
                        nullable5 = nullable3;
                        int? nullable6 = nullable2;
                        if ((nullable5.GetValueOrDefault() <= nullable6.GetValueOrDefault() ? (nullable5.HasValue & nullable6.HasValue ? 1 : 0) : 0) != 0)
                            return true;
                    }
                }
                return false;
            }
            private static int? ParseInt(string s)
            {
                int result;
                if (int.TryParse(s, out result))
                    return new int?(result);
                return new int?();
            }
        }
    }
}
