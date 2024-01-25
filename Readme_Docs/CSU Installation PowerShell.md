# This document is to demonstrate install sealed CSU by PowerShell.

- ## The benifit is Thumbprint and Client is put into the config file and then you don't replace the same thumbprint and client id in the command multiple time, that will help avoid mistakes.
- ## If you have new Thumbprint and Client, you will need replace one and feel quite convenient.

1. Please create a xml file that contains Client Id and ThumbPrint
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Configuration>
	<Thumbprint>***</Thumbprint>
	<ClientId>***</ClientId>
</Configuration>

```
Save it as config.xml

2.  Prepare power-shell script like below and save it as deployCSU.ps1
```console
# Load the XML configuration file
[xml]$config = Get-Content -Path 'config.xml'

# Extract the Thumbprint and ClientId
$thumbprint = $config.Configuration.Thumbprint
$clientId = $config.Configuration.ClientId

Write-Host "Thumbprint: $thumbprint"
Write-Host "ClientId: $clientId"

$command = '.\CommerceStoreScaleUnitSetup.exe install --port 443 --SslCertFullPath "store:///My/LocalMachine?FindByThumbprint=<Thumbprint>" --AsyncClientCertFullPath "store:///My/LocalMachine?FindByThumbprint=<Thumbprint>" --RetailServerCertFullPath "store:///My/LocalMachine?FindByThumbprint=<Thumbprint>" --RetailServerAadClientId "<ClientId>" --RetailServerAadResourceId "api://<ClientId>" --CposAadClientId "<ClientId>" --AsyncClientAadClientId "<ClientId>" --config StoreSystemSetup.xml --TrustSqlServerCertificate --SkipScaleUnitHealthCheck --SqlServerName "." --SkipTelemetryCheck --SkipSChannelCheck --TrustSqlServerCertificate'

# Replace the placeholders with the actual values
$command = $command.Replace('<Thumbprint>', $thumbprint)
$command = $command.Replace('<ClientId>', $clientId)

Write-Host "Command: $$command"

# Execute the command
Invoke-Expression -Command:$command
```

3. The folder run script should like this:
   - Config file with Thumbprint and Client id: config.xml
   - PowerShell Script: deployCSU.ps1
   - CSU config file, download from HQ  channel database: StoreSystemSetup.xml
   - CSU installer, download from LCS shared library: CommerceStoreScaleUnitSetup.exe
<img width="625" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/e66d530f-4b56-4be5-8980-57fefd1d1cbc">

4. It is verified working fine in my environment:<br/>
   Run it in powershell: <img width="276" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/3385e8ff-639d-480b-b943-77378fa9e7ef"><br/>

   <img width="1231" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9cad6c58-93dc-4f84-a3e8-9aef963ac2b6">







        




    
    














