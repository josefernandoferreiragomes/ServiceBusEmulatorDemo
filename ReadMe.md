## Steps

### Build emulator with Docker  

```bash
docker-compose build
```

### Run emulator with Docker (detached mode)

```bash
docker-compose up
```

Will present a log sequence such as:
```bash
publisher-demo  | A batch of 3 messages has been published to the topic.1. topic, for subscription.3 (no filter)
publisher-demo  | A batch of 3 messages has been published to the topic.1. topic, for subscription.2 filter
publisher-demo  | A batch of 3 messages has been published to the topic.1. topic, for subscription.1 filter
publisher-demo  | A batch of 3 messages has been published to the queue.1 queue.
publisher-demo  | Publisher completed
```

```bash
diagnostic-tool  | Peek topic.1 messages for subscription subscription.1
diagnostic-tool  | Peek topic.1 subscription.1 Active Messages:
diagnostic-tool  | - MessageId: msgid1
diagnostic-tool  |   Body: Hello Subscription 1!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: msgid1
diagnostic-tool  |   Body: Hello Subscription 1!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: msgid1
diagnostic-tool  |   Body: Hello Subscription 1!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: msgid1
diagnostic-tool  |   Body: Hello Subscription 1!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:28:35 +00:00
diagnostic-tool  | - MessageId: msgid1
diagnostic-tool  |   Body: Hello Subscription 1!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:28:35 +00:00
diagnostic-tool  | - MessageId: msgid1
diagnostic-tool  |   Body: Hello Subscription 1!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:28:35 +00:00
diagnostic-tool  | 🔸  topic.1 subscription.1 Dead-letter Messages:
diagnostic-tool  |
diagnostic-tool  | Peek topic.1 messages for subscription subscription.2
diagnostic-tool  | Peek topic.1 subscription.2 Active Messages:
diagnostic-tool  | - MessageId: 99830ba9dd314f3d810ed020919cd187
diagnostic-tool  |   Body: Hello Subscription 2!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: c9ea28fee6f14e70bba11491971c91de
diagnostic-tool  |   Body: Hello Subscription 2!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: 0798fe1a017d4ef7a37f113e243d246f
diagnostic-tool  |   Body: Hello Subscription 2!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: 8d66baf3112b4973841be6ccda402fa5
diagnostic-tool  |   Body: Hello Subscription 2!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:28:35 +00:00
diagnostic-tool  | - MessageId: 4f178731b39b4e1f963722473d0ed831
diagnostic-tool  |   Body: Hello Subscription 2!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:28:35 +00:00
diagnostic-tool  | - MessageId: 500f96a400fd4ad2a1db56541378357f
diagnostic-tool  |   Body: Hello Subscription 2!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:28:35 +00:00
diagnostic-tool  | 🔸  topic.1 subscription.2 Dead-letter Messages:
diagnostic-tool  |
diagnostic-tool  | Peek topic.1 messages for subscription subscription.3
diagnostic-tool  | Peek topic.1 subscription.3 Active Messages:
diagnostic-tool  | - MessageId: 7215a39fa4d347d0a52a4668371d2a38
diagnostic-tool  |   Body: Message 1
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: 860bd650f9154983acf75084fed1eed6
diagnostic-tool  |   Body: Message 2
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: 30af8af410e54af89a87d17afd37df2d
diagnostic-tool  |   Body: Message 3
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: 99830ba9dd314f3d810ed020919cd187
diagnostic-tool  |   Body: Hello Subscription 2!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: c9ea28fee6f14e70bba11491971c91de
diagnostic-tool  |   Body: Hello Subscription 2!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | - MessageId: 0798fe1a017d4ef7a37f113e243d246f
diagnostic-tool  |   Body: Hello Subscription 2!
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:07 +00:00
diagnostic-tool  | 🔸  topic.1 subscription.3 Dead-letter Messages:
diagnostic-tool
diagnostic-tool  | Peek queue queue.1 messages
diagnostic-tool  | Peek 🔹queue.1 Active Messages:
diagnostic-tool  | - MessageId: 62975659a5fa47f69e3b4601d6d26576
diagnostic-tool  |   Body: Message 1
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:23 +00:00
diagnostic-tool  | - MessageId: 26f5fb86230a4312839714938fc84707
diagnostic-tool  |   Body: Message 2
diagnostic-tool  |   EnqueuedTime: 10/04/2025 15:27:23 +00:00
diagnostic-tool  | - MessageId: 0eab3b86c5be4249a077610f271cbfa5
```

```bash
subscriber-demo  | Try to consume messages...
subscriber-demo  | messageFromTopic receiver started
subscriber-demo  | messageFromTopic message received...
subscriber-demo  | messageFromTopic Received: Hello Subscription 1!
subscriber-demo  |
subscriber-demo  | Try to consume messages...
subscriber-demo  | messageFromTopic receiver started
subscriber-demo  | messageFromTopic message received...
subscriber-demo  | messageFromTopic Received: Hello Subscription 2!
subscriber-demo  |
subscriber-demo  | Try to consume messages...
subscriber-demo  | messageFromTopic receiver started
subscriber-demo  | messageFromTopic message received...
subscriber-demo  | messageFromTopic Received: Message 1
subscriber-demo  |
subscriber-demo  | messageFromQueue receiver started
subscriber-demo  | messageFromQueue message received...
subscriber-demo  |  messageFromQueue Received: Message 1

```

### Troubleshooting

Go to command prompt and run the command below to check if port 5672 is listening:
```bash
netstat -an | findstr :5672
```

Should include an output like this:
```text
  TCP    127.0.0.1:5672    LISTENING
```

### Querying the emulator database:

Through sql server management studio / Visual studio Sql Server Object Explorer, using a connection like:
```bash
Data Source=localhost,1433;Persist Security Info=False;User ID=sa;Password=p@ssw0rd123;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name="SQL Server Management Studio";Command Timeout=30
```
Check trust server certificate is enabled.

Queries.sql file has sql queries to verify the messages are being sent and received.

## References

https://www.youtube.com/watch?v=7BbymqxkJKY

https://github.com/Azure/azure-service-bus-emulator-installer?tab=readme-ov-file

https://github.com/alex-wolf-ps/service-bus-emulator

https://learn.microsoft.com/en-us/azure/service-bus-messaging/test-locally-with-service-bus-emulator?tabs=docker-linux-container

## Future steps

### Add an azure function to act as publisher
Invoke the publisher with a curl
```bash
curl -v "http://localhost:5025/api/Function1?code=KEY"
```

### Add an azure function to act as subscriber