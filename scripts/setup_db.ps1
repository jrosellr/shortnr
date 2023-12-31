docker compose -f 'apps/compose.yaml' up database -d

[string]$url = "http://localhost:8000"
[string]$region = "eu-west-1"

aws dynamodb list-tables --endpoint-url $url --region $region

aws dynamodb create-table --endpoint-url $url --region $region `
    --table-name "Url" `
    --attribute-definitions `
    AttributeName="Key,AttributeType=S" `
    --key-schema `
    AttributeName="Key,KeyType=HASH" `
    --provisioned-throughput "ReadCapacityUnits=5,WriteCapacityUnits=5" `
    --table-class STANDARD
