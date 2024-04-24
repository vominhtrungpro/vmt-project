using Azure.Core;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using vmt_project.dal.Models.Entities;
using vmt_project.models.Request.UserInfo;
using vmt_project.services.Contracts;

public class Consumer : BackgroundService
{
    private readonly IKafkaService _kafkaService;
    private readonly List<string> _topics;
    public Consumer(IKafkaService kafkaService)
    {
        _kafkaService = kafkaService;
        _topics = new List<string>() { "insert-user-info", "update-user-info", "weather" };
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "vmt-consumer-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Null, string>(config).Build();

        var adminClientConfig = new AdminClientConfig { BootstrapServers = "localhost:9092" };
        using (var adminClient = new AdminClientBuilder(adminClientConfig).Build())
        {
            var existingTopics = adminClient.GetMetadata(TimeSpan.FromSeconds(10)).Topics;

            foreach (var topicName in _topics)
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

            consumer.Subscribe(_topics);
        }

        CancellationTokenSource token = new();

        try
        {
            while (true)
            {
                var response = consumer.Consume(token.Token);
                if (response.Message != null)
                {
                    switch (response.Topic)
                    {
                        case "insert-user-info":
                            try
                            {
                                _kafkaService.InsertUserInfoMessage(JsonConvert.DeserializeObject<UserInfo>(response.Message.Value));
                                Console.WriteLine($"Success: Message: {response.Message.Value}, Topic: {response.Topic}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex}");
                                continue;
                            }
                            
                            continue;
                        case "update-user-info":
                            try
                            {
                                _kafkaService.UpdateUserInfoMessage(JsonConvert.DeserializeObject<UserInfo>(response.Message.Value));
                                Console.WriteLine($"Success: Message: {response.Message.Value}, Topic: {response.Topic}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex}");
                                continue;
                            }

                            continue;
                    }

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
