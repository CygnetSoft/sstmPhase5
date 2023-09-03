using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.Config
{
    public class ConfigModel
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Profile name cannot be blank.")]
        public string AWSProfileName { get; set; }

        [Required(ErrorMessage = "Access key cannot be blank.")]
        public string AWSAccessKey { get; set; }

        [Required(ErrorMessage = "Secret key cannot be blank.")]
        public string AWSSecretKey { get; set; }

        [Required(ErrorMessage = "Bucket name cannot be blank.")]
        public string BucketName { get; set; }

        [Required(ErrorMessage = "Email cannot be blank.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password cannot be blank.")]
        public string Pass { get; set; }

        [Required(ErrorMessage = "Port cannot be blank.")]
        public string Port { get; set; }

        [Required(ErrorMessage = "Host cannot be blank.")]
        public string Host { get; set; }

        public string EnableSsl { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Zoho api key cannot be blank.")]
        public string ZohoApiKey { get; set; }
    }
}