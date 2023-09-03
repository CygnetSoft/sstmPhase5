using System.IO;
using System.Diagnostics;

namespace FindAndReplace
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "sample.pptx";
            string outputFile = "sample output.pptx";

            // Copy Word document.
            File.Copy(inputFile, outputFile, true);

            // Open copied document.
            using (var flatDocument = new FlatDocument(outputFile))
            {
                // Search and replace document's text content.
                flatDocument.FindAndReplace("[TITLE]", "md");
                flatDocument.FindAndReplace("[SUBTITLE]", "mayur");
                flatDocument.FindAndReplace("[NAME]", "dodiya mayur");
                flatDocument.FindAndReplace("[EMAIL]", "meet@email.com");
                flatDocument.FindAndReplace("[PHONE]", "7405770815");
                // Save document on Dispose.
            }

            // Open document in Word application.
            Process.Start(outputFile);
        }
    }
}
