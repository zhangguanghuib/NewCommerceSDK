# Call CSU server API from external application step by step.
# Background: 
#### CSU Server API can not only consumed by Microsoft D365 Store Commerce but also Microsoft D365 e-Commerce, but also it is accessible for external application,  this article provided the step by step to show you how to configure to enable the CSU API accessible by external third-party application.
#### Refer to this Microsoft document though some content out-of-dated: https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/consume-retail-server-api

#### Azure Portal Configuration Steps
1. Log in to Azure Portal, and go to App Registrations, and <mark>New registration<Mark> to create App Registration for Retail Server(CSU), and follow the document <br/>
   ![image](https://github.com/user-attachments/assets/e56d679c-9cd1-446f-8820-26e5aae30dae) <br/>
   ![image](https://github.com/user-attachments/assets/2e7f6282-af38-411b-b63a-5a8deacb53bf) <br/>
   ![image](https://github.com/user-attachments/assets/af619599-f194-4e58-932a-8a35eb52a625)  <br/>
   ![image](https://github.com/user-attachments/assets/e48384db-1bc1-4130-bc4b-50dc89370c9b)
2. Create the app registration for the client<br/>
   ![image](https://github.com/user-attachments/assets/0fd878a9-ae19-4498-81de-31567e583462)<br/>
   ![image](https://github.com/user-attachments/assets/432c7f55-6c6c-4f71-98a2-95a4d3f6c280)<br/>
   ![image](https://github.com/user-attachments/assets/b8b54399-1203-4813-9350-aa8c5d9143e9)<br/>
3. Create client secret:<br/>
   ![image](https://github.com/user-attachments/assets/591ae3b2-a10e-4614-9ddd-1745d65ccb18)<br/>
   ![image](https://github.com/user-attachments/assets/075b03b6-b588-4aba-9269-6eb43690596a)<br/>
   Copy the Secret Value since it will only shown once.
4. Config "Commerce Shared Parameters": <br/>
   ![image](https://github.com/user-attachments/assets/0cf5cd60-3535-4d4a-96e4-54e817c550a8)
#### External Application configuration:
5. Create a .net Core Console application, with these configuration<br/>
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <!-- CSU Client Application Client Id -->
    <add key="aadClientId" value="{CSU Client Application Client Id}" />
    <!-- CSU Client Application Secret Value,not Secret Id  -->
    <add key="aadClientSecret" value="{CSU Client Application Client Secret Value}" />
    <!-- aadAuthority: https://sts.windows.net/{tenantId} -->
    <add key="aadAuthority" value="https://sts.windows.net/{tenantId}/" />
    <!-- CSU (Retail Server URL) -->
    <add key="retailServerUrl" value="https://{CSU Server Host}:{Port}/RetailServer/Commerce" />
    <!-- CSU Server App Client Id -->
    <add key="resource" value="api://{ CSU Server App Client Id}" />
    <!-- Store's Operating Unit Id -->
    <add key="operatingUnitNumber" value="{Store's Operating Unit Id}" />
    <!-- SA Srinath. -->
    <!-- authority, that is a fixed value -->
    <add key="authority" value="https://login.microsoftonline.com/"/>
    <!-- tenantId -->
    <add key="tenantId"  value="{tenantid}"/>
    <!-- CSU Server App Client Id -->
    <add key="audience"   value= "api://{CSU Server App Client Id}" />
  </appSettings>
</configuration>
```
6. In the program file of the console application, write the below code to read these values from configuration:
```cs
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
```
7. Create the below method to get access token by C# code: <br/>
```cs
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
```
8. Create the below method to create the Manager Factory:<br/>
```cs
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
```

#### For Standard API, we can call it by the below code, the below code shows how to call GetOrderHistory API
```cs
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
```
#### For custom API, we can follow the below steps to  call it:<br/>
1. Create custom API  in Commerce SDK<br/>
```cs
 [HttpPost]
 [Authorization(CommerceRoles.Employee, CommerceRoles.Application)]
 public async Task<Cart> OverrideCartLinePrice(IEndpointContext context, string cartId, string lineId, decimal newPrice)
 {
     if (string.IsNullOrWhiteSpace(cartId))
     {
         throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_MissingParameter, "The cart identifier are missing.");
     }
     // Get the cart
     CartSearchCriteria cartSearchCriteria = new CartSearchCriteria(cartId);
     var request = new GetCartRequest(cartSearchCriteria, QueryResultSettings.SingleRecord);
     var response = await context.ExecuteAsync<GetCartResponse>(request).ConfigureAwait(false);
     Cart cart = response.Carts.SingleOrDefault();
     if (cart == null)
     {
         throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ObjectNotFound, "The cart is not found.");
     }

     // Override the price
     var overrideSalesTransactionLinePriceRequest = new OverrideSalesTransactionLinePriceRequest(cart, lineId, newPrice, CalculationModes.All);
     var saveCartResponse = await context.ExecuteAsync<SaveCartResponse>(overrideSalesTransactionLinePriceRequest).ConfigureAwait(false);

     return saveCartResponse.Cart;
 }

 [HttpPost]
 [Authorization(CommerceRoles.Employee, CommerceRoles.Application)]
 public async Task<PagedResult<Cart>> GetOnlineShoppingCartList(IEndpointContext context, QueryResultSettings queryResultSettings)
 {
     CartSearchCriteria cartSearchCriteria = new CartSearchCriteria();
     cartSearchCriteria.CartType = CartType.Checkout;
     cartSearchCriteria.StaffId = "";
     cartSearchCriteria.IncludeAnonymous = true;
     cartSearchCriteria.LastModifiedDateTimeFrom = System.DateTime.Now.AddDays(-10);
     cartSearchCriteria.LastModifiedDateTimeTo = System.DateTime.Now;

     if (cartSearchCriteria == null)
     {
         throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_MissingParameter, "The cart identifier are missing.");
     }

     // Get the cart
     var request = new GetCartRequest(cartSearchCriteria, queryResultSettings);
     var response = await context.ExecuteAsync<GetCartResponse>(request).ConfigureAwait(false);

     return response.Carts;
 }
```
2. Add the Commerce SDK CRT project to the CSharpProxyGenerator project reference:
   ![image](https://github.com/user-attachments/assets/464e7d82-285b-45ba-a70a-23d39b8e9d46)

3. Build Commerce SDK project and generate the C# proxy for external application use: <br/>
```cs
// <auto-generated />
namespace Contoso.Commerce.RetailProxy.Extension
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.RetailProxy;
    
    /// <summary>
    /// Class implements Store Operations Manager.
    /// </summary>
    [GeneratedCodeAttribute("Contoso.Commerce.RetailProxy.Extension", "1.0")]
    internal class StoreOperationsManager : IStoreOperationsManager
    {
        private IContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreOperationsManager"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public StoreOperationsManager(IContext context)
        {
            this.context = context;
        }
    
        
        /// <summary>
        /// OverrideCartLinePrice method.
        /// </summary>
        /// <param name="cartId">The cartId.</param>
        /// <param name="lineId">The lineId.</param>
        /// <param name="newPrice">The newPrice.</param>
        /// <returns>Cart object.</returns>
        public async Task<Cart> OverrideCartLinePrice(string cartId, string lineId, decimal newPrice)
        {       
            return await this.context.ExecuteOperationSingleResultAsync<Cart>(
                "",
                "StoreOperations",
                "OverrideCartLinePrice",
                true, null, OperationParameter.Create("cartId", cartId, false),
                  OperationParameter.Create("lineId", lineId, false),
                  OperationParameter.Create("newPrice", newPrice, false));
        }
    }  
 }

```
4. Call the C# proxy from the external application, and then the C# proxy can call CSU Server API finally:<br/>
```cs
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
```

5. Finally you can see the CSU API returned data to external application: <br/>
#####  For standard API
![image](https://github.com/user-attachments/assets/f986d284-0ca9-4b47-9571-68d8affc1bba)<br/>
#####  For custom API
![image](https://github.com/user-attachments/assets/1c64a58b-5a5c-4ee7-bc59-aed5267595a0)
