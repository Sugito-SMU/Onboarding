using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Syncfusion.XlsIO;

namespace Onboarding.ExcelWriter
{
    public static class ExcelWriter
    {
        public static MemoryStream WriteExcelContent(DataTable dt)
        {
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    IWorkbook wb = application.Workbooks.Create();
                    IWorksheet sheet = wb.ActiveSheet;
                    sheet.ImportDataTable(dt, true, 1, 1, true);
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (dt.Columns[i].DataType == typeof(DateTime))
                        {
                            for (int j = 0; j <= dt.Rows.Count; j++)
                            {
                                sheet.Range[j + 1, (i + 1)].CellStyle.NumberFormat = "d mmm yyyy";
                            }
                        }
                        sheet[1, i + 1].CellStyle.Font.Bold = true;
                    }
                    sheet.UsedRange.AutofitColumns();
                    wb.Version = ExcelVersion.Excel2007;
                    MemoryStream ms = new MemoryStream();
                    wb.SaveAs(ms);
                    wb.Close();
                    return ms;
                }
            }
            return null;
        }
    }
}
