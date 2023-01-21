using System.Linq;
using System.Threading.Tasks;
using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using Microsoft.Dynamics.Retail.Diagnostics;

namespace GHZ.BarcodeMsrDialogSample.CommerceRuntime.Handlers
{
    public class TransferCartRequestHandlerExtensionV2 : SingleAsyncRequestHandler<TransferCartRequest>
    {
        // A parameter to control whether to reclculate when transfer cart to offline
        private const string RetailRecalculateWhenTransferCartOffline = "RETAIL_RECALCULATE_WHEN_TRANSFER_CART_OFFLINE";

        /// <summary>
        /// Transfer the shopping cart on the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><see cref="NullResponse"/> object containing nothing.</returns>
        protected override async Task<Response> Process(TransferCartRequest request)
        {
            ThrowIf.Null(request, nameof(request));
            ThrowIf.Null(request.Cart, "request.Cart");

            // After transfering the cart, it would remain as offline till the whole transaction is done.
            request.Cart.IsCreatedOffline = true;

            var retailRecalculateWhenTransferCartOffline = await ShouldRecalculateWhenTransferCartOfflineEnabled(request.RequestContext).ConfigureAwait(false);

            // For offline cart, persist the object directly into database.
            var convertRequest = new ConvertCartToSalesTransactionRequest(request.Cart, shouldRecalculateSalesTransaction: retailRecalculateWhenTransferCartOffline);
            var transaction = (await request.RequestContext.ExecuteAsync<ConvertCartToSalesTransactionResponse>(convertRequest).ConfigureAwait(false)).SalesTransaction;

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
        /// Gets the boolean to determine if we will recalculate when transfer cart to offline.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <returns>True if we will reclculate.</returns>
        private static async Task<bool> ShouldRecalculateWhenTransferCartOfflineEnabled(RequestContext context)
        {
            var getConfigurationParametersDataRequest = new GetConfigurationParametersDataRequest(context.GetPrincipal().ChannelId);
            var getConfigurationParametersDataResponse = await context.ExecuteAsync<EntityDataServiceResponse<RetailConfigurationParameter>>(getConfigurationParametersDataRequest).ConfigureAwait(false);
            RetailConfigurationParameter retailRecalculateWhenTransferCartOfflineConfigurations = getConfigurationParametersDataResponse?.SingleOrDefault(configuration => configuration.Name == RetailRecalculateWhenTransferCartOffline);
            var retailRecalculateWhenTransferCartOffline = false;

            if (retailRecalculateWhenTransferCartOfflineConfigurations != null)
            {
                bool.TryParse(retailRecalculateWhenTransferCartOfflineConfigurations.Value, out retailRecalculateWhenTransferCartOffline);
            }

            return retailRecalculateWhenTransferCartOffline;
        }


        public static async Task TransferSalesTransaction(RequestContext context, SalesTransaction transaction)
        {
            ThrowIf.Null(context, nameof(context));
            ThrowIf.Null(transaction, nameof(transaction));

            RetailLogger.Log.CartWorkflowHelperTransferSalesTransactionBegin(transaction.InternalTransactionId.ToString(), transaction.CustomerId);

            // Transfer sales transaction has to be seamless.
            // Row version check need to be ignored since data gets transferred from different storage.
            await SaveSalesTransaction(context, transaction, ignoreVersionCheck: true).ConfigureAwait(false);
        }

        public static async Task SaveSalesTransaction(RequestContext context, SalesTransaction transaction, bool ignoreVersionCheck = false)
        {
            ThrowIf.Null(context, nameof(context));
            ThrowIf.Null(transaction, nameof(transaction));

            RetailLogger.Log.CartWorkflowHelperSaveSalesTransactionBegin(transaction.InternalTransactionId.ToString(), transaction.CustomerId);

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

    internal class TransferCartResponse : Response
    {
        private Cart cart;

        public TransferCartResponse(Cart cart)
        {
            this.cart = cart;
        }
    }
}
