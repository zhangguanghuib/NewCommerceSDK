namespace ExternalCSUConsoleClient
{
    using Microsoft.Dynamics.Commerce.RetailProxy;
    using Microsoft.Dynamics.Commerce.RetailProxy.Authentication;
    using Microsoft.Identity.Client;
    using System.Configuration;

    public class Program
    {
        private static string clientId = string.Empty;
        private static string clientSecret = string.Empty;
        private static Uri retailServerUrl = null!;
        private static string resource = string.Empty;
        private static string operatingUnitNumber = string.Empty;
        private static Uri aadAuthority = null!;
        // SA Team
        private static string authority = string.Empty;
        public static string tenantId = string.Empty;
        public static string audience = string.Empty;

        private static void GetConfiguration()
        {
            clientId = ConfigurationManager.AppSettings["aadClientId"] ?? throw new ArgumentNullException("aadClientId");
            clientSecret = ConfigurationManager.AppSettings["aadClientSecret"] ?? throw new ArgumentNullException("aadClientSecret");
            aadAuthority = new Uri(ConfigurationManager.AppSettings["aadAuthority"] ?? throw new ArgumentNullException("aadAuthority"));
            retailServerUrl = new Uri(ConfigurationManager.AppSettings["retailServerUrl"] ?? throw new ArgumentNullException("retailServerUrl"));
            operatingUnitNumber = ConfigurationManager.AppSettings["operatingUnitNumber"] ?? throw new ArgumentNullException("operatingUnitNumber");
            resource = ConfigurationManager.AppSettings["resource"] ?? throw new ArgumentNullException("resource");

            // From SA:
            tenantId = ConfigurationManager.AppSettings["tenantId"] ?? throw new ArgumentNullException("tenantId");
            authority = ConfigurationManager.AppSettings["authority"] ?? throw new ArgumentNullException("authority");
            audience = ConfigurationManager.AppSettings["audience"] ?? throw new ArgumentNullException("audience");
        }

        //public static async Task<string> GetAuthenticationResult(string clientId, string authority, string clientSecret, string tenantId, string audience)
        //{
        //    var confidentialClientApplication = ConfidentialClientApplicationBuilder.
        //        Create(clientId)
        //        .WithAuthority(authority + tenantId)
        //        .WithClientSecret(clientSecret);
        //    string[] scopes = new string[] { $"{audience}/.default" };
        //    AuthenticationResult authResult = await confidentialClientApplication
        //        .Build()
        //        .AcquireTokenForClient(scopes)
        //        .ExecuteAsync()
        //        .ConfigureAwait(false);

        //    return authResult.AccessToken;
        //}

        //private static async Task<ManagerFactory> CreateManagerFactory()
        //{
        //    try
        //    {
        //        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
        //            .WithClientSecret(clientSecret)
        //            .WithAuthority(authority)
        //            .Build();

        //        Microsoft.Identity.Client.AuthenticationResult authResult = await app.AcquireTokenForClient(new string[] { resource }).ExecuteAsync();

        //        ClientCredentialsToken clientCredentialsToken = new ClientCredentialsToken(authResult.AccessToken);
        //        RetailServerContext retailServerContext = RetailServerContext.Create(retailServerUrl, operatingUnitNumber, clientCredentialsToken);
        //        ManagerFactory factory = ManagerFactory.Create(retailServerContext);
        //        return factory;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        throw;
        //    }
        //}

        private static async Task<ManagerFactory> CreateManagerFactory()
        {
            try
            {
                string AccessToken = await AuthenticationHelper.GetAuthenticationResult(clientId, authority, clientSecret, tenantId, audience).ConfigureAwait(false);
                ClientCredentialsToken clientCredentialsToken = new ClientCredentialsToken(AccessToken);
#pragma warning disable CA2000 // Dispose objects before losing scope
                RetailServerContext retailServerContext = RetailServerContext.Create(retailServerUrl, operatingUnitNumber, clientCredentialsToken);
#pragma warning restore CA2000 // Dispose objects before losing scope
                ManagerFactory factory = ManagerFactory.Create(retailServerContext);
                return factory;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private static async Task<Microsoft.Dynamics.Commerce.RetailProxy.PagedResult<SalesOrder>> GetOrderHistory(string customerId)
        {
            QueryResultSettings querySettings = new QueryResultSettings
            {
                Paging = new PagingInfo() { Top = 10, Skip = 0 }
            };

            ManagerFactory managerFactory = await CreateManagerFactory().ConfigureAwait(false);
            ICustomerManager customerManage = managerFactory.GetManager<ICustomerManager>();
            return await customerManage.GetOrderHistory(customerId, querySettings).ConfigureAwait(false);
        }

        private static async Task PrintCustomerOrders(string customerId)
        {
            Microsoft.Dynamics.Commerce.RetailProxy.PagedResult<SalesOrder> orderHistory = await GetOrderHistory("004007").ConfigureAwait(false);

            foreach (var order in orderHistory)
            {
                Console.WriteLine(order.ToString());
            }
        }

        private static async Task<Microsoft.Dynamics.Commerce.RetailProxy.PagedResult<Microsoft.Dynamics.Commerce.RetailProxy.Cart>> GetOnlineShoppingCartList()
        {
            QueryResultSettings querySettings = new QueryResultSettings
            {
                Paging = new PagingInfo() { Top = 10, Skip = 0 }
            };

            ManagerFactory managerFactory = await CreateManagerFactory().ConfigureAwait(false);
            Contoso.Commerce.RetailProxy.Extension.IStoreOperationsManager storeOperationsManager = managerFactory.GetManager<Contoso.Commerce.RetailProxy.Extension.IStoreOperationsManager>();
            return await storeOperationsManager.GetOnlineShoppingCartList(querySettings).ConfigureAwait(false);
        }

        private static async Task OverrideCartLinePrice(string cartId, string lineId, decimal newPrice)
        {
            ManagerFactory managerFactory = await CreateManagerFactory().ConfigureAwait(false);
            Contoso.Commerce.RetailProxy.Extension.IStoreOperationsManager storeOperationsManager = managerFactory.GetManager<Contoso.Commerce.RetailProxy.Extension.IStoreOperationsManager>();
            await storeOperationsManager.OverrideCartLinePrice(cartId, lineId, newPrice).ConfigureAwait(false);
        }

        private static async Task GetCartListAndOverrideCartLinePrice()
        {
            Microsoft.Dynamics.Commerce.RetailProxy.PagedResult<Microsoft.Dynamics.Commerce.RetailProxy.Cart> cartList = await GetOnlineShoppingCartList().ConfigureAwait(false);
            foreach (Microsoft.Dynamics.Commerce.RetailProxy.Cart cart in cartList)
            {
                if (cart.CartLines.Count > 0)
                {
                    if (cart.CartLines[0].Price.HasValue)
                    {
                        await OverrideCartLinePrice(cart.Id, cart.CartLines[0].LineId, cart.CartLines[0].Price.GetValueOrDefault() + 10m).ConfigureAwait(false);
                        Console.WriteLine(cart.CartLines[0].LineId);
                    }
                }
            }
        }

        static async Task Main(string[] args)
        {
            GetConfiguration();

            try
            {
                await PrintCustomerOrders("004007").ConfigureAwait(false);
                await GetCartListAndOverrideCartLinePrice().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    internal class AuthenticationHelper
    {
        /// <summary>
        /// Get the access token to call CSU API
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="authority"></param>
        /// <param name="clientSecret"></param>
        /// <param name="tenantId"></param>
        /// <param name="audience"></param>
        /// <returns></returns>
        public static async Task<string> GetAuthenticationResult(string clientId, string authority, string clientSecret, string tenantId, string audience)
        {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder.
                Create(clientId)
                .WithAuthority(authority + tenantId)
                .WithClientSecret(clientSecret);
            string[] scopes = new string[] { $"{audience}/.default" };
            AuthenticationResult authResult = await confidentialClientApplication
                .Build()
                .AcquireTokenForClient(scopes)
                .ExecuteAsync()
                .ConfigureAwait(false);

            return authResult.AccessToken;
        }
    }
}
