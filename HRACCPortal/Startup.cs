using Microsoft.Owin;
using Owin;

using System.Web.Hosting;

[assembly: OwinStartupAttribute(typeof(HRACCPortal.Startup))]
namespace HRACCPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
           

        }



    }
}
