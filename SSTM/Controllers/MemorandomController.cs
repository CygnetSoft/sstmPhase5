using SSTM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SSTM.CourseService;
using SSTM.sg.com.eversafe.li;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using SSTM.Filters;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class MemorandomController : Controller
    {
        // GET: Memorandom
        public ActionResult Index()
        {
            CourseService.SSTM service = new SSTM.CourseService.SSTM();
            string data = service.MemorandomPath();
            List<MemorandomModel> model = (new JavaScriptSerializer()).Deserialize<List<MemorandomModel>>(data);
            //model = new JavaScriptSerializer().Deserialize<MemorandomModel>(service.MemorandomPath()); ;
            //var list = service.TodayCoursesOnlyTrainer("2021/07/17");
            return View(model);
        }
    }
}