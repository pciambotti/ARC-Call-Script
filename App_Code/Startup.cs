using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Application___Cash_Incentive.Startup))]
namespace Application___Cash_Incentive
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
