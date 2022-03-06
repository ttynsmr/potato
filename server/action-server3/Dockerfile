FROM ubuntu:latest AS builder

ENV TZ=Asia/Tokyo
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY apt.souces.list /etc/apt/sources.list

RUN apt update && apt install -yq \
    cmake \
    clang \
    ninja-build \
    libboost-all-dev \
    protobuf-compiler \
    libprotobuf-dev \
    python3 \
    python3-pip \
    && apt clean \
    && rm -rf /var/lib/apt/lists/*

RUN python3 -m pip install --upgrade pip \
    pip install pipenv --upgrade-strategy=only-if-needed

FROM builder AS action_server3_build
COPY . /opt/app/action-server3
RUN "/bin/bash -c 'cd /opt/app/action-server3 && pushd torikime && python3 -m pip install --upgrade pip && pip install pipenv --upgrade-strategy=only-if-needed && pipenv install && pipenv install pyyaml jinja2 inflection && popd && mkdir -p build.docker && cmake -H. -Bbuild.docker -GNinja && cmake --build build.docker/ --target all || cmake --build build.docker/ --target all'"
