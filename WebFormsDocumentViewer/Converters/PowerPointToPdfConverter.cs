using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WebFormsDocumentViewer.Converters
{
    /// <summary>
    /// Helper class to convert PowerPoint documents to PDF
    /// </summary>
    /// <remarks>
    /// You should also have Microsoft Office installed on the server for this to work
    /// </remarks>
    public class PowerPointToPdfConverter : Infrastructure.IConverter
    {
        /// <summary>
        /// Converts the PowerPoint document to PDF
        /// </summary>
        /// <param name="filePath">Path to the PowerPoint file</param>
        /// <param name="destinationPath">Directory where the PDF file will be saved</param>
        /// <returns>Name of the converted file</returns>
        public string Convert(string filePath, string destinationPath)
        {
          string  debugfile = Path.Combine(destinationPath, DateTime.Now.ToString("dd-MM-yyyy") + "PPtError.txt");
            string textwrite="";
            Application appPowerPoint = new Application();
            Presentation powerPointDocument = null;
            textwrite = "Load Path \n";
            try
            {
                textwrite +="path "+ filePath + " temp = "+ destinationPath;
                powerPointDocument = appPowerPoint.Presentations.Open2007(filePath, 
                    MsoTriState.msoFalse,
                    MsoTriState.msoFalse, 
                    MsoTriState.msoTrue);
                textwrite += "Open2007 \n";
                powerPointDocument.Final = false;
                string fileName = Path.GetFileNameWithoutExtension(filePath) + DateTime.Now.Ticks + ".pdf";
                textwrite += fileName +" \n";
                if (!Directory.Exists(destinationPath))
                    Directory.CreateDirectory(destinationPath);

                textwrite += destinationPath + " \n";
                powerPointDocument.ExportAsFixedFormat(Path.Combine(destinationPath, fileName), PpFixedFormatType.ppFixedFormatTypePDF);
                textwrite += "Export success \n";
                
                return fileName;
            }
            catch (Exception ex)
            {
                textwrite +="Error "+ ex.Message + " \n";

                errorcode(textwrite, debugfile);

                // File.AppendAllText(debugfile, textwrite);

                throw new Exception("An error ocurred while trying to convert the file " + ex.Message);
            }
            finally
            {
                textwrite += "Success";
                errorcode(textwrite, debugfile);
           
                powerPointDocument?.Close();
                appPowerPoint.Quit();

                // Power point not closing up so easily, so this works after a couple of tries from
                // https://stackoverflow.com/questions/981547/powerpoint-launched-via-c-sharp-does-not-quit
                Process[] processes = Process.GetProcesses();
                foreach(var process in processes.Where(x => x.ProcessName.ToLower().Contains("powerpnt")))
                {
                    process.Kill();
                }
            }
        }
        public void errorcode(string text,string debugfile)
        {

            if (!File.Exists(debugfile))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(debugfile))
                {

                }
            }

            using (StreamWriter sw = File.AppendText(debugfile))
            {
                sw.WriteLine(text);
                sw.WriteLine("\n");
            }
        }
    }
}
