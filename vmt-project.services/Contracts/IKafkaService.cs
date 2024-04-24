using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;

namespace vmt_project.services.Contracts
{
    public interface IKafkaService
    {
        void InsertUserInfoMessage(UserInfo request);
        void UpdateUserInfoMessage(UserInfo request);
    }
}
