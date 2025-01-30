## How to make the product image fallback to product master image if there is no product variant image exists on media server?

## Background 
When we open PDP page, only product master number is required, then the product master image will show on PDP page; Then you can change the product dimensions,  if the image for product dimension does not exist, the product master image will remine on the PDP page and will not leave the image as blank.

But for inventory lookup form,  when we open the inventory lookup page, we need provide the product master and its product dimension values,  if the image for product variant does not exist, the product image will be blank.

Basically this is by-design behavior, we expect customer should provide the product variant images,  but they want to keep the same behavior on PDP and Inventory Lookup Page,  that is reasonable requirement.

Below is an idea and sample code that help implement this requirement as a partner developmer.

## Finally effect:
. On Inventory Lookup form,  input "81101" and hit "Enter".<br/>
  ![image](https://github.com/user-attachments/assets/919dc747-31ac-490c-bd13-e3f5e26a1fb2)<br/>
. Choose the product dimension values, like Size, Style and Color <br/>
  ![image](https://github.com/user-attachments/assets/8fdf40d1-512f-42d6-99cb-90c743943037)<br/>
  ![image](https://github.com/user-attachments/assets/9b94120b-bd41-4d60-b07d-628500642202)<br/>
  ![image](https://github.com/user-attachments/assets/5703d7bb-8690-4326-9185-6103d501e29a)

. If finally the image for product variant image does exist,  then it will show the product variant image:<br/>
  ![image](https://github.com/user-attachments/assets/9cf8f15b-4cf0-4afa-80f0-ca58f7dcfc60)<br/>
. If finally the image for product variant image does not exist,  then it will fallback to show the product master image:<br/>
  ![image](https://github.com/user-attachments/assets/b1b1437f-b8f7-4bea-a32c-2a08158d58e7)
## Implementation details:
### Retail Server Side, we need implement the Post Trigger for the GetVariantProductsServiceRequest to change the SimpleProduct.PrimaryImageUrl to let it be the Product Master's image url, or store it into SimpleProduct.ExtensionProperties.

```csharp
namespace Contoso.GasStationSample.CommerceRuntime.Triggers
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetVariantProductsServiceRequestTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(GetVariantProductsServiceRequest) };
            }
        }
        /// <summary>
        /// Post trigger code.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public async Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");
            if ((request is GetVariantProductsServiceRequest) && (response is GetProductsServiceResponse))
            {
                var getVariantProductsServiceResponse = response as GetProductsServiceResponse;
                PagedResult<SimpleProduct> pagedResult = getVariantProductsServiceResponse.Products;
                foreach (SimpleProduct item in pagedResult)
                {
                    if (item.PrimaryImageUrl.Length >= 12)
                    {
                        //item.PrimaryImageUrl = "Products/" + item.ItemId + item.PrimaryImageUrl.Substring(item.PrimaryImageUrl.Length - 12);
                        string productMasterImageUrl = "Products/" + item.ItemId + item.PrimaryImageUrl.Substring(item.PrimaryImageUrl.Length - 12);
                        CommerceProperty commerceProperty = new CommerceProperty("ProductMasterImageUrl", productMasterImageUrl);
                        item.ExtensionProperties.Add(commerceProperty);
                    }
                }
            }
            await Task.CompletedTask.ConfigureAwait(false);
        }
        /// <summary>
        /// Pre trigger code
        /// </summary>
        /// <param name="request">The request.</param>
        public async Task OnExecuting(Request request)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
```



The doc is here:

https://learn.microsoft.com/en-us/dynamics365/fin-ops-core/dev-itpro/extensibility/extensibility-attributes

The whole project is here:<br/>

https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/Solutions/ImageExtensions




