namespace SSTM.Helpers.App
{
    public class AppSession
    {
        public long UserId { get; set; }

        public string EncryptedUserId { get; set; }
        public string TrainingCenterName { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string UserEmail { get; set; }

        public long CourseId { get; set; }
        public long BatchId { get; set; }

        public long CurrentCourseId { get; set; }
        public long CurrentBatchId { get; set; }

        public long TrainerCourseId { get; set; }
        public long TrainerBatchId { get; set; }

        public bool isOTPVerified { get; set; }
        public bool isLocationVerified { get; set; } = false;
        public long? Trainer_AirLine_id { get; set; }
        public string Macaddress { get; set; }
        public string Photo { get; set; }
    }
}