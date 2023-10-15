/// <summary>
///   class is used to execute PCEFTPOS payment integration
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso
{
    namespace Commerce.HardwareStation.Extension.UPOS_HardwareStation.UPOS_PCEFTPOS
    {
        using System;
        using System.Collections.Generic;
        using System.Composition;
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
        using Microsoft.Dynamics.Retail.Diagnostics;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;
        using Newtonsoft.Json.Linq;
        using PSDK = Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        // using DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.HardwareStation.Models;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Helper;
        using Microsoft.Dynamics.Commerce.Runtime;
        using UPOS_HardwareStation.DataModel;
        using PCEFTPOS.EFTClient.IPInterface;
        using Request = Microsoft.Dynamics.Retail.PaymentSDK.Portable.Request;

        //using PCEFTPOS.Messaging;

        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDispoUPOSle", Justification = "long running task, so the file system watcher will be disposed when the program ends")]
#pragma warning disable CS0618 // Type or member is obsolete
        public class PCEFTPOSAsync : INamedRequestHandler
#pragma warning restore CS0618 // Type or member is obsolete
        {
            public enum UPOS_PaymentType { payment, refund }

            private const string PaymentTerminalDevice = "PCEFTPOS";
            private const string PaymentDeviceSimulatorFileName = "PaymentDeviceSimulator";
            private const int TaskDelayInMilliSeconds = 10;

            // Cache to store credit card number, the key will be returned to client in Authorization payment sdk blob.
            private readonly string deviceSimFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            private CancellationTokenSource timeoutTask;
            private PSDK.PaymentProperty[] merchantProperties;
            private SettingsInfo terminalSettings;
            private string paymentConnectorName;
            //private readonly TenderInfo tenderInfo;
            //private PSDK.IPaymentProcessor processor;

            //-->UPOS/Wojtek/009443 EFT integration
            public string transactionid = string.Empty;

            public string defaultCard = string.Empty;
            public UPOS_CardTypeInfoHeader cardTypeCollection = null;

            public string deviceName = string.Empty;
            public int characterSet = 0;
            public string deviceType = string.Empty;
            public bool binaryConversion = false;
            public bool signatureVerified = false;
            public bool signatureVerifiedProcess = false;
            public string merchant = string.Empty;

            private Settings _settings;
            private EFTTransactionResponse transactionResponse;
            private EFTGetLastTransactionResponse getLastTransactionResponse;
            IEFTClientIPAsync _eftAsync = null;
            string errorMessage;
            //<--UPOS/Wojtek/009443 EFT integration

            public PCEFTPOSAsync()
            {
                // Do nothing.
            }
            /// <summary>
            /// Gets the specify the name of the request handler.
            /// </summary>
            public string HandlerName
            {
                get
                {
                    return PCEFTPOSAsync.PaymentTerminalDevice;
                }
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
                        typeof(ExecuteTaskPaymentTerminalDeviceRequest)
                    };
                }
            }

            /// <summary>
            /// Executes the payment device simulator operation based on the incoming request type.
            /// </summary>
            /// <param name="request">The payment terminal device simulator request message.</param>
            /// <returns>Returns the payment terminal device simulator response.</returns>
            public Microsoft.Dynamics.Commerce.Runtime.Messages.Response Execute(Microsoft.Dynamics.Commerce.Runtime.Messages.Request request)
            {
                Microsoft.Dynamics.Commerce.Runtime.ThrowIf.Null(request, "request");

                Type requestType = request.GetType();

                if (requestType == typeof(OpenPaymentTerminalDeviceRequest))
                {
                    this.Open((OpenPaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(BeginTransactionPaymentTerminalDeviceRequest))
                {
                    this.BeginTransaction((BeginTransactionPaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(UpdateLineItemsPaymentTerminalDeviceRequest))
                {
                    this.UpdateLineItems((UpdateLineItemsPaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(AuthorizePaymentTerminalDeviceRequest))
                {
                    return this.AuthorizePayment((AuthorizePaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(CapturePaymentTerminalDeviceRequest))
                {
                    return this.CapturePayment((CapturePaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(VoidPaymentTerminalDeviceRequest))
                {
                    return this.VoidPayment((VoidPaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(RefundPaymentTerminalDeviceRequest))
                {
                    return this.RefundPayment((RefundPaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(FetchTokenPaymentTerminalDeviceRequest))
                {
                    return this.FetchToken((FetchTokenPaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(EndTransactionPaymentTerminalDeviceRequest))
                {
                    this.EndTransaction((EndTransactionPaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(ClosePaymentTerminalDeviceRequest))
                {
                    this.Close((ClosePaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(CancelOperationPaymentTerminalDeviceRequest))
                {
                    this.CancelOperation((CancelOperationPaymentTerminalDeviceRequest)request);
                }
                else if (requestType == typeof(ActivateGiftCardPaymentTerminalRequest))
                {
                    return this.ActivateGiftCard((ActivateGiftCardPaymentTerminalRequest)request);
                }
                else if (requestType == typeof(AddBalanceToGiftCardPaymentTerminalRequest))
                {
                    return this.AddBalanceToGiftCard((AddBalanceToGiftCardPaymentTerminalRequest)request);
                }
                else if (requestType == typeof(GetGiftCardBalancePaymentTerminalRequest))
                {
                    return this.GetGiftCardBalance((GetGiftCardBalancePaymentTerminalRequest)request);
                }
                else if (requestType == typeof(ExecuteTaskPaymentTerminalDeviceRequest))
                {
                    return this.ExecutePaymentTerminal((ExecuteTaskPaymentTerminalDeviceRequest)request);
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }

#pragma warning disable CS0618 // Type or member is obsolete
                return new NullResponse();
#pragma warning restore CS0618 // Type or member is obsolete
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
                    //PSDK.PaymentProperty[] merchantProperties = CardPaymentManager.ToLocalProperties(request.MerchantInformation);
                    PSDK.PaymentProperty[] merchantProperties = ToLocalPropertiesAsync(request.MerchantInformation);
                    await BeginTransactionAsync(merchantProperties, request.PaymentConnectorName, request.InvoiceNumber, request.IsTestMode).ConfigureAwait(false);
                }));



            }
            public static PaymentProperty[] ToLocalPropertiesAsync(string paymentPropertiesXml)
            {
                PaymentProperty[] localProperties = (PaymentProperty[])null;
                if (!string.IsNullOrWhiteSpace(paymentPropertiesXml))
                    localProperties = PaymentProperty.ConvertXMLToPropertyArray(paymentPropertiesXml);
                return localProperties;
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
                //-->UPOS/Wojtek/009443 EFT integration
                //get extension properties
                this.GetExtensionProperties(request.ExtensionTransactionProperties);

                PaymentInfo paymentInfo = null;

                if (this.signatureVerifiedProcess == true)
                {
                    paymentInfo = Utilities.WaitAsyncTask(() => this.ExecuteIntegrationSignatureVeryfiedAsync(request.Amount, request.Currency, UPOS_PaymentType.payment));
                }
                else
                {
                    paymentInfo = Utilities.WaitAsyncTask(() => this.ExecuteIntegrationAsync(request.Amount, request.Currency, UPOS_PaymentType.payment));
                }

                //<--UPOS/Wojtek/009443 EFT integration
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

                //PSDK.PaymentProperty[] merchantProperties = ToLocalPropertiesAsync(request.PaymentPropertiesXml);
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

                //PSDK.PaymentProperty[] merchantProperties = ToLocalPropertiesAsync(request.PaymentPropertiesXml);

                //get extension properties
                //-->UPOS/Wojtek/009443 EFT integration
                this.GetExtensionProperties(request.ExtensionTransactionProperties);

                PaymentInfo paymentInfo = null;

                if (this.signatureVerifiedProcess == true)
                {
                    paymentInfo = Utilities.WaitAsyncTask(() => this.ExecuteIntegrationSignatureVeryfiedAsync(request.Amount, request.Currency, UPOS_PaymentType.refund));
                }
                else
                {
                    paymentInfo = Utilities.WaitAsyncTask(() => this.ExecuteIntegrationAsync(request.Amount, request.Currency, UPOS_PaymentType.refund));
                }
                //<--UPOS/Wojtek/009443 EFT integration
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
                //-->UPOS/Wojtek/009443 EFT integration
                //get extension properties
                this.GetExtensionProperties(request.ExtensionTransactionProperties);

                PaymentInfo paymentInfo = null;

                if (this.signatureVerifiedProcess == true)
                {
                    paymentInfo = Utilities.WaitAsyncTask(() => this.ExecuteIntegrationSignatureVeryfiedAsync(request.Amount, request.Currency, UPOS_PaymentType.refund));
                }
                else
                {
                    paymentInfo = Utilities.WaitAsyncTask(() => this.ExecuteIntegrationAsync(request.Amount, request.Currency, UPOS_PaymentType.refund));
                }
                //<--UPOS/Wojtek/009443 EFT integration
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

            //-->SAB/Wojtek/009443 EFT integration Power failure  
            /// <summary>
            /// Execute Task Card.
            /// </summary>
            /// <param name="request">The Execute Task Card request.</param>
            /// <returns>Execute Task Card response.</returns>
            public Microsoft.Dynamics.Commerce.Runtime.Messages.Response ExecutePaymentTerminal(ExecuteTaskPaymentTerminalDeviceRequest request)
            {
                Microsoft.Dynamics.Commerce.Runtime.ThrowIf.Null(request, "request");

                this.GetExtensionProperties(request.ExtensionTransactionProperties);

                ExtensionTransaction extensionTransaction = Utilities.WaitAsyncTask(() => this.ExecuteIntegrationPowerFailure());

                return new ExecuteTaskPaymentTerminalDeviceResponse(extensionTransaction);

            }
            //<--SAB/Wojtek/009443 EFT integration Power failure 

            /// <summary>
            /// Make authorization payment.
            /// </summary>
            /// <param name="amount">The amount.</param>
            /// <param name="currency">The currency.</param>
            /// <param name="voiceAuthorization">The voice approval code (optional).</param>
            /// <param name="isManualEntry">If manual credit card entry is required.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the authorization has completed.</returns>
            //public async Task<PaymentInfo> AuthorizePaymentAsync(decimal amount, string currency, string voiceAuthorization, bool isManualEntry, ExtensionTransaction extensionTransactionProperties)
            //{

            //}

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

                    dynamic info = new JObject();
                    info.PaymentConnectorName = paymentConnectorName;
                    info.InvoiceNumber = invoiceNumber;
                    info.IsTestMode = isTestMode;

                    FillPaymentProperties(merchantProperties, info);

                    string serializedInfo = info.ToString();

                });

                await beginTransactionTask.ConfigureAwait(false);
            }

            /// <summary>
            ///  Cancels an existing GetTender or RequestTenderApproval  operation on the payment terminal.
            /// </summary>
            /// <returns>A task that can be awaited until the operation is cancelled.</returns>
            public async Task CancelOperationAsync()
            {
                //-->UPOS/Wojtek/009443 EFT integration
                await Task.Run(() => InternalCancelOperation()).ConfigureAwait(false);
                //<--UPOS/Wojtek/009443 EFT integration
                // await Task.Delay(PCEFTPOSAsync.TaskDelayInMilliSeconds);
            }
            //-->UPOS/Wojtek/009443 EFT integration
            public async Task CancelOperation()
            {
                await Task.Run(() => InternalCancelOperation()).ConfigureAwait(false);
            }
            private async Task InternalCancelOperation()
            {
                if (EFTClientIPHelper._eftAsync != null)
                {
                    await EFTClientIPHelper._eftAsync.WriteRequestAsync(new EFTSendKeyRequest() { Key = EFTPOSKey.OkCancel }).ConfigureAwait(false);
                }
            }
            //<--UPOS/Wojtek/009443 EFT integration  

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
                return Task.FromResult<PaymentInfo>(new PaymentInfo()
                {
                    CardNumberMasked = string.Empty,
                    CardType = Microsoft.Dynamics.Commerce.HardwareStation.CardPayment.CardType.Unknown,
                    SignatureData = string.Empty,
                    PaymentSdkData = "<!--- payment sdk connector payment properties for capture response -->",
                    CashbackAmount = 0.0m,
                    ApprovedAmount = amount,
                    IsApproved = true,
                    Errors = null
                });

            }

            /// <summary>
            ///  Closes a connection to the payment terminal.
            /// </summary>
            /// <returns>A task that can be awaited until the connection is closed.</returns>
            public async Task CloseAsync()
            {
                await Task.Delay(TaskDelayInMilliSeconds).ConfigureAwait(false);
            }

            /// <summary>
            ///  Ends the transaction.
            /// </summary>
            /// <returns>A task that can be awaited until the end transaction screen is displayed.</returns>
            public async Task EndTransactionAsync()
            {
                await Task.Delay(TaskDelayInMilliSeconds).ConfigureAwait(false);
            }

            /// <summary>
            /// Fetch token for credit card.
            /// </summary>
            /// <param name="isManualEntry">The value indicating whether credit card should be entered manually.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the token generation has completed.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            public async Task<PaymentInfo> FetchTokenAsync(bool isManualEntry, ExtensionTransaction extensionTransactionProperties)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                PaymentInfo paymentInfo = new PaymentInfo();

                // Get tender
                //TenderInfo maskedTenderInfo = await FetchTenderInfoAsync().ConfigureAwait(false);

                //PSDK.PaymentProperty[] defaultMerchantProperties = this.merchantProperties;

                //paymentInfo.CardNumberMasked = maskedTenderInfo.CardNumber;
                //paymentInfo.CashbackAmount = maskedTenderInfo.CashBackAmount;
                //paymentInfo.CardType = (Microsoft.Dynamics.Commerce.HardwareStation.CardPayment.CardType)maskedTenderInfo.CardTypeId;

                //if (this.merchantProperties != null &&
                //    this.merchantProperties.Length > 0 &&
                //    this.merchantProperties[0].Namespace.Equals(GenericNamespace.Connector) &&
                //    this.merchantProperties[0].Name.Equals(ConnectorProperties.Properties))
                //{
                //    defaultMerchantProperties = this.merchantProperties[0].PropertyList;
                //}


                //if (this.processor == null)
                //{
                //    this.processor = GetPaymentProcessor(this.merchantProperties, this.paymentConnectorName);
                //}

                //// Generate card token
                //PSDK.Request request = GetTokenRequest(defaultMerchantProperties, this.tenderInfo, this.terminalSettings.Locale, extensionTransactionProperties);
                //PSDK.Response response = this.processor.GenerateCardToken(request, null);
                //CardPaymentManager.MapTokenResponse(response, paymentInfo);

                return paymentInfo;
            }

            /// <summary>Gets the payment processor.</summary>
            /// <param name="merchantProperties">The merchant properties.</param>
            /// <param name="paymentConnectorName">The payment connector name.</param>
            /// <returns>Returns the payment processor.</returns>

            ///// <summary>
            ///// Open payment device using simulator.
            /// </summary>
            /// <param name="peripheralName">Name of peripheral device.</param>
            /// <param name="terminalSettings">The terminal settings for the peripheral device.</param>
            /// <param name="deviceConfig">Device Configuration parameters.</param>
            /// <returns>A task that can be awaited until the connection is opened.</returns>
            [Obsolete("This method will be removed once IPaymentDevice is deprecated.")]
            public async Task OpenAsync(string peripheralName, SettingsInfo terminalSettings, IDictionary<string, string> deviceConfig)
            {
                await Task.Delay(TaskDelayInMilliSeconds).ConfigureAwait(false);
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

                    dynamic info = new JObject();

                    info.PeripheralName = peripheralName;
                    info.TerminalSettings = new JObject() as dynamic;

                    if (terminalSettings != null)
                    {
                        info.TerminalSettings.SignatureCaptureMinimumAmount = terminalSettings.SignatureCaptureMinimumAmount;
                        info.TerminalSettings.MinimumAmountAllowed = terminalSettings.MinimumAmountAllowed;
                        info.TerminalSettings.MaximumAmountAllowed = terminalSettings.MaximumAmountAllowed;
                        info.TerminalSettings.DebitCashbackLimit = terminalSettings.DebitCashbackLimit;
                        info.TerminalSettings.Locale = terminalSettings.Locale;
                        info.TerminalSettings.TerminalId = terminalSettings.TerminalId;
                    }

                    string serializedInfo = info.ToString();

                });

                await openTask.ConfigureAwait(false);
            }

            /// <summary>
            /// Make refund payment.
            /// </summary>
            /// <param name="amount">The amount.</param>
            /// <param name="currency">The currency.</param>
            /// <param name="isManualEntry">If manual credit card entry is required.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the refund has completed.</returns>
            //public async Task<PaymentInfo> RefundPaymentAsync(decimal amount, string currency, bool isManualEntry, ExtensionTransaction extensionTransactionProperties)
            //{

            //}

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
                    dynamic info = new JObject();

                    info.TotalAmount = totalAmount;
                    info.TaxAmount = taxAmount;
                    info.DiscountAmount = discountAmount;
                    info.SubTotalAmount = subTotalAmount;

                    FillItemInfo(items.ToArray(), info);

                    string serializedInfo = info.ToString();

                });

                await updateLineItemsTask.ConfigureAwait(false);
            }

            /// <summary>
            /// Make reversal/void a payment.
            /// </summary>
            /// <param name="amount">The amount.</param>
            /// <param name="currency">The currency.</param>
            /// <param name="paymentProperties">The payment properties of the authorization response.</param>
            /// <param name="extensionTransactionProperties">Optional extension transaction properties.</param>
            /// <returns>A task that can await until the void has completed.</returns>
            //public async Task<PaymentInfo> VoidPaymentAsync(decimal amount, string currency, PSDK.PaymentProperty[] paymentProperties, ExtensionTransaction extensionTransactionProperties)
            //{

            //}

            /// <summary>
            /// We don't have the payment provider authorization piece for this function, here we just
            /// assume we get the bank authorization.
            /// </summary>
            /// <param name="amount">Required payment amount.</param>
            /// <returns>TenderInfo object.</returns>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "amount", Justification = "Other devices support the amount for signature approval.")]
            //public async Task<string> RequestTenderApprovalAsync(decimal amount)
            //{
            //    /*this.SignatureCaptureHandler();*/

            //    string signature = await GetSignatureData().ConfigureAwait(false);

            //    // Show processing info here...
            //    return signature;
            //}

            private static void FillItemInfo(ItemInfo[] items, dynamic info)
            {
                info.Items = new JArray() as dynamic;
                foreach (ItemInfo item in items)
                {
                    dynamic itemInfo = new JObject();

                    itemInfo.LineItemId = item.LineItemId;
                    itemInfo.Sku = item.Sku;
                    itemInfo.Upc = item.Upc;
                    itemInfo.Description = item.Description;
                    itemInfo.Quantity = item.Quantity;
                    itemInfo.UnitPrice = item.UnitPrice;
                    itemInfo.ExtendedPriceWithTax = item.ExtendedPriceWithTax;
                    itemInfo.IsVoided = item.IsVoided;
                    itemInfo.Discount = item.Discount;

                    info.Items.Add(itemInfo);
                }
            }

            private static void FillPaymentProperties(PSDK.PaymentProperty[] merchantProperties, dynamic info)
            {
                if (merchantProperties == null || merchantProperties.Length == 0)
                {
                    throw new CardPaymentException(CardPaymentException.EmptyPaymentProperties, "The merchant payment properties are empty.");
                }

                info.MerchantProperties = new JArray() as dynamic;
                foreach (PSDK.PaymentProperty merchant in merchantProperties)
                {
                    dynamic merchantInfo = new JObject();

                    merchantInfo.Namespace = merchant.Namespace;
                    merchantInfo.Name = merchant.Name;
                    merchantInfo.ValueType = merchant.ValueType;
                    merchantInfo.StringValue = merchant.StringValue;
                    merchantInfo.StoredStringValue = merchant.StoredStringValue;
                    merchantInfo.DecimalValue = merchant.DecimalValue;
                    merchantInfo.DateValue = merchant.DateValue;
                    merchantInfo.DisplayName = merchant.DisplayName;
                    merchantInfo.Description = merchant.Description;
                    merchantInfo.SecurityLevel = merchant.SecurityLevel;
                    merchantInfo.IsEncrypted = merchant.IsEncrypted;
                    merchantInfo.IsPassword = merchant.IsPassword;
                    merchantInfo.IsReadOnly = merchant.IsReadOnly;
                    merchantInfo.IsHidden = merchant.IsHidden;
                    merchantInfo.DisplayHeight = merchant.DisplayHeight;
                    merchantInfo.SequenceNumber = merchant.SequenceNumber;

                    info.MerchantProperties.Add(merchantInfo);
                }
            }

            //private async Task<TenderInfo> FetchTenderInfoAsync()
            //{
            //    TenderInfo tenderInfo = await FillTenderInfo().ConfigureAwait(false);

            //    // Show processing info here...
            //    return tenderInfo;
            //}

            //private void SignatureCaptureHandler()
            //{
            //    var paymentTerminalSignatureCapStatePipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentTerminalDevice, PaymentTerminalMessageHandler.SignatureCaptureState));

            //    // Register the event handler for enable signature capture state device activities.
            //    EventHandler<VirtualPeripheralsEventArgs> signatureCaptureInfoEventHandler = null;
            //    signatureCaptureInfoEventHandler = (sender, args) =>
            //    {
            //        // Perform the device activity of entering the signature, once the card information is captured.
            //        var signatureCapturePipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentTerminalDevice, PaymentTerminalMessageHandler.SignatureCaptureData));
            //        string signatureData = this.GetSignatureDataByBytes();

            //        signatureCapturePipeline.CaptureSignatureCaptureData(signatureData);

            //        paymentTerminalSignatureCapStatePipeline.PaymentTerminalMessageHandler.OnPaymentTerminalSignatureCaptureStateMessage -= signatureCaptureInfoEventHandler;
            //    };

            //    paymentTerminalSignatureCapStatePipeline.PaymentTerminalMessageHandler.OnPaymentTerminalSignatureCaptureStateMessage += signatureCaptureInfoEventHandler;
            //}

            //private void CardSwipeHandler()
            //{
            //    var paymentTerminalCardSwipeState = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentTerminalDevice, PaymentTerminalMessageHandler.CardState));

            //    // Register the event handler for enable card swipe state device activities.
            //    EventHandler<VirtualPeripheralsEventArgs> cardStateInfoEventHandler = null;
            //    cardStateInfoEventHandler = (sender, args) =>
            //    {
            //        // Perform the device activity of swiping the card, once the update lines is complete.
            //        var paymentCardPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PaymentTerminalDevice, PaymentTerminalMessageHandler.CardData));

            //        PaymentCardData paymentCardData = this.GetCardInfo();
            //        string xmlPaymentCardData = XmlDataHelper.ConvertTypeToXml(paymentCardData);

            //        paymentCardPipeline.CaptureCardInfo(xmlPaymentCardData);

            //        paymentTerminalCardSwipeState.PaymentTerminalMessageHandler.OnPaymentTerminalCardSwipeStateMessage -= cardStateInfoEventHandler;
            //    };

            //    paymentTerminalCardSwipeState.PaymentTerminalMessageHandler.OnPaymentTerminalCardSwipeStateMessage += cardStateInfoEventHandler;
            //}

            private string GetSignatureDataByBytes()
            {
                // The signature capture data will be moved to test xml file.
                return string.Empty;
            }

            //private PaymentCardData GetCardInfo()
            //{
            //    // The card information is hard coded only for test cases. 
            //    // The card details will be moved to test xml file.
            //    return null;
            //}

            //private async Task<TenderInfo> FillTenderInfo()
            //{
            //    var tenderInfoTask = Task<TenderInfo>.Factory.StartNew(() =>
            //    {
            //        var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PCEFTPOSAsync.PaymentTerminalDevice, PaymentTerminalMessageHandler.CardData));

            //        var getCardTrackData = new TaskCompletionSource<PaymentCardData>();

            //        EventHandler<VirtualPeripheralsEventArgs> cardInfoEventHandler = null;
            //        cardInfoEventHandler = (sender, args) =>
            //        {
            //            try
            //            {
            //                PaymentCardData cardInfo = XmlDataHelper.ConvertXmlToType<PaymentCardData>(args.Contents.FirstOrDefault());
            //                getCardTrackData.SetResult(cardInfo);
            //            }
            //            catch
            //            {
            //                // Ignoring the exception - An attempt was made to transition a task to a final state when it had already completed.
            //                // The exception occurs inspite of unregistering the call back event in the finally. 
            //            }
            //            finally
            //            {
            //                paymentTerminalPipeline.PaymentTerminalMessageHandler.OnPaymentTerminalCardSwipeMessage -= cardInfoEventHandler;
            //            }
            //        };

            //        paymentTerminalPipeline.PaymentTerminalMessageHandler.OnPaymentTerminalCardSwipeMessage += cardInfoEventHandler;

            //        // Enable the card swipe in the device.
            //        var paymentTerminalCardSwipePipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PCEFTPOSAsync.PaymentTerminalDevice, PaymentTerminalMessageHandler.CardState));
            //        paymentTerminalCardSwipePipeline.EnableCardSwipe();

            //        PaymentCardData cardSwipeData = getCardTrackData.Task.Result;

            //        var tenderInfo = new TenderInfo();

            //        tenderInfo.CardTypeId = (int)Microsoft.Dynamics.Commerce.HardwareStation.CardPayment.CardType.InternationalCreditCard;
            //        tenderInfo.Track1 = cardSwipeData.Track1Data;
            //        tenderInfo.Track2 = cardSwipeData.Track2Data;
            //        tenderInfo.CardNumber = cardSwipeData.AccountNumber;
            //        tenderInfo.ExpirationMonth = cardSwipeData.ExpirationMonth;
            //        tenderInfo.ExpirationYear = cardSwipeData.ExpirationYear;

            //        this.tenderInfo = tenderInfo;

            //        return tenderInfo;
            //    });

            //    return await tenderInfoTask.ConfigureAwait(false);
            //}

            /// <summary>
            /// Get the signature data from the device.
            /// </summary>
            /// <returns>Returns the signature data task.</returns>
            //private async Task<string> GetSignatureData()
            //{
            //    var signatureInfoTask = Task<string>.Factory.StartNew(() =>
            //    {
            //        var paymentTerminalPipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PCEFTPOSAsync.PaymentTerminalDevice, PaymentTerminalMessageHandler.SignatureCaptureData));

            //        var signatureData = new TaskCompletionSource<string>();

            //        EventHandler<VirtualPeripheralsEventArgs> signatureCaptureEventHandler = null;
            //        signatureCaptureEventHandler = (sender, args) =>
            //        {
            //            try
            //            {
            //                signatureData.SetResult(SignatureCaptureHelper.ParsePointArray(args.Contents.FirstOrDefault()).Signature);
            //            }
            //            catch
            //            {
            //                // Ignoring the exception - An attempt was made to transition a task to a final state when it had already completed.
            //                // The exception occurs inspite of unregistering the call back event in the finally. 
            //            }
            //            finally
            //            {
            //                paymentTerminalPipeline.PaymentTerminalMessageHandler.OnPaymentTerminalSignatureCaptureDataMessage -= signatureCaptureEventHandler;
            //            }
            //        };

            //        paymentTerminalPipeline.PaymentTerminalMessageHandler.OnPaymentTerminalSignatureCaptureDataMessage += signatureCaptureEventHandler;

            //        // Enable the signature device form.
            //        var paymentTerminalSignatureCaptureStatePipeline = new PaymentTerminalPipeline(string.Format("{0}{1}", PCEFTPOSAsync.PaymentTerminalDevice, PaymentTerminalMessageHandler.SignatureCaptureState));
            //        paymentTerminalSignatureCaptureStatePipeline.EnableSignatureForm();

            //        string signature = signatureData.Task.Result;

            //        return signature;
            //    });

            //    return await signatureInfoTask;
            //}

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
                await Task.Delay(TaskDelayInMilliSeconds).ConfigureAwait(false);
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
                await Task.Delay(TaskDelayInMilliSeconds).ConfigureAwait(false);
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
                await Task.Delay(TaskDelayInMilliSeconds).ConfigureAwait(false);
                throw new PeripheralException(PeripheralException.PaymentTerminalError, "Operation is not supported by payment terminal.", inner: null);
            }

            /// <summary>
            /// Task that handles closing the connection after a timeout period.
            /// </summary>
            /// <param name="timeout">The timeout period in seconds.</param>
            private async Task Timeout(int timeout)
            {
                this.timeoutTask = new CancellationTokenSource();

                try
                {
                    await Task.Delay(timeout * 1000, timeoutTask.Token).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    RetailLogger.Instance.AxGenericInformationalEvent("Task is canceled.");
                }

                await EndTransactionAsync().ConfigureAwait(false);
            }
            //-->UPOS/Wojtek/009443 EFT integration
            #region PC-EFTPOS PRINTER
            public void CheckPrinter()
            {

                //try
                //{

                //    Microsoft.Dynamics.Commerce.HardwareStation.Models.PrintRequest printerRequest = new Microsoft.Dynamics.Commerce.HardwareStation.Models.PrintRequest();
                //    printerRequest.CharacterSet = this.characterSet;
                //    printerRequest.DeviceType = this.deviceType;
                //    printerRequest.DeviceName = this.deviceName;
                //    printerRequest.BinaryConversion = this.binaryConversion;
                //    printerRequest.Header = "";
                //    Microsoft.Dynamics.Commerce.HardwareStation.Controllers.PrinterController pc = new Microsoft.Dynamics.Commerce.HardwareStation.Controllers.PrinterController();

                //    System.Collections.ObjectModel.Collection<Microsoft.Dynamics.Commerce.HardwareStation.Models.PrintRequest> list = new System.Collections.ObjectModel.Collection<Microsoft.Dynamics.Commerce.HardwareStation.Models.PrintRequest>();
                //    list.Add(printerRequest);
                //    pc.Print(list);
                //}
                //catch (Exception ex)
                //{
                //    throw new CardPaymentException("Check printer error", "Printer '{0}' is not available. '{1}'", this.deviceName, ex.Message);
                //}
            }
            #endregion

            #region PC-EFTPOS PROPERTIES
            public void GetExtensionProperties(ExtensionTransaction extensionTransactionProperties)
            {
                if (extensionTransactionProperties == null)
                {
                    return;
                }
                CommerceProperty property = null;

                property = extensionTransactionProperties.ExtensionProperties.FirstOrDefault(line => line.Key == "UPOS_TRANSACTIONID");
                if (property != null && property.Value.HasBeenSet && property.Key.ToUpper() == "UPOS_TRANSACTIONID")
                {
                    transactionid = this.TransactionIdConverter((string)property.Value);
                }
                property = extensionTransactionProperties.ExtensionProperties.FirstOrDefault(line => line.Key == "UPOS_DEFAULTCARDNUMBER");
                if (property != null && property.Value.HasBeenSet && property.Key.ToUpper() == "UPOS_DEFAULTCARDNUMBER")
                {
                    defaultCard = (string)property.Value;
                }
                property = extensionTransactionProperties.ExtensionProperties.FirstOrDefault(line => line.Key == "UPOS_CARDTYPE");
                if (property != null && property.Value.HasBeenSet && property.Key.ToUpper() == "UPOS_CARDTYPE")
                {
                    string cardType = (string)property.Value;
                    cardTypeCollection = UPOS_CardTypeInfoHeader.FromXml(cardType);
                }
                property = extensionTransactionProperties.ExtensionProperties.FirstOrDefault(line => line.Key == "UPOS_DEVICENAME");
                if (property != null && property.Value.HasBeenSet && property.Key.ToUpper() == "UPOS_DEVICENAME")
                {
                    deviceName = (string)property.Value;
                }
                property = extensionTransactionProperties.ExtensionProperties.FirstOrDefault(line => line.Key == "UPOS_CHARACTERSET");
                if (property != null && property.Value.HasBeenSet && property.Key.ToUpper() == "UPOS_CHARACTERSET")
                {
                    characterSet = (int)property.Value.IntegerValue;
                }
                property = extensionTransactionProperties.ExtensionProperties.FirstOrDefault(line => line.Key == "UPOS_DEVICETYPE");
                if (property != null && property.Value.HasBeenSet && property.Key.ToUpper() == "UPOS_DEVICETYPE")
                {
                    deviceType = (string)property.Value;
                }
                property = extensionTransactionProperties.ExtensionProperties.FirstOrDefault(line => line.Key == "UPOS_BINARYCONVERSION");
                if (property != null && property.Value.HasBeenSet && property.Key.ToUpper() == "UPOS_BINARYCONVERSION")
                {
                    binaryConversion = (bool)property.Value.BooleanValue;
                }
                property = extensionTransactionProperties.ExtensionProperties.FirstOrDefault(line => line.Key == "UPOS_SIGNATUREVERIFIED");
                if (property != null && property.Value.HasBeenSet && property.Key.ToUpper() == "UPOS_SIGNATUREVERIFIED")
                {
                    signatureVerified = (bool)property.Value.BooleanValue;
                }
                else
                {
                    signatureVerified = false;
                }
                property = extensionTransactionProperties.ExtensionProperties.FirstOrDefault(line => line.Key == "UPOS_SIGNATUREVERIFIEDPROCESS");
                if (property != null && property.Value.HasBeenSet && property.Key.ToUpper() == "UPOS_SIGNATUREVERIFIEDPROCESS")
                {
                    signatureVerifiedProcess = (bool)property.Value.BooleanValue;
                }
                else
                {
                    signatureVerifiedProcess = false;
                }
                property = extensionTransactionProperties.ExtensionProperties.FirstOrDefault(line => line.Key == "UPOS_MERCHANT");
                if (property != null && property.Value.HasBeenSet && property.Key.ToUpper() == "UPOS_MERCHANT")
                {
                    merchant = property.Value.StringValue;
                }
            }

            private List<PaymentProperty> GetPaymentProperties(decimal amountPurchase, string cardType, string cardPAN)
            {
                List<PaymentProperty> paymentProperties = new List<PaymentProperty>();

                try
                {
                    PaymentProperty paymentProperty = new PaymentProperty(
                      GenericNamespace.Connector,
                      ConnectorProperties.ConnectorName,
                      "UPOS Connector");
                    paymentProperties.Add(paymentProperty);

                    paymentProperty = new PaymentProperty(
                     GenericNamespace.TransactionData,
                     TransactionDataProperties.IndustryType,
                     IndustryType.Retail.ToString());
                    paymentProperties.Add(paymentProperty);

                    // Amount.
                    paymentProperty = new PaymentProperty(
                        GenericNamespace.TransactionData,
                        TransactionDataProperties.Amount,
                        Math.Abs(amountPurchase)); // for refunds request amount must be positive
                    paymentProperties.Add(paymentProperty);

                    //cardType type
                    paymentProperty = new PaymentProperty(
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.Description,
                    cardType);
                    paymentProperties.Add(paymentProperty);

                    //MerchantAccount type
                    paymentProperty = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.ServiceAccountId,
                    "cc4686ae-95ed-4b34-90be-1818c58999b9");
                    paymentProperties.Add(paymentProperty);

                    string cardTypeInfo = string.Empty;
                    try
                    {
                        cardTypeInfo = Utilities.GetMaskedCardNumber(cardPAN);
                    }
                    catch (System.Exception ex)
                    {
                        RetailLogger.Instance.PaymentConnectorLogException("GetMaskedCardNumber", "Connector Name PCEFTPOS", "platform", ex);
                    }
                    if (!string.IsNullOrEmpty(cardTypeInfo))
                    {
                        paymentProperty = new PaymentProperty(
                           GenericNamespace.TransactionData,
                           TransactionDataProperties.CardNumber,
                           cardTypeInfo);
                        paymentProperties.Add(paymentProperty);
                    }

                }
                catch (System.Exception ex)
                {
                    RetailLogger.Instance.PaymentConnectorLogException("PaymentProperties", "Connector Name PCEFTPOS", "platform", ex);
                }
                return paymentProperties;
            }
            private string GetPaymentSdkData(List<PaymentProperty> paymentProperties)
            {
                string paymentSdkData = string.Empty;
                try
                {
                    if (paymentProperties != null && paymentProperties.Count > 0)
                    {
                        paymentSdkData = PaymentProperty.ConvertPropertyArrayToXML(paymentProperties.ToArray());
                    }
                    else
                    {
                        paymentSdkData = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    RetailLogger.Instance.PaymentConnectorLogException("ConvertPropertyArrayToXML", "Connector Name PCEFTPOS", "platform", ex);
                }
                return paymentSdkData;
            }
            #endregion

            #region PC-EFTPOS INTEGRATION
            public async Task<ExtensionTransaction> ExecuteIntegrationPowerFailure()
            {
                //check if printer is online             
                this.CheckPrinter();

                try
                {
                    await ConnectAsync().ConfigureAwait(false);
                    //Power failure call get last transaction       
                    EFTGetLastTransactionRequest r = new EFTGetLastTransactionRequest(this.transactionid);

                    await EftDoGetLastTransaction(r).ConfigureAwait(false);
                    //get last receipt
                    if (string.IsNullOrEmpty(this.errorMessage))
                    {
                        var request = new EFTReprintReceiptRequest()
                        {
                            ReprintType = ReprintType.GetLast,
                            ReceiptAutoPrint = ReceiptPrintModeType.POSPrinter,
                            CutReceipt = ReceiptCutModeType.Cut
                        };

                        await EftDoDuplicateReceipt(request).ConfigureAwait(false);
                    }

                }
                catch (System.Exception ex)
                {
                    RetailLogger.Instance.PaymentConnectorLogException("call frmEFT", "Connector Name PCEFTPOS", "platform", ex);
                    errorMessage = ex.Message;
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    throw new CardPaymentException("Error message", errorMessage);
                }

                List<PaymentProperty> paymentProperties = this.GetPaymentProperties(getLastTransactionResponse.AmtPurchase, getLastTransactionResponse.CardType, getLastTransactionResponse.Pan);

                string paymentSdkData = string.Empty;
                try
                {
                    if (paymentProperties != null && paymentProperties.Count > 0)
                    {
                        paymentSdkData = PaymentProperty.ConvertPropertyArrayToXML(paymentProperties.ToArray());
                    }
                    else
                    {
                        paymentSdkData = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    RetailLogger.Instance.PaymentConnectorLogException("ConvertPropertyArrayToXML", "Connector Name PCEFTPOS", "platform", ex);
                }

                EFTClientIPHelper._eftAsync = null;

                ExtensionTransaction extensionTransaction = new ExtensionTransaction();
                List<CommerceProperty> list = new List<CommerceProperty>();
                list.Add(new CommerceProperty("UPOS_MaskCard", CardTypeHelper.getCardNumber(this.getLastTransactionResponse.Pan, this.cardTypeCollection, this.defaultCard)));
                list.Add(new CommerceProperty("UPOS_Amount", getLastTransactionResponse.TxnType == PCEFTPOS.EFTClient.IPInterface.TransactionType.Refund ?
                    this.getLastTransactionResponse.AmtPurchase * -1 : this.getLastTransactionResponse.AmtPurchase));
                list.Add(new CommerceProperty("UPOS_IsApproved", this.getLastTransactionResponse.LastTransactionSuccess));
                list.Add(new CommerceProperty("UPOS_PaymentSdkData", paymentSdkData));
                list.Add(new CommerceProperty("UPOS_CardType", CardTypeHelper.getCardType(this.getLastTransactionResponse.Pan, this.cardTypeCollection, this.defaultCard)));

                extensionTransaction.ExtensionProperties = list;
                return extensionTransaction;

            }
            public async Task<PaymentInfo> ExecuteIntegrationSignatureVeryfiedAsync(decimal amount, string currency, UPOS_PaymentType paymentType)
            {

                try
                {

                    // send request accept/declined from MPOS
                    var key = this.signatureVerified == true ? EFTPOSKey.YesAccept : EFTPOSKey.NoDecline;

                    await EFTClientIPHelper._eftAsync.WriteRequestAsync(new EFTSendKeyRequest() { Key = key }).ConfigureAwait(false);

                    //call integration
                    EFTTransactionRequest r = new EFTTransactionRequest();

                    // TxnType is required
                    if (paymentType == UPOS_PaymentType.payment)
                    {
                        r.TxnType = PCEFTPOS.EFTClient.IPInterface.TransactionType.PurchaseCash;
                    }
                    else
                    {
                        r.TxnType = PCEFTPOS.EFTClient.IPInterface.TransactionType.Refund;
                    }
                    // Set ReferenceNumber to something unique
                    r.TxnRef = this.transactionid;

                    // Set AmountCash for cash out, and AmountPurchase for purchase/refund
                    r.AmtPurchase = amount;

                    r.ReceiptAutoPrint = ReceiptPrintModeType.POSPrinter;
                    // Set application. Used for gift card & 3rd party payment
                    r.Application = TerminalApplication.EFTPOS;
                    r.PurchaseAnalysisData = new PadField(this.merchant);
                    await EftOnTransaction(r).ConfigureAwait(false);

                }
                catch (System.Exception ex)
                {
                    RetailLogger.Instance.PaymentConnectorLogException("call frmEFT", "Connector Name PCEFTPOS", "platform", ex);
                }


                List<PaymentProperty> paymentProperties = this.GetPaymentProperties(transactionResponse.AmtPurchase, transactionResponse.CardType, transactionResponse.Pan);
                string paymentSdkData = string.Empty;

                try
                {
                    if (paymentProperties != null && paymentProperties.Count > 0)
                    {
                        paymentSdkData = PaymentProperty.ConvertPropertyArrayToXML(paymentProperties.ToArray());
                    }
                    else
                    {
                        paymentSdkData = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    RetailLogger.Instance.PaymentConnectorLogException("ConvertPropertyArrayToXML", "Connector Name PCEFTPOS", "platform", ex);
                }
                return new PaymentInfo
                {
                    CardNumberMasked = CardTypeHelper.getCardNumber(this.transactionResponse.Pan, this.cardTypeCollection, this.defaultCard),
                    CardType = Microsoft.Dynamics.Commerce.HardwareStation.CardPayment.CardType.Unknown,
                    SignatureData = "",
                    PaymentSdkData = paymentSdkData,
                    CashbackAmount = 0.0m,
                    ApprovedAmount = this.transactionResponse.AmtPurchase,
                    IsApproved = this.transactionResponse.Success,
                    Errors = null
                };
            }

            public async Task<PaymentInfo> ExecuteIntegrationAsync(decimal amount, string currency, UPOS_PaymentType paymentType)
            {
                bool showDialogInMPOS = false;

                //check if printer is online             
                this.CheckPrinter();

                try
                {
                    await ConnectAsync().ConfigureAwait(false);

                    EFTTransactionRequest r = new EFTTransactionRequest();

                    // TxnType is required
                    if (paymentType == UPOS_PaymentType.payment)
                    {
                        r.TxnType = PCEFTPOS.EFTClient.IPInterface.TransactionType.PurchaseCash;
                    }
                    else
                    {
                        r.TxnType = PCEFTPOS.EFTClient.IPInterface.TransactionType.Refund;
                    }
                    // Set ReferenceNumber to something unique
                    r.TxnRef = this.transactionid;

                    // Set AmountCash for cash out, and AmountPurchase for purchase/refund
                    r.AmtPurchase = amount;

                    r.ReceiptAutoPrint = ReceiptPrintModeType.POSPrinter;
                    // Set application. Used for gift card & 3rd party payment
                    r.Application = TerminalApplication.EFTPOS;

                    r.PurchaseAnalysisData = new PadField(this.merchant);

                    showDialogInMPOS = await EftOnTransaction(r).ConfigureAwait(false);
                }
                catch (System.Exception ex)
                {
                    RetailLogger.Instance.PaymentConnectorLogException("call frmEFT", "Connector Name PCEFTPOS", "platform", ex);
                }

                if (showDialogInMPOS == true)
                {
                    return new PaymentInfo
                    {
                        CardNumberMasked = "SHOWSIGNATUREVERIFIEDDIALOG",// change to showdialog
                        CardType = Microsoft.Dynamics.Commerce.HardwareStation.CardPayment.CardType.Unknown,
                        SignatureData = "",
                        PaymentSdkData = "",
                        CashbackAmount = 0.0m,
                        ApprovedAmount = 0.0m,
                        IsApproved = false,
                        Errors = null
                    };
                }

                List<PaymentProperty> paymentProperties = this.GetPaymentProperties(transactionResponse.AmtPurchase, transactionResponse.CardType, transactionResponse.Pan);

                EFTClientIPHelper._eftAsync = null;
                return new PaymentInfo
                {
                    CardNumberMasked = CardTypeHelper.getCardNumber(this.transactionResponse.Pan, this.cardTypeCollection, this.defaultCard),
                    CardType = Microsoft.Dynamics.Commerce.HardwareStation.CardPayment.CardType.Unknown,
                    SignatureData = this.transactionResponse.ResponseCode.ToString() + " , " + this.transactionResponse.ResponseText,
                    PaymentSdkData = GetPaymentSdkData(paymentProperties),
                    CashbackAmount = 0.0m,
                    ApprovedAmount = this.transactionResponse.AmtPurchase,

                    IsApproved = this.transactionResponse.Success,
                    Errors = null
                };

            }
            async Task<bool> EftDoGetLastTransaction(EFTGetLastTransactionRequest request)
            {
                return await this.EFT(request).ConfigureAwait(false);
            }
            async Task<bool> EftDoDuplicateReceipt(EFTReprintReceiptRequest request)
            {
                return await this.EFT(request).ConfigureAwait(false);
            }
            async Task<bool> EftOnTransaction(EFTTransactionRequest request)
            {
                return await this.EFT(request).ConfigureAwait(false);
            }

            async Task<bool> EFT(EFTRequest request)
            {
                bool showDialogInMPOS = false;

                EFTClientIPHelper._eftAsync = _eftAsync;
                if (!await _eftAsync.WriteRequestAsync(request).ConfigureAwait(false))
                {
                    throw new CardPaymentException("Error WriteRequestAsync", "PCEFTPOS FAILED TO SEND TXN");
                }
                else
                {
                    // Wait for response
                    var waitingForResponse = true;
                    do
                    {
                        try
                        {
                            //var timeoutToken = new CancellationTokenSource(new TimeSpan(0, 5, 0));
#pragma warning disable CA2000 // Dispose objects before losing scope
                            var timeoutToken = new CancellationTokenSource(new TimeSpan(0, 5, 0)).Token;
#pragma warning restore CA2000 // Dispose objects before losing scope
                            EFTResponse eftResponse = await _eftAsync.ReadResponseAsync(timeoutToken).ConfigureAwait(false);
                            {
                                // Handle response
                                if (eftResponse == null)
                                {
                                    // Error reading response
                                    waitingForResponse = false;
                                }
                                if (eftResponse is EFTReceiptResponse)
                                {
                                    OnReceipt(eftResponse as EFTReceiptResponse);
                                }
                                else if (eftResponse is EFTDisplayResponse)
                                {
                                    showDialogInMPOS = OnDisplay(eftResponse as EFTDisplayResponse);
                                    if (showDialogInMPOS == true)
                                    {
                                        waitingForResponse = false;
                                    }
                                }
                                else if (eftResponse is EFTTransactionResponse)
                                {
                                    EFTTransactionResponse TransactionRes = eftResponse as EFTTransactionResponse;
                                    if (TransactionRes.AmtPurchase != 0)
                                    {
                                        waitingForResponse = false;
                                        OnTransaction(eftResponse as EFTTransactionResponse);
                                    }
                                }
                                else if (eftResponse is EFTGetLastTransactionResponse)
                                {
                                    waitingForResponse = false;
                                    OnGetLastTransaction(eftResponse as EFTGetLastTransactionResponse);
                                }
                                else if (eftResponse is EFTReprintReceiptResponse)
                                {
                                    waitingForResponse = false;
                                    OnDuplicateReceipt(eftResponse as EFTReprintReceiptResponse);
                                }
                            }
                        }
                        catch (TaskCanceledException)
                        {
                            // EFT-Client timeout waiting for response                      
                            waitingForResponse = false;
                        }
                        catch (ConnectionException)
                        {
                            // Socket closed                  
                            waitingForResponse = false;
                        }
                        catch (Exception)
                        {
                            // Unhandled exception                        
                            waitingForResponse = false;
                        }
                    }
                    while (waitingForResponse);
                }
                return showDialogInMPOS;
            }
            #endregion

            #region PC-EFTPOS EVENTS
            //--> async events
            void OnGetLastTransaction(EFTGetLastTransactionResponse r)
            {
                this.errorMessage = string.Empty;

                this.getLastTransactionResponse = r;

                if (r.TxnRef.Replace(" ", string.Empty) != this.transactionid)
                {
                    this.getLastTransactionResponse.LastTransactionSuccess = false;
                    this.errorMessage = "System did not detect Power Failure for the current transaction. Please try tender payment";
                }
            }
            void OnDuplicateReceipt(EFTReprintReceiptResponse r)
            {
                if (r.ReceiptText.Length > 0)
                {
                    var receipt = new StringBuilder(26 * r.ReceiptText.Length);
                    foreach (var l in r.ReceiptText)
                    {
                        receipt.AppendLine(l);
                    }
                    PrintReceipt(receipt.ToString());
                }
            }
            void OnTransaction(EFTTransactionResponse r)
            {
                this.transactionResponse = r;
            }
            void OnReceipt(EFTReceiptResponse r)
            {
                if (!r.IsPrePrint)
                {
                    var receipt = new StringBuilder(26 * r.ReceiptText.Length);
                    foreach (var l in r.ReceiptText)
                    {
                        receipt.AppendLine(l);
                    }
                    PrintReceipt(receipt.ToString());
                }
            }
            void PrintReceipt(string receipt)
            {
                if (!String.IsNullOrEmpty(receipt))
                {
                    try
                    {
                        Microsoft.Dynamics.Commerce.HardwareStation.Models.PrintRequest printerRequest = new Microsoft.Dynamics.Commerce.HardwareStation.Models.PrintRequest();
                        printerRequest.CharacterSet = this.characterSet;
                        printerRequest.DeviceType = this.deviceType;
                        printerRequest.DeviceName = this.deviceName;
                        printerRequest.BinaryConversion = this.binaryConversion;
                        printerRequest.Header = "\n";
                        printerRequest.Lines = receipt.ToString();
                        printerRequest.Footer = "\n\n\n\n\n\n";
                        //using (Microsoft.Dynamics.Commerce.HardwareStation.Controllers.PrinterController pc = new Microsoft.Dynamics.Commerce.HardwareStation.Controllers.PrinterController())
                        //{
                        //    System.Collections.ObjectModel.Collection<Microsoft.Dynamics.Commerce.HardwareStation.Models.PrintRequest> list = new System.Collections.ObjectModel.Collection<Microsoft.Dynamics.Commerce.HardwareStation.Models.PrintRequest>();
                        //    list.Add(printerRequest);
                        //    pc.Print(list);
                        //}
                    }
                    catch (Exception ex)
                    {
                        throw new CardPaymentException("Print Receipt", String.Format("Printer '{0}' is not available. '{1}'", this.deviceName, ex.Message));
                    }
                    //    PrintPrinterDeviceRequest printPrinterDeviceRequest = new PrintPrinterDeviceRequest(printerRequest.Header, printerRequest.Lines, printerRequest.Footer);
                    //    if (this.deviceType == "OPOS")
                    //    {
                    //        OpenPrinterDeviceRequest request = new OpenPrinterDeviceRequest(this.deviceName, null, this.characterSet, this.binaryConversion);
                    //        //RetailLogger.Log.HardwareStationPrinterDeviceRequestInformation(printRequestConfigKey.DeviceName, printRequestConfigKey.DeviceType, printRequestConfigKey.CharacterSet, printRequestConfigKey.BinaryConversion);
                    //        var deviceTypeOpenRequest = request.RequestContext.Runtime.GetAsyncRequestHandler(typeof(OpenPrinterDeviceRequest), this.deviceType);//"OPOS"

                    //        deviceTypeOpenRequest.Execute(request);
                    //        var deviceTypeRequest = request.RequestContext.Runtime.GetAsyncRequestHandler(typeof(PrintPrinterDeviceRequest), this.deviceType);//"OPOS"
                    //        if (deviceTypeRequest != null)
                    //        {

                    //            deviceTypeRequest.Execute(printPrinterDeviceRequest);
                    //        }
                    //        else
                    //        {
                    //            var deviceNameRequest = request.RequestContext.Runtime.GetAsyncRequestHandler(typeof(PrintPrinterDeviceRequest), this.deviceName);
                    //            deviceNameRequest.Execute(printPrinterDeviceRequest);
                    //        }
                    //        ClosePrinterDeviceRequest request1 = new ClosePrinterDeviceRequest();

                    //        var deviceTypeCloseRequest = request1.RequestContext.Runtime.GetAsyncRequestHandler(typeof(ClosePrinterDeviceRequest), this.deviceType);//"OPOS"
                    //        deviceTypeCloseRequest.Execute(request1);

                    //    }
                    //    else
                    //    {
                    //        printPrinterDeviceRequest.RequestContext.Runtime.ExecuteAsync<Microsoft.Dynamics.Commerce.Runtime.Messages.Response>(printPrinterDeviceRequest, printPrinterDeviceRequest.RequestContext);
                    //    }


                    //}
                    //catch (Exception ex)
                    //{
                    //    ClosePrinterDeviceRequest request2 = new ClosePrinterDeviceRequest();

                    //    var deviceTypeCloseRequest = request2.RequestContext.Runtime.GetAsyncRequestHandler(typeof(ClosePrinterDeviceRequest), this.deviceType);//"OPOS"
                    //    deviceTypeCloseRequest.Execute(request2);
                    //    new PeripheralException(PeripheralException.PaymentTerminalError, String.Format("Printer is not available."), ex);
                    //}
                }
            }
            bool OnDisplay(EFTDisplayResponse r)
            {
                bool showDialogInMPOS = false;
                //"SIGNATURE VERIFIED?"
                if (r.GraphicCode == GraphicCode.Verify)
                {
                    showDialogInMPOS = true;
                }
                //if response is signatureStr == "SIGNATURE ERROR" || signatureStr == "APPROVED" then we want to trigger OKCancel action
                else if (r.OKKeyFlag == true)
                {
                    EFTClientIPHelper._eftAsync.WriteRequestAsync(new EFTSendKeyRequest() { Key = EFTPOSKey.OkCancel });
                }

                return showDialogInMPOS;
            }
            //<async events
            #endregion

            #region PC-EFTPOS TRANSACTION
            private string TransactionIdConverter(string transactionId)
            {
                string transactionIdLocal = string.Empty;

                transactionId = transactionId.Replace("-", "");

                if (transactionId.Length < 16)
                {
                    transactionIdLocal = transactionId;
                }
                else
                {
                    transactionIdLocal = transactionId.Substring(transactionId.Length - 16, 16);
                }
                return transactionIdLocal;
            }
            #endregion

            #region PC-EFTPOS CONNECT ASYNC
            async Task<bool> ConnectEFTAsync()
            {
                bool _isServerVerified = false;

                var enableCloud = false;// _settings.EnableCloud;
                var addr = _settings.EFTClientAddress.Split(new char[] { ':' });
                var tmpPort = 0;
                if (addr.Length < 2 || int.TryParse(addr[1], out tmpPort) == false)
                {
                    throw new CardPaymentException("ConnectEFTAsync", "PCEFTPOS INVALID ADDRESS");
                }

                bool connected = false;
                try
                {
                    connected = await _eftAsync.ConnectAsync(addr[0], tmpPort, enableCloud, enableCloud).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    connected = false;
                }

                var notifyText = "";

                if (connected && enableCloud)
                {
                    // Cloud login if in cloud mode
                    if (enableCloud)
                    {
                        var waiting = await _eftAsync.WriteRequestAsync(new EFTCloudLogonRequest() { ClientID = "", Password = "", PairingCode = "" }).ConfigureAwait(false);
                        //  var waiting = await _eft.WriteRequestAsync(new EFTCloudLogonRequest() { ClientID = _settings.Username, Password = _settings.Password, PairingCode = _settings.PairingCode });
                        while (waiting)
                        {

#pragma warning disable CA2000 // Dispose objects before losing scope
                            var eftResponse = await _eftAsync.ReadResponseAsync(new CancellationTokenSource(45000).Token).ConfigureAwait(false);
#pragma warning restore CA2000 // Dispose objects before losing scope

                            if (eftResponse is EFTCloudLogonResponse)
                            {
                                var cloudLogonResponse = eftResponse as EFTCloudLogonResponse;
                                if (!cloudLogonResponse.Success)
                                {
                                    connected = false;
                                    notifyText = $"{cloudLogonResponse.ResponseCode} {cloudLogonResponse.ResponseText}";
                                }
                                waiting = false;
                            }
                        }
                    }
                }

                if (connected)
                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                    var timeoutToken = new CancellationTokenSource(new TimeSpan(0, 5, 0)).Token;
#pragma warning restore CA2000 // Dispose objects before losing scope
                    _isServerVerified = true;
                    
                    // This is only required if we aren't using the PC-EFTPOS dialog
                    await _eftAsync.WriteRequestAsync(new SetDialogRequest() { DialogType = DialogType.Hidden }).ConfigureAwait(false);

                   await _eftAsync.ReadResponseAsync(timeoutToken).ConfigureAwait(false);

                }
                else
                {
                    throw new CardPaymentException("ConnectEFTAsync", "PCEFTPOS ERROR CONTACTING EFT - CLIENT");
                }
                return _isServerVerified;
            }
            async Task<bool> ConnectAsync()
            {
                // Set up PC-EFTPOS connection
                _eftAsync = new EFTClientIPAsync();
                _settings = new Settings();
                _settings.Load();

                // Try to connect if we have an address. Either navigate to the main 
                // page (if we are connected) or the settings page (if we aren't)
                var connected = false;
                if (_settings?.EFTClientAddress.Length > 0)
                {
                    try
                    {
                        connected = await ConnectEFTAsync().ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        connected = await ConnectEFTAsync().ConfigureAwait(false);
                    }

                }
                return connected;
            }

            #endregion ASYNC
            //<--UPOS/Wojtek/009443 EFT integration          
        }
    }
}
