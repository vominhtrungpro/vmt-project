using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.User;
using vmt_project.models.Request.Kafka;
using vmt_project.models.Request.UserInfo;
using vmt_project.services.Kafka.Base;

namespace vmt_project.services.Kafka
{
    public class UserInfoKafkaService : IUserInfoKafkaService
    {
        private readonly IGenericKafkaService _kafka;
        public UserInfoKafkaService(IGenericKafkaService kafka)
        {
            _kafka = kafka;
        }
        public void PublishInsertUserInfo(InsertUserInfoRequestMessage request)
        {
            _kafka.ProduceAsync("insert-user-info", JsonConvert.SerializeObject(request));
        }
        public void PublishUpdateUserInfo(UpdateUserInfoRequestMessage request)
        {
            _kafka.ProduceAsync("update-user-info", JsonConvert.SerializeObject(request));
        }
    }
}
