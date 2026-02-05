using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Configuration;
using Microsoft.Owin.Security;
using Microsoft.Identity.Web.OWIN;
using Microsoft.Identity.Web;
using SMU.Security.DPAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using System.Collections.Generic;
using System.IO;


[assembly: OwinStartup(typeof(Onboarding.Startup))]
namespace Onboarding
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            string OriginalFile = ConfigurationManager.AppSettings["MSALKeyFilePlain"];
            string EncryptedFile = ConfigurationManager.AppSettings["MSALKeyFile"];
            string RedirectURI = ConfigurationManager.AppSettings["RedirectURL"];
            string Instance = ConfigurationManager.AppSettings["Instance"];

            Dictionary<string, string> dAzureData = null;
            if (!string.IsNullOrEmpty(OriginalFile))
            {
                dAzureData = GetKeysFromString(File.ReadAllText(OriginalFile), ":");
            }
            else
            {
                string decyptedString = SecuredFileWriter.DecryptFileContent(EncryptedFile).Trim();
                dAzureData = GetKeysFromString(decyptedString, ":");
            }

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            OwinTokenAcquirerFactory owinTokenAcquirerFactory = TokenAcquirerFactory.GetDefaultInstance<OwinTokenAcquirerFactory>();

            owinTokenAcquirerFactory.Configuration.GetSection("AzureAD:ClientId").Value = dAzureData["ClientId"];
            owinTokenAcquirerFactory.Configuration.GetSection("AzureAD:TenantId").Value = dAzureData["TenantId"];
            owinTokenAcquirerFactory.Configuration.GetSection("AzureAD:ClientSecret").Value = dAzureData["ClientSecret"];
            owinTokenAcquirerFactory.Configuration.GetSection("AzureAD:RedirectUri").Value = RedirectURI;
            owinTokenAcquirerFactory.Configuration.GetSection("AzureAD:Instance").Value = Instance;

            app.AddMicrosoftIdentityWebApp(owinTokenAcquirerFactory,
                                           updateOptions: options => { });
            owinTokenAcquirerFactory.Services
                 .Configure<ConfidentialClientApplicationOptions>(options =>
                 { options.RedirectUri = RedirectURI; })
                .AddInMemoryTokenCaches();

            owinTokenAcquirerFactory.Build();
        }

        public static Dictionary<string, string> GetKeysFromString(string input, string separator)
        {
            Dictionary<string, string> keys = new Dictionary<string, string>();
            string s = string.Empty;
            string[] lines = input.Split(System.Environment.NewLine.ToCharArray());
            foreach (string line in lines)
            {
                string[] fields = null;
                if (separator == ":")
                    fields = line.Split(':');
                else
                    fields = line.Split('=');
                if ((fields.Length == 2) && (fields[0].Trim() != string.Empty))
                {
                    keys.Add(fields[0].Trim(), fields[1].Trim());
                }
            }
            return keys;
        }
    }
}