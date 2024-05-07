using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Response.Authentication
{
    public class GoogleUserInfoResponse
    {
        public string Sub { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Picture { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string Locale { get; set; }
    }
}
