using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Security.Application;

namespace Onboarding.Common
{
    public class Security
    {
        public static string Sanitize(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                return Sanitizer.GetSafeHtmlFragment(input);
            }
            else
            {
                return input;
            }
        }

        public static bool IsValidAlphaNum(string input)
        {
            Regex re = new Regex("^[a-zA-Z0-9]+$");
            if (!string.IsNullOrWhiteSpace(input))
            {
                return re.IsMatch(input);
            }
            else
            {
                return false;
            }
        }
        
        public static bool IsValidCostCentre(string input)
        {
            Regex re = new Regex("^[a-zA-Z0-9]+(-[a-zA-Z0-9]+)*$");
            if (!string.IsNullOrWhiteSpace(input))
            {
                return re.IsMatch(input);
            }
            else
            {
                return false;
            }
        }

        public static bool IsValidMobileNum(string input)
        {            
            Regex re = new Regex(@"^([\+]{0,1})\d+$");

            if (!string.IsNullOrWhiteSpace(input))
            {
                return re.IsMatch(input);
            }
            else
            {
                return false;
            }
        }


        public static bool IsValidEmail(string email)
        {
            bool isValid = false;
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    try
                    {
                        System.Net.Mail.MailAddress addr = new MailAddress(email);
                        addr = null;
                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Error", ex);
                    }
                }
                catch (ArgumentException ex2)
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        public static bool IsValidFilename(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                char[] invalidChars = Path.GetInvalidFileNameChars();
                if (filename.IndexOfAny(invalidChars) >= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

    }
}
