using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(imageConvert2BMP.Startup))]
namespace imageConvert2BMP
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            
        }
    }
}
