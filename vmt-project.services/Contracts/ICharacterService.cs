using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.models.DTO.Character;
using vmt_project.models.Request.Character;
using vmt_project.models.Response.Character;

namespace vmt_project.services.Contracts
{
    public interface ICharacterService
    {
        Task<AppActionResult> Create(CreateCharacterRequest request);
        Task<AppActionResultData<List<CharacterDto>>> List();
        Task<AppActionResultData<SearchCharacterResult>> Search(SearchCharacterRequest request);
        Task<AppActionResult> DapperCreate(CreateCharacterRequest request);
    }
}
