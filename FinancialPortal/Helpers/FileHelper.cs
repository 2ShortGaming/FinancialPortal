using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace FinancialPortal.Helpers
{
    public class FileHelper
    {
        public static string MakeUnique(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName);
            fileName = SlugMaker.MakeSlug(fileName);
            fileName = $"{fileName}{DateTime.Now.Ticks}{extension}";
            return fileName;
        }
        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return false;
            }
            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
            {
                return false;
            }

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                        ImageFormat.Png.Equals(img.RawFormat) ||
                        ImageFormat.Gif.Equals(img.RawFormat) ||
                        ImageFormat.Bmp.Equals(img.RawFormat) ||
                        ImageFormat.Tiff.Equals(img.RawFormat);

                }
            }
            catch
            {
                return false;
            }
        }

        public static bool IsWebFriendlyFile(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return false;
            }
            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 2048)
            {
                return false;
            }

            try
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var allowableExtensions = WebConfigurationManager.AppSettings["AllowableExtensions"].Split(',').ToList();
                return allowableExtensions.Contains(fileExtension);

            }
            catch
            {
                return false;
            }
        }

        public static string GetIcon(string fileExtension)
        {

            //.pdf,.doc,.docx,.xls,.xlsx,.txt
            var fileExtensions = WebConfigurationManager.AppSettings["AllowableExtensions"].Split(',');
            var imgExtensions = WebConfigurationManager.AppSettings["AllowableImageExtensions"].Split(',');
            foreach (var extension in fileExtensions.Concat(imgExtensions))
            {
                if (extension == fileExtension)
                    return $"/Images/{extension.TrimStart('.')}.png";


            }

            return "/Images/default.png";




        }


    }
}