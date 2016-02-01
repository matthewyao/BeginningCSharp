using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RoleIdentity.Startup))]
namespace RoleIdentity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
