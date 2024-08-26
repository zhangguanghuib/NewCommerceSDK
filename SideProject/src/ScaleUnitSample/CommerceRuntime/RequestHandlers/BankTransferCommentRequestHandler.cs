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
            long channel = setBankTransferCommentToTenderLineRequest.RequestContext.GetPrincipal().ChannelId;
            string terminal = setBankTransferCommentToTenderLineRequest.RequestContext.GetTerminal().TerminalId;
            string store = setBankTransferCommentToTenderLineRequest.RequestContext.GetOrgUnit().OrgUnitNumber;
            string dataAreaId = setBankTransferCommentToTenderLineRequest.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;

            bool updateSuccess = false;
            using (var databaseContext = new SqlServerDatabaseContext(setBankTransferCommentToTenderLineRequest.RequestContext))
            {
                ParameterSet parameters = new ParameterSet();
                parameters["@s_cartId"] = cartId;
                parameters["@d_lineNum"] = lineNum;
                parameters["@s_bankTransferComment"] = bankTransferComment;
                
                int sprocErrorCode =
                    await databaseContext
                    .ExecuteStoredProcedureNonQueryAsync("[ext].[INSERTCONTOSORETAILTRANSACTIONPAYMENTTRANS]", parameters, setBankTransferCommentToTenderLineRequest.QueryResultSettings)
                    .ConfigureAwait(continueOnCapturedContext: false);

                updateSuccess = (sprocErrorCode == 0);
            }


            Cart cart = null;

            return new SetBankTransferCommentToTenderLineResponse(cart);

        }
    }
}
