using System;

namespace SSTM.Core.ExceptionLog
{
    public class ExceptionLog
    {
        public long Id { get; set; }

        public string Message { get; set; }
        public string InnerException { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string Exception { get; set; }
        public string Controller { get; set; }

        public long? UserId { get; set; }

        public string URL { get; set; }
        public string ActionName { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? Createdby { get; set; }

        public string IPAddress { get; set; }
    }
}