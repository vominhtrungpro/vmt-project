using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.models.Request.Kafka;

namespace vmt_project.services.Contracts
{
    public interface IKafkaService
    {
        void InsertUserInfoMessage(InsertUserInfoRequestMessage request);
        void UpdateUserInfoMessage(UpdateUserInfoRequestMessage request);
    }
}
