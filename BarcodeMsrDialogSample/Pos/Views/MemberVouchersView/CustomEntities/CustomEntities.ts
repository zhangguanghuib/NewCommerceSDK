import { ProxyEntities } from "PosApi/Entities";


export interface IMemberVoucher {
    Id: number;
    VoucherCode: string;
    VoucherNumber: string;
    VoucherDescription: string;
    VoucherName: string;
    IssueDate: Date;
    CollectDate: Date;
    PrintDate: Date;
    UtilizeDate: Date;
    SalePerson: string;
    Registrator: string;
    MemberNumber: string;
    IssueReceipt: string;
    CollectReceipt: string;
    PrintReceipt: string;
    UtilizeReceipt: string;
    IssueLocationCode: string;
    CollectLocationCode: string;
    PrintLocationCode: string;
    UtilizeLocationCode: string;
    Status: string;
    ReferenceNumber: string;
    VoucherText: string;
    Source: string;
    ValidFrom: Date;
    ExpiryDate: Date;
    IssueLocation: string;
    CollectLocation: string;
    PrintLocation: string;
    UtilizeLocation: string;
    DenominationValue: number;
    DenominationCurrency: string;
    CostValue: number;
    CostCurrency: string;
    CanPrint: boolean;
    NeedApproval: boolean;
    VoucherType: string;
    VoucherTypeCode: string;
    VoucherSubType: string;
    VoucherSubTypeCode: string;
    Issuer: string;
    Description: string;
    CanUtilizeInLocation: string;
    ShortDescription: string;
    IsUtilize: number;
    Message: string;
    DollarValue: number;
    DiscountPercentage: number;
    ExtensionProperties: ProxyEntities.CommerceProperty[];
}


export interface IConfirm {
    yesno: ConfirmOption;
    displayText: string;
}

export enum ConfirmOption {
    Yes = 0,
    No = 1,
}

export interface IValueCaptionPair {
    value: number;
    caption: string;
}