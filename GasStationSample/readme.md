
Controller interface test:

1. GetGasPumpsByStore <br/>
curl --location --request POST 'https://usnconeboxax1ret.cloud.onebox.dynamics.com/Commerce/GasPumps/GetGasPumpsByStore?$top=250&$count=true&api-version=7.3' \
--header 'oun: 052' \
--header 'Content-Type: application/json' \
--data-raw '{
    "storeNumber": "HOUSTON"
}'
