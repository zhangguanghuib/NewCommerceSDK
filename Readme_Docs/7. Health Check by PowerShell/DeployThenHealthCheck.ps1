# Path to the first script
$deployScript = ".\deployCSU.ps1"

# Path to the second script
$healthCheckScript = ".\CSUHealthCheck.ps1"

# Execute the first script
& $deployScript

# Execute the second script after the first one completes
& $healthCheckScript
