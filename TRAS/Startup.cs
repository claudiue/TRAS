using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TRAS.Startup))]
namespace TRAS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
