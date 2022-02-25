FILE(GLOB_RECURSE RPC_SOURCES RELATIVE "${CMAKE_CURRENT_SOURCE_DIR}" CONFIGURE_DEPENDS "*.yaml")
MESSAGE("RPC_SOURCES: ${RPC_SOURCES}")

# file(READ ${CMAKE_CURRENT_SOURCE_DIR}/rpc.hash hash_old)
# message(${hash_old})

add_custom_target(rpc
  # file(TOUCH ${CMAKE_CURRENT_SOURCE_DIR}/rpc.hash hash_old)
  # file(READ ${CMAKE_CURRENT_SOURCE_DIR}/rpc.hash hash_old)
  # message(${hash_old})
  # file(READ ${CMAKE_CURRENT_SOURCE_DIR}/rpc.hash hash_new)
  # message(${hash_new})
  COMMAND pipenv run ${CMAKE_CURRENT_SOURCE_DIR}/../torikime/torikime.py -o ${CMAKE_CURRENT_SOURCE_DIR}/../proto/generated -c "${CMAKE_CURRENT_SOURCE_DIR}/../rpc/generated" -s "${CMAKE_SOURCE_DIR}/../../client/potato/Assets/Scripts/Rpc/Generated" -i ${CMAKE_CURRENT_SOURCE_DIR} --cache_dir ${CMAKE_CURRENT_BINARY_DIR}/torikime
  WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../torikime
  DEPENDS ${RPC_SOURCES}
  COMMAND find ${CMAKE_CURRENT_SOURCE_DIR} -type f -name '*.yaml' | sort | xargs sha1sum | sha1sum > "${CMAKE_CURRENT_SOURCE_DIR}/rpc.hash"
)

# file(READ ${CMAKE_CURRENT_SOURCE_DIR}/rpc.hash hash_new)
# message(${hash_new})

# add_custom_command(
#   TARGET rpc
#   PRE_BUILD
#   COMMAND pipenv run ${CMAKE_CURRENT_SOURCE_DIR}/../torikime/torikime.py -o ${CMAKE_CURRENT_SOURCE_DIR}/../proto/generated -c "${CMAKE_CURRENT_SOURCE_DIR}/../rpc/generated" -s "${CMAKE_SOURCE_DIR}/../../client/potato/Assets/Scripts/Rpc/Generated" -i ${CMAKE_CURRENT_SOURCE_DIR}
#   COMMAND touch ${CMAKE_CURRENT_SOURCE_DIR}/rpc.timestamp
#   WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../torikime
#   DEPENDS ${CMAKE_CURRENT_SOURCE_DIR}/${RPC_SOURCES}../torikime/proto.j2 ../torikime/rpc-cpp.j2 ../torikime/rpc-hpp.j2 ${CMAKE_CURRENT_SOURCE_DIR}/rpc.hash
#   COMMENT "Preprocessing rpcs"
#   BYPRODUCTS ${CMAKE_CURRENT_SOURCE_DIR}/../proto/generated/*/*.proto
#   )
