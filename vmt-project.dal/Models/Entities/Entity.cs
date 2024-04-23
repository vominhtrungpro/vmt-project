using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.dal.Models.Entities
{
    public abstract class Entity
    {
        [Key]
        public virtual Guid Id { get; set; }

        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime? ModifiedOn { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string? ModifiedBy { get; set; }

        public virtual void SetCreatedInfo(string userId)
        {
            CreatedOn = DateTime.UtcNow;
            CreatedBy = userId;
        }

        public virtual void SetModifiedInfo(string userId)
        {
            ModifiedOn = DateTime.UtcNow;
            ModifiedBy = userId;
        }
    }
}
