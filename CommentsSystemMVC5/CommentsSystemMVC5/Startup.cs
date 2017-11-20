using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CommentsSystemMVC5.Startup))]
namespace CommentsSystemMVC5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
