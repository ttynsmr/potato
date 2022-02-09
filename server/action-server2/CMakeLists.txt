# CMakeList.txt : action-server2 の CMake プロジェクト。ソースを含めて、次を定義します:
# プロジェクト専用ロジックはこちらです。
#
cmake_minimum_required (VERSION 3.8)

project ("action-server2")

# ソースをこのプロジェクトの実行可能ファイルに追加します。
add_executable (action-server2 "action-server2.cpp" "action-server2.h")

set(CMAKE_CXX_STANDARD 17)

set(CMAKE_THREAD_LIBS_INIT "-lpthread")
set(CMAKE_HAVE_THREADS_LIBRARY 1)
set(CMAKE_USE_WIN32_THREADS_INIT 0)
set(CMAKE_USE_PTHREADS_INIT 1)
set(THREADS_PREFER_PTHREAD_FLAG ON)

find_package (Threads REQUIRED)

target_link_libraries(action-server2 PUBLIC "-pthread") 

#set( BOOST_ROOT "/usr/lib/x86_64-linux-gnu/cmake/")
set( BOOST_ROOT "/usr/lib/x86_64-linux-gnu/")

set( Boost_USE_STATIC_LIBS OFF )
set( Boost_USE_MULTITHREADED ON )
set( Boost_USE_STATIC_RUNTIME OFF )

find_package( Boost 1.71.0 COMPONENTS system)
#find_package( Boost 1.72.0 COMPONENTS thread fiber context )

if ( Boost_FOUND )
    include_directories( ${Boost_INCLUDE_DIRS} )
    link_libraries( action-server2 ${Boost_LIBRARIES} )
else()
    message( FATAL_ERROR "Required Boost packages not found. Perhaps add -DBOOST_ROOT?" )
endif()

# set(CMAKE_C_COMPILER "/usr/bin/clang" CACHE string "clang compiler" FORCE)
# set(CMAKE_CXX_COMPILER "/usr/bin/clang++" CACHE string "clang++ compiler" FORCE)

#add_compile_options(-W"switch" -W"no-deprecated-declarations" -W"empty-body" -W"conversion" -W"return-type" -W"parentheses" -W"no-pointer-sign" -W"no-format" -W"uninitialized" -W"unreachable-code" -W"unused-function" -W"unused-value" -W"unused-variable" -std=c++17 -Wall -fno-strict-aliasing -I "/usr/include/boost" -g2 -gdwarf-2 "clang++" -O0 -fthreadsafe-statics -W"switch" -W"no-deprecated-declarations" -W"empty-body" -W"conversion" -W"return-type" -W"parentheses" -W"no-format" -W"uninitialized" -W"unreachable-code" -W"unused-function" -W"unused-value" -W"unused-variable" -frtti -fno-omit-frame-pointer -std=c11 -fexceptions "1")
#add_link_options(-Wl,-Wl,-z,relro -Wl,-z,noexecstack -Wl,--no-undefined "clang++" -Wl,-z,now)

# TODO: テストを追加し、必要な場合は、ターゲットをインストールします。
