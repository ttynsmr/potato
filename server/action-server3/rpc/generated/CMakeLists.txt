execute_process(COMMAND touch "${CMAKE_CURRENT_SOURCE_DIR}/x.cpp")

FILE(GLOB_RECURSE RPC_SOURCES RELATIVE "${CMAKE_CURRENT_SOURCE_DIR}" CONFIGURE_DEPENDS "*.cpp")
ADD_LIBRARY(rpc_contracts ${RPC_SOURCES})
add_dependencies(rpc_contracts rpc_generate proto)
