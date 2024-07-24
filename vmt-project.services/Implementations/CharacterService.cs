using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Contracts;
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.Character;
using vmt_project.models.Request.Character;
using vmt_project.services.Contracts;
using static vmt_project.common.Constants.ResponseMessages;

namespace vmt_project.services.Implementations
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        public CharacterService(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }
        public async Task<AppActionResult> Create(CreateCharacterRequest request)
        {
            var result = new AppActionResult();
            try
            {
                var character = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name
                };
                character.SetCreatedInfo("Admin");
                character.SetModifiedInfo("Admin");
                _characterRepository.Add(character);
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.Message);
            }
            return result.BuildResult(SUCCESS_CREATE_CHARACTER);
        }
        public async Task<AppActionResultData<List<CharacterDto>>> List()
        {
            var result = new AppActionResultData<List<CharacterDto>>();
            try
            {
                var list = _characterRepository
                    .GetAll()
                    .OrderByDescending(m => m.CreatedOn)
                    .Select(m => new CharacterDto
                    {
                        Id = m.Id,
                        Name = m.Name,
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
