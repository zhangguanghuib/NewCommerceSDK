using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
    public static class TerminalApplicationExtension
    {
        public static string ToApplicationString(this TerminalApplication v)
        {
            switch (v)
            {
                case TerminalApplication.EFTPOS:
                    return "00";
                case TerminalApplication.Agency:
                    return "01";
                case TerminalApplication.Loyalty:
                case TerminalApplication.PrePaidCard:
                case TerminalApplication.ETS:
                    return "02";                // PCEFTCSA
                case TerminalApplication.GiftCard:
                    return "03";
                case TerminalApplication.Fuel:
                    return "04";
                case TerminalApplication.Medicare:
                    return "05";
                case TerminalApplication.Amex:
                    return "06";
                case TerminalApplication.ChequeAuth:
                    return "07";
            }

            return "00";    // Default to EFTPOS.
        }

        //public static string ToMerchantString(this TerminalApplication v)
        //{
        //    switch (v)
        //    {
        //        case TerminalApplication.EFTPOS:
        //        case TerminalApplication.Agency:
        //            return "00";
        //        case TerminalApplication.GiftCard:
        //            return "01";
        //        case TerminalApplication.Loyalty:
        //            return "02";
        //        case TerminalApplication.ChequeAuth:
        //            return "03";
        //        case TerminalApplication.PrePaidCard:
        //            return "04";
        //        case TerminalApplication.Medicare:
        //            return "05";
        //        case TerminalApplication.Amex:
        //            return "08";
        //    }

        //    return "00";    // Default to EFTPOS.
        //}
    }

    public static class AccountTypeExtension
    {
        public static AccountType FromString(this AccountType v, string s)
        {
            if (s.ToUpper().TrimEnd() == "CREDIT")
                return AccountType.Credit;
            else if (s.ToUpper().TrimEnd() == "SAVINGS")
                return AccountType.Savings;
            else if (s.ToUpper().TrimEnd() == "CHEQUE")
                return AccountType.Cheque;

            return AccountType.Default;
        }
    }

    public static class TransactionTypeExtension
    {
        public static string ToTransactionString(this TransactionType t)
        {
            switch (t)
            {
                case TransactionType.PurchaseCash:
                    return "P";
                case TransactionType.AddCard:
                    return "L";
                case TransactionType.AddPointsToCard:
                    return "N";
                case TransactionType.AuthPIN:
                    return "X";
                case TransactionType.Balance:
                    return "B";
                case TransactionType.CancelVoid:
                    return "I";
                case TransactionType.CardActivate:
                    return "A";
                case TransactionType.CardDeactivate:
                    return "F";
                case TransactionType.CardSale:
                    return "D";
                case TransactionType.CardSaleTopUp:
                    return "T";
                case TransactionType.CashOut:
                    return "C";
                case TransactionType.Completion:
                    return "M";
                case TransactionType.DecrementPointsFromCard:
                    return "K";
                case TransactionType.MiniTransactionHistory:
                    return "H";
                case TransactionType.NotSet:
                    return " ";
                case TransactionType.OrderRequest:
                    return "O";
                case TransactionType.Refund:
                    return "R";
                case TransactionType.RefundFromCard:
                    return "W";
                case TransactionType.Voucher:
                    return "V";
                case TransactionType.None:
                    return "0";
                default:
                    return "0";
            }
        }
        //public static string ToTransactionString(this TransactionType t)
        //{
        //    switch (t)
        //    {
        //        case TransactionType.PurchaseCash:
        //        case TransactionType.Redemption:
        //            return "P";
        //        case TransactionType.AddCard:
        //            return "L";
        //        case TransactionType.AddPointsToCard:
        //            return "N";
        //        case TransactionType.AuthPIN:
        //        case TransactionType.CashBackFromCard:
        //            return "X";
        //        case TransactionType.Balance:
        //        case TransactionType.CardBalance:
        //            return "B";
        //        case TransactionType.CancelVoid:
        //            return "I";
        //        case TransactionType.CardActivate:
        //        case TransactionType.PreAuth:
        //            return "A";
        //        case TransactionType.CardDeactivate:
        //        case TransactionType.FundsTransfer:
        //            return "F";
        //        case TransactionType.CardSale:
        //        case TransactionType.Deposit:
        //            return "D";
        //        case TransactionType.CardSaleTopUp:
        //        case TransactionType.TipAdjust:
        //            return "T";
        //        case TransactionType.CashOut:
        //            return "C";
        //        case TransactionType.Completion:
        //        case TransactionType.TransferPoints:
        //            return "M";
        //        case TransactionType.DecrementPointsFromCard:
        //        case TransactionType.EnhancedPIN:
        //            return "K";
        //        case TransactionType.MiniTransactionHistory:
        //            return "H";
        //        case TransactionType.NotSet:
        //            return " ";
        //        case TransactionType.OrderRequest:
        //            return "O";
        //        case TransactionType.Refund:
        //        case TransactionType.RefundToCard:
        //            return "R";
        //        case TransactionType.RefundFromCard:
        //        case TransactionType.Withdrawal:
        //            return "W";
        //        case TransactionType.Voucher:
        //            return "V";
        //        case TransactionType.None:
        //            return "0";
        //        default:
        //            return "0";
        //    }
        //}

        //public static TransactionType FromString(this TransactionType t, string s)
        //{
        //    var x = s.ToUpper();

        //    if (x.Equals("P"))
        //    {
        //        return TransactionType.PurchaseCash;
        //    }
        //    else if (x.Equals("L"))
        //    {
        //        return TransactionType.AddCard;
        //    }
        //    else if (x.Equals("N"))
        //    {
        //        return TransactionType.AddPointsToCard;
        //    }
        //    else if (x.Equals("X"))
        //    {
        //        return TransactionType.AuthPIN;
        //    }
        //    else if (x.Equals("B"))
        //    {
        //        return TransactionType.Balance;
        //    }
        //    else if (x.Equals("I"))
        //    {
        //        return TransactionType.CancelVoid;
        //    }
        //    else if (x.Equals("A"))
        //    {
        //        return TransactionType.PreAuth;
        //    }
        //    else if (x.Equals("F"))
        //    {
        //        return TransactionType.FundsTransfer;
        //    }
        //    else if (x.Equals("D"))
        //    {
        //        return TransactionType.Deposit;
        //    }
        //    else if (x.Equals("T"))
        //    {
        //        return TransactionType.TipAdjust;
        //    }
        //    else if (x.Equals("C"))
        //    {
        //        return TransactionType.CashOut;
        //    }
        //    else if (x.Equals("M"))
        //    {
        //        return TransactionType.Completion;
        //    }
        //    else if (x.Equals("K"))
        //    {
        //        return TransactionType.EnhancedPIN;
        //    }
        //    else if (x.Equals("H"))
        //    {
        //        return TransactionType.MiniTransactionHistory;
        //    }
        //    else if (x.Equals(" "))
        //    {
        //        return TransactionType.NotSet;
        //    }
        //    else if (x.Equals("O"))
        //    {
        //        return TransactionType.OrderRequest;
        //    }
        //    else if (x.Equals("R"))
        //    {
        //        return TransactionType.Refund;
        //    }
        //    else if (x.Equals("W"))
        //    {
        //        return TransactionType.Withdrawal;
        //    }
        //    else if (x.Equals("V"))
        //    {
        //        return TransactionType.Voucher;
        //    }
        //    else
        //    {
        //        return TransactionType.None;
        //    }
        //}
    }
}
