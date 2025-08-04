using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace HtmlSampleAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;   
        }

        [HttpGet("GetInvoice")]
        public async Task<IActionResult> GetInvoice()
        {
            var document = new PdfSharpCore.Pdf.PdfDocument();
            string htmlpath = Path.Combine(_hostingEnvironment.ContentRootPath, "HtmlTemplates", "invoiceimage.html");


            string applicationPath = Directory.GetCurrentDirectory();

            string tnsdclogo = Path.GetFullPath(applicationPath + "\\HtmlTemplates\\Invoice_TNSDC_Logo.png").Replace("\\", "/");

            string body = string.Empty;

            if (System.IO.File.Exists(htmlpath))
            {
                string tnsdclogoDataUrl = "";
                if (System.IO.File.Exists(tnsdclogo))
                {
                    tnsdclogoDataUrl = FiletoByteString(Convert.ToBase64String(System.IO.File.ReadAllBytes(tnsdclogo)));
                }

                StreamReader reader = new StreamReader(htmlpath);
                body = reader.ReadToEnd();
                body = body.Replace("[ImageSRC]", tnsdclogoDataUrl);
            }


            //return File(response,"application/pdf","generatedPDF.pdf");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "generated.pdf");

            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            //System.IO.File.WriteAllBytes(filePath, response);

            var htmlToPDFConv = new NReco.PdfGenerator.HtmlToPdfConverter();

            htmlToPDFConv.GeneratePdf(body, null, filePath);

            return Ok("Success");
            
        }

        [NonAction]
        public void ConvertToPDF(byte[] jpegBytes, string outputPdfPath)
        {
            using (FileStream fs = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (Document doc = new Document())
            using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
            {
                doc.Open();

                // Create an image instance from JPEG bytes
                Image image = Image.GetInstance(jpegBytes);

                // Scale the image to fit the page
                image.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);

                // Add the image to the PDF document
                doc.Add(image);

                doc.Close();
            }

        }
        [NonAction]
        private string FiletoByteString(string FilePath)
        {
            return $"data:image/png;base64,{FilePath}";
        }
        
    }
}
