namespace SSTM.Models.Zoho
{
    public class ZohoResponseModel
    {
        public string session_delete_url { get; set; }
        public string save_url { get; set; }
        public string session_id { get; set; }
        public string document_delete_url { get; set; }
        public string document_id { get; set; }
        public string document_url { get; set; }

        public string code { get; set; }
        public string message { get; set; }
    }
}