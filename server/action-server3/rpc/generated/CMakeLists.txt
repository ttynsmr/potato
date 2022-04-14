execute_process(COMMAND touch "${CMAKE_CURRENT_BINARY_DIR}/x.cpp")

FILE(GLOB_RECURSE RPC_CPP_SOURCES "${CMAKE_CURRENT_BINARY_DIR}/*.cpp")
message("RPC_CPP_SOURCES.CMAKE_CURRENT_BINARY_DIR: ${CMAKE_CURRENT_BINARY_DIR}")
message("RPC_CPP_SOURCES: ${RPC_CPP_SOURCES}")
ADD_LIBRARY(rpc_contracts ${RPC_CPP_SOURCES})
add_dependencies(rpc_contracts rpc_generate proto)

target_include_directories(rpc_contracts PRIVATE ${CMAKE_SOURCE_DIR}/src)

set(GENERATED_RPC_DIR ${CMAKE_CURRENT_BINARY_DIR} PARENT_SCOPE)
