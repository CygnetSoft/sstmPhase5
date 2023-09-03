using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFormsDocumentViewer.Converters
{
    public class convertwithLibreOffice
    {
        public string convertallfile(string workingdirectory, string filePath, string destinationPath)
        {
            string extension = Path.GetFileNameWithoutExtension(filePath) + ".pdf";
            string destpath = destinationPath;
            string filename = Path.GetFileName(filePath);
            string mbdata = "";
            //Uri url = new Uri(filePath.ToString());
            if (CheckURLValid(filePath))
            {
                Uri url = new Uri(filePath.ToString());
                mbdata = GetFileSize(url);
            }
            else
            {
                long url = new System.IO.FileInfo(filePath).Length;
                mbdata = BytesToString(url);
            }


            if (!File.Exists(destinationPath + "/" + extension))
            {
                using (Process pdfprocess = new Process())
                {

                    pdfprocess.StartInfo.UseShellExecute = true;
                    pdfprocess.StartInfo.LoadUserProfile = true;
                    pdfprocess.StartInfo.FileName = "soffice.exe";
                    pdfprocess.StartInfo.Arguments = "--headless -convert-to pdf:writer_pdf_Export --outdir " + destinationPath + " " + filePath + "";
                    pdfprocess.StartInfo.WorkingDirectory = @"C:\Program Files\LibreOffice\program\";
                    pdfprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    pdfprocess.Start();
                    //if (!pdfprocess.WaitForExit(1000 * 60 * 1))
                    //{
                    //    pdfprocess.Kill();
                    //}
                    if (Convert.ToDouble(mbdata) < 10)
                    {
                        if (!pdfprocess.WaitForExit(1000 * 60 * 1))
                        {
                            pdfprocess.Kill();
                        }
                    }
                    else if (Convert.ToDouble(mbdata) < 25)
                    {
                        if (!pdfprocess.WaitForExit(1000 * 60 * 2))
                        {
                            pdfprocess.Kill();
                        }
                    }
                    else if (Convert.ToDouble(mbdata) < 50)
                    {
                        if (!pdfprocess.WaitForExit(1000 * 60 * 3))
                        {
                            pdfprocess.Kill();
                        }
                    }
                    else if (Convert.ToDouble(mbdata) < 75)
                    {
                        if (!pdfprocess.WaitForExit(1000 * 60 * 4))
                        {
                            pdfprocess.Kill();
                        }
                    }
                    else if (Convert.ToDouble(mbdata) < 100)
                    {
                        if (!pdfprocess.WaitForExit(1000 * 60 * 5))
                        {
                            pdfprocess.Kill();
                        }
                    }

                    else if (Convert.ToDouble(mbdata) >= 100)
                    {
                        if (!pdfprocess.WaitForExit(1000 * 60 * 6))
                        {
                            pdfprocess.Kill();
                        }
                    }

                    pdfprocess.Close();
                }
            }
            return extension;
        }
        static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString();
        }
        private static string GetFileSize(Uri uriPath)
        {
            var webRequest = System.Net.HttpWebRequest.Create(uriPath);
            webRequest.Method = "HEAD";

            using (var webResponse = webRequest.GetResponse())
            {
                var fileSize = webResponse.Headers.Get("Content-Length");
                var fileSizeInMegaByte = Math.Round(Convert.ToDouble(fileSize) / 1024.0 / 1024.0, 2);
                return fileSizeInMegaByte.ToString();
            }
        }
        public bool CheckURLValid(string source)
        {
            Uri uriResult;
            return Uri.TryCreate(source, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
