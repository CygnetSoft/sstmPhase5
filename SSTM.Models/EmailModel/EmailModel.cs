namespace SSTM.Models.EmailModel
{
    public class EmailModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Attachments { get; set; }
        public string SentMailDate { get; set; }
        public string ClientIp { get; set; }

        public bool isReadReceipt { get; set; }

        public string SMTPEmail { get; set; }
        public string SMTPPassword { get; set; }
        public string SMTPHost { get; set; }
        public string SMTPPort { get; set; }
        public string EnableSsl { get; set; }
    }
}