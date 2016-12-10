using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(C_Sharp_Project.Startup))]
namespace C_Sharp_Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
