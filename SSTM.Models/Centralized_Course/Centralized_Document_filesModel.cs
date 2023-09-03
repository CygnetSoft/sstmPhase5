using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.Centralized_Course
{
    public class Centralized_Document_filesModel
    {
        [Key]
        public string id { get; set; }
        public int? Central_Master_Id { get; set; }
        public string Document_File_Name { get; set; }
        public string Document_Type_Name { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
