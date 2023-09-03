namespace SSTM.Models.TrainingCenter
{
    public class TrainingCenterDataGridModel
    {
        public string Name { get; set; }
        public string PhysicalAddress { get; set; }
        public string PostalCode { get; set; }
        public string Status { get { return string.Format("<label class='badge badge-success'>{0}</label>"); } }
        public string Action { get; set; }
    }
}