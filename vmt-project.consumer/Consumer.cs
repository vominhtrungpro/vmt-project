using Confluent.Kafka.Admin;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

public class Consumer : BackgroundService
{
    private readonly IUserInfoService _userInfoService;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "weather-consumer-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Null, string>(config).Build();

        var adminClientConfig = new AdminClientConfig { BootstrapServers = "localhost:9092" };
        using (var adminClient = new AdminClientBuilder(adminClientConfig).Build())
        {
            var existingTopics = adminClient.GetMetadata(TimeSpan.FromSeconds(10)).Topics;

            var topicsToSubscribe = new List<string> { "upsert-user-info" };

            foreach (var topicName in topicsToSubscribe)
            {
                var topicExists = existingTopics.Any(topic => topic.Topic == topicName);

                if (!topicExists)
                {
                    List<TopicSpecification> topics = new List<TopicSpecification>
                    {
                        new TopicSpecification { Name = topicName, NumPartitions = 1, ReplicationFactor = 1 }
                    };

                    adminClient.CreateTopicsAsync(topics).GetAwaiter().GetResult();

                    Console.WriteLine($"Topic {topicName} created successfully.");
                }
                else
                {
                    Console.WriteLine($"Topic {topicName} already exists.");
                }
            }

            consumer.Subscribe(topicsToSubscribe);
        }

        CancellationTokenSource token = new();

        try
        {
            while (true)
            {
                var response = consumer.Consume(token.Token);
                if (response.Message != null)
                {
                    Console.WriteLine($"Message: {response.Message.Value}, Topic: {response.Topic}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Đã hủy bỏ token
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex}");
        }
    }
}
