using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace Onboarding.DataProvider
{
    public static class Common
    {
        private static bool instanceExists = System.Data.Entity.SqlServer.SqlProviderServices.Instance != null;

        public static DateTime GetSqlDateTime()
        {
            instanceExists = instanceExists;
            using (Model.OnboardingEntities obEntities = new Model.OnboardingEntities())
            {
                var dateQuery = obEntities.Database.SqlQuery<DateTime>("SELECT GETDATE()");
                DateTime serverDate = dateQuery.AsEnumerable().First();
                return serverDate;
            }
        }

        //Return true if there is changes, else false
        public static bool SaveDbChanges(DbContext context)
        {
            bool hasChanges = false;
            try
            {
                hasChanges = context.ChangeTracker.HasChanges();
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }
                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
            return hasChanges;
        }

        public static bool IsSameDescription(string str1, string str2)
        {
            if (string.IsNullOrWhiteSpace(str1) && string.IsNullOrWhiteSpace(str2))
            {
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(str1) && !string.IsNullOrWhiteSpace(str2))
            {
                return (str1.Trim() == str2.Trim());
            }
            else
            {
                return false;
            }
        }

    }
}
