using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.TrainerQPUpload
{
  public  class TrainerQP_Level_ApprovalModel
    {
        [Key]
        public long id { get; set; }
        public long? QP_Id { get; set; }

        public long? Level1_User_id { get; set; }
        public DateTime? Level1_Approve_date { get; set; }
        public string Level1_Comment { get; set; }
        public string Level1_IsAccept { get; set; }

        public long? Level2_User_id { get; set; }
        public DateTime? Level2_Approve_date { get; set; }
        public string Level2_Comment { get; set; }
        public string Level2_IsAccept { get; set; }

        public long? Level3_User_id { get; set; }
        public DateTime? Level3_Approve_date { get; set; }
        public string Level3_Comment { get; set; }
        public string Level3_IsAccept { get; set; }
    }
}
