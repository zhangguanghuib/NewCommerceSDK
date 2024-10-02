
import { ExtensionOperationRequestType, ExtensionOperationRequestHandlerBase } from "PosApi/Create/Operations";
import RefundTerminalTenderLineToCartResponse from "./RefundTerminalTenderLineToCartResponse";
import RefundTerminalTenderLineToCartRequest from "./RefundTerminalTenderLineToCartRequest";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { GetDeviceConfigurationClientRequest, GetDeviceConfigurationClientResponse } from "PosApi/Consume/Device";
import { AddPreprocessedTenderLineToCartClientRequest, AddPreprocessedTenderLineToCartClientResponse, ConcludeTransactionClientRequest, ConcludeTransactionClientResponse, GetCurrentCartClientRequest, GetCurrentCartClientResponse, RefreshCartClientRequest, RefreshCartClientResponse } from "PosApi/Consume/Cart";
import MessageDialog from "../../Dialogs/DialogSample/MessageDialog";

export default class RefundTerminalTenderLineToCartHandler<TResponse extends RefundTerminalTenderLineToCartResponse>
    extends ExtensionOperationRequestHandlerBase<TResponse> {

    public supportedRequestType(): ExtensionOperationRequestType<TResponse> {
        return RefundTerminalTenderLineToCartRequest;
    }

    public executeAsync(request: Commerce.OperationRequest<TResponse>): Promise<Commerce.Client.Entities.ICancelableDataResult<TResponse>> {

        const correlationId: string = this.context.logger.getNewCorrelationId();

        return Promise.all([
            this.context.runtime.executeAsync(new GetCurrentCartClientRequest<GetCurrentCartClientResponse>())
                .then((result: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>): ProxyEntities.Cart => {
                    return result.data.result;
                }),

            this.context.runtime.executeAsync(new GetDeviceConfigurationClientRequest())
                .then((response: ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>): ProxyEntities.DeviceConfiguration => {
                    return response.data.result;
                })
        ]).then((results: any[]): Promise<ClientEntities.ICancelableDataResult<AddPreprocessedTenderLineToCartClientResponse>> => {
            let currentCart: ProxyEntities.Cart = results[0];
            let deviceConfiguration: ProxyEntities.DeviceConfiguration = results[1];

            if (currentCart.AmountDue > 0) { //Based on UAT feedback allow credit cards to have negative amount
                MessageDialog.show(this.context, "The Amout Due should less than zero").then(() => {
                    return Promise.resolve({
                        canceled: true,
                        data: null
                    });
                });
            }
            return this.beginProcessingManualCardPayment("3", deviceConfiguration.Currency, currentCart.AmountDue, "VISA", currentCart.CustomerId);
        }).then((result: ClientEntities.ICancelableDataResult<AddPreprocessedTenderLineToCartClientResponse>) => {
            let currentCart: ProxyEntities.Cart = result.data.result;
            if (currentCart.AmountDue === 0) {
                let concludeTransactionClientRequest: ConcludeTransactionClientRequest<ConcludeTransactionClientResponse> = new ConcludeTransactionClientRequest(correlationId);
                this.context.runtime.executeAsync(concludeTransactionClientRequest);
            } else {
                let refreshCartClientRequest: RefreshCartClientRequest<RefreshCartClientResponse> = new RefreshCartClientRequest();
                this.context.runtime.executeAsync(refreshCartClientRequest);
            }
        }).then(() => {
            this.context.navigator.navigateToPOSView("CartView");
            return Promise.resolve({
                canceled: false,
                data: null
            });
        }).catch((reason: any) => {
            return MessageDialog.show(this.context, reason + '').then(() => {
                return Promise.resolve({
                    canceled: true,
                    data: null
                });
            });
        });
    }


    public beginProcessingManualCardPayment(tenderTypeId: string, currency: string, amountTendered: number, cardType: string, customerId: string)
        : Promise<ClientEntities.ICancelableDataResult<AddPreprocessedTenderLineToCartClientResponse>> {
        let preProcessedTenderLine: ProxyEntities.TenderLine = {
            Amount: amountTendered,
            TenderTypeId: tenderTypeId, // Card
            Currency: currency,
            CardTypeId: cardType,
            IsPreProcessed: false,
            IsDeposit: false,
            IsVoidable: true,
            //Authorization: "<![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfPaymentProperty xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <PaymentProperty>\r\n    <Namespace>MerchantAccount</Namespace>\r\n    <Name>AssemblyName</Name>\r\n    <ValueType>String</ValueType>\r\n    <StoredStringValue>Contoso.PaymentDeviceSample</StoredStringValue>\r\n    <DecimalValue>0</DecimalValue>\r\n    <DateValue>0001-01-01T00:00:00</DateValue>\r\n    <SecurityLevel>None</SecurityLevel>\r\n    <IsEncrypted>false</IsEncrypted>\r\n    <IsPassword>false</IsPassword>\r\n    <IsReadOnly>false</IsReadOnly>\r\n    <IsHidden>false</IsHidden>\r\n    <DisplayHeight>1</DisplayHeight>\r\n    <SequenceNumber>0</SequenceNumber>\r\n  </PaymentProperty>\r\n  <PaymentProperty>\r\n    <Namespace>Connector</Namespace>\r\n    <Name>ConnectorName</Name>\r\n    <ValueType>String</ValueType>\r\n    <StoredStringValue>TestConnector</StoredStringValue>\r\n    <DecimalValue>0</DecimalValue>\r\n    <DateValue>0001-01-01T00:00:00</DateValue>\r\n    <SecurityLevel>None</SecurityLevel>\r\n    <IsEncrypted>false</IsEncrypted>\r\n    <IsPassword>false</IsPassword>\r\n    <IsReadOnly>false</IsReadOnly>\r\n    <IsHidden>false</IsHidden>\r\n    <DisplayHeight>1</DisplayHeight>\r\n    <SequenceNumber>0</SequenceNumber>\r\n  </PaymentProperty>\r\n</ArrayOfPaymentProperty>]]>",
            Authorization: "<![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfPaymentProperty xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <PaymentProperty>\r\n    <Namespace>Connector</Namespace>\r\n    <Name>ConnectorName</Name>\r\n    <ValueType>String</ValueType>\r\n    <StoredStringValue>TestConnector</StoredStringValue>\r\n    <DecimalValue>0</DecimalValue>\r\n    <DateValue>0001-01-01T00:00:00</DateValue>\r\n    <SecurityLevel>None</SecurityLevel>\r\n    <IsEncrypted>false</IsEncrypted>\r\n    <IsPassword>false</IsPassword>\r\n    <IsReadOnly>false</IsReadOnly>\r\n    <IsHidden>false</IsHidden>\r\n    <DisplayHeight>1</DisplayHeight>\r\n    <SequenceNumber>0</SequenceNumber>\r\n  </PaymentProperty>\r\n</ArrayOfPaymentProperty>]]>",
            MaskedCardNumber: '411111****** 1111',
            TransactionStatusValue: 0,
            IncomeExpenseAccountTypeValue: -1,
            CashBackAmount: 0,
            AmountInTenderedCurrency: amountTendered,
            AmountInCompanyCurrency: amountTendered,
            ReasonCodeLines: [],
            CustomerId: customerId,
            IsChangeLine: false,
            IsHistorical: false,
            StatusValue: 4,
            VoidStatusValue: 0,
            ExtensionProperties: []
        };

        return this.addTenderToCart(preProcessedTenderLine);
    }

    public addTenderToCart(preProcessedTenderLine: ProxyEntities.TenderLine): Promise<ClientEntities.ICancelableDataResult<AddPreprocessedTenderLineToCartClientResponse>> {

        let addPreProcessedTenderLineToCartClientRequest: AddPreprocessedTenderLineToCartClientRequest<AddPreprocessedTenderLineToCartClientResponse> =
            new AddPreprocessedTenderLineToCartClientRequest<AddPreprocessedTenderLineToCartClientResponse>(preProcessedTenderLine);

        return this.context.runtime.executeAsync(addPreProcessedTenderLineToCartClientRequest)
            .then((result) => {
                return result;
            }).catch((reason: any) => {
                return MessageDialog.show(this.context, reason).then(() => {
                    this.context.logger.logInformational("MessageDialog closed");
                    return Promise.resolve({ canceled: true, data: null })
                });
            });
    }
}