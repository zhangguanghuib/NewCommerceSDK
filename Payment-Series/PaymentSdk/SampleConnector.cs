/*
 IMPORTANT!!!
 THIS IS SAMPLE CODE ONLY.
 THE CODE SHOULD BE UPDATED TO WORK WITH THE APPROPRIATE PAYMENT PROVIDERS.
 PROPER MESASURES SHOULD BE TAKEN TO ENSURE THAT THE PA-DSS AND PCI DSS REQUIREMENTS ARE MET.
*/
namespace Microsoft.Dynamics
{
    namespace Retail.SampleConnector.Portable
    {
        using System;
        using System.Collections.Generic;
        using System.Composition;
        using System.Diagnostics.CodeAnalysis;
        using System.Globalization;
        using System.Net;
        using System.Net.Http;
        using System.Net.Http.Headers;
        using System.Text;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Retail.Diagnostics;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        /// <summary>
        /// SampleConnector class (Portable Class Library version).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "SAMPLE CODE ONLY")]
        [Export(typeof(IPaymentProcessor))]
        public class SampleConnector : SampleProcessorIdentifier, IPaymentProcessor, IPaymentProcessorExtension, IPaymentReferenceProvider, IPaymentMethodInfo
        {
            #region Constants

            private const string Platform = "Portable";

            // Supported environments
            private const string EnvironmentONEBOX = "ONEBOX";
            private const string EnvironmentINT = "INT";
            private const string EnvironmentPROD = "PROD";
            private const string EnvironmentMock = "MockHtmlContents";

            private const string PaymentAcceptBaseAddressONEBOX = @"http://localhost:3973/Payments";
            private const string PaymentAcceptBaseAddressINT = @"https://paymentacceptsample.cloud.int.dynamics.com";
            private const string PaymentAcceptBaseAddressPROD = @"https://paymentacceptsample.cloud.dynamics.com";

            private const string OperationStarting = "starting";

#if DEBUG
#if USE_INT
        // INT
        private const string DefaultEnvironment = "INT";
#else
            // ONEBOX
            private const string DefaultEnvironment = "ONEBOX";
#endif
#else // ~DEBUG
            // PROD
            private const string DefaultEnvironment = "PROD";
#endif

            /// <summary>
            /// The relative URI of the web service for getting payment accepting point.
            /// </summary>
            private const string GetPaymentAcceptPointRelativeUri = "/Payments/GetPaymentAcceptPoint";

            /// <summary>
            /// The relative URI of the web service for retrieving payment accepting result.
            /// </summary>
            private const string RetrievePaymentAcceptResultRelativeUri = "/Payments/RetrievePaymentAcceptResult";

            /// <summary>
            /// The padding character.
            /// </summary>
            private const char PaddingCharacter = '*';

            private static Dictionary<string, decimal> activatedGiftCards = new Dictionary<string, decimal>();

            private static decimal declinedAmount = 5.12M;

            #endregion

            #region Constructors

            static SampleConnector()
            {
                // Force TLS 1.2
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                activatedGiftCards.Add("61234", 1000.0m);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SampleConnector"/> class.
            /// </summary>
            public SampleConnector()
            {
            }

            #endregion

            /// <summary>
            /// Gets the internal property activedGiftCards.
            /// </summary>
            internal static Dictionary<string, decimal> ActivatedGiftCards
            {
                get
                {
                    return activatedGiftCards;
                }
            }

            #region IPaymentProcessor methods

            /// <summary>
            /// Authorize the request with the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the authorize transaction.</param>
            /// <param name="requiredInteractionProperties">Properties required by authorization process.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response Authorize(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                string methodName = "Authorize";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request
                AuthorizeRequest authorizeRequest = null;
                try
                {
                    authorizeRequest = AuthorizeRequest.ConvertFrom(request);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                // Validate merchant account
                List<PaymentError> errors = new List<PaymentError>();
                ValidateMerchantProperties(authorizeRequest, errors);
                if (errors.Count > 0)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, authorizeRequest.Locale, errors);
                }

                // Authorize and create response
                var authorizeResponse = new AuthorizeResponse(authorizeRequest.Locale, authorizeRequest.ServiceAccountId, this.Name);
                authorizeResponse.IsSwipe = authorizeRequest.IsSwipe;
                authorizeResponse.CardType = authorizeRequest.CardType;
                authorizeResponse.CardToken = authorizeRequest.CardToken;
                authorizeResponse.UniqueCardId = authorizeRequest.UniqueCardId;
                authorizeResponse.VoiceAuthorizationCode = authorizeRequest.VoiceAuthorizationCode;
                authorizeResponse.AccountType = authorizeRequest.AccountType;
                authorizeResponse.CurrencyCode = authorizeRequest.CurrencyCode;
                authorizeResponse.TransactionType = TransactionType.Authorize.ToString();
                authorizeResponse.TransactionDateTime = DateTime.UtcNow;
                authorizeResponse.PaymentTrackingId = authorizeRequest.PaymentTrackingId;

                if (authorizeRequest.CardNumber != null)
                {
                    authorizeResponse.Last4Digit = GetLastFourDigits(authorizeRequest.CardNumber);
                }
                else if (authorizeRequest.Last4Digit != null)
                {
                    authorizeResponse.Last4Digit = authorizeRequest.Last4Digit;
                }

                if ((authorizeRequest.SupportCardTokenization ?? false) && string.IsNullOrWhiteSpace(authorizeRequest.CardToken))
                {
                    // Tokenize card
                    authorizeResponse.CardToken = GetToken(authorizeRequest.CardNumber);
                    authorizeResponse.UniqueCardId = Guid.NewGuid().ToString();
                }

                if (authorizeRequest.AuthorizationProviderTransactionId != null)
                {
                    // Check for ReAuth
                    authorizeResponse.ProviderTransactionId = authorizeRequest.AuthorizationProviderTransactionId;
                }
                else
                {
                    authorizeResponse.ProviderTransactionId = Guid.NewGuid().ToString();
                }

                // GIFT CARD (GC) AUTHORIZATION
                // If CardType==GiftCard, we need to:
                //  1. Check whether it has been activated ahead of time.
                //  2. If true, then check whether it has insufficient funds.
                if (authorizeRequest.CardType.Equals(CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    string giftCardNumber = authorizeRequest.CardNumber;

                    // If GC has been activated, check if there are sufficient funds
                    if (activatedGiftCards.ContainsKey(giftCardNumber))
                    {
                        decimal giftCardBalance = activatedGiftCards[giftCardNumber];

                        // Insufficient funds
                        if (giftCardBalance == decimal.Zero)
                        {
                            authorizeRequest.Amount = decimal.Parse(TestData.AuthorizationDeclined);
                            errors.Add(new PaymentError(ErrorCode.InsufficientFunds, "Insufficient funds on gift card!"));
                        }
                    }
                    else
                    {
                        authorizeRequest.Amount = decimal.Parse(TestData.AuthorizationDeclined);
                        errors.Add(new PaymentError(ErrorCode.CardNotActivated, "Card not activated!"));
                    }
                }

                ProcessAuthorizationResult(authorizeResponse, authorizeRequest, errors);

                // Convert response and return
                Response response = AuthorizeResponse.ConvertTo(authorizeResponse);
                if (errors.Count > 0)
                {
                    response.Errors = errors.ToArray();
                }

                PaymentUtilities.LogOperationResult(methodName, this.Name, SampleConnector.Platform, request, response);
                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// Capture the request with the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the Capture transaction.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response Capture(Request request)
            {
                string methodName = "Capture";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request
                CaptureRequest captureRequest = null;
                try
                {
                    captureRequest = CaptureRequest.ConvertFrom(request);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                // Validate merchant account
                List<PaymentError> errors = new List<PaymentError>();
                ValidateMerchantProperties(captureRequest, errors);
                if (errors.Count > 0)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, captureRequest.Locale, errors);
                }

                // Capture and create response
                var captureResponse = new CaptureResponse(captureRequest.Locale, captureRequest.ServiceAccountId, this.Name);
                captureResponse.CardType = captureRequest.CardType;
                captureResponse.CardToken = captureRequest.CardToken;
                captureResponse.Last4Digit = captureRequest.Last4Digit;
                captureResponse.UniqueCardId = captureRequest.UniqueCardId;
                captureResponse.CurrencyCode = captureRequest.CurrencyCode;
                captureResponse.TransactionType = TransactionType.Capture.ToString();
                captureResponse.TransactionDateTime = DateTime.UtcNow;
                captureResponse.ProviderTransactionId = Guid.NewGuid().ToString();
                captureResponse.PaymentTrackingId = captureRequest.PaymentTrackingId;

                ProcessCaptureResult(captureResponse, captureRequest, errors);

                // Convert response and return
                Response response = CaptureResponse.ConvertTo(captureResponse);
                if (errors.Count > 0)
                {
                    response.Errors = errors.ToArray();
                }

                PaymentUtilities.LogOperationResult(methodName, this.Name, SampleConnector.Platform, request, response);
                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// ImmediateCapture the request with the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the ImmediateCapture transaction.</param>
            /// <param name="requiredInteractionProperties">Properties required by ImmediateCapture process.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response ImmediateCapture(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                var ex = new NotImplementedException("ImmediateCapture Not Implemented");
                RetailLogger.Log.PaymentConnectorLogException("ImmediateCapture", this.Name, Platform, ex);
                throw ex;
            }

            /// <summary>
            /// Void the request with the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the Void transaction.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response Void(Request request)
            {
                string methodName = "Void";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request
                VoidRequest voidRequest = null;
                try
                {
                    voidRequest = VoidRequest.ConvertFrom(request);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                // Validate merchant account
                List<PaymentError> errors = new List<PaymentError>();
                ValidateMerchantProperties(voidRequest, errors);
                if (errors.Count > 0)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, voidRequest.Locale, errors);
                }

                // Void and create response
                var voidResponse = new VoidResponse(voidRequest.Locale, voidRequest.ServiceAccountId, this.Name);
                voidResponse.CardType = voidRequest.CardType;
                voidResponse.Last4Digit = voidRequest.Last4Digit;
                voidResponse.UniqueCardId = voidRequest.UniqueCardId;
                voidResponse.CurrencyCode = voidRequest.CurrencyCode;
                voidResponse.TransactionType = TransactionType.Void.ToString();
                voidResponse.TransactionDateTime = DateTime.UtcNow;
                voidResponse.ProviderTransactionId = Guid.NewGuid().ToString();
                voidResponse.PaymentTrackingId = voidRequest.PaymentTrackingId;

                errors = ProcessVoidResult(voidResponse, voidRequest);

                // Convert response and return
                Response response = VoidResponse.ConvertTo(voidResponse);
                if (errors.Count > 0)
                {
                    response.Errors = errors.ToArray();
                }

                PaymentUtilities.LogOperationResult(methodName, this.Name, SampleConnector.Platform, request, response);
                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// Refund the request with the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the Refund transaction.</param>
            /// <param name="requiredInteractionProperties">Properties required by Refund process.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Test code only")]
            public Response Refund(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                string methodName = "Refund";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request
                RefundRequest refundRequest = null;
                try
                {
                    refundRequest = RefundRequest.ConvertFrom(request);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                // Validate merchant account
                List<PaymentError> errors = new List<PaymentError>();
                ValidateMerchantProperties(refundRequest, errors);
                if (errors.Count > 0)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, refundRequest.Locale, errors);
                }

                // Refund and create response
                var refundResponse = new RefundResponse(refundRequest.Locale, refundRequest.ServiceAccountId, this.Name);
                refundResponse.CardType = refundRequest.CardType;
                refundResponse.CardToken = refundRequest.CardToken;
                refundResponse.UniqueCardId = refundRequest.UniqueCardId;
                refundResponse.CurrencyCode = refundRequest.CurrencyCode;
                refundResponse.TransactionType = TransactionType.Refund.ToString();
                refundResponse.TransactionDateTime = DateTime.UtcNow;
                refundResponse.ProviderTransactionId = Guid.NewGuid().ToString();
                refundResponse.PaymentTrackingId = refundRequest.PaymentTrackingId;

                if (refundRequest.CardNumber != null)
                {
                    refundResponse.Last4Digit = GetLastFourDigits(refundRequest.CardNumber);
                }
                else if (refundRequest.Last4Digit != null)
                {
                    refundResponse.Last4Digit = refundRequest.Last4Digit;
                }

                if ((refundRequest.SupportCardTokenization ?? false) && string.IsNullOrWhiteSpace(refundRequest.CardToken))
                {
                    // Tokenize card
                    refundResponse.CardToken = GetToken(refundRequest.CardNumber);
                    refundResponse.UniqueCardId = Guid.NewGuid().ToString();
                }

                errors = ProcessRefundResult(refundResponse, refundRequest);

                // Convert response and return
                Response response = RefundResponse.ConvertTo(refundResponse);
                if (errors.Count > 0)
                {
                    response.Errors = errors.ToArray();
                }

                PaymentUtilities.LogOperationResult(methodName, this.Name, SampleConnector.Platform, request, response);
                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// Reversal the request with the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the Reversal transaction.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response Reversal(Request request)
            {
                var ex = new NotImplementedException("Reversal Not Implemented");
                RetailLogger.Log.PaymentConnectorLogException("Reversal", this.Name, Platform, ex);
                throw ex;
            }

            /// <summary>
            /// ReAuthorize the request with the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the ReAuthorize transaction.</param>
            /// <param name="requiredInteractionProperties">Properties required by ReAuthorize process.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response Reauthorize(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                var ex = new NotImplementedException("Reauthorize Not Implemented");
                RetailLogger.Log.PaymentConnectorLogException("Reauthorize", this.Name, Platform, ex);
                throw ex;
            }

            /// <summary>
            /// GenerateCardToken get the token for the requested credit card from the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the GenerateCardToken transaction.</param>
            /// <param name="requiredInteractionProperties">Properties required by GenerateCardToken process.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response GenerateCardToken(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                string methodName = "GenerateCardToken";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request
                GenerateCardTokenRequest tokenizeRequest = null;
                try
                {
                    tokenizeRequest = GenerateCardTokenRequest.ConvertFrom(request, requiredInteractionProperties);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                // Validate merchant account
                List<PaymentError> errors = new List<PaymentError>();
                ValidateMerchantProperties(tokenizeRequest, errors);
                if (errors.Count > 0)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, tokenizeRequest.Locale, errors);
                }

                // Create response
                var tokenizeResponse = new GenerateCardTokenResponse(tokenizeRequest.Locale, tokenizeRequest.ServiceAccountId, this.Name);
                if (tokenizeRequest.CardNumber.Length > 6)
                {
                    tokenizeResponse.BankIdentificationNumberStart = tokenizeRequest.CardNumber.Substring(0, 6);
                }

                tokenizeResponse.CardType = tokenizeRequest.CardType;
                tokenizeResponse.Last4Digit = GetLastFourDigits(tokenizeRequest.CardNumber);
                tokenizeResponse.CardToken = GetToken(tokenizeRequest.CardNumber);
                tokenizeResponse.UniqueCardId = Guid.NewGuid().ToString();
                tokenizeResponse.ExpirationMonth = tokenizeRequest.ExpirationMonth;
                tokenizeResponse.ExpirationYear = tokenizeRequest.ExpirationYear;
                tokenizeResponse.Name = tokenizeRequest.Name;
                tokenizeResponse.StreetAddress = tokenizeRequest.StreetAddress;
                tokenizeResponse.StreetAddress2 = tokenizeRequest.StreetAddress2;
                tokenizeResponse.City = tokenizeRequest.City;
                tokenizeResponse.State = tokenizeRequest.State;
                tokenizeResponse.PostalCode = tokenizeRequest.PostalCode;
                tokenizeResponse.Country = tokenizeRequest.Country;
                tokenizeResponse.Phone = tokenizeRequest.Phone;
                tokenizeResponse.AccountType = tokenizeRequest.AccountType;
                tokenizeResponse.OtherCardProperties = tokenizeRequest.OtherCardProperties;

                // Convert response and return
                Response response = GenerateCardTokenResponse.ConvertTo(tokenizeResponse);
                PaymentUtilities.LogOperationResult(methodName, this.Name, SampleConnector.Platform, request, response);
                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// GetPaymentAcceptPoint gets the payment accepting point from the payment provider, e.g. a payment page URL.
            /// </summary>
            /// <param name="request">Request object needed to process the GetPaymentAcceptPoint transaction.</param>
            /// <returns>Response object.</returns>
            public Response GetPaymentAcceptPoint(Request request)
            {
                string methodName = "GetPaymentAcceptPoint";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request to validate
                GetPaymentAcceptPointRequest acceptPointRequest = null;
                Uri baseAddress = null;
                try
                {
                    acceptPointRequest = GetPaymentAcceptPointRequest.ConvertFrom(request);
                    baseAddress = GetPaymentAcceptBaseAddress(acceptPointRequest.Environment);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                // Process and create response
                List<PaymentError> errors = new List<PaymentError>();
                var acceptPointResponse = new GetPaymentAcceptPointResponse(acceptPointRequest.Locale, acceptPointRequest.ServiceAccountId, this.Name);

                if (acceptPointRequest.Environment.Equals(EnvironmentMock, StringComparison.OrdinalIgnoreCase))
                {
                    // Only used by automation testing, sets the payment accept page contents to HTML string.
                    acceptPointResponse.PaymentAcceptUrl = string.Empty;
                    acceptPointResponse.PaymentAcceptPageContents = "<H4>Sample test</H4>";
                }
                else
                {
                    // Do not validate merchant account here, because the REST service will validate it.
                    using (HttpClient client = new HttpClient())
                    {
                        // Serialize request content
                        string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                        HttpContent content = new StringContent(requestJson);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        // Call REST service
                        HttpResponseMessage httpResponse = null;
                        client.BaseAddress = baseAddress;
                        Task getPaymentAcceptPointTask = Task.Run(() => httpResponse = client.PostAsync(GetPaymentAcceptPointRelativeUri, content).Result);
                        getPaymentAcceptPointTask.Wait();

                        // Handle REST response
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Read and deserialize response content
                            string responseJson = null;
                            Task readAsStringAsyncTask = Task.Run(() => responseJson = httpResponse.Content.ReadAsStringAsync().Result);
                            readAsStringAsyncTask.Wait();

                            Response getPaymentAcceptPointResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(responseJson);

                            // Prepare response properties
                            if (getPaymentAcceptPointResponse.Errors == null || getPaymentAcceptPointResponse.Errors.Length == 0)
                            {
                                // Read PaymentEntryUrl from response
                                var getPaymentAcceptPointResponseProperties = PaymentProperty.ConvertToHashtable(getPaymentAcceptPointResponse.Properties);
                                acceptPointResponse.PaymentAcceptUrl = GetPropertyStringValue(
                                    getPaymentAcceptPointResponseProperties,
                                    GenericNamespace.TransactionData,
                                    TransactionDataProperties.PaymentAcceptUrl,
                                    throwExceptionIfNotFound: true);
                                acceptPointResponse.PaymentAcceptSubmitUrl = null;
                                acceptPointResponse.PaymentAcceptMessageOrigin = new Uri(acceptPointResponse.PaymentAcceptUrl).GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);
                            }
                            else
                            {
                                errors.AddRange(getPaymentAcceptPointResponse.Errors);
                            }
                        }
                        else
                        {
                            // GetPaymentAcceptPoint service failure
                            errors.Add(new PaymentError(ErrorCode.ApplicationError, "GetPaymentAcceptPoint failure: Internal server error."));
                        }
                    }
                }

                // Convert response and return
                Response response = GetPaymentAcceptPointResponse.ConvertTo(acceptPointResponse);
                if (errors.Count > 0)
                {
                    response.Errors = errors.ToArray();
                }

                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// RetrievePaymentAcceptResult retrieves the payment accepting result from the payment provider after the payment is processed externally.
            /// This method pairs with GetPaymentAcceptPoint.
            /// </summary>
            /// <param name="request">Request object needed to process the RetrievePaymentAcceptResult transaction.</param>
            /// <returns>Response object.</returns>
            public Response RetrievePaymentAcceptResult(Request request)
            {
                string methodName = "RetrievePaymentAcceptResult";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request to validate
                RetrievePaymentAcceptResultRequest acceptResultRequest = null;
                Uri baseAddress = null;
                try
                {
                    acceptResultRequest = RetrievePaymentAcceptResultRequest.ConvertFrom(request);
                    baseAddress = GetPaymentAcceptBaseAddress(acceptResultRequest.Environment);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                // Process and create response
                List<PaymentError> errors = new List<PaymentError>();
                var acceptResultResponse = new RetrievePaymentAcceptResultResponse(acceptResultRequest.Locale, acceptResultRequest.ServiceAccountId, this.Name);

                // Do not validate merchant account here, because the REST service will validate it.
                Response response = null;
                using (HttpClient client = new HttpClient())
                {
                    // Serialize request content
                    string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    HttpContent content = new StringContent(requestJson);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    // Call REST service
                    HttpResponseMessage httpResponse = null;
                    client.BaseAddress = baseAddress;
                    Task retrievePaymentAcceptResultTask = Task.Run(() => httpResponse = client.PostAsync(RetrievePaymentAcceptResultRelativeUri, content).Result);
                    retrievePaymentAcceptResultTask.Wait();

                    // Handle REST response
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        // Read and deserialize response content
                        string responseJson = null;
                        Task readAsStringAsyncTask = Task.Run(() => responseJson = httpResponse.Content.ReadAsStringAsync().Result);
                        readAsStringAsyncTask.Wait();

                        Response retrievePaymentAcceptResultResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(responseJson);

                        // Return the retrieved response as is.
                        response = retrievePaymentAcceptResultResponse;

                        // E-Comm now requires the token to include the BankIdentificationNumberStart.
                        AddDefaultBankIdentificationNumberStart(response);
                    }
                    else
                    {
                        // RetrievePaymentAcceptResult service failure
                        errors.Add(new PaymentError(ErrorCode.ApplicationError, "RetrievePaymentAcceptResult failure: Internal server error."));
                        response = RetrievePaymentAcceptResultResponse.ConvertTo(acceptResultResponse);
                    }
                }

                // Return response
                if (errors.Count > 0)
                {
                    response.Errors = errors.ToArray();
                }

                PaymentUtilities.LogOperationResult(methodName, this.Name, SampleConnector.Platform, request, response);
                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// ActivateGiftCard the request with the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the ActivateGiftCard transaction.</param>
            /// <param name="requiredInteractionProperties">Properties required by ActivateGiftCard process.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response ActivateGiftCard(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                string methodName = "ActivateGiftCard";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request
                ActivateGiftCardRequest activateGiftCardRequest = null;

                try
                {
                    activateGiftCardRequest = ActivateGiftCardRequest.ConvertFrom(request);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                // Validate merchant account
                List<PaymentError> errors = new List<PaymentError>();
                ValidateMerchantProperties(activateGiftCardRequest, errors);
                if (errors.Count > 0)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, activateGiftCardRequest.Locale, errors);
                }

                // Authorize and create response
                var activateGiftCardResponse = new ActivateGiftCardResponse(activateGiftCardRequest.Locale, activateGiftCardRequest.ServiceAccountId, this.Name);
                activateGiftCardResponse.CardType = activateGiftCardRequest.CardType;
                activateGiftCardResponse.GiftCardNumber = activateGiftCardRequest.CardNumber;
                if (activateGiftCardRequest.CardNumber != null)
                {
                    activateGiftCardResponse.Last4Digit = GetLastFourDigits(activateGiftCardRequest.CardNumber);
                }
                else
                {
                    activateGiftCardResponse.Last4Digit = activateGiftCardRequest.Last4Digit;
                }

                if (activateGiftCardRequest.IsIssueGiftCard.HasValue && activateGiftCardRequest.IsIssueGiftCard.Value)
                {
                    DateTime now = DateTime.Now;
                    activateGiftCardResponse.ExpirationMonth = now.Month;
                    activateGiftCardResponse.ExpirationYear = now.Year + 1;
                }

                activateGiftCardResponse.ProviderTransactionId = Guid.NewGuid().ToString();

                errors = ProcessActivateGiftCardResult(activateGiftCardResponse, activateGiftCardRequest);

                // Convert response and return
                Response response = ActivateGiftCardResponse.ConvertTo(activateGiftCardResponse);
                if (errors.Count > 0)
                {
                    response.Errors = errors.ToArray();
                }

                PaymentUtilities.LogOperationResult(methodName, this.Name, SampleConnector.Platform, request, response);
                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// LoadGiftCard the request with the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the LoadGiftCard transaction.</param>
            /// <param name="requiredInteractionProperties">Properties required by LoadGiftCard process.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response LoadGiftCard(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                string methodName = "LoadGiftCard";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request
                LoadGiftCardRequest loadGCRequest = null;
                try
                {
                    loadGCRequest = LoadGiftCardRequest.ConvertFrom(request);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                /*
                 * VALID MERCHANT ACCOUNT.
                 */
                List<PaymentError> errors = new List<PaymentError>();
                ValidateMerchantProperties(loadGCRequest, errors);
                if (errors.Count > 0)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, loadGCRequest.Locale, errors);
                }

                LoadGiftCardResponse loadGCResponse = new LoadGiftCardResponse(loadGCRequest.Locale, loadGCRequest.ServiceAccountId, this.Name);
                loadGCResponse.CardType = loadGCRequest.CardType;
                loadGCResponse.GiftCardNumber = loadGCRequest.CardNumber;

                if (loadGCRequest.CardNumber != null)
                {
                    loadGCResponse.Last4Digit = GetLastFourDigits(loadGCRequest.CardNumber);
                }
                else if (loadGCRequest.Last4Digit != null)
                {
                    loadGCResponse.Last4Digit = loadGCRequest.Last4Digit;
                }

                loadGCResponse.ProviderTransactionId = Guid.NewGuid().ToString();

                errors = ProcessLoadGiftCardResult(loadGCResponse, loadGCRequest);

                // Convert response and return
                Response response = LoadGiftCardResponse.ConvertTo(loadGCResponse);
                if (errors.Count > 0)
                {
                    response.Errors = errors.ToArray();
                }

                PaymentUtilities.LogOperationResult(methodName, this.Name, SampleConnector.Platform, request, response);
                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// BalanceOnGiftCard the request with the payment provider.
            /// </summary>
            /// <param name="request">Request object needed to process the BalanceOnGiftCard transaction.</param>
            /// <param name="requiredInteractionProperties">Properties required by BalanceOnGiftCard process.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response BalanceOnGiftCard(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                string methodName = "BalanceOnGiftCard";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request
                BalanceOnGiftCardRequest bogcRequest = null;
                try
                {
                    bogcRequest = BalanceOnGiftCardRequest.ConvertFrom(request);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                // Validate merchant account
                List<PaymentError> errors = new List<PaymentError>();
                ValidateMerchantProperties(bogcRequest, errors);
                if (errors.Count > 0)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, bogcRequest.Locale, errors);
                }

                // Create response
                BalanceOnGiftCardResponse bogcResponse = new BalanceOnGiftCardResponse(bogcRequest.Locale, bogcRequest.ServiceAccountId, this.Name);
                bogcResponse.CardType = bogcRequest.CardType;
                bogcResponse.Last4Digit = bogcRequest.Last4Digit;
                bogcResponse.ProviderTransactionId = Guid.NewGuid().ToString();

                ProcessBalanceOnGiftCardResult(bogcResponse, bogcRequest, errors);

                // Convert response and return
                Response response = BalanceOnGiftCardResponse.ConvertTo(bogcResponse);
                if (errors.Count > 0)
                {
                    response.Errors = errors.ToArray();
                }

                PaymentUtilities.LogOperationResult(methodName, this.Name, SampleConnector.Platform, request, response);
                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// GetMerchantAccountPropertyMetadata returns the merchant account properties need by the payment provider.
            /// </summary>
            /// <param name="request">Request object.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response GetMerchantAccountPropertyMetadata(Request request)
            {
                string methodName = "GetMerchantAccountPropertyMetadata";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Check null request
                List<PaymentError> errors = new List<PaymentError>();
                if (request == null)
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidRequest, "Request is null."));
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: null, properties: null, errors: errors);
                }

                // Prepare response
                List<PaymentProperty> properties = new List<PaymentProperty>();
                PaymentProperty property;
                property = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.AssemblyName,
                    this.GetAssemblyName());
                property.SetMetadata("Assembly Name:", "The assembly name of the test provider", isPasswordValue: false, isReadOnlyValue: true, 0);
                properties.Add(property);
                property = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.ServiceAccountId,
                    Guid.NewGuid().ToString());
                property.SetMetadata("Service account ID:", "The organization subscription Id for  Microsoft Test Provider", isPasswordValue: false, isReadOnlyValue: false, 1);
                properties.Add(property);
                property = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.MerchantId,
                    TestData.MerchantId.ToString());
                property.SetMetadata("Merchant ID:", "The merchant ID received from Microsoft Test Provider", isPasswordValue: false, isReadOnlyValue: false, 2);
                properties.Add(property);
                property = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    SampleMerchantAccountProperty.ProviderId,
                    TestData.ProviderId.ToString());
                property.SetMetadata("Provider ID:", "The provider ID received from Microsoft Test Provider", isPasswordValue: false, isReadOnlyValue: false, 3);
                properties.Add(property);
                property = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    SampleMerchantAccountProperty.Environment,
                    DefaultEnvironment);
                property.SetMetadata("Environment:", "Variable that sets which Payment Accepting service it points to i.e. INT, PROD", isPasswordValue: false, isReadOnlyValue: false, 4);
                properties.Add(property);
                property = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.SupportedCurrencies,
                    TestData.SupportedCurrencies);
                property.SetMetadata("Supported Currencies:", "The supported currencies (ISO 4217) for the Microsoft Test Provider", isPasswordValue: false, isReadOnlyValue: false, 5);
                properties.Add(property);
                property = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.SupportedTenderTypes,
                    TestData.SupportedTenderTypes);
                property.SetMetadata("Supported Tender Types:", "The supported tender types for the Microsoft Test Provider", isPasswordValue: false, isReadOnlyValue: false, 6);
                properties.Add(property);
                property = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    SampleMerchantAccountProperty.TestString,
                    TestData.TestString);
                property.SetMetadata("Test String:", "The TestString received from Microsoft Test Provider", isPasswordValue: false, isReadOnlyValue: false, 7);
                properties.Add(property);
                property = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    SampleMerchantAccountProperty.TestDecimal,
                    TestData.TestDecimal);
                property.SetMetadata("Test Decimal:", "The TestDecimal received from Microsoft Test Provider", isPasswordValue: false, isReadOnlyValue: false, 8);
                properties.Add(property);
                property = new PaymentProperty(
                    GenericNamespace.MerchantAccount,
                    SampleMerchantAccountProperty.TestDate,
                    TestData.TestDate);
                property.SetMetadata("Test Date:", "The TestDate received from Microsoft Test Provider", isPasswordValue: false, isReadOnlyValue: false, 9);
                properties.Add(property);

                Response response = new Response();
                response.Locale = request.Locale;
                response.Properties = properties.ToArray();
                if (errors.Count > 0)
                {
                    response.Errors = errors.ToArray();
                }

                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// ValidateMerchantAccount the passed merchant account properties with the payment provider.
            /// </summary>
            /// <param name="request">Request object to validate.</param>
            /// <returns>
            /// Response object.
            /// </returns>
            public Response ValidateMerchantAccount(Request request)
            {
                string methodName = "ValidateMerchantAccount";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Convert request
                ValidateMerchantAccountRequest validateRequest = null;
                try
                {
                    validateRequest = ValidateMerchantAccountRequest.ConvertFrom(request);
                }
                catch (SampleException ex)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: ex.Errors);
                }

                // Validate merchant account
                List<PaymentError> errors = new List<PaymentError>();
                ValidateMerchantProperties(validateRequest, errors);
                if (errors.Count > 0)
                {
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, validateRequest.Locale, errors);
                }

                // Create response
                var validateResponse = new ValidateMerchantAccountResponse(validateRequest.Locale, validateRequest.ServiceAccountId, this.Name);

                // Convert response and return
                Response response = ValidateMerchantAccountResponse.ConvertTo(validateResponse);
                PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
                return response;
            }

            /// <summary>
            /// Execute a task on the payment connector. This will be unique to each payment connector and will require customization on client.
            /// </summary>
            /// <param name="request">Request object needed to process the ExecuteTask transaction.</param>
            /// <returns>Response object.</returns>
            public Response ExecuteTask(Request request)
            {
                string methodName = "ExecuteTask";
                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationStarting, this.Name, Platform);

                // Check null request
                List<PaymentError> errors = new List<PaymentError>();
                if (request == null || request.Properties == null)
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidRequest, "Request is null."));
                    return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: request == null ? null : request.Locale, properties: null, errors: errors);
                }

                // Create hash to find required properties quickly
                Hashtable hashRequestProperties = PaymentProperty.ConvertToHashtable(request.Properties);

                // Get the task name from the request, For parameters please use the TransactionData name space for any parameters you pass for a task.
                string taskName = string.Empty;
                PaymentProperty.GetPropertyValue(hashRequestProperties, GenericNamespace.TransactionData, TransactionDataProperties.TaskName, out taskName);

                Response response = new Response();
                List<PaymentProperty> properties = new List<PaymentProperty>();

                // You can have many different tasks for your payment connector just make sure the client is updated to use your tasks request and response properties.
                switch (taskName.ToLower(CultureInfo.InvariantCulture))
                {
                    case "balanceoncard":
                        PaymentProperty property = new PaymentProperty("SampleTaskReturn", "Balance", 38.75m);
                        properties.Add(property);
                        break;
                    default:
                        break;
                }

                return response;
            }

            /// <summary>
            /// Gets the payment methods supported by the payment processor.
            /// </summary>
            /// <returns>A read-only collection of payment methods.</returns>
            public IEnumerable<PaymentMethod> GetSupportedPaymentMethods()
            {
                return TestData.SupportedPaymentMethods;
            }

            #endregion

            #region ITrackingSupport

            /// <summary>
            /// Get the payment providers reference to safe guard against duplicate requests.
            /// </summary>
            /// <param name="command">The payment operation that will uses the tracking id.</param>
            /// <param name="amount">The payment transaction amount.</param>
            /// <returns>Returns the PaymentTransactionReferenceData.</returns>
            /// <remarks>List of supported commands can be seen in the constants defined in <see cref="Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants.SupportedCorrelationCommands"/>.</remarks>
            public PaymentTransactionReferenceData GetPaymentReferenceData(string command, decimal amount)
            {
                PaymentTransactionReferenceData paymentTransactionReferenceData = new PaymentTransactionReferenceData();
                paymentTransactionReferenceData.Amount = amount;
                paymentTransactionReferenceData.Command = command;
                paymentTransactionReferenceData.IdFromConnector = Guid.NewGuid().ToString();
                paymentTransactionReferenceData.InitiatedDate = DateTime.UtcNow;

                return paymentTransactionReferenceData;
            }

            #endregion

            #region Private methods

            /// <summary>
            /// Validates merchant account properties.
            /// </summary>
            /// <param name="requestBase">The request.</param>
            /// <param name="errors">The errors.</param>
            private static void ValidateMerchantProperties(RequestBase requestBase, List<PaymentError> errors)
            {
                /*
                 IMPORTANT!!!
                 THIS IS SAMPLE CODE ONLY!
                 THE CODE SHOULD BE UPDATED TO VALIDATE MERCHANT FROM THE APPROPRIATE PAYMENT PROVIDERS.
                */
                if (!TestData.MerchantId.ToString("D").Equals(requestBase.MerchantId, StringComparison.Ordinal))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format("Invalid property value for {0}", MerchantAccountProperties.MerchantId)));
                }

                if (!TestData.ProviderId.ToString("D").Equals(requestBase.ProviderId, StringComparison.Ordinal))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format("Invalid property value for {0}", SampleMerchantAccountProperty.ProviderId)));
                }

                if (!TestData.TestString.Equals(requestBase.TestString, StringComparison.Ordinal))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format("Invalid property value for {0}", SampleMerchantAccountProperty.TestString)));
                }

                if (requestBase.TestDecimal != TestData.TestDecimal)
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format("Invalid property value for {0}", SampleMerchantAccountProperty.TestDecimal)));
                }

                if (requestBase.TestDate != TestData.TestDate)
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format("Invalid property value for {0}", SampleMerchantAccountProperty.TestDate)));
                }
            }

            /// <summary>
            /// Gets the token.
            /// </summary>
            /// <param name="cardNumber">The card number.</param>
            /// <returns>The token.</returns>
            private static string GetToken(string cardNumber)
            {
                /*
                 IMPORTANT!!!
                 THIS IS SAMPLE CODE ONLY!
                 THE CODE SHOULD BE UPDATED TO OBTAIN THE TOKEN FROM THE APPROPRIATE PAYMENT PROVIDERS.
                */
                var encoder = new UTF8Encoding();
                byte[] bytes = encoder.GetBytes(cardNumber);
                return Convert.ToBase64String(bytes);
            }

            /// <summary>
            /// Sets the authorization result to the response.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="authorizeAmount">The authorize amount.</param>
            /// <param name="result">The result.</param>
            /// <param name="message">The message.</param>
            /// <param name="avsResult">The AVS result.</param>
            /// <param name="avsDetail">The AVS detail.</param>
            /// <param name="errors">The list of payment errors.</param>
            /// <param name="errorCode">The error code.</param>
            private static void SetAuthorizationResult(AuthorizeResponse response, decimal authorizeAmount, AuthorizationResult result, string message, AVSResult avsResult, AVSDetail avsDetail, IList<PaymentError> errors, ErrorCode errorCode)
            {
                response.ApprovedAmount = authorizeAmount;
                response.AuthorizationResult = result.ToString();
                response.ProviderMessage = message;
                response.AVSResult = avsResult.ToString();
                response.AVSDetail = avsDetail.ToString();

                if (errorCode != ErrorCode.None)
                {
                    errors.Add(new PaymentError(errorCode, message));
                }
            }

            /// <summary>
            /// Sets the authorization result to the response.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="activateAmount">The authorize amount.</param>
            /// <param name="result">The result.</param>
            /// <param name="message">The message.</param>
            /// <param name="errors">The list of payment errors.</param>
            /// <param name="errorCode">The error code.</param>
            private static void SetActivateGiftCardResult(ActivateGiftCardResponse response, decimal activateAmount, ActivateGiftCardResult result, string message, IList<PaymentError> errors, ErrorCode errorCode)
            {
                response.AvailableBalance = activateAmount;
                response.ActivateGiftCardResult = result.ToString();

                if (errorCode != ErrorCode.None)
                {
                    errors.Add(new PaymentError(errorCode, message));
                }
            }

            /// <summary>
            /// Sets the load gift card result to the response.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="loadAmount">The load amount.</param>
            /// <param name="result">The result.</param>
            /// <param name="message">The message.</param>
            /// <param name="errors">The list of payment errors.</param>
            /// <param name="errorCode">The error code.</param>
            private static void SetLoadGiftCardResult(LoadGiftCardResponse response, decimal loadAmount, LoadGiftCardResult result, string message, IList<PaymentError> errors, ErrorCode errorCode)
            {
                response.AvailableBalance = loadAmount;
                response.LoadGiftCardResult = result.ToString();

                if (errorCode != ErrorCode.None)
                {
                    errors.Add(new PaymentError(errorCode, message));
                }
            }

            /// <summary>
            /// Processes and gets the authorization result.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="request">The request.</param>
            /// <returns>The result indicating whether the authorization is approved.</returns>
            private static List<PaymentError> ProcessActivateGiftCardResult(ActivateGiftCardResponse response, ActivateGiftCardRequest request)
            {
                /*
                 IMPORTANT!!!
                 THIS IS SAMPLE CODE ONLY!
                 THE CODE SHOULD BE UPDATED TO OBTAIN AUTHORIZATION RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
                */
                List<PaymentError> errors = new List<PaymentError>();

                string cardNumber = request.CardNumber;

                // Check whether gift card has already been activated.
                bool hasBeenActivated = activatedGiftCards.ContainsKey(cardNumber);
                if (hasBeenActivated)
                {
                    request.Amount = decimal.Parse(TestData.AuthorizationDeclined);
                    errors.Add(new PaymentError(ErrorCode.CardAlreadyActivated, "Card already activated!"));
                }

                switch (string.Format("{0:0.00}", request.Amount))
                {
                    // Authorization result
                    case TestData.AuthorizationDeclined:
                        SetActivateGiftCardResult(response, 0M, ActivateGiftCardResult.Failure, "Declined", errors, ErrorCode.Decline);
                        break;
                    case TestData.AuthorizationNone:
                        SetActivateGiftCardResult(response, 0M, ActivateGiftCardResult.None, "Declined", errors, ErrorCode.Decline);
                        break;

                    // AVS Result & Detail
                    case TestData.AVSReturnedBillingName:
                        SetActivateGiftCardResult(response, request.Amount.Value, ActivateGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                    case TestData.AVSReturnedBillingAddress:
                        SetActivateGiftCardResult(response, request.Amount.Value, ActivateGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                    case TestData.AVSReturnedBillingAndPostalCode:
                        SetActivateGiftCardResult(response, request.Amount.Value, ActivateGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                    case TestData.AVSReturnedBillingPostalCode:
                        SetActivateGiftCardResult(response, request.Amount.Value, ActivateGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                    case TestData.AVSReturnedNone:
                        SetActivateGiftCardResult(response, request.Amount.Value, ActivateGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                    case TestData.AVSNotReturned:
                        SetActivateGiftCardResult(response, request.Amount.Value, ActivateGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                    case TestData.AVSNone:
                        SetActivateGiftCardResult(response, request.Amount.Value, ActivateGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                    case TestData.AVSVerificationNotSupported:
                        SetActivateGiftCardResult(response, request.Amount.Value, ActivateGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                    case TestData.AVSSystemUnavailable:
                        SetActivateGiftCardResult(response, request.Amount.Value, ActivateGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                    default:
                        SetActivateGiftCardResult(response, request.Amount.Value, ActivateGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                }

                // Set approval state depending on Available Balance value and whether the gift card has already been activated.
                // If Available Balance is 5.12 OR it has already been activated, the card is not approved.
                bool approvalHasFailed = response.AvailableBalance == SampleConnector.declinedAmount || hasBeenActivated;

                // Add gift card to collection of activated gift cards if approval is successful.
                if (!approvalHasFailed)
                {
                    activatedGiftCards.Add(cardNumber, response.AvailableBalance.Value);
                }

                response.ApprovalCode = "GIFT123";
                return errors;
            }

            /// <summary>
            /// Processes and gets the load gift card result.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="request">The request.</param>
            /// <returns>The result indicating whether the load gift card is approved.</returns>
            private static List<PaymentError> ProcessLoadGiftCardResult(LoadGiftCardResponse response, LoadGiftCardRequest request)
            {
                /*
                 IMPORTANT!!!
                 THIS IS SAMPLE CODE ONLY!
                 THE CODE SHOULD BE UPDATED TO OBTAIN AUTHORIZATION RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
                */
                List<PaymentError> errors = new List<PaymentError>();

                string cardNumber = request.CardNumber;

                // Check whether gift card has NOT been activated.
                bool hasBeenActivated = activatedGiftCards.ContainsKey(cardNumber);
                if (!hasBeenActivated)
                {
                    request.Amount = decimal.Parse(TestData.AuthorizationDeclined);
                    errors.Add(new PaymentError(ErrorCode.CardNotActivated, "Card not activated!"));
                }

                switch (string.Format("{0:0.00}", request.Amount))
                {
                    // Authorization result
                    case TestData.AuthorizationDeclined:
                        SetLoadGiftCardResult(response, SampleConnector.declinedAmount, LoadGiftCardResult.Failure, "Declined", errors, ErrorCode.Decline);
                        break;
                    case TestData.AuthorizationNone:
                        SetLoadGiftCardResult(response, SampleConnector.declinedAmount, LoadGiftCardResult.None, "Declined", errors, ErrorCode.Decline);
                        break;
                    default:
                        SetLoadGiftCardResult(response, request.Amount.Value, LoadGiftCardResult.Success, "Approved", errors, ErrorCode.None);
                        break;
                }

                // Update the available balance on the gift if approval is successful.
                bool approvalHasFailed = response.AvailableBalance == 5.12M || !hasBeenActivated;
                decimal availableAmount = decimal.Zero;
                if (!approvalHasFailed)
                {
                    // Retrieve existing amount on the gift card.
                    availableAmount = activatedGiftCards[cardNumber];

                    // Add load amount from request to existing amount.
                    availableAmount += request.Amount.Value;

                    // Update gift card in dict with new amount.
                    activatedGiftCards[cardNumber] = availableAmount;

                    // Add new available amount to response.
                    response.AvailableBalance = availableAmount;
                }

                response.ApprovalCode = "GIFT123";
                return errors;
            }

            /// <summary>
            /// Processes and gets the authorization result.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="request">The request.</param>
            /// <param name="errors">The list of payment errors.</param>
            /// <returns>The result indicating whether the authorization is approved.</returns>
            private static bool ProcessAuthorizationResult(AuthorizeResponse response, AuthorizeRequest request, IList<PaymentError> errors)
            {
                /*
                 IMPORTANT!!!
                 THIS IS SAMPLE CODE ONLY!
                 THE CODE SHOULD BE UPDATED TO OBTAIN AUTHORIZATION RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
                */
                decimal partialAmount = request.Amount.Value - 0.05M;
                decimal giftCardBalance = 0M;
                bool isGiftCardTransaction = request.CardType.Equals(CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase);

                string giftCardNumber = string.Empty;
                if (isGiftCardTransaction)
                {
                    giftCardNumber = request.CardNumber;
                    if (activatedGiftCards.ContainsKey(giftCardNumber))
                    {
                        giftCardBalance = activatedGiftCards[giftCardNumber];

                        if (giftCardBalance > 0M && request.Amount.Value > giftCardBalance)
                        {
                            partialAmount = giftCardBalance;
                            request.Amount = decimal.Parse(TestData.AuthorizationPartialAuthorization);
                            giftCardBalance = decimal.Zero;
                        }
                        else
                        {
                            giftCardBalance -= request.Amount.Value;
                        }
                    }
                }

                switch (string.Format("{0:0.00}", request.Amount))
                {
                    // Authorization result
                    case TestData.AuthorizationDeclined:
                        SetAuthorizationResult(response, 0M, AuthorizationResult.Failure, "Declined", AVSResult.None, AVSDetail.None, errors, ErrorCode.Decline);
                        break;
                    case TestData.AuthorizationNone:
                        SetAuthorizationResult(response, 0M, AuthorizationResult.None, "Declined", AVSResult.Returned, AVSDetail.BillingAddress, errors, ErrorCode.Decline);
                        break;
                    case TestData.AuthorizationReferral:
                        SetAuthorizationResult(response, 0M, AuthorizationResult.Referral, "Declined", AVSResult.Returned, AVSDetail.BillingAddress, errors, ErrorCode.Decline);
                        break;
                    case TestData.AuthorizationPartialAuthorization:
                        if (request.AllowPartialAuthorization ?? false)
                        {
                            SetAuthorizationResult(response, partialAmount, AuthorizationResult.PartialAuthorization, "Partial Approval", AVSResult.Returned, AVSDetail.BillingAddress, errors, ErrorCode.None);
                        }
                        else
                        {
                            SetAuthorizationResult(response, 0M, AuthorizationResult.Failure, "Declined", AVSResult.Returned, AVSDetail.BillingAddress, errors, ErrorCode.Decline);
                        }

                        break;
                    case TestData.AuthorizationPartialAuthorizationReturnVoidFailure:
                        if (request.AllowPartialAuthorization ?? false)
                        {
                            SetAuthorizationResult(response, decimal.Parse(TestData.VoidFailure), AuthorizationResult.PartialAuthorization, "Partial Approval", AVSResult.Returned, AVSDetail.BillingAddress, errors, ErrorCode.None);
                        }
                        else
                        {
                            SetAuthorizationResult(response, 0M, AuthorizationResult.Failure, "Declined", AVSResult.Returned, AVSDetail.BillingAddress, errors, ErrorCode.Decline);
                        }

                        break;
                    case TestData.AuthorizationImmediateCaptureRequired:
                        SetAuthorizationResult(response, 0M, AuthorizationResult.ImmediateCaptureFailed, "Declined", AVSResult.None, AVSDetail.None, errors, ErrorCode.Decline);
                        break;

                    // AVS Result & Detail
                    case TestData.AVSReturnedBillingName:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.AccountholderName, errors, ErrorCode.None);
                        break;
                    case TestData.AVSReturnedBillingAddress:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.BillingAddress, errors, ErrorCode.None);
                        break;
                    case TestData.AVSReturnedBillingAndPostalCode:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.BillingAndPostalCode, errors, ErrorCode.None);
                        break;
                    case TestData.AVSReturnedBillingPostalCode:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.BillingPostalCode, errors, ErrorCode.None);
                        break;
                    case TestData.AVSReturnedNone:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.None, errors, ErrorCode.None);
                        break;
                    case TestData.AVSNotReturned:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Success, "Approved", AVSResult.NotReturned, AVSDetail.None, errors, ErrorCode.None);
                        break;
                    case TestData.AVSNone:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Success, "Approved", AVSResult.None, AVSDetail.None, errors, ErrorCode.None);
                        break;
                    case TestData.AVSVerificationNotSupported:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Success, "Approved", AVSResult.VerificationNotSupported, AVSDetail.None, errors, ErrorCode.None);
                        break;
                    case TestData.AVSSystemUnavailable:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Success, "Approved", AVSResult.SystemUnavailable, AVSDetail.None, errors, ErrorCode.None);
                        break;
                    case TestData.AuthorizationTimedOut:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Failure, "Timed out", AVSResult.SystemUnavailable, AVSDetail.None, errors, ErrorCode.CommunicationError);
                        break;
                    default:
                        SetAuthorizationResult(response, request.Amount.Value, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.BillingAddress, errors, ErrorCode.None);
                        break;
                }

                bool isApproved = response.ApprovedAmount != 0M;
                if (isApproved)
                {
                    ProcessCVV(response, request);
                    response.ApprovalCode = "AB123456";
                    response.ResponseCode = "00";
                    if (request.CardType.Equals(CardType.Debit.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        response.AvailableBalance = 100M;
                        response.CashBackAmount = request.CashBackAmount;
                    }
                    else if (isGiftCardTransaction)
                    {
                        response.AvailableBalance = giftCardBalance;
                    }
                    else if (TestData.AuthorizationAvailableBalance.Equals(request.Amount.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        response.AvailableBalance = 10M;
                    }
                    else
                    {
                        response.AvailableBalance = 0M;
                    }
                }
                else
                {
                    response.CVV2Result = CVV2Result.NotProcessed.ToString();
                    response.ResponseCode = "51";
                    response.CashBackAmount = 0M;
                }

                return isApproved;
            }

            /// <summary>
            /// Attempts to update the available balance on the gift card.
            /// If Gift Card has not already been activated, an <see cref="PaymentError"/> error will be added to passed-in list of errors
            /// and available balance will be set to 0.0.
            /// </summary>
            /// <param name="bogcResponse">The response for the request.</param>
            /// <param name="bogcRequest">The request to retrieve balance on gift card.</param>
            /// <param name="errors">List of errors while attempting to process the request and produce a result.</param>
            private static void ProcessBalanceOnGiftCardResult(BalanceOnGiftCardResponse bogcResponse, BalanceOnGiftCardRequest bogcRequest, List<PaymentError> errors)
            {
                string cardNumber = bogcRequest.CardNumber;
                decimal giftCardBalance = 0.0M;
                bool isApproved = true;

                // Check whether gift card has been activated.
                if (activatedGiftCards.ContainsKey(cardNumber))
                {
                    giftCardBalance = activatedGiftCards[cardNumber];
                }
                else
                {
                    errors.Add(new PaymentError(ErrorCode.CardNotActivated, "Card not activated!"));
                    isApproved = false;
                }

                // Set Result value based on approval state
                bogcResponse.BalanceOnGiftCardResult = isApproved
                    ? BalanceOnGiftCardResult.Success.ToString()
                    : BalanceOnGiftCardResult.Failure.ToString();

                // Set available balance from derived gift balance;
                bogcResponse.AvailableBalance = giftCardBalance;
            }

            /// <summary>
            /// Processes and gets the capture result.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="request">The request.</param>
            /// <param name="errors">The list of payment errors.</param>
            /// <returns>The result indicating whether the capture is approved.</returns>
            private static bool ProcessCaptureResult(CaptureResponse response, CaptureRequest request, IList<PaymentError> errors)
            {
                /*
                 IMPORTANT!!!
                 THIS IS SAMPLE CODE ONLY!
                 THE CODE SHOULD BE UPDATED TO OBTAIN CAPTURE RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
                */
                if (!AuthorizationResult.Success.ToString().Equals(request.AuthorizationResult, StringComparison.OrdinalIgnoreCase)
                    && !AuthorizationResult.PartialAuthorization.ToString().Equals(request.AuthorizationResult, StringComparison.OrdinalIgnoreCase))
                {
                    response.CaptureResult = CaptureResult.Failure.ToString();
                    response.ProviderMessage = "Authorization result is not expected.";
                    errors.Add(new PaymentError(ErrorCode.Decline, response.ProviderMessage));
                    return false;
                }

                if (request.Amount > request.AuthorizationApprovedAmount)
                {
                    response.CaptureResult = CaptureResult.Failure.ToString();
                    response.ProviderMessage = "Capture amount cannot be greater than approved amount.";
                    errors.Add(new PaymentError(ErrorCode.Decline, response.ProviderMessage));
                    return false;
                }

                switch (string.Format("{0:0.00}", request.Amount))
                {
                    case TestData.CaptureFailure:
                    case TestData.CaptureFailureVoidFailure:
                        response.CaptureResult = CaptureResult.Failure.ToString();
                        response.ProviderMessage = "Declined";
                        errors.Add(new PaymentError(ErrorCode.Decline, response.ProviderMessage));
                        break;
                    case TestData.CaptureNone:
                        response.CaptureResult = CaptureResult.None.ToString();
                        response.ProviderMessage = "Unknown error has occurred";
                        errors.Add(new PaymentError(ErrorCode.Decline, response.ProviderMessage));
                        break;
                    case TestData.CaptureFailureInvalidVoiceAuthCode:
                        response.CaptureResult = CaptureResult.Failure.ToString();
                        response.ProviderMessage = "Invalid voice authorization code";
                        errors.Add(new PaymentError(ErrorCode.InvalidArgumentVoiceAuthCode, response.ProviderMessage));
                        break;
                    case TestData.CaptureFailureMultipleCapture:
                        response.CaptureResult = CaptureResult.Failure.ToString();
                        response.ProviderMessage = "Multiple captures are not supported.";
                        errors.Add(new PaymentError(ErrorCode.MultipleCaptureNotSupported, response.ProviderMessage));
                        break;
                    case TestData.CaptureQueuedForBatch:
                        response.CaptureResult = CaptureResult.Success.ToString();
                        response.ProviderMessage = "Capture queued for batch";
                        break;
                    case TestData.CaptureTimedOut:
                        response.CaptureResult = CaptureResult.Failure.ToString();
                        response.ProviderMessage = "Capture timed out";
                        errors.Add(new PaymentError(ErrorCode.CommunicationError, response.ProviderMessage));
                        break;
                    default:
                        response.CaptureResult = CaptureResult.Success.ToString();
                        response.ProviderMessage = "Approved";
                        break;
                }

                bool isApproved = CaptureResult.Success.ToString().Equals(response.CaptureResult, StringComparison.Ordinal);

                // Handle Gift Card capture
                bool isGiftCardTransaction = request.CardType.Equals(CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase);
                if (isGiftCardTransaction)
                {
                    string cardNumber = request.CardNumber;
                    if (!activatedGiftCards.ContainsKey(cardNumber))
                    {
                        errors.Add(new PaymentError(ErrorCode.CardNotActivated, "Card not activated!"));
                        isApproved = false;
                    }
                    else
                    {
                        if (isApproved)
                        {
                            activatedGiftCards[cardNumber] -= request.Amount.Value;
                        }
                    }
                }

                if (isApproved)
                {
                    response.ApprovalCode = "CC222222";
                    response.ResponseCode = "00";
                }
                else
                {
                    response.ResponseCode = "61";
                }

                return isApproved;
            }

            /// <summary>
            /// Processes and gets the void result.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="request">The request.</param>
            /// <returns>The result indicating whether the void is approved.</returns>
            private static List<PaymentError> ProcessVoidResult(VoidResponse response, VoidRequest request)
            {
                /*
                 IMPORTANT!!!
                 THIS IS SAMPLE CODE ONLY!
                 THE CODE SHOULD BE UPDATED TO OBTAIN VOID RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
                */
                List<PaymentError> errors = new List<PaymentError>();

                // Check whether gift card has been activated
                // If not, then force a decline. Otherwise, add request.Amount back to gift card.
                bool isGiftCardTransaction = request.CardType.Equals(CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase);
                bool giftCardHasBeenActivated = false;

                string cardNumber = request.CardNumber;

                // Handle Gift Card transaction
                if (isGiftCardTransaction)
                {
                    giftCardHasBeenActivated = activatedGiftCards.ContainsKey(cardNumber);

                    if (!giftCardHasBeenActivated)
                    {
                        // Force decline
                        request.AuthorizationApprovedAmount = decimal.Parse(TestData.VoidFailure);
                        errors.Add(new PaymentError(ErrorCode.CardNotActivated, "Card not activated!"));
                    }
                }

                if (!AuthorizationResult.Success.ToString().Equals(request.AuthorizationResult, StringComparison.OrdinalIgnoreCase)
                    && !AuthorizationResult.PartialAuthorization.ToString().Equals(request.AuthorizationResult, StringComparison.OrdinalIgnoreCase)
                    && !isGiftCardTransaction)
                {
                    response.VoidResult = CaptureResult.Failure.ToString();

                    string msg = "Authorization result is not expected.";
                    response.ProviderMessage = msg;
                    errors.Add(new PaymentError(ErrorCode.AuthorizationFailure, msg));

                    return errors;
                }

                switch (string.Format("{0:0.00}", request.AuthorizationApprovedAmount))
                {
                    case TestData.VoidFailure:
                    case TestData.CaptureFailureVoidFailure:
                        response.VoidResult = VoidResult.Failure.ToString();
                        response.ProviderMessage = "Declined";
                        errors.Add(new PaymentError(ErrorCode.Decline, response.ProviderMessage));
                        break;
                    case TestData.VoidNone:
                        response.VoidResult = VoidResult.None.ToString();
                        response.ProviderMessage = "Unknown error has occurred";
                        errors.Add(new PaymentError(ErrorCode.Decline, response.ProviderMessage));
                        break;
                    case TestData.VoidFailureAuthorizationVoided:
                        response.VoidResult = VoidResult.Failure.ToString();
                        response.ProviderMessage = "Authorization is already voided.";
                        errors.Add(new PaymentError(ErrorCode.AuthorizationIsVoided, response.ProviderMessage));
                        break;
                    case TestData.VoidTimedOut:
                        response.VoidResult = VoidResult.Failure.ToString();
                        response.ProviderMessage = "Void timed out.";
                        errors.Add(new PaymentError(ErrorCode.CommunicationError, response.ProviderMessage));
                        break;
                    default:
                        response.VoidResult = VoidResult.Success.ToString();
                        response.ProviderMessage = "Approved";
                        break;
                }

                bool isApproved = VoidResult.Success.ToString().Equals(response.VoidResult, StringComparison.Ordinal);

                if (isApproved)
                {
                    response.ResponseCode = "00";

                    if (giftCardHasBeenActivated)
                    {
                        // Handle close out gift card if Authorization response is missing from Request.
                        bool authResponseIsMissing = request.AuthorizationResponseCode == null;
                        if (authResponseIsMissing)
                        {
                            // Remove gift card from activated list
                            if (activatedGiftCards.ContainsKey(cardNumber))
                            {
                                decimal cacheAmount = activatedGiftCards[cardNumber] - request.AuthorizationApprovedAmount.Value;
                                activatedGiftCards[cardNumber] = cacheAmount;
                                response.CloseAmount = cacheAmount;

                                if (cacheAmount == decimal.Zero)
                                {
                                    activatedGiftCards.Remove(cardNumber);
                                }
                            }
                        }
                        else
                        {
                            if (activatedGiftCards.ContainsKey(cardNumber))
                            {
                                activatedGiftCards[cardNumber] += request.AuthorizationApprovedAmount.Value;
                            }
                        }
                    }
                }
                else
                {
                    response.ResponseCode = "81";
                }

                return errors;
            }

            /// <summary>
            /// Processes and gets the refund result.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="request">The request.</param>
            /// <returns>The result indicating whether the refund is approved.</returns>
            private static List<PaymentError> ProcessRefundResult(RefundResponse response, RefundRequest request)
            {
                /*
                 IMPORTANT!!!
                 THIS IS SAMPLE CODE ONLY!
                 THE CODE SHOULD BE UPDATED TO OBTAIN REFUND RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
                */
                List<PaymentError> errors = new List<PaymentError>();

                // Check whether gift card has been activated
                // If not, then force a decline. Otherwise, add request.Amount back to gift card.
                bool giftCardHasBeenActivated = false;
                bool isGiftCardTransaction = request.CardType.Equals(CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase);
                if (isGiftCardTransaction)
                {
                    string cardNumber = request.CardNumber;
                    giftCardHasBeenActivated = activatedGiftCards.ContainsKey(cardNumber);
                    if (!giftCardHasBeenActivated)
                    {
                        // Force decline
                        request.Amount = decimal.Parse(TestData.RefundFailure);
                        errors.Add(new PaymentError(ErrorCode.CardNotActivated, "Card not activated!"));
                    }
                    else
                    {
                        activatedGiftCards[cardNumber] += request.Amount.Value;
                    }
                }

                switch (string.Format("{0:0.00}", request.Amount))
                {
                    case TestData.RefundFailure:
                        response.RefundResult = RefundResult.Failure.ToString();
                        response.ProviderMessage = "Declined";
                        response.ApprovedAmount = 0M;
                        errors.Add(new PaymentError(ErrorCode.Decline, response.ProviderMessage));
                        break;
                    case TestData.RefundQueueForBatch:
                        response.RefundResult = RefundResult.Success.ToString();
                        response.ProviderMessage = "Approved";
                        response.ApprovedAmount = request.Amount;
                        break;
                    case TestData.RefundNone:
                        response.RefundResult = RefundResult.None.ToString();
                        response.ProviderMessage = "Unknown error has occurred";
                        response.ApprovedAmount = 0M;
                        errors.Add(new PaymentError(ErrorCode.Decline, response.ProviderMessage));
                        break;
                    case TestData.RefundNotSupported:
                        response.RefundResult = RefundResult.Failure.ToString();
                        response.ProviderMessage = "Refund is not supported";
                        response.ApprovedAmount = 0M;
                        errors.Add(new PaymentError(ErrorCode.CardTypeNotSupported, response.ProviderMessage));
                        break;
                    case TestData.RefundTimedOut:
                        response.RefundResult = RefundResult.Failure.ToString();
                        response.ProviderMessage = "Timed out";
                        response.ApprovedAmount = 0M;
                        errors.Add(new PaymentError(ErrorCode.CommunicationError, response.ProviderMessage));
                        break;
                    default:
                        response.RefundResult = RefundResult.Success.ToString();
                        response.ProviderMessage = "Approved";
                        response.ApprovedAmount = request.Amount;
                        break;
                }

                // Set approval state
                bool isApproved = isGiftCardTransaction
                    ? RefundResult.Success.ToString().Equals(response.RefundResult, StringComparison.Ordinal) || giftCardHasBeenActivated
                    : RefundResult.Success.ToString().Equals(response.RefundResult, StringComparison.Ordinal);
                if (isApproved)
                {
                    response.ApprovalCode = "RR654321";
                    response.ResponseCode = "00";
                }
                else
                {
                    response.ResponseCode = "71";
                }

                return errors;
            }

            /// <summary>
            /// Processes the CVV result.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <param name="request">The request.</param>
            /// <returns>The result indicating whether CVV result is success.</returns>
            private static bool ProcessCVV(AuthorizeResponse response, AuthorizeRequest request)
            {
                /*
                 IMPORTANT!!!
                 THIS IS SAMPLE CODE ONLY!
                 THE CODE SHOULD BE UPDATED TO OBTAIN CVV RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
                */
                string cvvValue = request.CardVerificationValue;
                bool isSuccess = false;
                switch (cvvValue)
                {
                    // CVV2 result
                    case TestData.CVV2Failure:
                        response.CVV2Result = CVV2Result.Failure.ToString();
                        break;
                    case TestData.CVV2IssuerNotRegistered:
                        response.CVV2Result = CVV2Result.IssuerNotRegistered.ToString();
                        break;
                    case TestData.CVV2NotProcessed:
                        response.CVV2Result = CVV2Result.NotProcessed.ToString();
                        break;
                    case TestData.CVV2Unknown:
                        response.CVV2Result = CVV2Result.Unknown.ToString();
                        break;
                    default:
                        response.CVV2Result = CVV2Result.Success.ToString();
                        isSuccess = true;
                        break;
                }

                return isSuccess;
            }

            private static string GetLastFourDigits(string cardNumber)
            {
                return cardNumber.Length < 4 ? cardNumber.PadLeft(4, PaddingCharacter) : cardNumber.Substring(cardNumber.Length - 4);
            }

            private static string GetPropertyStringValue(Hashtable propertyHashtable, string propertyNamespace, string propertyName, bool throwExceptionIfNotFound)
            {
                string propertyValue;
                bool found = PaymentProperty.GetPropertyValue(
                                propertyHashtable,
                                propertyNamespace,
                                propertyName,
                                out propertyValue);
                if (!found && throwExceptionIfNotFound)
                {
                    ThrowPropertyNotSetException(propertyName);
                }

                return propertyValue;
            }

            private static void ThrowPropertyNotSetException(string propertyName)
            {
                throw new ArgumentException(string.Format("Property '{0}' is null or not set", propertyName));
            }

            private static Uri GetPaymentAcceptBaseAddress(string requestEnvironment)
            {
                string paymentAcceptBaseAddress = null;
                if (string.IsNullOrWhiteSpace(requestEnvironment))
                {
                    paymentAcceptBaseAddress = PaymentAcceptBaseAddressINT;
                }
                else
                {
                    switch (requestEnvironment)
                    {
                        case EnvironmentONEBOX:
                            paymentAcceptBaseAddress = PaymentAcceptBaseAddressONEBOX;
                            break;
                        case EnvironmentINT:
                            paymentAcceptBaseAddress = PaymentAcceptBaseAddressINT;
                            break;
                        case EnvironmentPROD:
                            paymentAcceptBaseAddress = PaymentAcceptBaseAddressPROD;
                            break;
                        case EnvironmentMock:
                            paymentAcceptBaseAddress = string.Empty;
                            break;
                        default:
                            var errors = new List<PaymentError>();
                            errors.Add(new PaymentError(ErrorCode.ApplicationError, "Environment is not valid."));
                            throw new SampleException(errors);
                    }
                }

                if (!string.IsNullOrWhiteSpace(paymentAcceptBaseAddress))
                {
                    return new Uri(paymentAcceptBaseAddress);
                }
                else
                {
                    return null;
                }
            }

            private static void AddDefaultBankIdentificationNumberStart(Response response)
            {
                if (response != null && response.Properties != null)
                {
                    var properties = new List<PaymentProperty>(response.Properties);
                    var propertiesHash = PaymentProperty.ConvertToHashtable(response.Properties);
                    var cardTypeString = GetPropertyStringValue(
                        propertiesHash,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardType,
                        throwExceptionIfNotFound: false);

                    if (string.IsNullOrWhiteSpace(cardTypeString))
                    {
                        // POS does not set the card type so cannot default bankIdentificationNumberStart
                        return;
                    }

                    Enum.TryParse<CardType>(cardTypeString, out CardType cardType);

                    var bankIdentificationNumberStart = string.Empty;
                    switch (cardType)
                    {
                        case CardType.MasterCard:
                            bankIdentificationNumberStart = "5";
                            break;
                        case CardType.Discover:
                            bankIdentificationNumberStart = "6";
                            break;
                        case CardType.Amex:
                            bankIdentificationNumberStart = "3";
                            break;
                        default:
                            // Default to visa
                            bankIdentificationNumberStart = "4";
                            break;
                    }

                    var paymentProperty = new PaymentProperty(GenericNamespace.PaymentCard, PaymentCardProperties.BankIdentificationNumberStart, bankIdentificationNumberStart);
                    properties.Add(paymentProperty);
                    response.Properties = properties.ToArray();
                }
            }

            private string GetAssemblyName()
            {
                string asemblyQualifiedName = this.GetType().AssemblyQualifiedName;
                int commaIndex = asemblyQualifiedName.IndexOf(',');
                return asemblyQualifiedName.Substring(commaIndex + 1).Trim();
            }

            #endregion
        }
    }
}
