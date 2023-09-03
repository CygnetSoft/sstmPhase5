using System.IO;

namespace SSTM.Helpers.Model
{
    public class AWSModel
    {
        public string AccessKey { get; set; }
        public string SecreteKey { get; set; }
        public string BucketName { get; set; }
        public string BucketDirectory { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public Stream LocalFileStream { get; set; }
    }
}