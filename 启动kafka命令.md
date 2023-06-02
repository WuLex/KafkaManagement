## windows下使用kafka
### 启动zookeeper服务
下载后，打开zookeeper安装目录，进入bin文件夹，双击zkServer.cmd启动zookeeper服务。
http://localhost:5253/Kafka/Topics
http://localhost:5253/Kafka/Status
http://localhost:5253/Kafka/ConsumeMessage
### 启动Kafka
```bash
# kafka-server-start ..\..\config\server.properties
kafka-server-start E:\worksoftware\kafka3.4\config\server.properties
```
### 使用以下命令验证 Kafka 是否正常工作
```shell
kafka-topics.bat --list --bootstrap-server localhost:9092
```
###  创建 Kafka 主题（Topic）
```shell
kafka-topics.bat --create --topic quick_start_topic --bootstrap-server localhost:9092
```
### 默认情况下，您输入的每一行都会导致一个单独的事件被写入主题。
```bash
kafka-console-producer.bat --topic quick_start_topic --bootstrap-server localhost:9092

kafka-console-producer.bat --topic my-topic --bootstrap-server localhost:9092
```
### 打开另一个终端会话并运行控制台消费者客户端以读取刚刚创建的事件：
```bash
kafka-console-consumer.bat --topic quick_start_topic --from-beginning --bootstrap-server localhost:9092

kafka-console-consumer.bat --topic mytopic --from-beginning --bootstrap-server localhost:9092
```
