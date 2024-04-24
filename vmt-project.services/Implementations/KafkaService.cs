using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Contracts;
using vmt_project.dal.Models.Entities;
using vmt_project.models.Request.UserInfo;
using vmt_project.services.Contracts;

namespace vmt_project.services.Implementations
{
    public class KafkaService : IKafkaService
    {
        private readonly IUserInfoRepository _userInfoRepository;
        public KafkaService(IUserInfoRepository userInfoRepository)
        {
                    _userInfoRepository = userInfoRepository;
        }
        public void InsertUserInfoMessage(UserInfo request)
        {
            _userInfoRepository.ClearTracker();
            _userInfoRepository.Add(request);
        }
        public void UpdateUserInfoMessage(UserInfo request)
        {
            _userInfoRepository.ClearTracker();
            _userInfoRepository.Edit(request);
        }
    }
}
