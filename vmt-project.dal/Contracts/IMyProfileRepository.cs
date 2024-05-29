using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Base;
using vmt_project.dal.Models.Entities;

namespace vmt_project.dal.Contracts
{
    public interface IMyProfileRepository : IGenericRepository<MyProfile>
    {
    }
}
