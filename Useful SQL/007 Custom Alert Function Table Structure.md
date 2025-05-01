# 1. D365 Commerce:  how to create and config aler rules<br/>

This document shows how to create an alert rule  https://learn.microsoft.com/en-us/dynamics365/fin-ops-core/fin-ops/get-started/alerts-overview <br/>

# 2. The key classes that process the custom alert <br/>
Here is the reversed order of the list:

1. `EventJobCUDTask.runCudEventsForUser()`
2. `EventProcessorCUD.run()`
3. `EventProcessorCUD.process()`
4. `EventProcessorCUD.processUpdate()`
5. `EventProcessorCUD.processRuleUpdate()`
6. `EventProcessorCUD.executeActionsV2()`
7. `EventProcessorCUD.executeAction()`
8. `EventActionAlert.execute()`
9. `EventActionAlert.sendAlertEmailNotifications`
<hr/>
. Class: EventActionAlert (method: execute), from this method we can see the EventInboxId is inserted into the database<br/>

![image](https://github.com/user-attachments/assets/5cd1b531-02de-430b-a06f-020523557ec2)

. Method: sendAlertEmailNotifications
```
 System.Exception ex;
 try
 {
     Email _emailfrom;
     if(_userInfo != null && _userInfo.RecId)
     {
         _emailfrom = _userInfo.emailDisplay();
     }

     var emailBody = this.getEmailBody(_eventInbox, _eventRule, _buffer);
     
     var messageBuilder = new SysMailerMessageBuilder();
     
     var recipients = _eventRule.getEmailRecipients();
     var enumerator = recipients.getEnumerator();
     while(enumerator.moveNext())
     {
         var recipient = enumerator.current();
         if (recipient != '')
         {
             System.Exception e;
             try
             {
                 messageBuilder.reset()
                   .addTo(recipient)
                   .setSubject(_eventInbox.Subject)
                   .setBody(emailBody);

                 if(_emailfrom)
                 {
                     messageBuilder.setFrom(_emailFrom);
                 }
                 

                 mailer.sendNonInteractive(messageBuilder.getMessage());
             }
             catch(e)
             {
                 EventDefinitionAndProcessingProviderEventSource::EventWriteEventActionAlertInfo("sendAlertEmailNotifications", strFmt("Failed to send email. Exception: %1", e.ToString()));
             }
         }
     }
 }
```


# 2. The Key Tables and the Relations:<br/>

```sql

-- From Table Name to get TableId
select  T.ID, * from dbo.TABLEIDTABLE as T where T.Name = 'CustTable'
-- From TableId to get the Alert Event Rule Id
select T.RULEID,  * from dbo.EVENTRULE as T where T.ALERTTABLEID = 10347
-- From Rule Id to get the Rule Details
select  T.USERID, T.ALERTFIELDID, T.ALERTFIELDLABEL, T.ALERTTABLEID, T.COMPANYID, T.MESSAGE, T.SUBJECT, * from dbo.EVENTRULE as T where T.RULEID = 000263

-- From EventInBox to get a lot of more details
select T.ALERTTABLEID, T.RULEID, T.EMAILTEMPLATEID, T.EMAILID, T.INBOXID, T.ALERTEDFOR, T.SENDEMAIL, T.SUBJECT, T.MESSAGE, T.EMAILRECIPIENT, T.INBOXID, * from dbo.EVENTINBOX as T where T.ALERTTABLEID = 10347 and T.RULEID = 000263

-- Get EmailTemplate
select * from dbo.SYSEMAILTABLE as T where T.EMAILID = 'EmailTemplateId from EVENTINBOX' -- EmailTemplateId

-- Get the Real Email
select T.EMAILITEMID, * from dbo.SYSOUTGOINGEMAILTABLE as T where T.EMAILITEMID = 'EmailId from EVENTINBOX '

-- Get the EventInboxData
select * from dbo.EventInboxData(nolock) as T where T.INBOXID = 5637144581-- 'InBoxId from dbo.EVENTINBOX'

select * from dbo.CUSTTABLE as T where T.RECID = 22565431406

select T.CUDRECID, * from dbo.EventCUD as T where T.CUDTABLEID = 10347
select * from dbo.EventCUD as T where T.CUDRECID = 22565431406
```








  
