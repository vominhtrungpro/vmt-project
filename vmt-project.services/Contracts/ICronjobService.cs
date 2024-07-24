using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;

namespace vmt_project.services.Contracts
{
    public interface ICronjobService
    {
        int CountUser();
        Task<List<Character>> ElasticSearchCharacter(DateTime time);
    }
}
