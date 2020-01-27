using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TryTestWeb.Startup))]
namespace TryTestWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
