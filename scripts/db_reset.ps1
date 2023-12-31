[string]$url = "http://localhost:8000"
[string]$region = "eu-west-1"

aws dynamodb delete-table --endpoint-url $url --region $region `
    --table-name "Url"
