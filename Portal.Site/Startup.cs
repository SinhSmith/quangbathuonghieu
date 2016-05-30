using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Portal.Site.Startup))]
namespace Portal.Site
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
