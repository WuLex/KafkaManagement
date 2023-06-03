## windows下使用kafka
### 启动zookeeper服务
下载后，打开zookeeper安装目录，进入bin文件夹，双击zkServer.cmd启动zookeeper服务。
http://localhost:5253/Kafka/Topics
http://localhost:5253/Kafka/Status
http://localhost:5253/Kafka/ConsumeMessage

### 启动Zookeeper
```bash
zkServer
```
### 启动Kafka
```bash
# kafka-server-start ..\..\config\server.properties
kafka-server-start E:\worksoftware\kafka3.4\config\server.properties
```
### 查看目前Kafka中的主题,验证 Kafka 是否正常工作,
```shell
kafka-topics.bat --list --bootstrap-server localhost:9092
```
### 创建 Kafka 主题（Topic）
```shell
kafka-topics.bat --create --topic quick_start_topic --bootstrap-server localhost:9092

#创建一个1个分区1个副本的topic: benchmark
kafka-topics.bat --bootstrap-server localhost:9092 --create --topic benchmark --partitions 1 --replication-factor 1
```
### 消息基准测试
```shell

#生产消息基准测试
#在生产环境中，推荐使用生产5000W消息，这样会性能数据会更准确些。为了方便测试，课程上演示测试100W的消息作为基准测试。
kafka-producer-perf-test.bat --topic benchmark --num-records 1000000 --throughput -1 --record-size 1000 --producer-props bootstrap.servers=localhost:9092 acks=1

#消费消息基准测试
kafka-consumer-perf-test.bat --broker-list localhost:9092 --topic benchmark --fetch-size 1048576 --messages 1000000
```

### 默认情况下，您输入的每一行都会导致一个单独的事件被写入主题。
```bash
kafka-console-producer.bat --topic quick_start_topic --bootstrap-server localhost:9092

kafka-console-producer.bat --topic mytopic --bootstrap-server localhost:9092
```
### 打开另一个终端会话并运行控制台消费者客户端以读取刚刚创建的事件：
```bash
kafka-console-consumer.bat --topic quick_start_topic --from-beginning --bootstrap-server localhost:9092

kafka-console-consumer.bat --topic mytopic --from-beginning --bootstrap-server localhost:9092
```
