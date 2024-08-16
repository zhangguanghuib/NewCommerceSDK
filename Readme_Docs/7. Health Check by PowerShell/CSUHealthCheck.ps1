$computerName = $env:COMPUTERNAME #should be the certificate name if computer name different from Certificate name
$port = "446"
$baseUrl = "https://$($computerName):$($port)/RetailServer"
$healthcheckEndpoint = "/healthcheck?testname=ping"
$ServerEndPoint = "/Commerce"
$healthCheckUrl = $baseUrl + $healthcheckEndpoint 
$ServerUrl =  $baseUrl + $ServerEndPoint 

 Write-Output "$($baseUrl)"
 Write-Output "$($healthCheckUrl)"

try {
    $response = Invoke-WebRequest -Uri $healthCheckUrl -UseBasicParsing

    if ($response.StatusCode -eq 200) {
        Write-Output "$($ServerUrl) is running properly. Status code: $($response.StatusCode)"
    } else {
        Write-Output "$($ServerUrl) responded, but with an unexpected status code: $($response.StatusCode)"
    }
} catch {
    Write-Output "Failed to reach the server. Error: $_"
}
