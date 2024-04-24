using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.services.Kafka.Base
{
    public interface IGenericKafkaService
    {
        Task ProduceAsync(string topic, string message);
    }
}
