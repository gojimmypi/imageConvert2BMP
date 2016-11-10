using System;
using System.Web;
using System.Web.UI;
using System.Drawing;
using System.IO;

namespace imageConvert2BMP
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String appDirectory = HttpContext.Current.Request.PhysicalPath;
            String targetImageName = HttpContext.Current.Request.QueryString["targetImageName"];
            if (targetImageName != "")
            {
                String imageLocalFullPath = appDirectory + "\\" + targetImageName;
                if (File.Exists(imageLocalFullPath))
                {
                    Image thisImage = Image.FromFile(imageLocalFullPath);
                    thisImage.Save(HttpContext.Current.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                else
                {
                    HttpContext.Current.Response.Write("File not found");
                }
            }
            {
                HttpContext.Current.Response.Write("File not specified");
            }
        }
    }
}