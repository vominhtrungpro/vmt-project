using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.services.Kafka.Base
{
    public class GenericKafkaService : IGenericKafkaService
    {
        private readonly IConfiguration _configuration;
        private readonly IProducer<Null, string> _producer;
        public GenericKafkaService(IConfiguration configuration)
        {
            _configuration = configuration;
            var producerconfig = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KafkaBootstrapServers")
            };
            _producer = new ProducerBuilder<Null, string>(producerconfig).Build();
        }
        public async Task ProduceAsync(string topic, string message)
        {
            var kafkamessage = new Message<Null, string> { Value = message, };

            await _producer.ProduceAsync(topic, kafkamessage);
        }
    }
}
