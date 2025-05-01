# 1. D365 Commerce:  how to create and config aler rules<br/>

This document shows how to create an alert rule  https://learn.microsoft.com/en-us/dynamics365/fin-ops-core/fin-ops/get-started/alerts-overview <br/>

# 2. The key classes that process the custom alert <br/>
. Class: EventActionAlert (method: execute)
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


# 2. You should check if the feature is really disabled:<br/>
![image](https://github.com/user-attachments/assets/2f5bff87-edc3-4bf6-938e-6ac96c835304)








  
