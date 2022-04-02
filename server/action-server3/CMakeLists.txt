# CMakeList.txt : action-server3 の CMake プロジェクト。ソースを含めて、次を定義します:
# プロジェクト専用ロジックはこちらです。
#
cmake_minimum_required (VERSION 3.8)

set_property(GLOBAL PROPERTY GLOBAL_DEPENDS_DEBUG_MODE 1)

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
  set(CMAKE_CXX_FLAGS_DEBUG "-g3 -O0 -pg")
  set(CMAKE_CXX_FLAGS_RELEASE "-O2 -s -DNDEBUG -march=native")
endif()

# eventpp
ADD_SUBDIRECTORY(externals/eventpp)

# Eigen3
INCLUDE_DIRECTORIES(externals/eigen)

# fmt
ADD_SUBDIRECTORY(externals/fmt)
INCLUDE_DIRECTORIES(externals/fmt/include)

# strong_type
ADD_SUBDIRECTORY(externals/strong_type)
INCLUDE_DIRECTORIES(externals/strong_type/include)

INCLUDE_DIRECTORIES(${CMAKE_CURRENT_BINARY_DIR})

# add protocol buffer settings
ADD_SUBDIRECTORY(proto)

ADD_SUBDIRECTORY(rpc)
ADD_SUBDIRECTORY(rpc/generated)
INCLUDE_DIRECTORIES(rpc)

INCLUDE_DIRECTORIES(src)

list(APPEND ALL_SOURCES ${SUB_SOURCES} ${GEN_SOURCES})
list(APPEND ALL_SOURCES "action-server3.cpp")

message("ALL_SOURCES: ${ALL_SOURCES}")

# ソースをこのプロジェクトの実行可能ファイルに追加します。
add_executable (action-server3  ${ALL_SOURCES}
  "src/Payload.h" "src/Payload.cpp" "src/rpc.h" "src/rpc.cpp"
  "src/session/session.h" "src/session/session.cpp" "src/session/session_types.h"
  "src/units/unit.cpp" "src/units/unit.h" "src/units/unit_types.h"
  "src/units/unit_registory.cpp" "src/units/unit_registory.h"
  "src/units/components/component_types.h"
  "src/units/components/npc_component.cpp" "src/units/components/npc_component.h"
  "src/units/components/status_component.cpp" "src/units/components/status_component.h"
  "src/core/core.h"
  "src/core/configured_eigen.h"
  "src/services/service.cpp" "src/services/service.h"
  "src/services/service_provider.h"
  "src/services/game_service_provider.cpp" "src/services/game_service_provider.h"
  "src/services/network_service_provider.cpp" "src/services/network_service_provider.h"
  "src/services/serialize_service_provider.h" "src/services/serialize_service_provider.cpp"
  "src/area/area.h" "src/area/area.cpp" "src/area/area_types.h"
  "src/user/user.cpp" "src/user/user.h" "src/user/user_types.h"
  "src/user/user_registory.h" "src/user/user_registory.cpp"
  "src/utility/vector_utility.h" "src/utility/vector_utility.cpp"
  )

find_package (Threads REQUIRED)

INCLUDE(FindProtobuf)
FIND_PACKAGE(Protobuf REQUIRED)

target_link_libraries(action-server3 pthread eventpp fmt::fmt ${PROTOBUF_LIBRARY} proto rpc_contracts)

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
