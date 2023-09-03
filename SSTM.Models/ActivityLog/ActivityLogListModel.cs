namespace SSTM.Models.ActivityLog
{
    public class ActivityLogListModel
    {
        public long Id { get; set; }

        public string Date { get; set; }
        public string User { get; set; }
        public string Duration { get; set; }
    }
}