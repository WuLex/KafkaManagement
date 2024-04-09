# Kafka simple management system

## 常用命令
[kafka相关命令](https://github.com/WuLex/KafkaMonitor/blob/71c5eaccad7516af480738f0547a95f18d0ff2c1/%E5%90%AF%E5%8A%A8kafka%E5%91%BD%E4%BB%A4.md)

## 如图：
生产消息：
![img](https://raw.githubusercontent.com/WuLex/UsefulPicture/main/kafkaimg/command%20(3).png)

展示消息列表：
![img](https://raw.githubusercontent.com/WuLex/UsefulPicture/main/kafkaimg/result.png)


## 创建Kafka集群,如图
![img](https://raw.githubusercontent.com/WuLex/UsefulPicture/main/kafkaimg/kafkacluster/dockerkafka.png)

### 创建一个 `docker-compose.yml` 文件

用于定义 Kafka 集群的服务和配置

```yaml
version: '3.7'

services:
  zookeeper-1:
    image: wurstmeister/zookeeper:3.4.6
    container_name: zookeeper-1
    hostname: zookeeper-1
    ports:
      - "2181:2181"  # 将 Zookeeper 服务端口映射到主机端口

    environment:
      ZOO_MY_ID: 1  # 设置当前 Zookeeper 实例的 ID
      ZOO_SERVERS: server.1=zookeeper-1:2888:3888 server.2=zookeeper-2:2888:3888 server.3=zookeeper-3:2888:3888  # 配置 Zookeeper 集群中所有节点的信息

    networks:
      - kafka-network

  zookeeper-2:
    image: wurstmeister/zookeeper:3.4.6
    container_name: zookeeper-2
    hostname: zookeeper-2
    ports:
      - "2182:2181"  # 将 Zookeeper 服务端口映射到主机端口

    environment:
      ZOO_MY_ID: 2  # 设置当前 Zookeeper 实例的 ID
      ZOO_SERVERS: server.1=zookeeper-1:2888:3888 server.2=zookeeper-2:2888:3888 server.3=zookeeper-3:2888:3888  # 配置 Zookeeper 集群中所有节点的信息

    networks:
      - kafka-network

  zookeeper-3:
    image: wurstmeister/zookeeper:3.4.6
    container_name: zookeeper-3
    hostname: zookeeper-3
    ports:
      - "2183:2181"  # 将 Zookeeper 服务端口映射到主机端口

    environment:
      ZOO_MY_ID: 3  # 设置当前 Zookeeper 实例的 ID
      ZOO_SERVERS: server.1=zookeeper-1:2888:3888 server.2=zookeeper-2:2888:3888 server.3=zookeeper-3:2888:3888  # 配置 Zookeeper 集群中所有节点的信息

    networks:
      - kafka-network

  kafka-1:
    image: wurstmeister/kafka:2.13-2.8.1
    container_name: kafka-1
    hostname: kafka-1
    ports:
      - "9092:9092"  # 将 Kafka 服务端口映射到主机端口

    environment:
      KAFKA_BROKER_ID: 1  # 设置当前 Kafka Broker 的 ID
      KAFKA_LISTENERS: PLAINTEXT://:9092  # 配置 Kafka 监听器
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka-1:9092  # 配置 Kafka 广告监听器，用于外部访问
      KAFKA_ZOOKEEPER_CONNECT: zookeeper-1:2181,zookeeper-2:2182,zookeeper-3:2183  # 指定 Kafka 连接到的 Zookeeper 集群
      KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT  # 设置 Kafka 内部 Broker 间通信使用的监听器
      KAFKA_AUTO_CREATE_TOPICS_ENABLE: "true"  # 允许自动创建主题

    depends_on:
      - zookeeper-1
      - zookeeper-2
      - zookeeper-3

    networks:
      - kafka-network

  kafka-2:
    image: wurstmeister/kafka:2.13-2.8.1
    container_name: kafka-2
    hostname: kafka-2
    ports:
      - "9093:9092"  # 将 Kafka 服务端口映射到主机端口

    environment:
      KAFKA_BROKER_ID: 2  # 设置当前 Kafka Broker 的 ID
      KAFKA_LISTENERS: PLAINTEXT://:9092  # 配置 Kafka 监听器
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka-2:9092  # 配置 Kafka 广告监听器，用于外部访问
      KAFKA_ZOOKEEPER_CONNECT: zookeeper-1:2181,zookeeper-2:2182,zookeeper-3:2183  # 指定 Kafka 连接到的 Zookeeper 集群
      KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT  # 设置 Kafka 内部 Broker 间通信使用的监听器
      KAFKA_AUTO_CREATE_TOPICS_ENABLE: "true"  # 允许自动创建主题

    depends_on:
      - zookeeper-1
      - zookeeper-2
      - zookeeper-3

    networks:
      - kafka-network

  kafka-3:
    image: wurstmeister/kafka:2.13-2.8.1
    container_name: kafka-3
    hostname: kafka-3
    ports:
      - "9094:9092"  # 将 Kafka 服务端口映射到主机端口

    environment:
      KAFKA_BROKER_ID: 3  # 设置当前 Kafka Broker 的 ID
      KAFKA_LISTENERS: PLAINTEXT://:9092  # 配置 Kafka 监听器
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka-3:9092  # 配置 Kafka 广告监听器，用于外部访问
      KAFKA_ZOOKEEPER_CONNECT: zookeeper-1:2181,zookeeper-2:2182,zookeeper-3:2183  # 指定 Kafka 连接到的 Zookeeper 集群
      KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT  # 设置 Kafka 内部 Broker 间通信使用的监听器
      KAFKA_AUTO_CREATE_TOPICS_ENABLE: "true"  # 允许自动创建主题

    depends_on:
      - zookeeper-1
      - zookeeper-2
      - zookeeper-3

    networks:
      - kafka-network

networks:
  kafka-network:
    driver: bridge
```

### 解释：

- 这个 YAML 文件定义了一个包含三个 Zookeeper 节点和三个 Kafka 节点的 Docker Compose 配置。

- 每个 Zookeeper 节点使用 `wurstmeister/zookeeper` 镜像，每个 Kafka 节点使用 `wurstmeister/kafka` 镜像。

- Zookeeper 节点配置了唯一的 `ZOO_MY_ID`，并通过 `ZOO_SERVERS` 变量指定了集群中的其他 Zookeeper 节点。

- Kafka 节点配置了唯一的 `KAFKA_BROKER_ID`，`KAFKA_LISTENERS` 和 `KAFKA_ADVERTISED_LISTENERS` 用于指定监听的地址和端口。`KAFKA_ZOOKEEPER_CONNECT` 用于连接到 Zookeeper 集群。

- 每个 Kafka 节点都依赖于对应的 Zookeeper 节点。

- 所有节点都连接到名为 `kafka-network` 的自定义 Docker 网络。

### 详细说明

以上是一个 Kafka 集群和一个 Zookeeper 集群的配置。这个配置是一个典型的 Kafka 高可用部署，因为 Kafka 依赖于 Zookeeper 来实现元数据的存储和管理。

具体来说：
- **Zookeeper 集群**：
  - `zookeeper-1`, `zookeeper-2`, `zookeeper-3` 是三个 Zookeeper 实例，组成了一个 Zookeeper 集群。
  - 每个 Zookeeper 实例都有一个独立的 ID (`ZOO_MY_ID`)，并且在 `ZOO_SERVERS` 中指定了所有 Zookeeper 节点的地址和通信端口。
  - 这种部署方式可以确保 Zookeeper 服务的高可用性和容错性。

- **Kafka 集群**：
  - `kafka-1`, `kafka-2`, `kafka-3` 是三个 Kafka Broker 实例，组成了一个 Kafka 集群。
  - 每个 Kafka Broker 配置了自己的 Broker ID (`KAFKA_BROKER_ID`) 和监听器 (`KAFKA_LISTENERS`)。
  - 在 `KAFKA_ADVERTISED_LISTENERS` 中设置了广告监听器，用于外部客户端访问。
  - `KAFKA_ZOOKEEPER_CONNECT` 指定了 Kafka 连接到的 Zookeeper 集群，以便存储元数据、协调分区分配和故障恢复。
  - `KAFKA_INTER_BROKER_LISTENER_NAME` 设置了 Broker 间通信使用的监听器。
  - `KAFKA_AUTO_CREATE_TOPICS_ENABLE` 允许自动创建主题，方便管理。

综上所述，这个配置提供了一个具有高可用性和容错性的 Kafka 集群。Zookeeper 集群的高可用性保证了 Kafka 的元数据管理和协调工作，而 Kafka 集群的多个 Broker 提供了数据分布和容错。这种部署方式使得 Kafka 集群能够继续工作，即使其中的某些节点发生故障。

因此，您可以使用这个配置来构建一个具有高可用性和可靠性的 Kafka 集群。
