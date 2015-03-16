using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelGolfBooking.Controllers
{
    public class LogoController : Controller
    {
        //
        // GET: /Logo/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcess(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.LogoImagePath + "\\");
            string nameFile = String.Format("{0}.jpg", filename);
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            for (int i = 0; i < countFile; i++)
            {
                try
                {
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                catch (Exception ex1) {
                    return "LoiXoaAnh";
                }
                try
                {
                    Request.Files[i].SaveAs(fullPath);
                }
                catch (Exception ex2) {
                    return "LoiSaveAnh";
                }
                break;
            }
            string ok = "";
            try
            {
                if (filename.Equals("logo"))
                {
                    ok = resizeImage(Config.imgWidthLogo, Config.imgHeightLogo, fullPath, Config.LogoImagePath + "/" + nameFile);
                }
                else
                {
                    ok = resizeImage(Config.imgWidthLogo2, Config.imgHeightLogo2, fullPath, Config.LogoImagePath + "/" + nameFile);
                }
            }
            catch (Exception ex3)
            {
                return "LoiResize";
            }
            return Config.LogoImagePath + "/" + nameFile;
        }
        public string resizeImage(int maxWidth, int maxHeight, string fullPath, string path)
        {

            var image = System.Drawing.Image.FromFile(fullPath);
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);
            var newWidth = (int)(image.Width * ratioX);
            var newHeight = (int)(image.Height * ratioY);
            var newImage = new Bitmap(newWidth, newHeight);
            Graphics thumbGraph = Graphics.FromImage(newImage);

            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
            //thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            thumbGraph.DrawImage(image, 0, 0, newWidth, newHeight);
            image.Dispose();

            string fileRelativePath = fullPath;// path;// "newsizeimages/" + maxWidth + Path.GetFileName(path);
            //newImage.Save(HttpContext.Server.MapPath(fileRelativePath), newImage.RawFormat);
            //if (System.IO.File.Exists(fileRelativePath))
            //{
            //    System.IO.File.Delete(fileRelativePath);
            //}
            newImage.Save(fileRelativePath, newImage.RawFormat);
            return fileRelativePath;
        }
    }
}
