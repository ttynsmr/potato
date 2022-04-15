execute_process(COMMAND touch "${CMAKE_CURRENT_BINARY_DIR}/x.cpp")

file(GLOB_RECURSE RPC_CPP_SOURCES "${CMAKE_CURRENT_BINARY_DIR}/*.cpp")
# message("RPC_CPP_SOURCES.CMAKE_CURRENT_BINARY_DIR: ${CMAKE_CURRENT_BINARY_DIR}")
# message("RPC_CPP_SOURCES: ${RPC_CPP_SOURCES}")
add_library(rpc_contracts ${RPC_CPP_SOURCES})
add_dependencies(rpc_contracts rpc_generate proto)

target_include_directories(rpc_contracts PRIVATE ${CMAKE_SOURCE_DIR}/src)
target_include_directories(rpc_contracts PRIVATE ${GENERATED_PROTO_DIR})

set_target_properties(rpc_contracts PROPERTIES INTERFACE_INCLUDE_DIRECTORIES ${CMAKE_CURRENT_BINARY_DIR})
