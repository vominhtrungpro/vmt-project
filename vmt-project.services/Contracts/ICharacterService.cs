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
        Task<AppActionResultData<CharacterDto>> Get_Dapper(Guid id);
        Task<AppActionResult> Create(CreateCharacterRequest request);
        Task<AppActionResult> Create_Dapper(CreateCharacterRequest request);
        Task<AppActionResult> Update_Dapper(UpdateCharacterRequest request, Guid id);
        Task<AppActionResultData<List<CharacterDto>>> List();
        Task<AppActionResultData<SearchCharacterResult>> Search(SearchCharacterRequest request);
        Task<AppActionResult> Delete_Dapper(Guid id);


    }
}
