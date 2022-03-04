FILE(GLOB_RECURSE RPC_SOURCES RELATIVE "${CMAKE_CURRENT_SOURCE_DIR}" CONFIGURE_DEPENDS "*.yaml")
MESSAGE("RPC_SOURCES: ${RPC_SOURCES}")

add_custom_target(rpc_generate
  COMMAND pipenv run ${CMAKE_CURRENT_SOURCE_DIR}/../torikime/torikime.py -v -o ${CMAKE_CURRENT_SOURCE_DIR}/../proto -c "${CMAKE_CURRENT_SOURCE_DIR}/../rpc/generated" -s "${CMAKE_SOURCE_DIR}/../../client/potato/Assets/Scripts/Rpc/Generated" -i ${CMAKE_CURRENT_SOURCE_DIR} --cache_dir ${CMAKE_CURRENT_BINARY_DIR}/torikime
  WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../torikime
  DEPENDS ${RPC_SOURCES}
  COMMAND find ${CMAKE_CURRENT_SOURCE_DIR} -type f -name '*.yaml' | sort | xargs sha1sum | sha1sum > "${CMAKE_CURRENT_SOURCE_DIR}/rpc.hash"
)
