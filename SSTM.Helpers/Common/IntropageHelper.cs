using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Helpers.Common
{
    public  class IntropageHelper
    {
        public static string ApiBaseUrl = ConfigurationManager.AppSettings["li_ApiServices"];
        public static string Exist_ApiBaseUrl = ConfigurationManager.AppSettings["li_Exist_ApiServices"];
        public static string Push_Noti_ApiBaseUrl = ConfigurationManager.AppSettings["Cloud_Notification_ApiServices"];
        public static string Sql_Connection = ConfigurationManager.ConnectionStrings["SSTMDbContext"].ConnectionString;
        public static string Singapore_Code = "+65";
    }
}
