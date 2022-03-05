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
    pip install pipenv --upgrade-strategy=only-if-needed \

    # FROM builder AS action_server3
