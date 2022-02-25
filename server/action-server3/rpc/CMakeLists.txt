# add_custom_target(rpc
#   DEPENDS ${CMAKE_CURRENT_SOURCE_DIR}/rpc.timestamp
# )

FILE(GLOB_RECURSE RPC_SOURCES RELATIVE "${CMAKE_CURRENT_SOURCE_DIR}" CONFIGURE_DEPENDS "*.yaml")
# set(RPC_SOURCES
# room.yaml
# rpc.yaml
# )
MESSAGE("RPC_SOURCES: ${RPC_SOURCES}")

# add_custom_command(
#   OUTPUT ${CMAKE_CURRENT_SOURCE_DIR}/rpc.timestamp
#   COMMAND ${CMAKE_CURRENT_SOURCE_DIR}/../torikime/torikime.py -o ${CMAKE_CURRENT_SOURCE_DIR}/../proto -i ${CMAKE_CURRENT_SOURCE_DIR}/${RPC_SOURCES}
#   COMMAND touch ${CMAKE_CURRENT_SOURCE_DIR}/rpc.timestamp
#   WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../torikime
#   DEPENDS ${CMAKE_CURRENT_SOURCE_DIR}/${RPC_SOURCES} ../torikime/proto.j2 ../torikime/rpc-cpp.j2
#   COMMENT "Preprocessing rpcs"
#   BYPRODUCTS ${CMAKE_CURRENT_SOURCE_DIR}/../proto/*/*.proto ${CMAKE_CURRENT_SOURCE_DIR}/rpc.timestamp
#   )

add_custom_target(rpc
  DEPENDS ${RPC_SOURCES}
)

# add_custom_command(
#   TARGET rpc
#   COMMAND ${CMAKE_CURRENT_SOURCE_DIR}/../torikime/torikime.py -o ${CMAKE_CURRENT_SOURCE_DIR}/../proto -i ${CMAKE_CURRENT_SOURCE_DIR}/rpc.yaml
#   COMMAND touch ${CMAKE_CURRENT_SOURCE_DIR}/rpc.timestamp
#   WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../torikime
#   DEPENDS ${CMAKE_CURRENT_SOURCE_DIR}/rpc.yaml ../torikime/proto.j2 ../torikime/rpc-cpp.j2
#   COMMENT "Preprocessing rpcs"
#   BYPRODUCTS ${CMAKE_CURRENT_SOURCE_DIR}/../proto/*/*.proto COMMAND touch ${CMAKE_CURRENT_SOURCE_DIR}/rpc.timestamp
#   )


# function(torikime output_dir input)
#   execute_process(COMMAND ${CMAKE_CURRENT_SOURCE_DIR}/../torikime/torikime.py -o ${output_dir} -i ${input})
# endfunction()


add_custom_command(
  TARGET rpc
  COMMAND pipenv run ${CMAKE_CURRENT_SOURCE_DIR}/../torikime/torikime.py -o ${CMAKE_CURRENT_SOURCE_DIR}/../proto/generated -c "${CMAKE_CURRENT_SOURCE_DIR}/../generated/rpc" -s "${CMAKE_SOURCE_DIR}/../../client/potato/Assets/Scripts/Rpc/Generated" -i ${CMAKE_CURRENT_SOURCE_DIR}
  COMMAND touch ${CMAKE_CURRENT_SOURCE_DIR}/rpc.timestamp
  WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../torikime
  DEPENDS ${CMAKE_CURRENT_SOURCE_DIR}/${RPC_SOURCES} ../torikime/proto.j2 ../torikime/rpc-cpp.j2 ../torikime/rpc-hpp.j2
  COMMENT "Preprocessing rpcs"
  BYPRODUCTS ${CMAKE_CURRENT_SOURCE_DIR}/../proto/*/*.proto ${CMAKE_CURRENT_SOURCE_DIR}/rpc.timestamp
  )
