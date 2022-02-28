# CMakeList.txt : action-server3 の CMake プロジェクト。ソースを含めて、次を定義します:
# プロジェクト専用ロジックはこちらです。
#
cmake_minimum_required (VERSION 3.8)

project ("action-server3")

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

set(CMAKE_THREAD_LIBS_INIT "-lpthread")
set(CMAKE_HAVE_THREADS_LIBRARY 1)
set(CMAKE_USE_WIN32_THREADS_INIT 0)
set(CMAKE_USE_PTHREADS_INIT 1)
set(THREADS_PREFER_PTHREAD_FLAG ON)

if(MSVC)
  # Force to always compile with W4
  if(CMAKE_CXX_FLAGS MATCHES "/W[0-4]")
    string(REGEX REPLACE "/W[0-4]" "/W4" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
  else()
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /W4")
  endif()
elseif(CMAKE_COMPILER_IS_GNUCC OR CMAKE_COMPILER_IS_GNUCXX)
  # Update if necessary
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall -Wextra -Wno-long-long -pedantic")
endif()

# # torikime
# # ./torikime/torikime.py -o ./proto -i ./rpc/rpc.yaml

# add_custom_target(
#   rpc
#   DEPENDS ${CMAKE_CURRENT_SOURCE_DIR}/rpc/rpc.yaml
#   )

# add_custom_command(
#   TARGET rpc
#   COMMAND ${CMAKE_CURRENT_SOURCE_DIR}/torikime/torikime.py -o ${CMAKE_CURRENT_SOURCE_DIR}/proto -i ${CMAKE_CURRENT_SOURCE_DIR}/rpc/rpc.yaml
#   WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/torikime
#   DEPENDS ${CMAKE_CURRENT_SOURCE_DIR}/rpc/rpc.yaml
#   COMMENT "Preprocessing rpcs"
#   )

# add_custom_target(
#   rpc
#   COMMAND ${CMAKE_CURRENT_SOURCE_DIR}/torikime/torikime.py -o ${CMAKE_CURRENT_SOURCE_DIR}/proto -i ${CMAKE_CURRENT_SOURCE_DIR}/rpc/rpc.yaml
#   WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/torikime
#   DEPENDS ${CMAKE_CURRENT_SOURCE_DIR}/rpc/rpc.yaml
#   COMMENT "Preprocessing rpcs"
#   BYPRODUCTS ${CMAKE_CURRENT_SOURCE_DIR}/proto/*
#   )

# eventpp
ADD_SUBDIRECTORY(externals/eventpp)

# add protocol buffer settings
INCLUDE(FindProtobuf)
FIND_PACKAGE(Protobuf REQUIRED)
ADD_SUBDIRECTORY(proto)
INCLUDE_DIRECTORIES(${CMAKE_CURRENT_BINARY_DIR})

ADD_SUBDIRECTORY(rpc)
ADD_SUBDIRECTORY(rpc/generated)
add_dependencies(proto rpc)
add_dependencies(rpc_contracts proto)

# ADD_SUBDIRECTORY(${CMAKE_CURRENT_SOURCE_DIR}/rpc/generated/cpp)
# file(GLOB SUB_SOURCES RELATIVE "${CMAKE_SOURCE_DIR}" CONFIGURE_DEPENDS "generated/rpc/cpp/*")

list(APPEND ALL_SOURCES ${SUB_SOURCES} ${GEN_SOURCES})
list(APPEND ALL_SOURCES "action-server3.cpp")

message("ALL_SOURCES: ${ALL_SOURCES}")

# ソースをこのプロジェクトの実行可能ファイルに追加します。
add_executable (action-server3  ${ALL_SOURCES} "src/Payload.h" "src/Payload.cpp" "src/rpc.h" "src/rpc.cpp" "src/session.h" "src/session.cpp")

#target_compile_features(${TARGET_NAME} PRIVATE cxx_std_17)

find_package (Threads REQUIRED)

target_link_libraries(action-server3 pthread eventpp proto ${PROTOBUF_LIBRARY} rpc_contracts)

set( BOOST_ROOT "/usr/lib/x86_64-linux-gnu/")

set( Boost_USE_STATIC_LIBS OFF )
set( Boost_USE_MULTITHREADED ON )
set( Boost_USE_STATIC_RUNTIME OFF )

find_package( Boost 1.71.0 COMPONENTS system)

if ( Boost_FOUND )
    include_directories( ${Boost_INCLUDE_DIRS} )
    link_libraries( action-server3 ${Boost_LIBRARIES} )
else()
    message( FATAL_ERROR "Required Boost packages not found. Perhaps add -DBOOST_ROOT?" )
endif()

# message(STATUS "*** dump start cmake variables ***")
# get_cmake_property(_variableNames VARIABLES)
# foreach(_variableName ${_variableNames})
#         message(STATUS "${_variableName}=${${_variableName}}")
# endforeach()
# message(STATUS "*** dump end ***")
