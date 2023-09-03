using System;

namespace SSTM.Core.Config
{
    public class Config
    {
        public long Id { get; set; }

        public string AWSProfileName { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string BucketName { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Port { get; set; }
        public string Host { get; set; }
        public string EnableSsl { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public bool IsDeleted { get; set; }
        public string ZohoApiKey { get; set; }
    }
}