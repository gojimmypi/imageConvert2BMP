using System;
using System.Web;
using System.Web.UI;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace imageConvert2BMP
{
    public partial class _Default : Page
    {

        #region "Image Processing"
        void CropImage(Stream myStream,  float HorizontalOffsetPercent = 0, float VerticalOffsetPercent   = 0)
        {
            // CropImage is currently here for reference, and not yet fully implemented
            int targetW = 100;
            int targetH = 100;
            int srcX = 0;
            int srcY = 0;
            Image imgPhoto = Image.FromStream(myStream);
            float ImageAspectRatio = targetW / (float)targetH; // this is the (Width / Height) 
            float CurrentAspectRatio = imgPhoto.Width / (float)imgPhoto.Height;

            if ((int)(ImageAspectRatio * 100) == (int)(CurrentAspectRatio * 100))
            { // no adjustment needed
                targetW = imgPhoto.Width;
                targetH = imgPhoto.Height;
                srcX = 0;
                srcY = 0;
            }
            else
            { // ' need to adjust
              // Portrait or square orientation...
                if (imgPhoto.Height >= imgPhoto.Width)
                {
                    //targetW = imgPhoto.Width;
                    targetH = (int)(targetW * (1 / ImageAspectRatio));
                    srcY = (int)(((imgPhoto.Height - imgPhoto.Width) * ImageAspectRatio) / 2); // add half the total excess width as an offset
                    srcY = (int)(srcY + imgPhoto.Height * (VerticalOffsetPercent / 100)); // then add any manual additional offset
                }
                else
                {
                    srcY = 0;
                } // Portrait or square orientation

                // landscape orientation...
                if (imgPhoto.Width > imgPhoto.Height)
                {
                    //targetH = imgPhoto.Height;
                    targetW = (int)(targetH * ImageAspectRatio);
                    srcX = (int)(((imgPhoto.Width - imgPhoto.Height) * ImageAspectRatio) / 2);
                    srcX = (int)(srcX + imgPhoto.Width * (HorizontalOffsetPercent / 100));
                }
                else
                {
                    srcX = 0;
                }
            } //  landscape orientation
            Bitmap bmPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
            // bmPhoto.SetResolution(72, 72);

            Graphics grPhoto   = Graphics.FromImage(bmPhoto);
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, targetW, targetH), srcX, srcY, targetW, targetH, GraphicsUnit.Pixel);

            //Save out to memory stream.  We dispose of all objects to make sure the files don't stay locked.

            // send to web client
            using (MemoryStream ss = new MemoryStream())
            {
                bmPhoto.Save(ss, System.Drawing.Imaging.ImageFormat.Bmp);
                ss.WriteTo(myStream);
            }
            //bmPhoto.Save(myStream, System.Drawing.Imaging.ImageFormat.Bmp);
            imgPhoto.Dispose();
            bmPhoto.Dispose();
            grPhoto.Dispose();
        }

        //byte []ScaleImage(byte [] ImageContent, float HorizontalOffsetPercent = 0, float VerticalOffsetPercent   = 0)
        //{
        //    return ScaleImage(new MemoryStream(ImageContent), HorizontalOffsetPercent, VerticalOffsetPercent);
        //}

        // see http://stackoverflow.com/questions/1922040/resize-an-image-c-sharp
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to (set to a value less than 1 to keep this size or scale).</param>
        /// <param name="height">The height to resize to (set to a value less than 1 to keep this size or scale)</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width = 0, int height = 0)
        {
            // first see if we will maintain aspect ratio if only width or only height is specified
            if (width < 1) {
                if (height < 1)
                {
                    width = image.Width; // if netiher width nor height is specified, the new width is the current image width
                }
                else
                {
                    width = (image.Width * height) / image.Height; // if width not specified, the new width proportional to new height scale
                }
            }

            if (height < 1) {
                if (width < 1)
                {
                    height = image.Height; // if netiher width nor height is specified, the new width is the current image width
                }
                else
                {
                    height = (image.Height * width) / image.Width; // if width not specified, the new width proportional to new height scale
                }
            }

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        #endregion




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

        public Boolean IsNumber(String s)
        {
            if (s == null || s == String.Empty) { return false; }
            Boolean value = true;
            foreach (Char c in s.ToCharArray())
            {
                value = value && Char.IsDigit(c);
            }

            return value;
        }

        protected int newImageSizeX()
        {
            String param = HttpContext.Current.Request.QueryString["newImageSizeX"];
            if (IsNumber(param))
            {
                return Convert.ToInt16(param);
            }
            else
            {
                return -1;
            }
        }

        protected int newImageSizeY()
        {
            String param = HttpContext.Current.Request.QueryString["newImageSizeY"];
            if (IsNumber(param))
            {
                return Convert.ToInt16(param);
            }
            else
            {
                return -1;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String appDirectory = Path.GetDirectoryName(HttpContext.Current.Request.PhysicalPath);
            String targetImageName = HttpContext.Current.Request.QueryString["targetImageName"];
            
            if (targetImageName != null && targetImageName != String.Empty)
            {
                targetImageName = targetImageName.ToString();
                // clean up the name for web security / display
                targetImageName = Server.UrlDecode(targetImageName);
                targetImageName = Server.HtmlDecode(targetImageName);
                targetImageName.Replace(">", "_");
                targetImageName.Replace("<", "_");
                targetImageName.Replace(";", "_");
                targetImageName.Replace(":", "_");
                targetImageName.Replace("&", "_");

                String imageLocalFullPath = appDirectory + "\\images\\" + targetImageName;
                if (File.Exists(imageLocalFullPath))
                {
                    Image thisImage = Image.FromFile(imageLocalFullPath);

                    //  first thing: decide if the source needs t0 be resized:
                    if  (this.newImageSizeX() > 0 || this.newImageSizeY() > 0)
                    {
                        thisImage = ResizeImage(thisImage, this.newImageSizeX(), this.newImageSizeY());
                    }
                    else { // don't even touch the image if it does not need to be resized!
                    } 


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
                    // see https://msdn.microsoft.com/en-us/library/system.drawing.imaging.encoderparameter(v=vs.110).aspx
                    EncoderParameter myEncoderParameter;
                    EncoderParameters myEncoderParameters;

                    // Save the image with a color depth of 24 bits per pixel.
                    // see https://msdn.microsoft.com/en-us/library/system.drawing.imaging.encoder.colordepth(v=vs.110).aspx
                    myEncoderParameter =  new EncoderParameter(myEncoder, 24L); // the ILI4321 library only currently works with 24 bit color depth

                    myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameters.Param[0] = myEncoderParameter;


                    // save an example file to local directory
                    // thisImage.Save(imageLocalFullPath + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp); // this will save as 32 bit
                    thisImage.Save(imageLocalFullPath + ".bmp", myImageCodecInfo, myEncoderParameters);

                    
                    // see https://www.iana.org/assignments/media-types/image/bmp
                    // Response.ClearHeaders();
                    // Response.AddHeader("content-transfer-encoding", "image/bmp");
                    // Response.AddHeader("Content-Type", "image/bmp");
                    Response.ContentType =  "image/bmp";

                    // this code I would have expected to work, but seems to abort before sending all data:
                    // thisImage.Save(HttpContext.Current.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);

                    // see http://stackoverflow.com/questions/5629251/c-sharp-outputting-image-to-response-output-stream-giving-gdi-error
                    // PNGs (and other formats) need to be saved to a seekable stream. Using an intermediate MemoryStream will do the trick:
                    using (MemoryStream ms = new MemoryStream())
                    {
                        thisImage.Save(ms, myImageCodecInfo, myEncoderParameters);

                        // send to web client
                        ms.WriteTo(HttpContext.Current.Response.OutputStream);
                        Response.End();
                    }

                }
                else
                {
                    HttpContext.Current.Response.Write("File not found: images\\" + targetImageName);
                }
            }
            else {
                HttpContext.Current.Response.Write("File not specified; add QueryString parameters:<br />");
                HttpContext.Current.Response.Write("");
                HttpContext.Current.Response.Write("targetImageName=[file name in server ./images/ directory].<br />");
                HttpContext.Current.Response.Write("<br />");
                HttpContext.Current.Response.Write("scaleX=[new X-dimension scale]<br />");
                HttpContext.Current.Response.Write("<br />");
                HttpContext.Current.Response.Write("scaleY=[new Y-dimension scale]<br />");
                HttpContext.Current.Response.Write("<br />");
                HttpContext.Current.Response.Write("<br />");
            }
        }
    }
}