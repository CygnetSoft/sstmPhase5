using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.Role
{
    public class RoleModel
    {
        [Key]
        public long Id { get; set; }

        public string RoleName { get; set; }

        public bool isActive { get; set; }
        public bool isDeleted { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public long? UpdatedBy { get; set; }

        public string HomePage { get; set; }
    }
}