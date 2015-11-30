using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KSLClassifiedAlerts.Startup))]
namespace KSLClassifiedAlerts
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
