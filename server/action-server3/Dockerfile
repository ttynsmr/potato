FROM ubuntu:latest AS builder

ENV TZ=Asia/Tokyo
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# COPY apt.souces.list /etc/apt/sources.list

RUN apt-get update && apt-get install -yq \
    cmake \
    clang \
    ninja-build \
    libboost-all-dev \
    protobuf-compiler \
    libprotobuf-dev \
    python3 \
    python3-pip \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

RUN python3 -m pip install --upgrade pip \
    pip install pipenv --upgrade-strategy=only-if-needed

FROM builder AS action_server3_build
COPY . /opt/app/action-server3
RUN cd /opt/app/action-server3 \
    && cd torikime && pipenv sync && cd ..
RUN cd /opt/app/action-server3 \
    && mkdir -p build.docker \
    && cmake -DCMAKE_BUILD_TYPE=Release -S . -H. -Bbuild.docker -GNinja \
    && cmake -DCMAKE_BUILD_TYPE=Release -S . --build build.docker/ --target all || cmake -DCMAKE_BUILD_TYPE=Release -S . --build build.docker/ --target all

FROM ubuntu:latest AS action_server3_application
COPY --from=action_server3_build /opt/app/action-server3/build.docker/action-server3 ./
RUN apt-get update && apt-get install -yq \
    libprotobuf17 \
    && apt clean \
    && rm -rf /var/lib/apt/lists/*
CMD ["./action-server3", "28888"]
