# Audit Event Log 
Please check the code in the POS project
https://github.com/zhangguanghuib/NewCommerceSDK/blob/main/POS_Samples/ManagerOverride.RecordForReturn/CommerceRuntime/AddReturnCartLinesRequestTrigger.cs
1.  Set functional profile:
    ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/73c387b3-03d9-4267-91b4-837c796ed5b2)

2. The code section is like this:
   ```csharp
    public async Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");

            AddReturnCartLinesRequest addReturnCartLinesRequest = (AddReturnCartLinesRequest)request;
            ICommercePrincipal commercePrincipal = addReturnCartLinesRequest.RequestContext.GetPrincipal();

            if (commercePrincipal.OriginalUserId != commercePrincipal.UserId)
            {
               string originTransactionId =  addReturnCartLinesRequest.ReturnCartLines.AsPagedResult().ToList().First().ReturnTransactionId;

                var message = string.Format(
                          "Manager with id '{0}' has approved override for operation with id '{1}' for original transaction {3} to the operator with id '{2}'.",
                          commercePrincipal.StaffId,
                          RetailOperation.ReturnTransaction,
                          commercePrincipal.OriginalUserId,
                          originTransactionId);

                await LogAuditEntry(
                           request.RequestContext,
                           ExtensibleAuditEventType.ManagerOverride,
                           "ElevateUser",
                           message,
                           AuditLogTraceLevel.Trace).ConfigureAwait(false);
            }
        }
     private static async Task LogAuditEntry(RequestContext context, ExtensibleAuditEventType eventType, string source, string value, AuditLogTraceLevel logTraceLevel = AuditLogTraceLevel.Trace)
        {
            var auditEvent = new AuditEvent();
            auditEvent.InitializeEventInfo(source, value, logTraceLevel, eventType);

            var auditLogServiceRequest = new RegisterAuditEventServiceRequest(auditEvent);
            await context.ExecuteAsync<NullResponse>(auditLogServiceRequest).ConfigureAwait(false);
        }
   ```

   3.  Finally you should see the the original transaction is logged into the Audit Event Log Table, there is another version to log the original transaction id or Receipt Id
      ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c127746e-aef8-45d2-a13e-49f76743ead9)



