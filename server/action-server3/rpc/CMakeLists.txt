add_custom_target(rpc)

add_custom_command(
  TARGET rpc
  COMMAND ${CMAKE_CURRENT_SOURCE_DIR}/../torikime/torikime.py -o ${CMAKE_CURRENT_SOURCE_DIR}/../proto -i ${CMAKE_CURRENT_SOURCE_DIR}/rpc.yaml
  WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../torikime
  DEPENDS ${CMAKE_CURRENT_SOURCE_DIR}/rpc.yaml
  COMMENT "Preprocessing rpcs"
  BYPRODUCTS ${CMAKE_CURRENT_SOURCE_DIR}/../proto/*
  )
