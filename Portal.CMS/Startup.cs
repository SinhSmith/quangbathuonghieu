using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Portal.CMS.Startup))]
namespace Portal.CMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
