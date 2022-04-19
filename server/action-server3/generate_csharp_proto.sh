#!/usr/bin/env bash

basepath=$(basename "$0")
tmpd=$(mktemp -dt "$basepath.XXXXXXXX")/
echo "$tmpd"

pushd torikime || exit
pipenv run python torikime.py -v -o "$tmpd"proto -s "../../../client/potato/Assets/Scripts/Rpc/Generated" -i "../rpc" --cache_dir "$tmpd"cache
popd || exit

cp ./proto/*.proto "$tmpd"proto/

find "$tmpd"proto -name "*.proto"
protoc --proto_path="$tmpd"proto --csharp_out=../../client/potato/Assets/Scripts/Proto "$tmpd"proto/*.proto
