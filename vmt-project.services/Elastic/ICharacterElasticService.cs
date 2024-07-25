using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;

namespace vmt_project.services.Elastic
{
    public interface ICharacterElasticService
    {
        Task<Character> GetCharacterById(string id);
        Task<bool> BulkInsert(List<Character> characters);
        Task<bool> Insert(Character character);
        Task<List<Character>> GetAll();
    }
}
