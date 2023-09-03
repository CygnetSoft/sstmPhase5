using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.TrainingCenter
{
    public class TrainingCenterModel
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Name cannot be blank.")]
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        [Required(ErrorMessage ="Network IP is mandatory.")]
        public string NetworkIP { get; set; }

        public bool isActive { get; set; }
        public bool isDeleted { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public long? UpdatedBy { get; set; }
    }
}