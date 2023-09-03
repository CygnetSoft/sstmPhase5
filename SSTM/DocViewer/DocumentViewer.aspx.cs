using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SSTM.Views.DocViewer
{
    public partial class DocumentViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                setpath(Request.QueryString["path"]);
            }
            catch (Exception)
            {
                setpath("");
            }
        }
        public void setpath(string path)
        {
            viewer.FilePath = path;
        }
    }
}