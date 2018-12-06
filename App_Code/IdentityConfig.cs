using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web;
using System;
using Application___Cash_Incentive;
using Microsoft.AspNet.Identity.Owin;

using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;

/// <summary>
/// Summary description for IdentityConfig
/// </summary>
namespace Application___Cash_Incentive
{
    //public class ApplicationUserManager : UserManager<ApplicationUser, int>
    //{
    //    public ApplicationUserManager(IUserStore<ApplicationUser, int> store) : base(store) { }
    //    public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
    //    {
    //        var manager = new ApplicationUserManager(new CustomUserStore(context.Get<ApplicationDbContext>()));

    //        manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
    //        {
    //            AllowOnlyAlphanumericUserNames = false
    //        };

    //        var dataProtectionProvider = options.DataProtectionProvider;
    //        if (dataProtectionProvider != null)
    //        {
    //            manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(dataProtectionProvider.Create("ApplicationToken"));
    //        }
    //        return manager;
    //    }
    //}
}