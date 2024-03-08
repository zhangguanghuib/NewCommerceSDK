/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso.CommerceRuntime.RequestHandlers
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
    using System.IO;
    using Newtonsoft.Json;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;

    /// <summary>
    /// Sample service to demonstrate managing a collection of entities.
    /// </summary>
    public class ExampleDataService : IRequestHandlerAsync
    {
        /// <summary>
        /// Gets the collection of supported request types by this handler.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(CreateExampleEntityDataRequest),
                    typeof(ExampleEntityDataRequest),
                    typeof(UpdateExampleEntityDataRequest),
                    typeof(DeleteExampleEntityDataRequest),
                };
            }
        }

        /// <summary>
        /// Entry point to StoreHoursDataService service.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <returns>Result of executing request, or null object for void operations.</returns>
        public Task<Response> Execute(Request request)
        {
            ThrowIf.Null(request, nameof(request));

            switch (request)
            {
                case CreateExampleEntityDataRequest createExampleEntityDataRequest:
                    return this.CreateExampleEntity(createExampleEntityDataRequest);
                case ExampleEntityDataRequest exampleEntityDataRequest:
                    return this.GetExampleEntities(exampleEntityDataRequest);
                case UpdateExampleEntityDataRequest updateExampleEntityDataRequest:
                    return this.UpdateExampleEntity(updateExampleEntityDataRequest);
                case DeleteExampleEntityDataRequest deleteExampleEntityDataRequest:
                    return this.DeleteExampleEntity(deleteExampleEntityDataRequest);
                default:
                    throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
            }
        }

        private async Task<Response> CreateExampleEntity(CreateExampleEntityDataRequest request)
        {
            ThrowIf.Null(request, nameof(request));
            ThrowIf.Null(request.EntityData, nameof(request.EntityData));

            string jsonstr = await getJsonString().ConfigureAwait(false);

            FiscalIntegrationDocumentRetrievalCriteria fiscalIntegrationDocumentRetrievalCriteria = JsonConvert.DeserializeObject<FiscalIntegrationDocumentRetrievalCriteria>(jsonstr);

            long insertedId = 0;
            using (var databaseContext = new SqlServerDatabaseContext(request.RequestContext))
            {
                ParameterSet parameters = new ParameterSet();
                parameters["@i_ExampleInt"] = request.EntityData.IntData;
                parameters["@s_ExampleString"] = request.EntityData.StringData;
                var result = await databaseContext
                    .ExecuteStoredProcedureAsync<ExampleEntity>("[ext].CONTOSO_INSERTEXAMPLE", parameters, request.QueryResultSettings)
                    .ConfigureAwait(continueOnCapturedContext: false);
                insertedId = result.Item2.Single().UnusualEntityId;
            }

            return new CreateExampleEntityDataResponse(insertedId);
        }

        private async Task<Response> GetExampleEntities(ExampleEntityDataRequest request)
        {
            ThrowIf.Null(request, "request");

            using (DatabaseContext databaseContext = new DatabaseContext(request.RequestContext))
            {
                var query = new SqlPagedQuery(request.QueryResultSettings)
                {
                    DatabaseSchema = "ext",
                    Select = new ColumnSet("EXAMPLEINT", "EXAMPLESTRING", "EXAMPLEID"),
                    From = "CONTOSO_EXAMPLEVIEW",
                    OrderBy = "EXAMPLEID",
                };

                var queryResults =
                    await databaseContext
                    .ReadEntityAsync<Entities.DataModel.ExampleEntity>(query)
                    .ConfigureAwait(continueOnCapturedContext: false);
                return new ExampleEntityDataResponse(queryResults);
            }
        }

        private async Task<Response> UpdateExampleEntity(UpdateExampleEntityDataRequest request)
        {
            ThrowIf.Null(request, nameof(request));
            ThrowIf.Null(request.UpdatedExampleEntity, nameof(request.UpdatedExampleEntity));

            if (request.ExampleEntityKey == 0)
            {
                throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ValueOutOfRange, $"{nameof(request.ExampleEntityKey)} cannot be 0");
            }



            bool updateSuccess = false;
            using (var databaseContext = new SqlServerDatabaseContext(request.RequestContext))
            {
                ParameterSet parameters = new ParameterSet();
                parameters["@bi_Id"] = request.ExampleEntityKey;
                parameters["@i_ExampleInt"] = request.UpdatedExampleEntity.IntData;
                parameters["@s_ExampleString"] = request.UpdatedExampleEntity.StringData;
                int sprocErrorCode =
                    await databaseContext
                    .ExecuteStoredProcedureNonQueryAsync("[ext].CONTOSO_UPDATEEXAMPLE", parameters, request.QueryResultSettings)
                    .ConfigureAwait(continueOnCapturedContext: false);
                updateSuccess = (sprocErrorCode == 0);
            }

            return new UpdateExampleEntityDataResponse(updateSuccess);
        }

        private async Task<Response> DeleteExampleEntity(DeleteExampleEntityDataRequest request)
        {
            ThrowIf.Null(request, nameof(request));

            if (request.ExampleEntityKey == 0)
            {
                throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ValueOutOfRange, $"{nameof(request.ExampleEntityKey)} cannot be 0");
            }

            bool deleteSuccess = false;
            using (var databaseContext = new SqlServerDatabaseContext(request.RequestContext))
            {
                ParameterSet parameters = new ParameterSet();
                parameters["@bi_Id"] = request.ExampleEntityKey;
                int sprocErrorCode =
                    await databaseContext
                    .ExecuteStoredProcedureNonQueryAsync("[ext].CONTOSO_DELETEEXAMPLE", parameters, request.QueryResultSettings)
                    .ConfigureAwait(continueOnCapturedContext: false);
                deleteSuccess = sprocErrorCode == 0;
            }

            return new DeleteExampleEntityDataResponse(deleteSuccess);
        }

        private async Task<string> getJsonString()
        {
            string jsonstrt = @"{
            ""FiscalRegistrationEventTypeValue"": 21,
            ""IsRemoteTransaction"": false,
            ""QueryBySalesId"": false,
            ""ShiftId"": 4,
            ""ShiftTerminalId"": ""000031"",
            ""TransactionId"": """",
            ""DocumentContext"": {
                ""AuditEvent"": {
                    ""EventId"": 638441313542432500,
                    ""EventIdString"": ""638441313542432512"",
                    ""Channel"": 5637144576,
                    ""Store"": ""FR01"",
                    ""Terminal"": ""000031"",
                    ""UploadType"": 0,
                    ""EventType"": 0,
                    ""EventDateTime"": ""2024-02-21T16:55:54.266Z"",
                    ""DurationInMilliseconds"": 0,
                    ""Source"": ""AddOnFeatures.Localization.AuditEvent.Common.AuditEventManager.registerConnectionStatusChangeAsync()"",
                    ""EventMessage"": ""Offline mode is activated."",
                    ""LogLevel"": 0,
                    ""Staff"": ""000011"",
                    ""ShiftId"": 4,
                    ""ClosedShiftId"": 0,
                    ""ReferenceId"": 0,
                    ""RefChannel"": 0,
                    ""RefStore"": """",
                    ""RefTerminal"": """",
                    ""RefTransactionId"": """",
                    ""AuditEventTypeValue"": 11,
                    ""FiscalTransactions"": [],
                    ""FiscalRegistrationLines"": [],
                    ""ExtensionProperties"": []
                },
                ""SignatureData"": [
                    {
                        ""SequenceKey"": ""LAST_SEQUENTIAL_SIGNATURE_4DDAB597-69E6-41C3-BA63-9FEFCFF3A112"",
                        ""SequentialNumber"": 5,
                        ""LastRegisterResponse"": ""{\""ExtensionProperties\"":[],\""CertificateThumbprint\"":\""3D11E96D8D788221216FA97C1C6AE019DFCB53DF\"",\""HashAlgorithm\"":\""SHA256\"",\""Signature\"":\""Q_jZfikPB3Y1e4c28hOJDhHWLd0guuCqJyIXGuVfth3aPaAXBzPArvWMdO07YhsvYdoMYUz8b3X05pi1W7Bn0o0xPCqJ9x1uESFqFjh4LYFVEbWlyK02ZuFEXWZ149ZfI8ujADQ7bR9rD_w3Ml-xJcobVyFkH9388yflQqsvbBUW8kN5Uqwrb9UxH0sLasMq6qotNv8Bn6jqMSE6npnxoUJcApaGFVfBgH_1tvRF2mxNmzF9KpE2rpk6FROBsZyS8BVsWZcyIua1-SyKR4eWaCypOJqN1GZtWXEDfJ8tG_pQZXTCIGj34QLTu411LDBbOmZ6U0aJ972yC7CFymlWrg\""}"",
                        ""IsOffline"": false
                    },
                    {
                        ""LastRegisterResponse"": """",
                        ""SequenceKey"": ""LAST_RECEIPT_COPY_91C74450-D4B3-4067-AEB3-D9C66D0BE6CF"",
                        ""IsOffline"": false,
                        ""SequentialNumber"": 0,
                        ""ExtensionProperties"": []
                    },
                    {
                        ""SequenceKey"": ""LAST_TECHNICAL_EVENT_2426E85B-A2E6-4842-B5B0-F422CE60FBCF"",
                        ""SequentialNumber"": 18,
                        ""LastRegisterResponse"": ""{\""ExtensionProperties\"":[],\""CertificateThumbprint\"":\""3D11E96D8D788221216FA97C1C6AE019DFCB53DF\"",\""HashAlgorithm\"":\""SHA256\"",\""Signature\"":\""K9Sc32Kd9TBfVxdz4WWmLi-0VwD4oyl0VqrO_efD52neZj1oSu8HOFkgyNpBkHygw76zoxct7t9W3r7AR11-lODA-CaykvU5m3ib5pYHOhPE_MqjWHrmmBk_8byt3P4mY1bbPsQOy0HAggPWNaXSCeXbnOCDk1D8fbs19mxo_xO_CUnEIue6zxcNXEp1qwoQkOyV9AuufQo8UxOcgUv2vlciTA4VpPtpE073h00eYKzh7nizPhUJZdnCiKKuPA3MB8SAynmL6xDig1CjSWSaV5wnt5Mhfnahwnb4B_Duc--vuREsHLGlBC0lel0eONCe0sqRFKGrsrnaz7L85RssWA\""}"",
                        ""IsOffline"": false
                    },
                    {
                        ""SequenceKey"": ""LAST_CLOSE_SHIFT_EVENT_74E9D344-4133-4C88-BE45-EEF2572CB2EA"",
                        ""SequentialNumber"": 5,
                        ""LastRegisterResponse"": ""{\""ExtensionProperties\"":[],\""CertificateThumbprint\"":\""3D11E96D8D788221216FA97C1C6AE019DFCB53DF\"",\""HashAlgorithm\"":\""SHA256\"",\""Signature\"":\""O8NyLjPv-OZX_EZVH0J46GoHHJh8AJomAkzpddlCw308o03hbgv1wTeEg05eEqt8Y1GzM9HN0Tt8H7WgR8uba926RmsnLp6j9l7crp1WfA9yZ-FPxEXwQCEzC8ACGq9VSa8jbRRCwJ0bwVP2lawpHaOshuFn8nUgUQbs8_u1PQK_DAqvdm3BZHWs1zVUSb9T7wN4krRcK2bAGP9foyFrQ-CUUCLgG-H66xdL8_fJcbigh8885DgRsWwww_und8SDoC9jdcrQbnNE8WGYR7AdfUIVjtT6J3rpGgIS5gdX5sCN7Y3dZeTI2ocr97YrDYd2vV6T6Us1R-G8Oi2QUmf2UA\""}"",
                        ""IsOffline"": false
                    }
                ]
            }
        }";

            return await Task<string>.FromResult(jsonstrt).ConfigureAwait(false); ;

        }
    }
}