using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web;
using System;
using Application___Cash_Incentive;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Application___Cash_Incentive
{
    // You can add User data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        // public DateTime? DateRegistered { get; set; }
        public DateTime? DateLastLogin { get; set; }
        public string TimeZone { get; set; }

        // Use this to add more fields to the register function:
        // https://www.itorian.com/2013/11/customizing-users-profile-to-add-new.html
        // Check out the migration stuff
        // Also look at this:
        // https://stackoverflow.com/questions/38647335/how-to-add-additional-fields-to-register-page-in-asp-net-mvc

    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public ApplicationDbContext(): base("DefaultConnection")
        {
        }
        // Added based on this: https://stackoverflow.com/questions/25671881/how-do-i-extend-identityrole-using-web-api-2-aspnet-identity-2
        // Fixed some issues
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
    #region Helpers INT User ID
    public class CustomUserRole : IdentityUserRole<int> { }
    public class CustomUserClaim : IdentityUserClaim<int> { }
    public class CustomUserLogin : IdentityUserLogin<int> { }
    public class CustomRole : IdentityRole<int, CustomUserRole>
    {
        public CustomRole() { }
        public CustomRole(string name) { Name = name; }
    }
    public class CustomUserStore : UserStore<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(ApplicationDbContext context): base(context)
        {
        }
    }
    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {
        public CustomRoleStore(ApplicationDbContext context): base(context)
        {
        }
    }
    #endregion
    #region Helpers: UserManager
    public class UserManager : UserManager<ApplicationUser, int>
    {
        public UserManager(): base(new UserStore<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>(new ApplicationDbContext()))
        {
        }
        public static UserManager<ApplicationUser, int> Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {

            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("CIPApplication");

            var userManager = new UserManager<ApplicationUser, int>(new CustomUserStore(context.Get<ApplicationDbContext>()));

            userManager.UserValidator = new UserValidator<ApplicationUser, int>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };

            userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("CIPApplicationToken"));

            return userManager;
        }
    }
    public static class TokenProvider
    {
        private static DataProtectorTokenProvider<IdentityUser> _tokenProvider;

        public static DataProtectorTokenProvider<IdentityUser> Provider
        {
            get
            {

                if (_tokenProvider != null)
                    return _tokenProvider;
                var dataProtectionProvider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider();
                _tokenProvider = new DataProtectorTokenProvider<IdentityUser>(dataProtectionProvider.Create());
                return _tokenProvider;
            }
        }
    }
    #endregion
}
namespace Application___Cash_Incentive
{
    #region Helpers: IdentityHelper
    public static class IdentityHelper
    {
        // Used for XSRF when linking external logins
        public const string XsrfKey = "XsrfId";

        public static void SignIn(UserManager manager, ApplicationUser user, bool isPersistent)
        {
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        public const string ProviderNameKey = "providerName";
        public static string GetProviderNameFromRequest(HttpRequest request)
        {
            return request[ProviderNameKey];
        }

        public static string GetExternalLoginRedirectUrl(string accountProvider)
        {
            return "/Account/RegisterExternalLogin?" + ProviderNameKey + "=" + accountProvider;
        }

        private static bool IsLocalUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }

        public static void RedirectToReturnUrl(string returnUrl, HttpResponse response)
        {
            if (!String.IsNullOrEmpty(returnUrl) && IsLocalUrl(returnUrl))
            {
                response.Redirect(returnUrl);
            }
            else
            {
                response.Redirect("~/");
            }
        }
    }
    #endregion
}

namespace Application___Cash_Incentive
{
    public class ApplicationRoleManager : RoleManager<IdentityRole, string>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore): base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole, string, IdentityUserRole>(context.Get<ApplicationDbContext>()));
        }
    }
}

/*
 * Change USER ID from NVARCHAR to INT
 * https://stackoverflow.com/questions/34505904/change-user-id-type-to-int-in-asp-net-identity-in-vs2015
 * https://docs.microsoft.com/en-us/aspnet/identity/overview/extensibility/change-primary-key-for-users-in-aspnet-identity
 * 
 * 
 *
 *
 *
 */
