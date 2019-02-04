using Api.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace site.Controllers
{
    [RoutePrefix("File")]
    public class FileController : Controller
    {
        
        public static readonly List<string> ImageExtensions = new List<string> { ".PDF", ".JPG", ".JPEG", ".BMP", ".GIF", ".PNG" };

        public bool ThumbnailCallback()
        {
            return false;
        }
        
        [Route("Upload")]
        [HttpPost]
        public JsonResult Upload()
        {
            
            List<Upload> uploads = new List<Upload>();
            string localPath = System.Configuration.ConfigurationManager.AppSettings["filePath"];
            string dirPath = localPath;
            string dirPathThumb = localPath + "\\thumbnails\\";
            
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            if (!Directory.Exists(dirPathThumb)) Directory.CreateDirectory(dirPathThumb);

            for (int i = 0; i < Request.Files.Count; i++)
            {

                HttpPostedFileBase file = Request.Files[i];
                Stream fileContent = file.InputStream;
                
                Guid id = Guid.NewGuid();
                string fileTypeExtension = Path.GetExtension(file.FileName);
                string fileName = id + Path.GetExtension(file.FileName);
                string path = Path.Combine(dirPath, fileName);
                string pathThumb = Path.Combine(dirPathThumb, fileName);

                Upload upload = new Upload();

                if (ImageExtensions.Contains(fileTypeExtension.ToUpperInvariant()))
                {
                    
                    Image image = Image.FromStream(fileContent);

                    float maxWidth = 2000;
                    float maxHeight = 2000;
                    float ratio = (image.Width > maxWidth || image.Height > maxHeight) ? Math.Min(maxWidth / image.Width, maxHeight / image.Height) : 1;

                    Bitmap objBit = new Bitmap(image, (int)(image.Width * ratio), (int)(image.Height * ratio));

                    float maxWidthThumb = 700; //HAS TO BE 700 WITH TEMPLATE CROPS sR
                    float maxHeightThumb = 700; //HAS TO BE 700 WITH TEMPLATE CROPS sR
                    float ratioThumb = (image.Width > maxWidthThumb || image.Height > maxHeightThumb) ? Math.Min(maxWidthThumb / image.Width, maxHeightThumb / image.Height) : 1;

                    Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    objBit.GetThumbnailImage((int)(image.Width * ratioThumb), (int)(image.Height * ratioThumb), myCallback, IntPtr.Zero).Save(pathThumb, image.RawFormat);
                    objBit.Save(path, image.RawFormat);
                    
                    upload = new Upload()
                    {
                        id                      = id,
                        path                    = "https://files.bambino.software/" + fileName,
                        originalFileName        = file.FileName,
                        width                   = objBit.Width,
                        height                  = objBit.Height,
                        resolutionHorizontal    = Convert.ToInt32(image.HorizontalResolution),
                        resolutionVertical      = Convert.ToInt32(image.VerticalResolution),
                        contentLength           = file.ContentLength
                    };
                    
                    image.Dispose();
                    objBit.Dispose();
                    
                }
                else
                {

                    file.SaveAs(path);
                    
                    upload = new Upload()
                    {
                        id                      = id,
                        path                    = "https://files.bambino.software/" + fileName,
                        originalFileName        = file.FileName,
                        width                   = 0,
                        height                  = 0,
                        resolutionHorizontal    = 0,
                        resolutionVertical      = 0,
                        contentLength           = file.ContentLength
                    };

                }

                uploads.Add(upload);

                fileContent.Dispose();
                fileContent.Close();
                file.InputStream.Dispose();
                file.InputStream.Close();
                
            }

            return Json(uploads, JsonRequestBehavior.AllowGet);

        }
        
    }
}