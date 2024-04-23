using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Model.Config
{
    public class JwtConfig
    {
        public double AccessTokenExpiresIn { get; set; }
        public double RefreshTokenExpiresIn { get; set; }
        public string? Secret { get; set; }
        public string? ValidIssuer { get; set; }
        public string? ValidAudience { get; set; }
    }
}
