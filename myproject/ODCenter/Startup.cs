using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ODCenter.Startup))]
namespace ODCenter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
