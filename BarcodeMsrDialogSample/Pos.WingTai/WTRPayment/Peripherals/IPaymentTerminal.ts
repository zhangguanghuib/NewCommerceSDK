/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

/**
 * Provides interface for interaction with the HW station fiscal register.
 */
export interface IPaymentTerminal {
    /**
     * Checks if HW station payment terminal is ready for accepting payments.
     * @return {Promise<boolean>} True if ready, otherwise false.
     */
    //isReady(): Promise<boolean>;

    /**
         * Makes the payment using payment terminals
         *
         * @param {any} [callerContext] The callback context.
         * @return {IVoidAsyncResult} The async result.
         */
    makePayment(comPort: string, amount: number, tenderType: number, callerContext?: any): Promise<IWTR_PaymentTerminalEntity>;

  
}


export interface IWTR_PaymentTerminalEntity {
    CardNumber?: string;
    CardNumberMasked?: string;
    TransactionDate?: string;
    Amount?: number;
    ExpiryDate?: string;
    EntryMode?: string;
    ApprovalCode?: string;
    ResponseCode?: string;
    TerminalId?: string;
    MerchantId?: string;
    Host?: string;
    TVRTSI?: string;
    AID?: string;
    ApplicationLabel?: string;
    TC?: string;
    CardLabel?: string;
    CardType?: string;
    HostType?: string;
    CommandIdentifier?: string;
    CustomData2?: string;
    CustomData3?: string;
    UTRN?: string;
    InvoiceNumber?: string;
    TransactionInfo?: string;
    BatchNumber?: string;
    CardHolderName?: string;
    ResponseMessage?: string;
    Status?: string;
    RetrievalReferenceNumber?: string;
    Stan?: string;
    TransactionTime?: string;
    StatsMessage?: string;
    EftTerminalID?: string;
    EftCardNumber?: string;
    AquirerName?: string;
    OutputMessage?: string;
    BatchSaleCount?: number;
    BatchSaleAmount?: number;
    BatchRefundCount?: number;
    BatchRefundAmount?: number;
    DataAreaId?: string;
    ShiftId?: number;
    Terminal?: string;
    ChannelId?: number;
    Staff?: string;
    ResponseText?: string;
}

export class WTR_PaymentTerminalEntityClass implements IWTR_PaymentTerminalEntity {
    //ingenico
    public CardNumber: string;
    public TransactionDate: string;
    public Amount: number;
    public ExpiryDate: string;
    public EntryMode: string;
    public RetrievalReferenceNumber: string;
    public ApprovalCode: string;
    public ResponseCode: string;
    public TransactionID: string;
    public MerchantID: string;
    public Host: string;
    //EMV Data
    public TVRTSI: string;
    public AID: string;
    public ApplicationLabel: string;
    public TC: string;
    // End EMV Data
    public CardLabel: string;
    public CardType: string;
    public HostType: string;
    public CommandIdentifier: string;
    public CustomData2: string;
    public CustomData3: string;
    public UTRN: string;
    public InvoiceNumber: string;
    public BatchNumber: string;
    public CardHolderName: string;
    //Unique to NETS Fields
    public Status: string;
    public Stan: string;
    public TransactionTime: string;
    public ResponseMessage: string;
    public StatsMessage: string;
    public EftTerminalID: string;
    public EftCardNumber: string;
    public AquirerName: string;
    public OutputMessage: string;
    public CardNumberMasked: string;
    public CardEntryStatus: string;
    public BatchSaleCount: number;
    public BatchSaleAmount: number;
    public BatchRefundCount: number;
    public BatchRefundAmount: number;
    public ShiftId: number;
    public Terminal: string;
    public ChannelId: number;
    public Staff: string;

    constructor(odataObject?: any) {
        odataObject = odataObject || {};
        this.CardNumber = odataObject.CardNumber;
        this.TransactionDate = odataObject.TransactionDate;
        this.Amount = odataObject.Amount;
        this.ExpiryDate = odataObject.ExpiryDate;
        this.EntryMode = odataObject.EntryMode;
        this.RetrievalReferenceNumber = odataObject.RetrievalReferenceNumber;
        this.ApprovalCode = odataObject.ApprovalCode;
        this.ResponseCode = odataObject.ResponseCode;
        this.TransactionID = odataObject.TransactionID;
        this.MerchantID = odataObject.MerchantID;
        this.Host = odataObject.Host;
        this.TVRTSI = odataObject.TVRTSI;
        this.AID = odataObject.AID;
        this.ApplicationLabel = odataObject.ApplicationLabel;
        this.TC = odataObject.TC;

        this.CardLabel = odataObject.CardLabel;
        this.CardType = odataObject.CardType;
        this.HostType = odataObject.HostType;
        this.CommandIdentifier = odataObject.CommandIdentifier;
        this.CustomData2 = odataObject.CustomData2;
        this.CustomData3 = odataObject.CustomData3;
        this.UTRN = odataObject.UTRN;
        this.InvoiceNumber = odataObject.InvoiceNumber;
        this.BatchNumber = odataObject.BatchNumber;
        this.CardHolderName = odataObject.CardHolderName;
        this.CardNumberMasked = odataObject.CardNumberMasked;
        this.CardEntryStatus = odataObject.CardEntryStatus;
        this.Status = odataObject.Status;
        this.Stan = odataObject.Stan;
        this.TransactionTime = odataObject.TransactionTime;
        this.ResponseMessage = odataObject.ResponseMessage;
        this.StatsMessage = odataObject.StatsMessage;
        this.EftTerminalID = odataObject.EftTerminalID;
        this.EftCardNumber = odataObject.EftCardNumber;
        this.AquirerName = odataObject.AquirerName;
        this.OutputMessage = odataObject.OutputMessage;
        this.BatchSaleCount = odataObject.BatchSaleCount;
        this.BatchSaleAmount = odataObject.BatchSaleAmount;
        this.BatchRefundAmount = odataObject.BatchRefundAmount;
        this.BatchRefundCount = odataObject.BatchRefundCount;
        this.ShiftId = odataObject.ShiftId;
    }
}



export enum WTR_PaymentTerminalType {
    None = 0,
    CreditCard = 1,
    Nets = 2
}

export enum WTR_PaymentTerminalEx {
    CREDITCARD = 10002,
    NETS = 10003,
    FLASHPAY = 10004,
    CONTACTLESS = 10005,
}