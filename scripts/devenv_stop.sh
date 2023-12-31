#!/bin/sh

docker compose -f './apps/compose.yaml' down
docker compose -f './apps/compose.yaml' rm

