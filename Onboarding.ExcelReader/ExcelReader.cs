using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Syncfusion.XlsIO;

namespace Onboarding.ExcelReader
{
    public static class ExcelReader
    {
        private static int _NoOfColumns = 49;

        private static int _MaxNoOfRows = 200; // default is 200 rows, but configurable

        public static DataTable ReadExcelContent(Stream excelMS)
        {
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["BulkRequestMaxRows"]))
            {
                int.TryParse(ConfigurationManager.AppSettings["BulkRequestMaxRows"].Trim(), out _MaxNoOfRows);
            }

            DataTable xlsDT = null;
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                IWorkbook wb = application.Workbooks.Open(excelMS);
                if (wb.Worksheets.Count > 0)
                {
                    IWorksheet sheet = wb.Worksheets[0];
                    xlsDT = sheet.ExportDataTable(1, 1, _MaxNoOfRows + 1, _NoOfColumns, ExcelExportDataTableOptions.PreserveOleDate);
                }
            }
            return xlsDT;
        }

        public static DateTime? ToDateTime(object xlsOleDate)
        {
            DateTime? dte = null;
            if (xlsOleDate.GetType() == typeof(string))
            {
                if (!string.IsNullOrWhiteSpace((string)xlsOleDate))
                {
                    double oleNum = 0;
                    if (double.TryParse(((string)xlsOleDate).Trim(), out oleNum))
                    {
                        dte = DateTime.FromOADate(oleNum);
                    }
                }
            }
            return dte;
        }

    }
}
