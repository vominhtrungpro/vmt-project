using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Search : BackgroundService
{
    public Search()
    {
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

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
                                _kafkaService.InsertUserInfoMessage(JsonConvert.DeserializeObject<InsertUserInfoRequestMessage>(response.Message.Value));
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
                                _kafkaService.UpdateUserInfoMessage(JsonConvert.DeserializeObject<UpdateUserInfoRequestMessage>(response.Message.Value));
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
