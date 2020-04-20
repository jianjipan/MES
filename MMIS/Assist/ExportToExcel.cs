using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;

namespace MMIS
{
    public class ExportToExcel
    {
        public bool Export(DataGrid dataGrid, string excelTitle)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (dataGrid.Columns[i].Visibility == System.Windows.Visibility.Visible)//只导出可见列  
                {
                    dt.Columns.Add(dataGrid.Columns[i].Header.ToString());//构建表头  
                }
            }

            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                int columnsIndex = 0;
                System.Data.DataRow row = dt.NewRow();
                for (int j = 0; j < dataGrid.Columns.Count; j++)
                {
                    if (dataGrid.Columns[j].Visibility == System.Windows.Visibility.Visible)
                    {
                        if (dataGrid.Items[i] != null && (dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock) != null)//填充可见列数据  
                        {
                            row[columnsIndex] = (dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock).Text.ToString();
                        }
                        else row[columnsIndex] = "";

                        columnsIndex++;
                    }
                }
                dt.Rows.Add(row);
            }

            if (ExcelExport(dt, excelTitle) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ExcelExport(System.Data.DataTable DT, string title)
        {
            try
            {
                //创建Excel
                Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook ExcelBook = ExcelApp.Workbooks.Add(System.Type.Missing);
                //创建工作表（即Excel里的子表sheet） 1表示在子表sheet1里进行数据导出
                Microsoft.Office.Interop.Excel.Worksheet ExcelSheet = (Microsoft.Office.Interop.Excel.Worksheet)ExcelBook.Worksheets[1];

                //如果数据中存在数字类型 可以让它变文本格式显示
                ExcelSheet.Cells.NumberFormat = "@";

                //设置工作表名
                ExcelSheet.Name = title;

                //设置Sheet标题
                string start = "A1";
                string end = ChangeASC(DT.Columns.Count) + "1";

                Microsoft.Office.Interop.Excel.Range _Range = (Microsoft.Office.Interop.Excel.Range)ExcelSheet.get_Range(start, end);
                _Range.Merge(0);                     //单元格合并动作(要配合上面的get_Range()进行设计)
                _Range = (Microsoft.Office.Interop.Excel.Range)ExcelSheet.get_Range(start, end);
                _Range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                _Range.Font.Size = 22; //设置字体大小
                _Range.Font.Name = "宋体"; //设置字体的种类 
                ExcelSheet.Cells[1, 1] = title;    //Excel单元格赋值
                _Range.EntireColumn.AutoFit(); //自动调整列宽

                //写表头
                for (int m = 1; m <= DT.Columns.Count; m++)
                {
                    ExcelSheet.Cells[2, m] = DT.Columns[m - 1].ColumnName.ToString();

                    start = "A2";
                    end = ChangeASC(DT.Columns.Count) + "2";

                    _Range = (Microsoft.Office.Interop.Excel.Range)ExcelSheet.get_Range(start, end);
                    _Range.Font.Size = 15; //设置字体大小
                    _Range.Font.Bold = true;//加粗
                    _Range.Font.Name = "宋体"; //设置字体的种类  
                    _Range.EntireColumn.AutoFit(); //自动调整列宽 
                    _Range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                }

                //写数据
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    for (int j = 1; j <= DT.Columns.Count; j++)
                    {
                        //Excel单元格第一个从索引1开始
                        // if (j == 0) j = 1;
                        ExcelSheet.Cells[i + 3, j] = DT.Rows[i][j - 1].ToString();
                    }
                }

                //表格属性设置
                for (int n = 0; n < DT.Rows.Count + 1; n++)
                {
                    start = "A" + (n + 3).ToString();
                    end = ChangeASC(DT.Columns.Count) + (n + 3).ToString();

                    //获取Excel多个单元格区域
                    _Range = (Microsoft.Office.Interop.Excel.Range)ExcelSheet.get_Range(start, end);

                    _Range.Font.Size = 12; //设置字体大小
                    _Range.Font.Name = "宋体"; //设置字体的种类

                    _Range.EntireColumn.AutoFit(); //自动调整列宽
                    _Range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter; //设置字体在单元格内的对其方式 _Range.EntireColumn.AutoFit(); //自动调整列宽 
                }

                ExcelApp.DisplayAlerts = false; //保存Excel的时候，不弹出是否保存的窗口直接进行保存 

                //弹出保存对话框,并保存文件
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.DefaultExt = ".xlsx";
                sfd.Filter = "Office 2007 File|*.xlsx|Office 2000-2003 File|*.xls|所有文件|*.*";
                if (sfd.ShowDialog() == true)
                {
                    if (sfd.FileName != "")
                    {
                        ExcelBook.SaveAs(sfd.FileName);  //将其进行保存到指定的路径
                        //    MessageBox.Show("导出文件已存储为: " + sfd.FileName, "温馨提示");
                    }
                }

                //释放可能还没释放的进程
                ExcelBook.Close();
                ExcelApp.Quit();
                // PubHelper.Instance.KillAllExcel(ExcelApp);

                return sfd.FileName;
            }
            catch
            {
                //   MessageBox.Show("导出文件保存失败！", "警告！");
                return null;
            }
        }

        /// <summary>
        /// 获取当前列列名,并得到EXCEL中对应的列
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private string ChangeASC(int count)
        {
            string ascstr = "";

            switch (count)
            {
                case 1:
                    ascstr = "A";
                    break;
                case 2:
                    ascstr = "B";
                    break;
                case 3:
                    ascstr = "C";
                    break;
                case 4:
                    ascstr = "D";
                    break;
                case 5:
                    ascstr = "E";
                    break;
                case 6:
                    ascstr = "F";
                    break;
                case 7:
                    ascstr = "G";
                    break;
                case 8:
                    ascstr = "H";
                    break;
                case 9:
                    ascstr = "I";
                    break;
                case 10:
                    ascstr = "J";
                    break;
                case 11:
                    ascstr = "K";
                    break;
                case 12:
                    ascstr = "L";
                    break;
                case 13:
                    ascstr = "M";
                    break;
                case 14:
                    ascstr = "N";
                    break;
                case 15:
                    ascstr = "O";
                    break;
                case 16:
                    ascstr = "P";
                    break;
                case 17:
                    ascstr = "Q";
                    break;
                case 18:
                    ascstr = "R";
                    break;
                case 19:
                    ascstr = "S";
                    break;
                case 20:
                    ascstr = "Y";
                    break;
                default:
                    ascstr = "U";
                    break;
            }
            return ascstr;
        }
    }
    
}
