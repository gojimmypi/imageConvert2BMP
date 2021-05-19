# imageConvert2BMP

This C# ASP.Net web app converts files in the local `./images/` directory, or provided via URL, to 24 bit BMP files.

Why BMP files? Well, they are the simplest to decode and currently the only supported format for my [Desktop Dashboard ESP8266 app](https://github.com/gojimmypi/DesktopDashboard) that uses an ILI9341 SPI display module as seen [here](https://www.youtube.com/watch?v=TmvaU6EQsAc).

## Demo

See this app in use at: https://imageconvert2bmp20210518160938.azurewebsites.net/ 

previously published at http://gojimmypi-dev-imageconvert2bmp.azurewebsites.net/ but associated with deactivated / expired Azure subscription. :/

## How to use

There's some built-in [help](https://github.com/gojimmypi/imageConvert2BMP/blob/26d11af66202017ff3ab54a6d35a51fae963d811/imageConvert2BMP/Default.aspx.cs#L347) that should be displayed when no parameters are provided:

`targetHttpImage=`[http link to image]

OR

`targetImageName=`[file name in server ./images/ directory].


Optional resizing (specify only 1 to maintain aspect ratio, )

`newImageSizeX=`[new X-dimension scale]

`newImageSizeY=`[new Y-dimension scale]

## Examples:

See examples [here](https://imageconvert2bmp20210518160938.azurewebsites.net/SampleConversions.html).

### Local web image

Image `image.png` hosted on local web site: 

https://imageconvert2bmp20210518160938.azurewebsites.net/images/image.png

Above image passed as a parameter:

https://imageconvert2bmp20210518160938.azurewebsites.net?targetImageName=image.png

Above image passed as parameter, and scaled to 42px:

https://imageconvert2bmp20210518160938.azurewebsites.net?targetImageName=image.png&newImageSizeX=42

### Web link image

Image `_92466284_img-20161116-wa0085.jpg` on BBC news site:
http://ichef.bbci.co.uk/news/304/cpsprodpb/BC8A/production/_92466284_img-20161116-wa0085.jpg

Above image passed as `targetHttpImage` parameter, scaled to 80px via `newImageSizeX`:
https://imageconvert2bmp20210518160938.azurewebsites.net/default.aspx?newImageSizeX=80&targetHttpImage=http://ichef.bbci.co.uk/news/304/cpsprodpb/BC8A/production/_92466284_img-20161116-wa0085.jpg

Source code: https://github.com/gojimmypi/imageConvert2BMP

