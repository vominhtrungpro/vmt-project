using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Contracts;
using vmt_project.dal.Models.Entities;
using vmt_project.models.Request.Kafka;
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
        public void InsertUserInfoMessage(InsertUserInfoRequestMessage request)
        {
            var userInfo = new UserInfo()
            {
                AvatarUrl = request.AvatarUrl,
                UserId = request.UserId,
            };
            userInfo.SetCreatedInfo(request.CurrentUserId);
            _userInfoRepository.Add(userInfo);
        }
        public void UpdateUserInfoMessage(UpdateUserInfoRequestMessage request)
        {
            var userEntity = _userInfoRepository.FindBy(m => m.User.Id == request.UserId).FirstOrDefault();
            if (userEntity != null)
            {
                userEntity.AvatarUrl = request.AvatarUrl;
                userEntity.SetModifiedInfo(request.CurrentUserId);
                _userInfoRepository.ClearTracker();
                _userInfoRepository.Edit(userEntity);
            }
        }
    }
}
