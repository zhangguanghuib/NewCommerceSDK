## How to call API to check On Hand Quantity for a serial number?

1. This code comes from UserAlertService.cs
2. It does not support wmsLocationId,  please notice actually InventLocationId is actually WareHouse

   ```cs
        private async Task GetSerialInsufficientStockUserAlertsForCart(
            RequestContext context,
            IEnumerable<KeyValuePair<int, CartLine>> filteredCartLines,
            CustomerOrderMode customerOrderMode,
            PagedResult<SimpleProduct> products,
            List<CartLineUserAlerts> userAlertsList)
        {
            // Skip validation if there is no serial active items.
            if (!products.Where(p => p.Behavior.IsSerialNumberRequired).Any())
            {
                return;
            }

            ChannelConfiguration channelConfiguration = context.GetChannelConfiguration();
            IEnumerable<KeyValuePair<int, CartLine>> cartLinesWithSerialNumber = filteredCartLines.Where(p => !string.IsNullOrEmpty(p.Value.SerialNumber));

            List<ItemEstimatedOnHandQuantitySearchCriteria> searchCriteria = cartLinesWithSerialNumber.Select(p => new ItemEstimatedOnHandQuantitySearchCriteria()
            {
                ProductId = p.Value.ProductId,
                ItemId = p.Value.ItemId,
                UnitOfMeasure = p.Value.UnitOfMeasureSymbol,
                SerialNumber = p.Value.SerialNumber,
                Warehouse = string.IsNullOrWhiteSpace(p.Value.WarehouseId) ? channelConfiguration.InventLocation : p.Value.WarehouseId,
            }).ToList();

            var getInventoryEstimatedOnHandQuantityRequest = new GetEstimatedOnHandQuantityForInventoryDimensionsDataRequest(searchCriteria, QueryResultSettings.AllRecords);
            var onHandQuantities = await context.ExecuteAsync<EntityDataServiceResponse<ItemOnHandQuantity>>(getInventoryEstimatedOnHandQuantityRequest).ConfigureAwait(false);

            string carryOutDeliveryModeCode = channelConfiguration.CarryoutDeliveryModeCode;

            foreach (var cartLineWithIndex in cartLinesWithSerialNumber)
            {
                CartLine cartLine = cartLineWithIndex.Value;
                SimpleProduct product = products.Single(p => p.RecordId == cartLine.ProductId);
                string warehouseId = string.IsNullOrWhiteSpace(cartLine.WarehouseId) ? channelConfiguration.InventLocation : cartLine.WarehouseId;

                // No need to perform stock validation if the product is not serial active.
                if (!product.Behavior.IsSerialNumberRequired)
                {
                    continue;
                }

                decimal onHandQuantity = onHandQuantities
                    .Where(p => p.ProductId == cartLine.ProductId && p.InventorySerialId == cartLine.SerialNumber && string.Equals(p.InventoryLocationId, warehouseId, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.PhysicalInventory)
                    .Sum();

                if (onHandQuantity < cartLine.Quantity)
                {
                    bool allowNegativePhysicalInventory = await this.IsNegativePhysicalInventoryAllowed(context, product, warehouseId).ConfigureAwait(false);

                    var isDeliveryModePickupServiceRequest = new IsDeliveryModePickupServiceRequest(cartLine.DeliveryMode);
                    var isDeliveryModePickupServiceResponse = await context.ExecuteAsync<IsDeliveryModePickupServiceResponse>(isDeliveryModePickupServiceRequest).ConfigureAwait(false);

                    UserAlert alert;
                    if (!allowNegativePhysicalInventory &&
                        (string.IsNullOrEmpty(cartLine.DeliveryMode) ||
                        string.Equals(cartLine.DeliveryMode, carryOutDeliveryModeCode, StringComparison.OrdinalIgnoreCase) ||
                         (isDeliveryModePickupServiceResponse.IsPickupDeliveryMode && customerOrderMode == CustomerOrderMode.Pickup)))
                    {
                        alert = UserAlertFactory.CreateBlockingUserAlert(UserAlertSourceType.Inventory_SerialNumberInsufficientStock);
                    }
                    else
                    {
                        alert = UserAlertFactory.CreateNonBlockingUserAlert(UserAlertSourceType.Inventory_SerialNumberInsufficientStock);
                    }

                    alert.LocalizedMessageParameters = new string[] { cartLine.ItemId, cartLine.SerialNumber };
                    alert.LocalizedMessage = await UserAlertLocalizer.GetUserAlertLocalizedMessage(
                        context.LanguageId,
                        alert.AlertSourceId,
                        alert.IsBlocking,
                        this.GetLocalizedMessageKeyword(alert, product),
                        alert.LocalizedMessageParameters).ConfigureAwait(false);

                    var alerts = new CartLineUserAlerts(cartLine.LineNumber, cartLineWithIndex.Key);
                    alerts.UserAlerts.Add(alert);

                    userAlertsList.Add(alerts);
                }
            }
        }

```


![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/a45cd859-fa5f-41b7-940a-dbafb0c31135)


