using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Base;
using vmt_project.dal.Contracts;
using vmt_project.dal.Models.Context;
using vmt_project.dal.Models.Entities;

namespace vmt_project.dal.Implementations
{
    public class CharacterRepository : GenericRepository<Character>, ICharacterRepository
    {
        public CharacterRepository(VmtDbContext dbContext) : base(dbContext)
        {
            _context = DbContext;
        }
    }
}
