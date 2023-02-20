using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Reflection;
using System.ServiceModel;

namespace HeartLandClient_DotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            SendMultiuseTokenRequest();
        }
        public static HeartLandReference.DoTransactionResponse SendMultiuseTokenRequest()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Name = "PosGatewayInterface";

            string porticoGatewayURL = "https://cert.api2.heartlandportico.com/Hps.Exchange.PosGateway/POSgatewayservice.asmx?wsdl";                         
            HeartLandReference.PosGatewayInterface gatewayinterface = new HeartLandReference.PosGatewayInterfaceClient(binding, new EndpointAddress(porticoGatewayURL));  //"https://cert.api2.heartlandportico.com/Hps.Exchange.PosGateway/POSgatewayservice.asmx?wsdl"
            HeartLandReference.DoTransactionRequest tokenreq = new HeartLandReference.DoTransactionRequest();

            tokenreq.PosRequest = new HeartLandReference.PosRequest();
            tokenreq.PosRequest.Ver10 = new HeartLandReference.PosRequestVer10();
            tokenreq.PosRequest.Ver10.Header = new HeartLandReference.PosRequestVer10Header();

            //tokenreq.PosRequest.Ver10.Header.SecretAPIKey = _service.SecretKey;     //"skapi_cert_MUHgAQB4_14AeHbaUcYZI1UuBg7XBhqPPcm626K1wQ"
            tokenreq.PosRequest.Ver10.Header.SecretAPIKey = "skapi_cert_MUHgAQB4_14AeHbaUcYZI1UuBg7XBhqPPcm626K1wQ";     //"skapi_cert_MUHgAQB4_14AeHbaUcYZI1UuBg7XBhqPPcm626K1wQ"
            tokenreq.PosRequest.Ver10.Header.LicenseId = 0;
            tokenreq.PosRequest.Ver10.Header.DeviceId = 0;
            tokenreq.PosRequest.Ver10.Header.DeveloperID = "002914";
            tokenreq.PosRequest.Ver10.Header.VersionNbr = "4860";
            tokenreq.PosRequest.Ver10.Header.UserName = null;
            tokenreq.PosRequest.Ver10.Header.Password = null;
            tokenreq.PosRequest.Ver10.Header.SiteTrace = null;

            HeartLandReference.PosRequestVer10Transaction txn = new HeartLandReference.PosRequestVer10Transaction();

            HeartLandReference.PosCreditAccountVerifyReqType creditAccVerify = new HeartLandReference.PosCreditAccountVerifyReqType();
            txn.Item = creditAccVerify;

            txn.ItemElementName = HeartLandReference.ItemChoiceType1.CreditAccountVerify;
            tokenreq.PosRequest.Ver10.Transaction = txn;

            creditAccVerify.Block1 = new HeartLandReference.CreditAccountVerifyBlock1Type();
            creditAccVerify.Block1.CardData = new HeartLandReference.CardDataType();

            creditAccVerify.Block1.CardHolderData = new HeartLandReference.CardHolderDataType();
            creditAccVerify.Block1.CardHolderData.CardHolderZip = "65000";
            //creditAccVerify.Block1.CardHolderData.CardHolderZip = request.ZipCode;
            // creditAccVerify.Block1.CardHolderData.CardHolderZip = "95742";

            HeartLandReference.CardDataTypeTokenData tok = new HeartLandReference.CardDataTypeTokenData();
            //tok.TokenValue = request.SecureCardData;   // "supt_gmqPtNmHPBhbfHzJQZ5QopZE";
            tok.TokenValue = "supt_RnadCIQLef53epZajracEnp0";

            bool cardPresent = false;

            tok.CardPresent = cardPresent ? HeartLandReference.booleanType.Y : HeartLandReference.booleanType.N;
            tok.CardPresentSpecified = true;

            bool readerPresent = false;
            tok.ReaderPresent = readerPresent ? HeartLandReference.booleanType.Y : HeartLandReference.booleanType.N;
            tok.ReaderPresentSpecified = true;

            creditAccVerify.Block1.CardData.Item = tok;

            bool requestMultiUseToken = true;
            creditAccVerify.Block1.CardData.TokenRequest = requestMultiUseToken ? HeartLandReference.booleanType.Y : HeartLandReference.booleanType.N;

            //CardDataTypeManualEntry mentry = new CardDataTypeManualEntry();
            //mentry.CardNbr = "4012002000060016";
            //mentry.ExpMonth = 12;
            //mentry.ExpYear = 2025;

            //creditsale.Block1.CardData.Item = mentry;

            HeartLandReference.DoTransactionResponse Tokenres;

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
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
    }
}
