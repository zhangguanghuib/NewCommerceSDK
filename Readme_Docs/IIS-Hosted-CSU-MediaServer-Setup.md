# This document is to demonstrate IIS-Hosted CSU Media Server Setup.

- ## The background is currently the Sealed-Version CSU installation does not include Media Server Setup, so user need manually set the media server externally or internally.

The steps:
1. Go to IIS, create a new website:<br/>
<img width="312" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/60915e40-b876-47bd-bd4c-b292742aa8ed">
<br/>


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

   <img width="900" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/5b896ea2-052e-4a20-9652-fae85494863d">








        




    
    














