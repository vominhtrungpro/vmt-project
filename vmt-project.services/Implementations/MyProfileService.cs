using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Contracts;
using vmt_project.dal.Implementations;
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.Character;
using vmt_project.models.DTO.MyProfile.cs;
using vmt_project.models.Request.MyProfile;
using vmt_project.services.Contracts;

namespace vmt_project.services.Implementations
{
    public class MyProfileService : IMyProfileService
    {
        public readonly IMyProfileRepository _myProfileRepository;
        public readonly IMyProfilePictureRepository _myProfilePictureRepository;
        public MyProfileService(IMyProfileRepository myProfileRepository, IMyProfilePictureRepository myProfilePictureRepository)
        {
            _myProfileRepository = myProfileRepository;
            _myProfilePictureRepository = myProfilePictureRepository;
        }
        public async Task<AppActionResult> Create(CreateMyProfileRequest request)
        {
            var result = new AppActionResult();
            try
            {
                var myProfile = new MyProfile()
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Slug = request.Slug,
                    Order = request.Order,
                    Content = request.Content
                };
                myProfile.SetCreatedInfo("");
                _myProfileRepository.Add(myProfile);
                if (request.PictureUrls.Count > 0)
                {
                    foreach (var pictureUrl in request.PictureUrls)
                    {
                        var myProfilePicture = new MyProfilePicture()
                        {
                            Id = Guid.NewGuid(),
                            PictureUrl = pictureUrl,
                            MyProfileId = myProfile.Id,
                        };
                        myProfilePicture.SetCreatedInfo("");
                        _myProfilePictureRepository.Add(myProfilePicture);
                    }
                }
                return result.BuildResult("Success!");
            }
            catch (Exception e)
            {
                return result.BuildError(e.Message);
            }
        }
        public async Task<AppActionResult> Update(UpdateMyProfileRequest request)
        {
            var result = new AppActionResult();
            try
            {
                var myProfileEntity = _myProfileRepository.FindBy(m => m.Id == request.Id).FirstOrDefault();
                if (myProfileEntity == null)
                {
                    return result.BuildResult("Not found!");
                }
                else
                {
                    myProfileEntity.Name = request.Name;
                    myProfileEntity.Slug = request.Slug;
                    myProfileEntity.Content = request.Content;
                    myProfileEntity.Order = request.Order;
                    myProfileEntity.SetModifiedInfo("");
                    _myProfileRepository.Edit(myProfileEntity);
                    if (request.PictureUrls.Count > 0)
                    {
                        foreach (var pictureUrl in request.PictureUrls)
                        {
                            var myProfilePicture = new MyProfilePicture()
                            {
                                Id = Guid.NewGuid(),
                                PictureUrl = pictureUrl,
                                MyProfileId = myProfileEntity.Id,
                            };
                            myProfilePicture.SetCreatedInfo("");
                            _myProfilePictureRepository.DeleteRange(_myProfilePictureRepository.FindBy(m => m.MyProfileId == myProfileEntity.Id).ToList());
                            _myProfilePictureRepository.Add(myProfilePicture);
                        }
                    }
                    return result.BuildResult("Success!");
                }
            }
            catch (Exception e)
            {
                return result.BuildError(e.Message);
            }
        }
        public async Task<AppActionResultData<List<MyProfileDto>>> List()
        {
            var result = new AppActionResultData<List<MyProfileDto>>();
            try
            {
                var list = _myProfileRepository
                    .GetAll()
                    .OrderBy(m => m.Order)
                    .Select(m => new MyProfileDto
                    {
                        Id = m.Id,
                        Slug = m.Slug,
                        Name = m.Name,
                        Content = m.Content,
                        Order = m.Order,
                        PictureUrls = m.MyProfilePictures.Select(p => p.PictureUrl).ToList()
                    })
                    .ToList();

                return result.BuildResult(list);
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message);
            }
        }
    }
}
