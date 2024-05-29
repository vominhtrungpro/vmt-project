using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.dal.Models.Entities
{
    public class MyProfile : Entity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public virtual ICollection<MyProfilePicture> MyProfilePictures { get; set; }
    }
}
