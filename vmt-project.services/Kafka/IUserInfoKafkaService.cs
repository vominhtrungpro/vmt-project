using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.models.Request.UserInfo;

namespace vmt_project.services.Kafka
{
    public interface IUserInfoKafkaService
    {
        void PublishInsertUserInfo(UserInfo request);
        void PublishUpdateUserInfo(UserInfo request);
    }
}
