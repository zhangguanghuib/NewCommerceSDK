namespace Contoso
{
    namespace Commerce.HardwareStation.PaymentSample
    {
        using System;
        using System.Collections.Generic;
        using System.Diagnostics.CodeAnalysis;
        using System.IO;
        using System.Linq;
        using System.Threading;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.CardPayment;
        using Microsoft.Dynamics.Commerce.HardwareStation.PeripheralRequests;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.PaymentTerminal;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.VirtualPeripherals.Framework;
        using Microsoft.Dynamics.Commerce.VirtualPeripherals.MessagePipelineHandler;
        using Microsoft.Dynamics.Commerce.VirtualPeripherals.MessagePipelineProxy;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;
        using Newtonsoft.Json.Linq;
        using PSDK = Microsoft.Dynamics.Retail.PaymentSDK.Portable;

        /// <summary>
        /// <c>Simulator</c> manager payment device class.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "long running task, so the file system watcher will be disposed when the program ends")]
#pragma warning disable CS0618 // JUSTIFICATION: Pending migration to asynchronous APIs.
        public class PaymentDeviceSample : INamedRequestHandler
#pragma warning restore CS0618
        {
            private const string PaymentTerminalDevice = "MOCKPAYMENTTERMINAL";
            private const string PaymentDeviceSimulatorFileName = "PaymentDeviceSimulator";
            private const int TaskDelayInMilliSeconds = 10;

            // Cache to store credit card number, the key will be returned to client in Authorization payment sdk blob.
            private static Dictionary<Guid, TemporaryCardMemoryStorage<string>> cardCache = new Dictionary<Guid, TemporaryCardMemoryStorage<string>>();
            private readonly string deviceSimFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            private CancellationTokenSource timeoutTask;
            private PSDK.PaymentProperty[] merchantProperties;
            private SettingsInfo terminalSettings;
            private string paymentConnectorName;
            private TenderInfo tenderInfo;
            private PSDK.IPaymentProcessor processor;

            /// <summary>
            /// Initializes a new instance of the <see cref="PaymentDeviceSample"/> class.
            /// </summary>
            public PaymentDeviceSample()
            {
                // Do nothing.
            }

            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(LockPaymentTerminalDeviceRequest),
                        typeof(ReleasePaymentTerminalDeviceRequest),
                        typeof(OpenPaymentTerminalDeviceRequest),
                        typeof(BeginTransactionPaymentTerminalDeviceRequest),
                        typeof(UpdateLineItemsPaymentTerminalDeviceRequest),
                        typeof(CancelOperationPaymentTerminalDeviceRequest),
                        typeof(AuthorizePaymentTerminalDeviceRequest),
                        typeof(CapturePaymentTerminalDeviceRequest),
                        typeof(VoidPaymentTerminalDeviceRequest),
                        typeof(RefundPaymentTerminalDeviceRequest),
                        typeof(FetchTokenPaymentTerminalDeviceRequest),
                        typeof(EndTransactionPaymentTerminalDeviceRequest),
                        typeof(ClosePaymentTerminalDeviceRequest),
                        typeof(ActivateGiftCardPaymentTerminalRequest),
                        typeof(AddBalanceToGiftCardPaymentTerminalRequest),
                        typeof(GetGiftCardBalancePaymentTerminalRequest),
                        typeof(GetPrivateTenderPaymentTerminalDeviceRequest)
                    };
                }
            }

            /// <summary>
            /// Gets the specify the name of the request handler.
            /// </summary>
            public string HandlerName
            {
                get
                {
                    return PaymentDeviceSample.PaymentTerminalDevice;
                }
            }

            /// <summary>
            /// Executes the payment device simulator operation based on the incoming request type.
            /// </summary>
            /// <param name="request">The payment terminal device simulator request message.</param>
            /// <returns>Returns the payment terminal device simulator response.</returns>
            public Response Execute(Request request)
            {
                Microsoft.Dynamics.Commerce.Runtime.ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case OpenPaymentTerminalDeviceRequest openPaymentTerminalDeviceRequest:
                        this.Open(openPaymentTerminalDeviceRequest);
                        break;
                    case BeginTransactionPaymentTerminalDeviceRequest beginTransactionPaymentTerminalDeviceRequest:
                        this.BeginTransaction(beginTransactionPaymentTerminalDeviceRequest);
                        break;
                    case UpdateLineItemsPaymentTerminalDeviceRequest updateLineItemsPaymentTerminalDeviceRequest:
                        this.UpdateLineItems(updateLineItemsPaymentTerminalDeviceRequest);
                        break;
                    case AuthorizePaymentTerminalDeviceRequest authorizePaymentTerminalDeviceRequest:
                        return this.AuthorizePayment(authorizePaymentTerminalDeviceRequest);
                    case CapturePaymentTerminalDeviceRequest capturePaymentTerminalDeviceRequest:
                        return this.CapturePayment(capturePaymentTerminalDeviceRequest);
                    case VoidPaymentTerminalDeviceRequest voidPaymentTerminalDeviceRequest:
                        return this.VoidPayment(voidPaymentTerminalDeviceRequest);
                    case RefundPaymentTerminalDeviceRequest refundPaymentTerminalDeviceRequest:
                        return this.RefundPayment(refundPaymentTerminalDeviceRequest);
                    case FetchTokenPaymentTerminalDeviceRequest fetchTokenPaymentTerminalDeviceRequest:
                        return this.FetchToken(fetchTokenPaymentTerminalDeviceRequest);
                    case EndTransactionPaymentTerminalDeviceRequest endTransactionPaymentTerminalDeviceRequest:
                        this.EndTransaction(endTransactionPaymentTerminalDeviceRequest);
                        break;
                    case ClosePaymentTerminalDeviceRequest closePaymentTerminalDeviceRequest:
                        this.Close(closePaymentTerminalDeviceRequest);
                        break;
                    case CancelOperationPaymentTerminalDeviceRequest cancelOperationPaymentTerminalDeviceRequest:
                        this.CancelOperation(cancelOperationPaymentTerminalDeviceRequest);
                        break;
                    case ActivateGiftCardPaymentTerminalRequest activateGiftCardPaymentTerminalRequest:
                        return this.ActivateGiftCard(activateGiftCardPaymentTerminalRequest);
                    case AddBalanceToGiftCardPaymentTerminalRequest addBalanceToGiftCardPaymentTerminalRequest:
                        return this.AddBalanceToGiftCard(addBalanceToGiftCardPaymentTerminalRequest);
                    case GetGiftCardBalancePaymentTerminalRequest getGiftCardBalancePaymentTerminalRequest:
                        return this.GetGiftCardBalance((GetGiftCardBalancePaymentTerminalRequest)request);
                    default:
                        throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }

                return new NullResponse();
            }

            /// <summary>
            /// Opens the payment terminal device.
            /// </summary>
            /// <param name="request">The open payment terminal device request.</param>
            public void Open(OpenPaymentTerminalDeviceRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                Utilities.WaitAsyncTask(() => this.OpenAsync(request.DeviceName, request.TerminalSettings, request.DeviceConfig));
            }

            /// <summary>
            ///  Begins the transaction.
            /// </summary>
            /// <param name="request">The begin transaction request.</param>
            public void BeginTransaction(BeginTransactionPaymentTerminalDeviceRequest request)
            {
                Utilities.WaitAsyncTask(() => Task.Run(async () =>
                {
                    PSDK.PaymentProperty[] merchantProperties = CardPaymentManager.ToLocalProperties(request.MerchantInformation);

                    await this.BeginTransactionAsync(merchantProperties, request.PaymentConnectorName, request.InvoiceNumber, request.IsTestMode);
                }));
            }

            /// <summary>
            /// Update the line items.
            /// </summary>
            /// <param name="request">The update line items request.</param>
            public void UpdateLineItems(UpdateLineItemsPaymentTerminalDeviceRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                Utilities.WaitAsyncTask(() => this.UpdateLineItemsAsync(request.TotalAmount, request.TaxAmount, request.DiscountAmount, request.SubTotalAmount, request.Items));
            }

            /// <summary>
            /// Authorize payment.
            /// </summary>
            /// <param name="request">The authorize payment request.</param>
            /// <returns>The authorize payment response.</returns>
            public AuthorizePaymentTerminalDeviceResponse AuthorizePayment(AuthorizePaymentTerminalDeviceRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                PaymentInfo paymentInfo = Utilities.WaitAsyncTask(() => this.AuthorizePaymentAsync(request.Amount, request.Currency, request.VoiceAuthorization, request.IsManualEntry, request.ExtensionTransactionProperties));

                return new AuthorizePaymentTerminalDeviceResponse(paymentInfo);
            }

            /// <summary>
            /// Capture payment.
            /// </summary>
            /// <param name="request">The capture payment request.</param>
            /// <returns>The capture payment response.</returns>
            public CapturePaymentTerminalDeviceResponse CapturePayment(CapturePaymentTerminalDeviceRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                PSDK.PaymentProperty[] merchantProperties = CardPaymentManager.ToLocalProperties(request.PaymentPropertiesXml);
                PaymentInfo paymentInfo = Utilities.WaitAsyncTask(() => this.CapturePaymentAsync(request.Amount, request.Currency, merchantProperties, request.ExtensionTransactionProperties));

                return new CapturePaymentTerminalDeviceResponse(paymentInfo);
            }

            /// <summary>
            /// Voids payment.
            /// </summary>
            /// <param name="request">The void payment request.</param>
            /// <returns>The void payment response.</returns>
            public VoidPaymentTerminalDeviceResponse VoidPayment(VoidPaymentTerminalDeviceRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                PSDK.PaymentProperty[] merchantProperties = CardPaymentManager.ToLocalProperties(request.PaymentPropertiesXml);
                PaymentInfo paymentInfo = Utilities.WaitAsyncTask(() => this.VoidPaymentAsync(request.Amount, request.Currency, merchantProperties, request.ExtensionTransactionProperties));

                return new VoidPaymentTerminalDeviceResponse(paymentInfo);
            }

            /// <summary>
            /// Refund payment.
            /// </summary>
            /// <param name="request">The refund payment request.</param>
            /// <returns>The refund payment response.</returns>
            public RefundPaymentTerminalDeviceResponse RefundPayment(RefundPaymentTerminalDeviceRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                PaymentInfo paymentInfo = Utilities.WaitAsyncTask(() => this.RefundPaymentAsync(request.Amount, request.Currency, request.IsManualEntry, request.ExtensionTransactionProperties));

                return new RefundPaymentTerminalDeviceResponse(paymentInfo);
            }

            /// <summary>
            /// Fetch token.
            /// </summary>
            /// <param name="request">The fetch token request.</param>
            /// <returns>The fetch token response.</returns>
            public FetchTokenPaymentTerminalDeviceResponse FetchToken(FetchTokenPaymentTerminalDeviceRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                PaymentInfo paymentInfo = Utilities.WaitAsyncTask(() => this.FetchTokenAsync(request.IsManualEntry, request.ExtensionTransactionProperties));

                return new FetchTokenPaymentTerminalDeviceResponse(paymentInfo);
            }

            /// <summary>
            /// Ends the transaction.
            /// </summary>
            /// <param name="request">The end transaction request.</param>
            public void EndTransaction(EndTransactionPaymentTerminalDeviceRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                Utilities.WaitAsyncTask(() => this.EndTransactionAsync());
            }

            /// <summary>
            /// Closes the payment terminal device.
            /// </summary>
            /// <param name="request">The close payment terminal request.</param>
            public void Close(ClosePaymentTerminalDeviceRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                Utilities.WaitAsyncTask(() => this.CloseAsync());
            }

            /// <summary>
            /// Cancels the operation.
            /// </summary>
            /// <param name="request">The cancel operation request.</param>
            public void CancelOperation(CancelOperationPaymentTerminalDeviceRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                Utilities.WaitAsyncTask(() => this.CancelOperationAsync());
            }

            /// <summary>
            /// Activate the gift card.
            /// </summary>
            /// <param name="request">The activate gift card request.</param>
            /// <returns>The gift card payment response.</returns>
            public GiftCardPaymentResponse ActivateGiftCard(ActivateGiftCardPaymentTerminalRequest request)
            {
                Microsoft.Dynamics.Commerce.Runtime.ThrowIf.Null(request, "request");

                PaymentInfo paymentInfo = Utilities.WaitAsyncTask(() => this.ActivateGiftCardAsync(request.PaymentConnectorName, request.Amount, request.Currency, request.TenderInfo, request.ExtensionTransactionProperties));

                return new GiftCardPaymentResponse(paymentInfo);
            }

            /// <summary>
            /// Add balance to the gift card.
            /// </summary>
            /// <param name="request">The add balance to gift card request.</param>
            /// <returns>The gift card payment response.</returns>
            public GiftCardPaymentResponse AddBalanceToGiftCard(AddBalanceToGiftCardPaymentTerminalRequest request)
            {
                Microsoft.Dynamics.Commerce.Runtime.ThrowIf.Null(request, "request");
                Microsoft.Dynamics.Commerce.Runtime.ThrowIf.Null(request.TenderInfo, "tenderInfo");

                PaymentInfo paymentInfo = Utilities.WaitAsyncTask(() => this.AddBalanceToGiftCardAsync(request.PaymentConnectorName, request.Amount, request.Currency, request.TenderInfo, request.ExtensionTransactionProperties));

                return new GiftCardPaymentResponse(paymentInfo);
            }

            /// <summary>
            /// Get gift card balance.
            /// </summary>
            /// <param name="request">The get gift card balance request.</param>
            /// <returns>The gift card payment response.</returns>
            public GiftCardPaymentResponse GetGiftCardBalance(GetGiftCardBalancePaymentTerminalRequest request)
            {
                Microsoft.Dynamics.Commerce.Runtime.ThrowIf.Null(request, "request");
                Microsoft.Dynamics.Commerce.Runtime.ThrowIf.Null(request.TenderInfo, "tenderInfo");

                PaymentInfo paymentInfo = Utilities.WaitAsyncTask(() => this.GetGiftCardBalanceAsync(request.PaymentConnectorName, request.Currency, request.TenderInfo, request.ExtensionTransactionProperties));

                return new GiftCardPaymentResponse(paymentInfo);
            }

            /// <summary>
            /// Make authorization payment.
            /// </summary>
            /// <param name="amount">The amount.</param>
            /// <param name="currency">The currency.</param>
            /// <param name="voiceAuthorization">The voice approval code (optional).</param>
            /// <param name="isManualEntry">If manual credit card entry is required.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the authorization has completed.</returns>
            public async Task<PaymentInfo> AuthorizePaymentAsync(decimal amount, string currency, string voiceAuthorization, bool isManualEntry, ExtensionTransaction extensionTransactionProperties)
            {
                try
                {
                    var info = new JObject();

                    info["Amount"] = amount;
                    info["Currency"] = currency;
                    info["VoiceAuthorization"] = voiceAuthorization;
                    info["IsManualEntry"] = isManualEntry;

                    string serializedInfo = info.ToString();

                    /* this.CardSwipeHandler(); */

                    var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.AuthorizationData));
                    paymentTerminalPipeline.AuthorizePayment(serializedInfo);

                    PaymentInfo paymentInfo = new PaymentInfo();

                    // Get tender
                    TenderInfo maskedTenderInfo = await this.FetchTenderInfoAsync();
                    if (maskedTenderInfo == null)
                    {
                        return paymentInfo;
                    }

                    paymentInfo.CardNumberMasked = Utilities.GetMaskedCardNumber(maskedTenderInfo.CardNumber);
                    paymentInfo.CashbackAmount = maskedTenderInfo.CashBackAmount;
                    paymentInfo.CardType = (Microsoft.Dynamics.Commerce.HardwareStation.CardPayment.CardType)maskedTenderInfo.CardTypeId;

                    if (paymentInfo.CashbackAmount > this.terminalSettings.DebitCashbackLimit)
                    {
                        throw new CardPaymentException(CardPaymentException.CashbackAmountExceedsLimit, "Cashback amount exceeds the maximum amount allowed.");
                    }

                    // Authorize
                    PSDK.Response response = CardPaymentManager.InvokeAuthorizationCall(this.merchantProperties, this.paymentConnectorName, this.tenderInfo, amount, currency, this.terminalSettings.Locale, true, this.terminalSettings.TerminalId, extensionTransactionProperties);

                    Guid cardStorageKey = Guid.NewGuid();
                    CardPaymentManager.MapAuthorizeResponse(response, paymentInfo, cardStorageKey, this.terminalSettings.TerminalId);

                    if (paymentInfo.IsApproved)
                    {
                        // Backup credit card number
                        TemporaryCardMemoryStorage<string> cardStorage = new TemporaryCardMemoryStorage<string>(DateTime.UtcNow, this.tenderInfo.CardNumber);
                        cardStorage.StorageInfo = paymentInfo.PaymentSdkData;
                        cardCache.Add(cardStorageKey, cardStorage);

                        // need signature?
                        if (this.terminalSettings.SignatureCaptureMinimumAmount < paymentInfo.ApprovedAmount)
                        {
                            paymentInfo.SignatureData = await this.RequestTenderApprovalAsync(paymentInfo.ApprovedAmount);
                        }
                    }

                    return paymentInfo;
                }
                catch (Exception ex)
                {
                    var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.Error));
                    paymentTerminalPipeline.SendError(ex.Message);
                    throw;
                }
            }

            /// <summary>
            ///  Begins the transaction.
            /// </summary>
            /// <param name="merchantProperties">The merchant provider payment properties for the peripheral device.</param>
            /// <param name="paymentConnectorName">The payment connector name.</param>
            /// <param name="invoiceNumber">The invoice number associated with the transaction (6 characters long).</param>
            /// <param name="isTestMode">Is test mode for payments enabled for the peripheral device.</param>
            /// <returns>A task that can be awaited until the begin transaction screen is displayed.</returns>
            public async Task BeginTransactionAsync(PSDK.PaymentProperty[] merchantProperties, string paymentConnectorName, string invoiceNumber, bool isTestMode)
            {
                var beginTransactionTask = Task.Factory.StartNew(() =>
                {
                    this.merchantProperties = merchantProperties;
                    this.paymentConnectorName = paymentConnectorName;

                    var info = new JObject();

                    info["PaymentConnectorName"] = paymentConnectorName;
                    info["InvoiceNumber"] = invoiceNumber;
                    info["IsTestMode"] = isTestMode;

                    FillPaymentProperties(merchantProperties, info);

                    string serializedInfo = info.ToString();

                    var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.PaymentProperties));
                    paymentTerminalPipeline.BeginTransaction(serializedInfo);
                });

                await beginTransactionTask;
            }

            /// <summary>
            ///  Cancels an existing GetTender or RequestTenderApproval  operation on the payment terminal.
            /// </summary>
            /// <returns>A task that can be awaited until the operation is cancelled.</returns>
            public async Task CancelOperationAsync()
            {
                await Task.Delay(PaymentDeviceSample.TaskDelayInMilliSeconds);
            }

            /// <summary>
            /// Make settlement of a payment.
            /// </summary>
            /// <param name="amount">The amount.</param>
            /// <param name="currency">The currency.</param>
            /// <param name="paymentProperties">The payment properties of the authorization response.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the settlement has completed.</returns>
            public Task<PaymentInfo> CapturePaymentAsync(decimal amount, string currency, PSDK.PaymentProperty[] paymentProperties, ExtensionTransaction extensionTransactionProperties)
            {
                try
                {
                    var info = new JObject();

                    info["Amount"] = amount;
                    info["Currency"] = currency;
                    FillPaymentProperties(paymentProperties, info);

                    string serializedInfo = info.ToString();

                    var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.PaymentDetails));
                    paymentTerminalPipeline.CapturePayment(serializedInfo);

                    if (amount < this.terminalSettings.MinimumAmountAllowed)
                    {
                        throw new CardPaymentException(CardPaymentException.AmountLessThanMinimumLimit, "Amount does not meet minimum amount allowed.");
                    }

                    if (this.terminalSettings.MaximumAmountAllowed > 0 && amount > this.terminalSettings.MaximumAmountAllowed)
                    {
                        throw new CardPaymentException(CardPaymentException.AmountExceedsMaximumLimit, "Amount exceeds the maximum amount allowed.");
                    }

                    if (this.processor == null)
                    {
                        this.processor = CardPaymentManager.GetPaymentProcessor(this.merchantProperties, this.paymentConnectorName);
                    }

                    PaymentInfo paymentInfo = new PaymentInfo();

                    // Handle multiple chain connectors by returning single instance used in capture.
                    PSDK.IPaymentProcessor currentProcessor = null;
                    PSDK.PaymentProperty[] currentMerchantProperties = null;
                    CardPaymentManager.GetRequiredConnector(this.merchantProperties, paymentProperties, this.processor, out currentProcessor, out currentMerchantProperties);

                    PSDK.Request request = CardPaymentManager.GetCaptureRequest(currentMerchantProperties, paymentProperties, amount, currency, this.terminalSettings.Locale, true, this.terminalSettings.TerminalId, cardCache, extensionTransactionProperties);
                    PSDK.Response response = currentProcessor.Capture(request);
                    CardPaymentManager.MapCaptureResponse(response, paymentInfo);

                    return Task.FromResult(paymentInfo);
                }
                catch (Exception ex)
                {
                    var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.Error));
                    paymentTerminalPipeline.SendError(ex.Message);
                    throw;
                }
            }

            /// <summary>
            ///  Closes a connection to the payment terminal.
            /// </summary>
            /// <returns>A task that can be awaited until the connection is closed.</returns>
            public async Task CloseAsync()
            {
                await Task.Delay(PaymentDeviceSample.TaskDelayInMilliSeconds);
            }

            /// <summary>
            ///  Ends the transaction.
            /// </summary>
            /// <returns>A task that can be awaited until the end transaction screen is displayed.</returns>
            public async Task EndTransactionAsync()
            {
                await Task.Delay(PaymentDeviceSample.TaskDelayInMilliSeconds);
            }

            /// <summary>
            /// Fetch token for credit card.
            /// </summary>
            /// <param name="isManualEntry">The value indicating whether credit card should be entered manually.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the token generation has completed.</returns>
            public async Task<PaymentInfo> FetchTokenAsync(bool isManualEntry, ExtensionTransaction extensionTransactionProperties)
            {
                PaymentInfo paymentInfo = new PaymentInfo();

                // Get tender
                TenderInfo maskedTenderInfo = await this.FetchTenderInfoAsync();

                PSDK.PaymentProperty[] defaultMerchantProperties = this.merchantProperties;

                paymentInfo.CardNumberMasked = maskedTenderInfo.CardNumber;
                paymentInfo.CashbackAmount = maskedTenderInfo.CashBackAmount;
                paymentInfo.CardType = (Microsoft.Dynamics.Commerce.HardwareStation.CardPayment.CardType)maskedTenderInfo.CardTypeId;

                if (this.merchantProperties != null &&
                    this.merchantProperties.Length > 0 &&
                    this.merchantProperties[0].Namespace.Equals(GenericNamespace.Connector) &&
                    this.merchantProperties[0].Name.Equals(ConnectorProperties.Properties))
                {
                    defaultMerchantProperties = this.merchantProperties[0].PropertyList;
                }

                if (this.processor == null)
                {
                    this.processor = CardPaymentManager.GetPaymentProcessor(this.merchantProperties, this.paymentConnectorName);
                }

                // Generate card token
                PSDK.Request request = CardPaymentManager.GetTokenRequest(defaultMerchantProperties, this.tenderInfo, this.terminalSettings.Locale, extensionTransactionProperties);
                PSDK.Response response = this.processor.GenerateCardToken(request, null);
                CardPaymentManager.MapTokenResponse(response, paymentInfo);

                return paymentInfo;
            }

            /// <summary>
            /// Open payment device using simulator.
            /// </summary>
            /// <param name="peripheralName">Name of peripheral device.</param>
            /// <param name="terminalSettings">The terminal settings for the peripheral device.</param>
            /// <param name="deviceConfig">Device Configuration parameters.</param>
            /// <returns>A task that can be awaited until the connection is opened.</returns>
            [Obsolete("Obsolete since version 8.1.3. This method will be removed once IPaymentDevice is deprecated.")]
            public async Task OpenAsync(string peripheralName, SettingsInfo terminalSettings, IDictionary<string, string> deviceConfig)
            {
                await Task.Delay(PaymentDeviceSample.TaskDelayInMilliSeconds);
            }

            /// <summary>
            /// Open payment device using simulator.
            /// </summary>
            /// <param name="peripheralName">Name of peripheral device.</param>
            /// <param name="terminalSettings">The terminal settings for the peripheral device.</param>
            /// <param name="deviceConfig">Device Configuration parameters.</param>
            /// <returns>A task that can be awaited until the connection is opened.</returns>
            public async Task OpenAsync(string peripheralName, SettingsInfo terminalSettings, Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.PeripheralConfiguration deviceConfig)
            {
                var openTask = Task.Factory.StartNew(() =>
                {
                    this.terminalSettings = terminalSettings;

                    var info = new JObject();

                    info["PeripheralName"] = peripheralName;
                    info["TerminalSettings"] = new JObject();

                    if (terminalSettings != null)
                    {
                        info["TerminalSettings"]["SignatureCaptureMinimumAmount"] = terminalSettings.SignatureCaptureMinimumAmount;
                        info["TerminalSettings"]["MinimumAmountAllowed"] = terminalSettings.MinimumAmountAllowed;
                        info["TerminalSettings"]["MaximumAmountAllowed"] = terminalSettings.MaximumAmountAllowed;
                        info["TerminalSettings"]["DebitCashbackLimit"] = terminalSettings.DebitCashbackLimit;
                        info["TerminalSettings"]["Locale"] = terminalSettings.Locale;
                        info["TerminalSettings"]["TerminalId"] = terminalSettings.TerminalId;
                    }

                    string serializedInfo = info.ToString();

                    var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.PaymentOpenTerminal));
                    paymentTerminalPipeline.OpenTransaction(serializedInfo);
                });

                await openTask;
            }

            /// <summary>
            /// Make refund payment.
            /// </summary>
            /// <param name="amount">The amount.</param>
            /// <param name="currency">The currency.</param>
            /// <param name="isManualEntry">If manual credit card entry is required.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the refund has completed.</returns>
            public async Task<PaymentInfo> RefundPaymentAsync(decimal amount, string currency, bool isManualEntry, ExtensionTransaction extensionTransactionProperties)
            {
                try
                {
                    var info = new JObject();

                    info["Amount"] = amount;
                    info["Currency"] = currency;
                    info["IsManualEntry"] = isManualEntry;

                    string serializedInfo = info.ToString();
                    File.WriteAllText(string.Format(@"{0}\{1}_RefundPayment.json", this.deviceSimFolderPath, PaymentDeviceSimulatorFileName), serializedInfo);

                    if (amount < this.terminalSettings.MinimumAmountAllowed)
                    {
                        throw new CardPaymentException(CardPaymentException.AmountLessThanMinimumLimit, "Amount does not meet minimum amount allowed.");
                    }

                    if (this.terminalSettings.MaximumAmountAllowed > 0 && amount > this.terminalSettings.MaximumAmountAllowed)
                    {
                        throw new CardPaymentException(CardPaymentException.AmountExceedsMaximumLimit, "Amount exceeds the maximum amount allowed.");
                    }

                    if (this.processor == null)
                    {
                        this.processor = CardPaymentManager.GetPaymentProcessor(this.merchantProperties, this.paymentConnectorName);
                    }

                    PaymentInfo paymentInfo = new PaymentInfo();

                    // Get tender
                    TenderInfo maskedTenderInfo = await this.FetchTenderInfoAsync();
                    if (maskedTenderInfo == null)
                    {
                        return paymentInfo;
                    }

                    paymentInfo.CardNumberMasked = maskedTenderInfo.CardNumber;
                    paymentInfo.CashbackAmount = maskedTenderInfo.CashBackAmount;
                    paymentInfo.CardType = (Microsoft.Dynamics.Commerce.HardwareStation.CardPayment.CardType)maskedTenderInfo.CardTypeId;

                    if (paymentInfo.CashbackAmount > this.terminalSettings.DebitCashbackLimit)
                    {
                        throw new CardPaymentException(CardPaymentException.CashbackAmountExceedsLimit, "Cashback amount exceeds the maximum amount allowed.");
                    }

                    // Refund
                    PSDK.Response response = CardPaymentManager.ChainedRefundCall(this.merchantProperties, string.Empty, this.tenderInfo, amount, currency, this.terminalSettings.Locale, true, this.terminalSettings.TerminalId, extensionTransactionProperties);

                    CardPaymentManager.MapRefundResponse(response, paymentInfo);

                    if (paymentInfo.IsApproved)
                    {
                        // need signature?
                        if (this.terminalSettings.SignatureCaptureMinimumAmount < paymentInfo.ApprovedAmount)
                        {
                            paymentInfo.SignatureData = await this.RequestTenderApprovalAsync(paymentInfo.ApprovedAmount);
                        }
                    }

                    return paymentInfo;
                }
                catch (Exception ex)
                {
                    var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.Error));
                    paymentTerminalPipeline.SendError(ex.Message);
                    throw;
                }
            }

            /// <summary>
            /// Update the line items on the current open session.  This method will compare against previous lines specified and make the appropriate device calls.
            /// </summary>
            /// <param name="totalAmount">The total amount of the transaction, including tax.</param>
            /// <param name="taxAmount">The total tax amount on the transaction.</param>
            /// <param name="discountAmount">The total discount amount on the transaction.</param>
            /// <param name="subTotalAmount">The sub-total amount on the transaction.</param>
            /// <param name="items">The items in the transaction.</param>
            /// <returns>A task that can be awaited until the text is displayed on the screen.</returns>
            public async Task UpdateLineItemsAsync(string totalAmount, string taxAmount, string discountAmount, string subTotalAmount, IEnumerable<ItemInfo> items)
            {
                var updateLineItemsTask = Task.Factory.StartNew(() =>
                {
                    var info = new JObject();

                    info["TotalAmount"] = totalAmount;
                    info["TaxAmount"] = taxAmount;
                    info["DiscountAmount"] = discountAmount;
                    info["SubTotalAmount"] = subTotalAmount;

                    FillItemInfo(items.ToArray(), info);

                    string serializedInfo = info.ToString();

                    var paymentCardPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.LineItems));
                    paymentCardPipeline.UpdateCartLines(serializedInfo);
                });

                await updateLineItemsTask;
            }

            /// <summary>
            /// Make reversal/void a payment.
            /// </summary>
            /// <param name="amount">The amount.</param>
            /// <param name="currency">The currency.</param>
            /// <param name="paymentProperties">The payment properties of the authorization response.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the void has completed.</returns>
            public Task<PaymentInfo> VoidPaymentAsync(decimal amount, string currency, PSDK.PaymentProperty[] paymentProperties, ExtensionTransaction extensionTransactionProperties)
            {
                try
                {
                    var info = new JObject();

                    info["Amount"] = amount;
                    info["Currency"] = currency;
                    FillPaymentProperties(paymentProperties, info);

                    string serializedInfo = info.ToString();
                    File.WriteAllText(string.Format(@"{0}\{1}_VoidPayment.json", this.deviceSimFolderPath, PaymentDeviceSimulatorFileName), serializedInfo);

                    if (amount < this.terminalSettings.MinimumAmountAllowed)
                    {
                        throw new CardPaymentException(CardPaymentException.AmountLessThanMinimumLimit, "Amount does not meet minimum amount allowed.");
                    }

                    if (this.terminalSettings.MaximumAmountAllowed > 0 && amount > this.terminalSettings.MaximumAmountAllowed)
                    {
                        throw new CardPaymentException(CardPaymentException.AmountExceedsMaximumLimit, "Amount exceeds the maximum amount allowed.");
                    }

                    if (this.processor == null)
                    {
                        this.processor = CardPaymentManager.GetPaymentProcessor(this.merchantProperties, this.paymentConnectorName);
                    }

                    PaymentInfo paymentInfo = new PaymentInfo();

                    // Handle multiple chain connectors by returning single instance used in capture.
                    PSDK.IPaymentProcessor currentProcessor = null;
                    PSDK.PaymentProperty[] currentMerchantProperties = null;
                    CardPaymentManager.GetRequiredConnector(this.merchantProperties, paymentProperties, this.processor, out currentProcessor, out currentMerchantProperties);

                    PSDK.Request request = CardPaymentManager.GetCaptureRequest(currentMerchantProperties, paymentProperties, amount, currency, this.terminalSettings.Locale, true, this.terminalSettings.TerminalId, cardCache, extensionTransactionProperties);
                    PSDK.Response response = currentProcessor.Void(request);
                    CardPaymentManager.MapVoidResponse(response, paymentInfo);

                    return Task.FromResult(paymentInfo);
                }
                catch (Exception ex)
                {
                    var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.Error));
                    paymentTerminalPipeline.SendError(ex.Message);
                    throw;
                }
            }

            /// <summary>
            /// We don't have the payment provider authorization piece for this function, here we just
            /// assume we get the bank authorization.
            /// </summary>
            /// <param name="amount">Required payment amount.</param>
            /// <returns>TenderInfo object.</returns>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "amount", Justification = "Other devices support the amount for signature approval.")]
            public async Task<string> RequestTenderApprovalAsync(decimal amount)
            {
                /*this.SignatureCaptureHandler();*/

                string signature = await this.GetSignatureData();

                // Show processing info here...
                return signature;
            }

            private static void FillItemInfo(ItemInfo[] items, JToken info)
            {
                var array = new JArray();
                info["Items"] = array;

                foreach (ItemInfo item in items)
                {
                    var itemInfo = new JObject();

                    itemInfo["LineItemId"] = item.LineItemId;
                    itemInfo["Sku"] = item.Sku;
                    itemInfo["Upc"] = item.Upc;
                    itemInfo["Description"] = item.Description;
                    itemInfo["Quantity"] = item.Quantity;
                    itemInfo["UnitPrice"] = item.UnitPrice;
                    itemInfo["ExtendedPriceWithTax"] = item.ExtendedPriceWithTax;
                    itemInfo["IsVoided"] = item.IsVoided;
                    itemInfo["Discount"] = item.Discount;

                    array.Add(itemInfo);
                }
            }

            private static void FillPaymentProperties(PSDK.PaymentProperty[] merchantProperties, JToken info)
            {
                if (merchantProperties == null || merchantProperties.Length == 0)
                {
                    throw new CardPaymentException(CardPaymentException.EmptyPaymentProperties, "The merchant payment properties are empty.");
                }

                var array = new JArray();
                info["MerchantProperties"] = array;

                foreach (PSDK.PaymentProperty merchant in merchantProperties)
                {
                    var merchantInfo = new JObject();

                    merchantInfo["Namespace"] = merchant.Namespace;
                    merchantInfo["Name"] = merchant.Name;
                    merchantInfo["ValueType"] = merchant.ValueType.ToString();
                    merchantInfo["StringValue"] = merchant.StringValue;
                    merchantInfo["StoredStringValue"] = merchant.StoredStringValue;
                    merchantInfo["DecimalValue"] = merchant.DecimalValue;
                    merchantInfo["DateValue"] = merchant.DateValue;
                    merchantInfo["DisplayName"] = merchant.DisplayName;
                    merchantInfo["Description"] = merchant.Description;
                    merchantInfo["SecurityLevel"] = merchant.SecurityLevel.ToString();
                    merchantInfo["IsEncrypted"] = merchant.IsEncrypted;
                    merchantInfo["IsPassword"] = merchant.IsPassword;
                    merchantInfo["IsReadOnly"] = merchant.IsReadOnly;
                    merchantInfo["IsHidden"] = merchant.IsHidden;
                    merchantInfo["DisplayHeight"] = merchant.DisplayHeight;
                    merchantInfo["SequenceNumber"] = merchant.SequenceNumber;

                    array.Add(merchantInfo);
                }
            }

            private async Task<TenderInfo> FetchTenderInfoAsync()
            {
                TenderInfo tenderInfo = await this.FillTenderInfo();

                // Show processing info here...
                return tenderInfo;
            }

            private void SignatureCaptureHandler()
            {
                var paymentTerminalSignatureCapStatePipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentTerminalDevice, PaymentTerminalMessageHandler.SignatureCaptureState));

                // Register the event handler for enable signature capture state device activities.
                EventHandler<VirtualPeripheralsEventArgs> signatureCaptureInfoEventHandler = null;
                signatureCaptureInfoEventHandler = (sender, args) =>
                {
                    // Perform the device activity of entering the signature, once the card information is captured.
                    var signatureCapturePipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentTerminalDevice, PaymentTerminalMessageHandler.SignatureCaptureData));
                    string signatureData = this.GetSignatureDataByBytes();

                    signatureCapturePipeline.CaptureSignatureCaptureData(signatureData);

                    paymentTerminalSignatureCapStatePipeline.PaymentTerminalMessageHandler.OnPaymentTerminalSignatureCaptureStateMessage -= signatureCaptureInfoEventHandler;
                };

                paymentTerminalSignatureCapStatePipeline.PaymentTerminalMessageHandler.OnPaymentTerminalSignatureCaptureStateMessage += signatureCaptureInfoEventHandler;
            }

            private void CardSwipeHandler()
            {
                var paymentTerminalCardSwipeState = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentTerminalDevice, PaymentTerminalMessageHandler.CardState));

                // Register the event handler for enable card swipe state device activities.
                EventHandler<VirtualPeripheralsEventArgs> cardStateInfoEventHandler = null;
                cardStateInfoEventHandler = (sender, args) =>
                {
                    // Perform the device activity of swiping the card, once the update lines is complete.
                    var paymentCardPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentTerminalDevice, PaymentTerminalMessageHandler.CardData));

                    PaymentCardData paymentCardData = this.GetCardInfo();
                    string xmlPaymentCardData = XmlDataHelper.ConvertTypeToXml(paymentCardData);

                    paymentCardPipeline.CaptureCardInfo(xmlPaymentCardData);

                    paymentTerminalCardSwipeState.PaymentTerminalMessageHandler.OnPaymentTerminalCardSwipeStateMessage -= cardStateInfoEventHandler;
                };

                paymentTerminalCardSwipeState.PaymentTerminalMessageHandler.OnPaymentTerminalCardSwipeStateMessage += cardStateInfoEventHandler;
            }

            private string GetSignatureDataByBytes()
            {
                // The signature capture data will be moved to test xml file.
                return string.Empty;
            }

            private PaymentCardData GetCardInfo()
            {
                // The card information is hard coded only for test cases. 
                // The card details will be moved to test xml file.
                return null;
            }

            private async Task<TenderInfo> FillTenderInfo()
            {
                var tenderInfoTask = Task<TenderInfo>.Factory.StartNew(() =>
                {
                    var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.CardData));

                    var getCardTrackData = new TaskCompletionSource<PaymentCardData>();

                    EventHandler<VirtualPeripheralsEventArgs> cardInfoEventHandler = null;
                    cardInfoEventHandler = (sender, args) =>
                    {
                        try
                        {
                            PaymentCardData cardInfo = XmlDataHelper.ConvertXmlToType<PaymentCardData>(args.Contents.FirstOrDefault());
                            getCardTrackData.SetResult(cardInfo);
                        }
                        catch
                        {
                            // Ignoring the exception - An attempt was made to transition a task to a final state when it had already completed.
                            // The exception occurs inspite of unregistering the call back event in the finally. 
                        }
                        finally
                        {
                            paymentTerminalPipeline.PaymentTerminalMessageHandler.OnPaymentTerminalCardSwipeMessage -= cardInfoEventHandler;
                        }
                    };

                    paymentTerminalPipeline.PaymentTerminalMessageHandler.OnPaymentTerminalCardSwipeMessage += cardInfoEventHandler;

                    // Enable the card swipe in the device.
                    var paymentTerminalCardSwipePipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.CardState));
                    paymentTerminalCardSwipePipeline.EnableCardSwipe();

                    PaymentCardData cardSwipeData = getCardTrackData.Task.Result;

                    var tenderInfo = new TenderInfo();

                    tenderInfo.CardTypeId = (int)Microsoft.Dynamics.Commerce.HardwareStation.CardPayment.CardType.InternationalCreditCard;
                    tenderInfo.Track1 = cardSwipeData.Track1Data;
                    tenderInfo.Track2 = cardSwipeData.Track2Data;
                    tenderInfo.CardNumber = cardSwipeData.AccountNumber;
                    tenderInfo.ExpirationMonth = cardSwipeData.ExpirationMonth;
                    tenderInfo.ExpirationYear = cardSwipeData.ExpirationYear;

                    this.tenderInfo = tenderInfo;

                    return tenderInfo;
                });

                return await tenderInfoTask;
            }

            /// <summary>
            /// Get the signature data from the device.
            /// </summary>
            /// <returns>Returns the signature data task.</returns>
            private async Task<string> GetSignatureData()
            {
                var signatureInfoTask = Task<string>.Factory.StartNew(() =>
                {
                    var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.SignatureCaptureData));

                    var signatureData = new TaskCompletionSource<string>();

                    EventHandler<VirtualPeripheralsEventArgs> signatureCaptureEventHandler = null;
                    signatureCaptureEventHandler = (sender, args) =>
                    {
                        try
                        {
                            signatureData.SetResult(SignatureCaptureHelper.ParsePointArray(args.Contents.FirstOrDefault()).Signature);
                        }
                        catch
                        {
                            // Ignoring the exception - An attempt was made to transition a task to a final state when it had already completed.
                            // The exception occurs inspite of unregistering the call back event in the finally. 
                        }
                        finally
                        {
                            paymentTerminalPipeline.PaymentTerminalMessageHandler.OnPaymentTerminalSignatureCaptureDataMessage -= signatureCaptureEventHandler;
                        }
                    };

                    paymentTerminalPipeline.PaymentTerminalMessageHandler.OnPaymentTerminalSignatureCaptureDataMessage += signatureCaptureEventHandler;

                    // Enable the signature device form.
                    var paymentTerminalSignatureCaptureStatePipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentDeviceSample.PaymentTerminalDevice, PaymentTerminalMessageHandler.SignatureCaptureState));
                    paymentTerminalSignatureCaptureStatePipeline.EnableSignatureForm();

                    string signature = signatureData.Task.Result;

                    return signature;
                });

                return await signatureInfoTask;
            }

            /// <summary>
            /// Activate gift card.
            /// </summary>
            /// <param name="paymentConnectorName">The payment connector name.</param>
            /// <param name="amount">The amount.</param>
            /// <param name="currencyCode">The currency.</param>
            /// <param name="tenderInfo">The tender information.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the void has completed.</returns>
            private async Task<PaymentInfo> ActivateGiftCardAsync(string paymentConnectorName, decimal? amount, string currencyCode, TenderInfo tenderInfo, ExtensionTransaction extensionTransactionProperties)
            {
                await Task.Delay(PaymentDeviceSample.TaskDelayInMilliSeconds);
                throw new PeripheralException(PeripheralException.PaymentTerminalError, "Operation is not supported by payment terminal.", inner: null);
            }

            /// <summary>
            /// Add balance to gift card.
            /// </summary>
            /// <param name="paymentConnectorName">The payment connector name.</param>
            /// <param name="amount">The amount.</param>
            /// <param name="currencyCode">The currency.</param>
            /// <param name="tenderInfo">The tender information.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the void has completed.</returns>
            private async Task<PaymentInfo> AddBalanceToGiftCardAsync(string paymentConnectorName, decimal? amount, string currencyCode, TenderInfo tenderInfo, ExtensionTransaction extensionTransactionProperties)
            {
                await Task.Delay(PaymentDeviceSample.TaskDelayInMilliSeconds);
                throw new PeripheralException(PeripheralException.PaymentTerminalError, "Operation is not supported by payment terminal.", inner: null);
            }

            /// <summary>
            /// Get gift card balance.
            /// </summary>
            /// <param name="paymentConnectorName">The payment connector name.</param>
            /// <param name="currencyCode">The currency.</param>
            /// <param name="tenderInfo">The tender information.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the void has completed.</returns>
            private async Task<PaymentInfo> GetGiftCardBalanceAsync(string paymentConnectorName, string currencyCode, TenderInfo tenderInfo, ExtensionTransaction extensionTransactionProperties)
            {
                await Task.Delay(PaymentDeviceSample.TaskDelayInMilliSeconds);
                throw new PeripheralException(PeripheralException.PaymentTerminalError, "Operation is not supported by payment terminal.", inner: null);
            }

            /// <summary>
            /// Task that handles closing the connection after a timeout period.
            /// </summary>
            /// <param name="timeout">The timeout period in seconds.</param>
            private async void Timeout(int timeout)
            {
                this.timeoutTask = new CancellationTokenSource();

                try
                {
                    await Task.Delay(timeout * 1000, this.timeoutTask.Token);
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                await this.EndTransactionAsync();
            }
        }
    }
}
