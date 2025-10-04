## Steps

### Run With Docker

```bash
docker-compose build --no-cache
docker-compose up
```

### Run emulator with Docker (detached mode)

```bash
docker-compose up -d
```
### Start publisher and consumer from Visual Studio

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

Through sql server management studio, using a connection like:
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