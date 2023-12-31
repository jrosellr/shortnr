#!/usr/bin/env sh

url="http://localhost:8000"
region="eu-west-1"

aws dynamodb describe-table --endpoint-url "$url" --region "$region" \
    --table-name "Url"

aws dynamodb scan --endpoint-url "$url" --region "$region" \
    --table-name "Url"

