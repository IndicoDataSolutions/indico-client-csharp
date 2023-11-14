#!/bin/bash

if [ $# -ne 1 ]; then
    echo "Usage:"
    echo "$0 {token}"
    echo
    exit 1;
fi

REFRESH_TOKEN=$1
URL=https://try.indico.io/graph/api/graphql
# URL=https://dev.indico.io/graph/api/graphql
PROJ=./

TOKEN=$(curl --location --request POST 'https://try.indico.io/auth/users/refresh_token' \
--header "Authorization: Bearer $REFRESH_TOKEN" \
 | jq .auth_token \
 | tr -d '"')

dotnet graphql init -n IndicoGqlClient --token $TOKEN --scheme Bearer -p $PROJ $URL