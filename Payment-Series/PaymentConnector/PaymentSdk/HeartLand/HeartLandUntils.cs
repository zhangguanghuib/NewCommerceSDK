namespace Contoso.Retail.SampleConnector.Portable
{
    using Newtonsoft.Json;
    using System;
    using System.ServiceModel;
    using System.Reflection;

    public class HeartLandUntils
    {
        public static  Microsoft.Dynamics.Retail.SampleConnector.Portable.DoTransactionResponse SendMultiuseTokenRequest()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Name = "PosGatewayInterface";

            string porticoGatewayURL = "https://cert.api2.heartlandportico.com/Hps.Exchange.PosGateway/POSgatewayservice.asmx?wsdl";
            Microsoft.Dynamics.Retail.SampleConnector.Portable.PosGatewayInterface gatewayinterface = new Microsoft.Dynamics.Retail.SampleConnector.Portable.PosGatewayInterfaceClient(binding, new EndpointAddress(porticoGatewayURL));  //"https://cert.api2.heartlandportico.com/Hps.Exchange.PosGateway/POSgatewayservice.asmx?wsdl"
            Microsoft.Dynamics.Retail.SampleConnector.Portable.DoTransactionRequest tokenreq = new Microsoft.Dynamics.Retail.SampleConnector.Portable.DoTransactionRequest();

            //
            //string asmName = typeof(Microsoft.Dynamics.Retail.SampleConnector.Portable.PosGatewayInterfaceClient).Assembly.FullName;
            //string typeName = typeof(Microsoft.Dynamics.Retail.SampleConnector.Portable.PosGatewayInterfaceClient).FullName;
            //Assembly assem = AppDomain.CurrentDomain.Load(asmName);

            //System.ServiceModel.Channels.Binding binding1 = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            //binding1.Name = "PosGatewayInterface";

            //Type t = assem.GetType(typeName);
            //ConstructorInfo st = t.GetConstructors()[3];
            //object o2 = st.Invoke(new object[] { binding1, new EndpointAddress(porticoGatewayURL) });

            tokenreq.PosRequest = new Microsoft.Dynamics.Retail.SampleConnector.Portable.PosRequest();
            tokenreq.PosRequest.Ver10 = new Microsoft.Dynamics.Retail.SampleConnector.Portable.PosRequestVer10();
            tokenreq.PosRequest.Ver10.Header = new Microsoft.Dynamics.Retail.SampleConnector.Portable.PosRequestVer10Header();

            //tokenreq.PosRequest.Ver10.Header.SecretAPIKey = _service.SecretKey;     //"skapi_cert_MUHgAQB4_14AeHbaUcYZI1UuBg7XBhqPPcm626K1wQ"
            tokenreq.PosRequest.Ver10.Header.SecretAPIKey = "skapi_cert_MUHgAQB4_14AeHbaUcYZI1UuBg7XBhqPPcm626K1wQ";     //"skapi_cert_MUHgAQB4_14AeHbaUcYZI1UuBg7XBhqPPcm626K1wQ"
            tokenreq.PosRequest.Ver10.Header.LicenseId = 0;
            tokenreq.PosRequest.Ver10.Header.DeviceId = 0;
            tokenreq.PosRequest.Ver10.Header.DeveloperID = "002914";
            tokenreq.PosRequest.Ver10.Header.VersionNbr = "4860";
            tokenreq.PosRequest.Ver10.Header.UserName = null;
            tokenreq.PosRequest.Ver10.Header.Password = null;
            tokenreq.PosRequest.Ver10.Header.SiteTrace = null;

            Microsoft.Dynamics.Retail.SampleConnector.Portable.PosRequestVer10Transaction txn = new Microsoft.Dynamics.Retail.SampleConnector.Portable.PosRequestVer10Transaction();

            Microsoft.Dynamics.Retail.SampleConnector.Portable.PosCreditAccountVerifyReqType creditAccVerify = new Microsoft.Dynamics.Retail.SampleConnector.Portable.PosCreditAccountVerifyReqType();
            txn.Item = creditAccVerify;

            txn.ItemElementName = Microsoft.Dynamics.Retail.SampleConnector.Portable.ItemChoiceType1.CreditAccountVerify;
            tokenreq.PosRequest.Ver10.Transaction = txn;

            creditAccVerify.Block1 = new Microsoft.Dynamics.Retail.SampleConnector.Portable.CreditAccountVerifyBlock1Type();
            creditAccVerify.Block1.CardData = new Microsoft.Dynamics.Retail.SampleConnector.Portable.CardDataType();

            creditAccVerify.Block1.CardHolderData = new Microsoft.Dynamics.Retail.SampleConnector.Portable.CardHolderDataType();
            creditAccVerify.Block1.CardHolderData.CardHolderZip = "65000";
            //creditAccVerify.Block1.CardHolderData.CardHolderZip = request.ZipCode;
            // creditAccVerify.Block1.CardHolderData.CardHolderZip = "95742";

            Microsoft.Dynamics.Retail.SampleConnector.Portable.CardDataTypeTokenData tok = new Microsoft.Dynamics.Retail.SampleConnector.Portable.CardDataTypeTokenData();
            //tok.TokenValue = request.SecureCardData;   // "supt_gmqPtNmHPBhbfHzJQZ5QopZE";
            tok.TokenValue = "supt_RnadCIQLef53epZajracEnp0";

            bool cardPresent = false;

            tok.CardPresent = cardPresent ? Microsoft.Dynamics.Retail.SampleConnector.Portable.booleanType.Y : Microsoft.Dynamics.Retail.SampleConnector.Portable.booleanType.N;
            tok.CardPresentSpecified = true;

            bool readerPresent = false;
            tok.ReaderPresent = readerPresent ? Microsoft.Dynamics.Retail.SampleConnector.Portable.booleanType.Y : Microsoft.Dynamics.Retail.SampleConnector.Portable.booleanType.N;
            tok.ReaderPresentSpecified = true;

            creditAccVerify.Block1.CardData.Item = tok;

            bool requestMultiUseToken = true;
            creditAccVerify.Block1.CardData.TokenRequest = requestMultiUseToken ? Microsoft.Dynamics.Retail.SampleConnector.Portable.booleanType.Y : Microsoft.Dynamics.Retail.SampleConnector.Portable.booleanType.N;

            //CardDataTypeManualEntry mentry = new CardDataTypeManualEntry();
            //mentry.CardNbr = "4012002000060016";
            //mentry.ExpMonth = 12;
            //mentry.ExpYear = 2025;

            //creditsale.Block1.CardData.Item = mentry;

            Microsoft.Dynamics.Retail.SampleConnector.Portable.DoTransactionResponse Tokenres;

            //Tokenres = gatewayinterface.DoTransaction(tokenreq);
            try
            {
                Tokenres = gatewayinterface.DoTransactionAsync(tokenreq).Result;

                //_requestData = Newtonsoft.Json.JsonConvert.SerializeObject(tokenreq);
                //_responseData = Newtonsoft.Json.JsonConvert.SerializeObject(Tokenres);

                var _requestData = JsonConvert.SerializeObject(tokenreq);
                var _responseData = JsonConvert.SerializeObject(Tokenres);
                return Tokenres;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
