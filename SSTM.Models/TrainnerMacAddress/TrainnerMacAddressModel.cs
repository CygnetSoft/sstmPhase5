using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.TrainnerMacAddress
{
   public class TrainnerMacAddressModel
    {
        [Key]
        public long Id { get; set; }
        public string MacAddress { get; set; }
    }
}
