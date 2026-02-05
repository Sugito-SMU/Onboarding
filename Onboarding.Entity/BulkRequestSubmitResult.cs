using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onboarding.Entity
{
    public class BulkRequestSubmitResult
    {
        public List<BulkRequestSubmitResultRow> Rows { get; set; }

        public bool IsAllResultPass
        {
            get
            {
                bool pass = true;
                if (Rows != null)
                {
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        if ((Rows[i].Errors != null) && (Rows[i].Errors.Count > 0))
                        {
                            pass = false;
                            break;
                        }
                    }
                }
                return pass;
            }
        }

    }
}
