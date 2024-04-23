using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.models.Model.Config;

namespace vmt_project.services.Contracts
{
    public interface IConfigurationService
    {
        JwtConfig GetJwtConfig();
    }
}
