using System.Collections.Generic;
using System.Composition;
using System.Net;
using Microsoft.Dynamics.Retail.PaymentSDK.Portable;

namespace Microsoft.Dynamics
{
    namespace Retail.SampleConnector.Portable
    {
        [Export(typeof(IPaymentProcessor))]
        public class SampleConnector : SampleProcessorIdentifier, IPaymentProcessor, IPaymentProcessorExtension, IPaymentReferenceProvider, IPaymentMethodInfo
        {
            #region Constants

            private const string Platform = "Portable";

            private const string EnvironmentONEBOX = "ONEBOX";
            private const string EnvironmentINT = "INT";
            private const string EnvironmentPROD = "PROD";
            private const string EnvironmentMock = "MockHtmlContents";

            private const string PaymentAcceptBaseAddressONEBOX = @"http://localhost:3973/payments";
            private const string PaymentAcceptBaseAddressINT = @"https://paymentacceptsample.cloud.int.dynamics.com";
            private const string PaymentAcceptBaseAddressPROD = @"https://paymentacceptsample.cloud.dynamics.com";

            private const string OperationStarting = "starting";

#if DEBUG
#if USE_INT
            private const string DefaultEnvironment = "INT";
#else
            private const string DefaultEnvironment = "ONEBOX";
#endif
#else
            private const string DefaultEnvironment = "PROD";
#endif

            private const string GetPaymentAcceptPointRelativeUri = "/Payments/GetPaymentAcceptPoint";

            private const string RetrievePaymentAcceptResultRelativeUri = "/Payments/RetrievePaymentAcceptResult";

            private const char PaddingCharacter = '*';

            private static Dictionary<string, decimal> activatedGiftCards = new Dictionary<string, decimal>();

            private static decimal declineAmount = 5.12M;

            #endregion

            #region Constructors
            static SampleConnector()
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                activatedGiftCards.Add("61234", 1000.0m);
            }

            public SampleConnector()
            {

            }

            #endregion


            string IProcessorIdentifierV1.Name => throw new System.NotImplementedException();

            string IProcessorIdentifierV1.Copyright => throw new System.NotImplementedException();

            ArrayList IProcessorIdentifierV1.SupportedCountries => throw new System.NotImplementedException();

            public Response ExecuteTask(Request request)
            {
                throw new System.NotImplementedException();
            }

            public PaymentTransactionReferenceData GetPaymentReferenceData(string command, decimal amount)
            {
                throw new System.NotImplementedException();
            }

            public IEnumerable<PaymentMethod> GetSupportedPaymentMethods()
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.ActivateGiftCard(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.Authorize(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.BalanceOnGiftCard(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.Capture(Request request)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.GenerateCardToken(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.GetMerchantAccountPropertyMetadata(Request request)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.GetPaymentAcceptPoint(Request request)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.ImmediateCapture(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.LoadGiftCard(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.Reauthorize(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.Refund(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.RetrievePaymentAcceptResult(Request request)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.Reversal(Request request)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.ValidateMerchantAccount(Request request)
            {
                throw new System.NotImplementedException();
            }

            Response IPaymentProcessorV1.Void(Request request)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
