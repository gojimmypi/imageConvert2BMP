using System;
using System.Web;
using System.Web.UI;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace imageConvert2BMP
{
    public partial class _Default : Page
    {
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            // HttpContext.Current.Response.Write("encoders.Length =" + encoders.Length);
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    // HttpContext.Current.Response.Write("value =" + j + " : " + encoders[j].MimeType);
                    return encoders[j];
                }
            }
            return null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String appDirectory = Path.GetDirectoryName(HttpContext.Current.Request.PhysicalPath);
            String targetImageName = HttpContext.Current.Request.QueryString["targetImageName"];
            
            // clean up the name for web security / display
            targetImageName = Server.UrlDecode(targetImageName);
            targetImageName = Server.HtmlDecode(targetImageName);
            targetImageName.Replace(">", "");
            targetImageName.Replace("<", "");
            targetImageName.Replace(";", "");
            targetImageName.Replace(":", "");
            targetImageName.Replace("&", "");

            if (targetImageName != "")
            {
                String imageLocalFullPath = appDirectory + "\\images\\" + targetImageName;
                if (File.Exists(imageLocalFullPath))
                {
                    Image thisImage = Image.FromFile(imageLocalFullPath);

                    // see https://msdn.microsoft.com/en-us/library/system.drawing.imaging.encoderparameter(v=vs.110).aspx
                    //Encoder myEncoder = Encoder.ColorDepth;
                    //EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    //EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 24);
                    //myEncoderParameters.Param[0] = myEncoderParameter;

                    // see https://msdn.microsoft.com/en-us/library/system.drawing.imaging.encoder.colordepth(v=vs.110).aspx

                    ImageCodecInfo myImageCodecInfo;
                    // Get an ImageCodecInfo object that represents the TIFF codec.
                    myImageCodecInfo = GetEncoderInfo("image/bmp");

                    Encoder myEncoder;
                    // Create an Encoder object based on the GUID
                    // for the ColorDepth parameter category.
                    myEncoder = Encoder.ColorDepth;

                    // Create an EncoderParameters object.
                    // An EncoderParameters object has an array of EncoderParameter
                    // objects. In this case, there is only one
                    // EncoderParameter object in the array.
                    EncoderParameter myEncoderParameter;
                    EncoderParameters myEncoderParameters;
                    // Save the image with a color depth of 24 bits per pixel.
                    myEncoderParameter =  new EncoderParameter(myEncoder, 24L); // the ILI4321 library only currently works with 24 bit color depth

                    myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameters.Param[0] = myEncoderParameter;


                    // save an example file to local directory
                    // thisImage.Save(imageLocalFullPath + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp); // this will save as 32 bit
                    thisImage.Save(imageLocalFullPath + ".bmp", myImageCodecInfo, myEncoderParameters);
                    // send to web client

                    // see https://www.iana.org/assignments/media-types/image/bmp
                    //Response.ClearHeaders();
                    //Response.AddHeader("content-transfer-encoding", "image/bmp");
                    // Response.AddHeader("Content-Type", "image/bmp");
                    Response.ContentType =  "image/bmp";


                    // thisImage.Save(HttpContext.Current.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);

                    // see http://stackoverflow.com/questions/5629251/c-sharp-outputting-image-to-response-output-stream-giving-gdi-error
                    // PNGs (and other formats) need to be saved to a seekable stream. Using an intermediate MemoryStream will do the trick:
                    using (MemoryStream ms = new MemoryStream())
                    {
                        thisImage.Save(ms, myImageCodecInfo, myEncoderParameters);
                        ms.WriteTo(HttpContext.Current.Response.OutputStream);
                    }


                    //                   Response.End();
                }
                else
                {
                    HttpContext.Current.Response.Write("File not found: images\\" + targetImageName);
                }
            }
            else {
                HttpContext.Current.Response.Write("File not specified");
            }
        }
    }
}