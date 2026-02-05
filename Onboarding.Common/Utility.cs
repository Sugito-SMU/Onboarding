using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SMU.Security.DPAPI;

namespace Onboarding.Common
{
    public class Utility
    {

        public static Dictionary<string, string> GetSecuredKeys(string filePath)
        {
            Dictionary<string, string> keys = new Dictionary<string, string>();
            string s = File.ReadAllText(filePath);
            string[] lines = s.Split(Environment.NewLine.ToCharArray());
            foreach (string line in lines)
            {
                string[] fields = line.Split('=');
                if ((fields.Length == 2) && (fields[0].Trim() != string.Empty))
                {
                    keys.Add(fields[0].Trim(), fields[1].Trim());
                }
            }
            return keys;
        }
        public static Dictionary<string, string> GetSecuredEncrytedKeys(string filePath)
        {
            Dictionary<string, string> keys = new Dictionary<string, string>();
            string s = SecuredFileReader.DecryptFileContent(filePath);
            string[] lines = s.Split(Environment.NewLine.ToCharArray());
            foreach (string line in lines)
            {
                string[] fields = line.Split('=');
                if ((fields.Length == 2) && (fields[0].Trim() != string.Empty))
                {
                    keys.Add(fields[0].Trim(), fields[1].Trim());
                }
            }
            return keys;
        }
        public static string ContentParser(Dictionary<string, string> DataKeys, string Content)
        {
            foreach (string Key in DataKeys.Keys)
            {
                Content = Content.Replace("#" + Key + "#", DataKeys[Key]);
            }

            return Content;
        }

    }
}
