using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.dal.Models.Entities
{
    public class MyProfilePicture : Entity
    {
        public Guid MyProfileId { get; set; }

        [ForeignKey("MyProfileId")]
        public virtual MyProfile? MyProfile { get; set; }
        public string PictureUrl { get; set; }
    }
}
