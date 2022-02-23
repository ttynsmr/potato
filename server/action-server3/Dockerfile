FROM ubuntu:latest

ENV TZ=Asia/Tokyo
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

RUN apt-get update && apt-get install -yq \
    cmake \
    clang \
    libboost-all-dev \
    protobuf-compiler \
    libprotobuf-dev \
    python3-pip \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

RUN python -m pip install --upgrade pip \
    pip install pipenv --upgrade-strategy=only-if-needed
