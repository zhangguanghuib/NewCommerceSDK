using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHZ.BarcodeMsrDialogSample.CommerceRuntime.Handlers
{
    public class TransferCartRequestHandlerExtension : SingleAsyncRequestHandler<TransferCartRequest>
    {
        private const string RetailRecalculateTaxWhenTransferReturnCartOffline = "RETAIL_RECALCULATE_WHEN_TRANSFER_CART_OFFLINE";

        protected override async Task<Response> Process(TransferCartRequest request)
        {
            ThrowIf.Null(request, nameof(request));
            ThrowIf.Null(request.Cart, "request.Cart");

            // After transfering the cart, it would remain as offline till the whole transaction is done.
            request.Cart.IsCreatedOffline = true;

            CalculationModes? calculationMode = null;

            // When transfer cart to offline, we should not recalculate charge and tax.
            if (!(await ShouldRecalculateTaxWhenTransferReturnCartOfflineEnabled(request.RequestContext).ConfigureAwait(false)))
            {
                calculationMode = ~(CalculationModes.Charges | CalculationModes.Taxes);
            }

            // For offline cart, persist the object directly into database.
            var convertRequest = new ConvertCartToSalesTransactionRequest(request.Cart, calculationMode: calculationMode);
            var transaction = (await request.RequestContext.ExecuteAsync<ConvertCartToSalesTransactionResponse>(convertRequest).ConfigureAwait(false)).SalesTransaction;

            // Recalculate the whole sales transaction to gets sales tax group assigned and tax amount calculated 
            // after switching from online to offline.
            // CalculateSalesTransactionServiceRequest calculateServiceRequest = new CalculateSalesTransactionServiceRequest(transaction, null);
            // transaction =  (await request.RequestContext.ExecuteAsync<CalculateSalesTransactionServiceResponse>(calculateServiceRequest).ConfigureAwait(false)).Transaction;

            // Assign tax codes for tax calculation.
            var assignTaxCodesServiceRequest = new AssignTaxCodesServiceRequest(transaction);
            assignTaxCodesServiceRequest.RequestContext = request.RequestContext;
            transaction = (await request.RequestContext.ExecuteAsync<AssignTaxCodesServiceResponse>(assignTaxCodesServiceRequest).ConfigureAwait(false)).Transaction;

            await TransferSalesTransaction(request.RequestContext, transaction).ConfigureAwait(false);

            // The transferred cart will have cart version from the new offline storage
            var cart = await ConvertToCart(request.RequestContext, transaction).ConfigureAwait(false);

            return new TransferCartResponse(cart);
        }

        /// <summary>
        /// Gets the boolean to determine if we will recalculate tax when transfer return cart to offline.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <returns>True if we will reclculate.</returns>
        private static async Task<bool> ShouldRecalculateTaxWhenTransferReturnCartOfflineEnabled(RequestContext context)
        {
            var getConfigurationParametersDataRequest = new GetConfigurationParametersDataRequest(context.GetPrincipal().ChannelId);
            var getConfigurationParametersDataResponse = await context.ExecuteAsync<EntityDataServiceResponse<RetailConfigurationParameter>>(getConfigurationParametersDataRequest).ConfigureAwait(false);
            RetailConfigurationParameter retailRecalculateTaxWhenTransferReturnCartOfflineConfigurations = getConfigurationParametersDataResponse?.SingleOrDefault(configuration => configuration.Name == RetailRecalculateTaxWhenTransferReturnCartOffline);
            var retailRecalculateTaxWhenTransferReturnCartOffline = false;

            if (retailRecalculateTaxWhenTransferReturnCartOfflineConfigurations != null)
            {
                bool.TryParse(retailRecalculateTaxWhenTransferReturnCartOfflineConfigurations.Value, out retailRecalculateTaxWhenTransferReturnCartOffline);
            }

            return retailRecalculateTaxWhenTransferReturnCartOffline;
        }

        public static async Task TransferSalesTransaction(RequestContext context, SalesTransaction transaction)
        {
            ThrowIf.Null(context, nameof(context));
            ThrowIf.Null(transaction, nameof(transaction));

            // Transfer sales transaction has to be seamless.
            // Row version check need to be ignored since data gets transferred from different storage.
            await SaveSalesTransaction(context, transaction, ignoreVersionCheck: true).ConfigureAwait(false);
        }

        public static async Task SaveSalesTransaction(RequestContext context, SalesTransaction transaction, bool ignoreVersionCheck = false)
        {
            ThrowIf.Null(context, nameof(context));
            ThrowIf.Null(transaction, nameof(transaction));

            try
            {
                var request = new SaveCartVersionedDataRequest(transaction, ignoreVersionCheck: ignoreVersionCheck);
                transaction.LongVersion = (await context.ExecuteAsync<SingleEntityDataServiceResponse<long>>(request).ConfigureAwait(false)).Entity;
            }
            catch (StorageException e)
            {
                HandleSaveTransactionStorageException(e, transaction.Id);

                throw;
            }
        }

        private static void HandleSaveTransactionStorageException(StorageException e, string transactionId)
        {
            if (e.ErrorResourceId == StorageErrors.Microsoft_Dynamics_Commerce_Runtime_UpdateOfReadOnlyRowError.ToString())
            {
                // if there is a read-only conflict, surface this to the the caller as a validation issue that can be retried
                throw new CartValidationException(
                    DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_UpdateOfReadOnlyCart,
                    transactionId,
                    e,
                    "Cannot update read-only cart");
            }
            else if (e.ErrorResourceId == StorageErrors.Microsoft_Dynamics_Commerce_Runtime_ObjectVersionMismatchError.ToString())
            {
                // if there is a concurrency exception, surface this to the the caller as a validation issue that can be retried
                throw new CartValidationException(
                    DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidCartVersion,
                    transactionId,
                    e,
                    "Cart version mismatch");
            }
        }

        public static async Task<Cart> ConvertToCart(RequestContext context, SalesTransaction salesTransaction)
        {
            if (salesTransaction == null)
            {
                return null;
            }

            ConvertSalesTransactionToCartServiceRequest serviceRequest = new ConvertSalesTransactionToCartServiceRequest(salesTransaction);
            return (await context.ExecuteAsync<UpdateCartServiceResponse>(serviceRequest).ConfigureAwait(false)).Cart;
        }
    }
}
