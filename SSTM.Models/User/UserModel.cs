using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.User
{
    public class UserModel
    {
        [Key]
        public long Id { get; set; }

        //[Required(ErrorMessage = "Please select valid Training Center for the user.")]
        public long? TrainingCenterId { get; set; }

        [Required(ErrorMessage = "Please select valid Role for the user.")]
        public long? RoleId { get; set; }

        [Required(ErrorMessage = "First name cannot be blank.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name cannot be blank.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Mobile number cannot be blank."), MaxLength(10), MinLength(8, ErrorMessage = "Please insert valid mobile number.")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Email address cannot be blank."), EmailAddress(ErrorMessage = "Please insert valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password cannot be blank."), MinLength(8, ErrorMessage = "Password length must be greater than 8 character.")]
        public string Password { get; set; }
        public string OTP { get; set; }

        [Required(ErrorMessage = "Mac Address can not be blank.")]
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