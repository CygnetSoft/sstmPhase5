using System;

namespace SSTM.Core.User
{
    public class User
    {
        public long Id { get; set; }
        public long? TrainingCenterId { get; set; }
        public long? RoleId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OTP { get; set; }
        public string MacAddress { get; set; }
        public string MacAddress1 { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public long? Trainer_AirLine_id { get; set; }
       
    }
}