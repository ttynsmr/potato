find ./proto -name "*.proto"
protoc --proto_path=./proto --csharp_out=../../client/potato/Assets/Scripts/Proto ./proto/*.proto
