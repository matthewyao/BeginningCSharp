using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using RoleIdentity.Models;

namespace RoleIdentity
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 在此处插入电子邮件服务可发送电子邮件。
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 在此处插入 SMS 服务可发送短信。
            return Task.FromResult(0);
        }
    }

    // 配置此应用程序中使用的应用程序用户管理器。UserManager 在 ASP.NET Identity 中定义，并由此应用程序使用。
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // 配置用户名的验证逻辑
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // 配置密码的验证逻辑
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // 配置用户锁定默认值
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // 注册双重身份验证提供程序。此应用程序使用手机和电子邮件作为接收用于验证用户的代码的一个步骤
            // 你可以编写自己的提供程序并将其插入到此处。
            manager.RegisterTwoFactorProvider("电话代码", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "你的安全代码是 {0}"
            });
            manager.RegisterTwoFactorProvider("电子邮件代码", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "安全代码",
                BodyFormat = "你的安全代码是 {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //创建用户名为admin@123.com，密码为“Admin@123456”并把该用户添加到角色组"Admin"中
        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "admin@pptv.com";//用户名
            const string password = "90()IOpl";//密码
            //List<string> roles = new List<string>() { "查看菜单", "仅查看用户", "可修改用户", "仅查看角色", "可修改角色" };
            const string roleName = "Admin";//用户要添加到的角色组

            //foreach (var roleName in roles)
            //{
                //如果没有Admin用户组则创建该组
                var role = roleManager.FindByName(roleName);
                if (role == null)
                {
                    role = new IdentityRole(roleName);
                    var roleresult = roleManager.Create(role);
                }

                //如果没有admin@123.com用户则创建该用户
                var user = userManager.FindByName(name);
                if (user == null)
                {
                    user = new ApplicationUser { UserName = name, Email = name };
                    var result = userManager.Create(user, password);
                    result = userManager.SetLockoutEnabled(user.Id, false);
                }

                // 把用户admin@123.com添加到用户组Admin中
                var rolesForUser = userManager.GetRoles(user.Id);
                if (!rolesForUser.Contains(role.Name))
                {
                    var result = userManager.AddToRole(user.Id, role.Name);
                }
            }           
        //}
    }

    // 配置要在此应用程序中使用的应用程序登录管理器。
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore) : base(roleStore)
        {
        }
        
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }
}
