using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using WebFormsDocumentViewer.Converters;
using WebFormsDocumentViewer.Infrastructure;

namespace WebFormsDocumentViewer
{
    [DefaultProperty("FilePath")]
    [ToolboxData("<{0}:DocumentViewer runat=server></{0}:DocumentViewer>")]
    public class DocumentViewer : WebControl
    {
        private string filePath;
        private string tempDirectoryPath;
        private PdfRenderers pdfRenderers;

        [Category("Source File")]
        [Browsable(true)]
        [Description("Set path to source file.")]
        [UrlProperty, Editor(typeof(UrlEditor), typeof(UITypeEditor))]
        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = string.IsNullOrEmpty(value) ? string.Empty : value;
            }
        }

        [Category("Temporary Directory Path")]
        [Browsable(true)]
        [Description("Set path to the directory where the files will be converted.")]
        [UrlProperty, Editor(typeof(UrlEditor), typeof(UITypeEditor))]
        public string TempDirectoryPath
        {
            get
            {
                return string.IsNullOrEmpty(tempDirectoryPath) ? "~/Temp" : tempDirectoryPath;
            }
            set
            {
                tempDirectoryPath = string.IsNullOrEmpty(value) ? string.Empty : value;
            }
        }

        [Category("PDF Renderer")]
        [Browsable(true)]
        [Description("Set the PDF renderer for PDF documents or documents that are converted to PDF. Adobe Reader is used by default")]
        public PdfRenderers PdfRenderer
        {
            get
            {
                return pdfRenderers;
            }
            set
            {
                pdfRenderers = value;
            }
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            try
            {

                if (CheckURLValid(FilePath))
                {
                    string fileExtension = Path.GetExtension(FilePath);

                    //if (fileExtension.ToString().ToLower() != "pdf")
                    //{
                    //    if (CheckURLValid(FilePath))
                    //    {
                    //        using (var client = new WebClient())
                    //        {
                    //            string FilePath1 = "";
                    //            int position = FilePath.LastIndexOf('/');
                    //            if (position > -1)
                    //                FilePath1 = FilePath.Substring(position + 1);

                    //            client.DownloadFile(FilePath, HttpContext.Current.Server.MapPath(TempDirectoryPath) + "/" + FilePath1.ToString());
                    //            FilePath = HttpContext.Current.Server.MapPath(TempDirectoryPath) + @"\" + FilePath1.ToString();
                    //        }
                    //    }
                    //}

                    writer.Write(BuildControl(FilePath, ResolveUrl(FilePath), HttpContext.Current.Server.MapPath(TempDirectoryPath),
                        ResolveUrl(TempDirectoryPath), HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority), ResolveUrl("~/")).ToString());
                }
                else
                {
                    writer.Write(BuildControl(HttpContext.Current.Server.MapPath(FilePath), ResolveUrl(FilePath), HttpContext.Current.Server.MapPath(TempDirectoryPath),
                        ResolveUrl(TempDirectoryPath), HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority), ResolveUrl("~/")).ToString());
                }
            }
            catch
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write("Cannot display document viewer");
                writer.RenderEndTag();
            }
        }
        public bool CheckURLValid(string source)
        {
            Uri uriResult;
            return Uri.TryCreate(source, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
        public StringBuilder BuildControl(string filePhysicalPath, string fileVirtualPath, string tempDirectoryPhysicalPath,
            string tempDirectoryVirtualPath, string appDomain, string appRootUrl)
        {
            try
            {
                try
                {
                    Directory.GetFiles(tempDirectoryPhysicalPath)
                          .Select(f => new FileInfo(f))
                          .Where(f => f.LastAccessTime < DateTime.Now.AddDays(-1))
                          .ToList()
                          .ForEach(f => f.Delete());
                }
                catch (Exception)
                { }

                string fileExtension = Path.GetExtension(fileVirtualPath);
                string frameSource = fileVirtualPath;
                SupportedExtensions extension = (SupportedExtensions)Enum.Parse(typeof(SupportedExtensions), fileExtension.Replace(".", ""));
                IConverter converter = ConverterFactory.GetConverter(extension);
                string tempFileName = "";
                if (converter != null)
                {

                    convertwithLibreOffice convert = new convertwithLibreOffice();
                    tempFileName = convert.convertallfile(@"C:\Program Files\LibreOffice\program\", filePhysicalPath, tempDirectoryPhysicalPath);
                    // string tempFileName = converter.Convert(filePhysicalPath, tempDirectoryPhysicalPath);
                    if (string.IsNullOrEmpty(tempFileName))
                        throw new Exception("An error ocurred while trying to convert the file");


                    frameSource = string.Format("{0}/{1}", tempDirectoryVirtualPath, tempFileName);

                }

                if (extension.ToString().ToLower() == "pdf")
                {
                    using (var client = new WebClient())
                    {
                        string FilePath1 = "";
                        int position = FilePath.LastIndexOf('/');
                        if (position > -1)
                            FilePath1 = FilePath.Substring(position + 1);

                        frameSource = string.Format("{0}/{1}", tempDirectoryVirtualPath, FilePath1);
                        //if (!File.Exists(HttpContext.Current.Server.MapPath(TempDirectoryPath) + "/" + FilePath1.ToString()))
                        //{
                        //    client.DownloadFile(FilePath, HttpContext.Current.Server.MapPath(TempDirectoryPath) + "/" + FilePath1.ToString());
                        //}


                        frameSource = string.Format("{0}{1}Scripts/pdf.js/web/viewer.html?file={0}{2}", appDomain, appRootUrl, FilePath.TrimStart('~'));
                        //frameSource = ResolveUrl(TempDirectoryPath) + "/" + tempFileName.ToString();
                    }
                }
                if (extension.ToString().ToLower() != "pdf")
                {

                    if (PdfRenderer == PdfRenderers.PdfJs && Enum.IsDefined(typeof(PdfJsSupportedExtensions), extension.ToString()))
                        frameSource = string.Format("{0}{1}Scripts/pdf.js/web/viewer.html?file={0}{2}", appDomain, appRootUrl, frameSource);
                    else if (extension.ToString().ToLower() == "xml" || extension.ToString().ToLower() == "xlsx")
                        frameSource = string.Format("{0}/{1}", appDomain, frameSource);
                    else
                        frameSource = string.Format("{0}{1}Scripts/pdf.js/web/viewer.html?file={0}{2}", appDomain, appRootUrl, frameSource);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("<iframe ");
                if (!string.IsNullOrEmpty(ID))
                    sb.Append("id=" + ClientID + " ");
                sb.Append("src=" + frameSource + " ");
                sb.Append("width=" + Width.ToString() + " ");
                sb.Append("height=" + Height.ToString() + ">");
                sb.Append("</iframe>");
                return sb;
            }
            catch (Exception ex)
            {
                return new StringBuilder(ex.Message);
            }
        }

    }
}
