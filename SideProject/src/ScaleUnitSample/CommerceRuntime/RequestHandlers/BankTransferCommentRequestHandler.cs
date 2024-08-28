namespace CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Contoso.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Localization.Services.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class BankTransferCommentRequestHandler : IRequestHandlerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(SetBankTransferCommentToTenderLineRequest),
                };
            }
        }

        public Task<Response> Execute(Request request)
        {
            ThrowIf.Null(request, nameof(request));

            switch (request)
            {
                case SetBankTransferCommentToTenderLineRequest setBankTransferCommentToTenderLineRequest:
                    return this.SetBankTransferCommentToTenderLine(setBankTransferCommentToTenderLineRequest);
                default:
                    throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
            }
        }

        private async Task<Response> SetBankTransferCommentToTenderLine(SetBankTransferCommentToTenderLineRequest setBankTransferCommentToTenderLineRequest)
        {
            ThrowIf.Null(setBankTransferCommentToTenderLineRequest, nameof(setBankTransferCommentToTenderLineRequest));
            ThrowIf.Null(setBankTransferCommentToTenderLineRequest.SaveTenderLineRequest, nameof(setBankTransferCommentToTenderLineRequest.SaveTenderLineRequest));
            ThrowIf.Null(setBankTransferCommentToTenderLineRequest.SaveTenderLineRequest.TenderLine, nameof(setBankTransferCommentToTenderLineRequest.SaveTenderLineRequest.TenderLine));
            ThrowIf.Null(setBankTransferCommentToTenderLineRequest.SaveTenderLineRequest.CartId, nameof(setBankTransferCommentToTenderLineRequest.SaveTenderLineRequest.CartId));
            
            string cartId = setBankTransferCommentToTenderLineRequest.SaveTenderLineRequest.CartId;
            decimal lineNum = setBankTransferCommentToTenderLineRequest.SaveTenderLineRequest.TenderLine.LineNumber;
            string bankTransferComment = setBankTransferCommentToTenderLineRequest.BankTransferComment;
            long channelId = setBankTransferCommentToTenderLineRequest.RequestContext.GetPrincipal().ChannelId;
            string terminal = setBankTransferCommentToTenderLineRequest.RequestContext.GetTerminal().TerminalId;
            string store = setBankTransferCommentToTenderLineRequest.RequestContext.GetOrgUnit().OrgUnitNumber;
            string dataAreaId = setBankTransferCommentToTenderLineRequest.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;

            bool updateSuccess = false;
            using (var databaseContext = new SqlServerDatabaseContext(setBankTransferCommentToTenderLineRequest.RequestContext))
            {
                ParameterSet parameters = new ParameterSet();
                parameters["@b_CHANNEL"] = channelId;
                parameters["@s_STORE"] = store;
                parameters["@s_TERMINAL"] = terminal;
                parameters["@s_DATAAREAID"] = dataAreaId;
                parameters["@s_TRANSACTIONID"] = cartId;
                parameters["@n_LINENUM"] = lineNum;
                parameters["@s_BANKTRANSFERCOMMENT"] = bankTransferComment;              
                int sprocErrorCode =
                    await databaseContext
                    .ExecuteStoredProcedureNonQueryAsync("[ext].[INSERTCONTOSORETAILTRANSACTIONPAYMENTTRANS]", parameters, setBankTransferCommentToTenderLineRequest.QueryResultSettings)
                    .ConfigureAwait(continueOnCapturedContext: false);

                updateSuccess = (sprocErrorCode == 0);
            }


            CartSearchCriteria cartSearchCriteria = new CartSearchCriteria(cartId);
            GetCartServiceRequest getCartServiceRequest = new GetCartServiceRequest(cartSearchCriteria, QueryResultSettings.SingleRecord);
            GetCartServiceResponse getCartServiceResponse = await setBankTransferCommentToTenderLineRequest.RequestContext.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false);
            var cart = getCartServiceResponse.Carts.SingleOrDefault<Cart>();

            return new SetBankTransferCommentToTenderLineResponse(cart);
        }
    }
}
