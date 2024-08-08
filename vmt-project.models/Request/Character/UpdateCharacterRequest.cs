using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Request.Character
{
    public class UpdateCharacterRequest
    {
        public string Name { get; set; }
        public bool IsUsingDapper { get; set; }
    }
}
