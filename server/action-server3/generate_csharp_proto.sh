#/bin/bash

find ./proto -name "*.proto"
mkdir -p ../../client/potato/Assets/Scripts/Proto
# protoc --proto_path=./proto --csharp_out=../../client/potato/Assets/Scripts/Proto ./proto/*.proto
# protoc --proto_path=./proto --csharp_out=../../client/potato/Assets/Scripts/Proto ./proto/generated/*/*.proto
protoc --proto_path=./proto/generated --csharp_out=../../client/potato/Assets/Scripts/Proto ./proto/generated/*.proto
