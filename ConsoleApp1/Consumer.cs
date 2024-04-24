using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Consumer : BackgroundService
    {
        public Consumer()
        {

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
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

                        var topicExists = existingTopics.Any(topic => topic.Topic == "weather-topic");

                        if (!topicExists)
                        {
                            // Tạo danh sách chứa thông tin chủ đề mới
                            List<TopicSpecification> topics = new List<TopicSpecification>
            {
                new TopicSpecification { Name = "weather-topic", NumPartitions = 1, ReplicationFactor = 1 }
            };

                            // Tạo chủ đề mới
                            adminClient.CreateTopicsAsync(topics).GetAwaiter().GetResult();

                            Console.WriteLine("Topic created successfully.");
                        }

                        Console.WriteLine("Topic exist!.");
                    }


                    consumer.Subscribe("weather-topic");

                    CancellationTokenSource token = new();

                    try
                    {
                        while (true)
                        {
                            var response = consumer.Consume(token.Token);
                            if (response.Message != null)
                            {
                                Console.WriteLine($"Message: {response.Message.Value}");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex}");
                }
            }
        }
    }
}
