[string]$url = "http://localhost:8000"
[string]$region = "eu-west-1"

aws dynamodb get-item --endpoint-url $url --region $region `
    --table-name "Url" `
    --key file://scripts/get_item.json
