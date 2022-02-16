# CMakeList.txt : action-server2 の CMake プロジェクト。ソースを含めて、次を定義します:
# プロジェクト専用ロジックはこちらです。
#
cmake_minimum_required (VERSION 3.8)

project ("action-server2")

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

# file(GLOB Fbs_Definitions RELATIVE "../protocol" "*.fbs")
# add_custom_command(OUTPUT "${protocol_fb_hdrs}" "${protocol_fb_srcs}"
#                    COMMAND flatc
#                    ARGS -o "${CMAKE_CURRENT_SOURCE_DIR}/generated"
#                              --cpp "${Fbs_Definitions}"
#                    DEPENDS "${Fbs_Definitions}")

# flatc --cpp -o ./cpp ./hello.fbs
# flatc --csharp -o ./cs ./hello.fbs

set(FLATBUFFERS_FLATC_EXECUTABLE flatc)
function(compile_flatbuffers_schema_to_cpp SRC_FBS)
  get_filename_component(SRC_FBS_DIR ${SRC_FBS} PATH)
  string(REGEX REPLACE "\\.fbs$" "_generated.h" GEN_HEADER ${SRC_FBS})
  add_custom_command(
    OUTPUT ${GEN_HEADER}
    COMMAND "${FLATBUFFERS_FLATC_EXECUTABLE}" -c --no-includes --gen-mutable -o "${SRC_FBS_DIR}" "${CMAKE_CURRENT_SOURCE_DIR}/${SRC_FBS}"
    )
  # message("${FLATBUFFERS_FLATC_EXECUTABLE} -c --no-includes --gen-mutable -o ${SRC_FBS_DIR} ${CMAKE_CURRENT_SOURCE_DIR}/${SRC_FBS}")
  # message("compile_flatbuffers_schema_to_cpp -- ${GEN_HEADER}")
  set(GEN_HEADER ${GEN_HEADER} PARENT_SCOPE)
endfunction()

file(GLOB FBS_SOURCES RELATIVE "${CMAKE_SOURCE_DIR}" "protocol/*")
compile_flatbuffers_schema_to_cpp(${FBS_SOURCES} GEN_HEADER)

file(GLOB SUB_SOURCES RELATIVE "${CMAKE_SOURCE_DIR}" "src/*")
# file(GLOB GEN_SOURCES RELATIVE "${CMAKE_SOURCE_DIR}" "generated/*")

include_directories("${CMAKE_CURRENT_BINARY_DIR}/")

list(APPEND ALL_SOURCES ${SUB_SOURCES} ${GEN_SOURCES} ${GEN_HEADER})
list(APPEND ALL_SOURCES "action-server2.cpp")

message("GEN_HEADER: ${GEN_HEADER}")
message("ALL_SOURCES: ${ALL_SOURCES}")

# ソースをこのプロジェクトの実行可能ファイルに追加します。
add_executable (action-server2  ${ALL_SOURCES})

#target_compile_features(${TARGET_NAME} PRIVATE cxx_std_17)

find_package (Threads REQUIRED)

target_link_libraries(action-server2 PUBLIC pthread PRIVATE flatbuffers) 

set( BOOST_ROOT "/usr/lib/x86_64-linux-gnu/")

set( Boost_USE_STATIC_LIBS OFF )
set( Boost_USE_MULTITHREADED ON )
set( Boost_USE_STATIC_RUNTIME OFF )

find_package( Boost 1.71.0 COMPONENTS system)

if ( Boost_FOUND )
    include_directories( ${Boost_INCLUDE_DIRS} )
    link_libraries( action-server2 ${Boost_LIBRARIES} )
else()
    message( FATAL_ERROR "Required Boost packages not found. Perhaps add -DBOOST_ROOT?" )
endif()

message(STATUS "*** dump start cmake variables ***")
get_cmake_property(_variableNames VARIABLES)
foreach(_variableName ${_variableNames})
        message(STATUS "${_variableName}=${${_variableName}}")
endforeach()
message(STATUS "*** dump end ***")
