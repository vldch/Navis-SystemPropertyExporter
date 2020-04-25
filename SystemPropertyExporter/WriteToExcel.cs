﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Internal.ApiImplementation;
using Autodesk.Navisworks.Api.Automation;
using Autodesk.Navisworks.Api.Plugins;
using System.Collections.ObjectModel;
using SystemPropertyExporter;

namespace SystemPropertyExporter
{
    class WriteToExcel
    {
//        //public static string Curr1 {get; set;}
        //public static string Curr2 { get; set; }
        //public static string Curr3 { get; set; }
        //public static string Curr4 { get; set; }
        //public static string Curr5 { get; set; }
        //public static string Curr6 { get; set; }
//        //public static string Check { get; set; }
        public static int IdxCounter { get; set; }

        public static void ExcelReport()
        {
            try
            {
//MessageBox.Show($"{ExportProperties.ExportItems.Count}, {ExportProperties.ExportItems[0]}, {ExportProperties.ItemIdx.Count}, {ExportProperties.ExportProp.Count}, {ExportProperties.ExportVal.Count}");
                //MessageBox.Show(ExportProperties.ItemIdx[3796].ToString());
//                //MessageBox.Show(ExportProperties.ItemIdx[2845].ToString());
                //Launch or access Excel via COM Interop:
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkbook;

                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed!");
                }

                //Create New Workbook & Worksheets
                xlWorkbook = xlApp.Workbooks.Add(Missing.Value);
                Excel.Worksheet xlWorksheet = (Excel.Worksheet)xlWorkbook.Worksheets.Add(
                    Type.Missing,Type.Missing, ExportProperties.UserItems.Count+1, Type.Missing);
        
                int rowNum = 2;
                int colNum = 7 ;
                int modelIdx = 0;
                IdxCounter = 0;
                bool match = false;

                foreach (Export item in ExportProperties.ExportItems)
                {
                    //MessageBox.Show($"{item.ExpDiscipline}, {item.ExpModFile}, {item.ExpHierLvl}, {item.ExpCategory}");
                    //Excel.Worksheet xlWorksheet;
                    //TRY NEXT
                    foreach (Excel.Worksheet sheet in xlWorkbook.Worksheets)
                    {
                        if (sheet.Name == item.ExpDiscipline) /*-{item.ExpCategory}")*/
                        {
                            match = true;
                            xlWorksheet = (Excel.Worksheet)xlWorkbook.Sheets[sheet.Name];
                            xlWorksheet.Select();
                            xlWorksheet.Activate();

                            //IF DISCIPLINE SHEET NAME MATCHES, SELECTS NEXT BLANK ROW TO START STORING VALUES
                            Excel.Range last = xlWorksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
                            rowNum = last.Row + 1;
                            break;
                        }
                    }

                    if (match == false)
                    {
                        //xlWorksheet = (Excel.Worksheet)xlWorkbook.Worksheets.Add();
                        xlWorksheet = (Excel.Worksheet)xlWorkbook.Sheets[modelIdx + 1];
                        xlWorksheet.Select();
                        xlWorksheet.Activate();

                        xlWorksheet.Name = item.ExpDiscipline; /*{item.ExpCategory}";*/
                        xlWorksheet.Cells[1, 1] = "DISCIPLINE";
                        xlWorksheet.Cells[1, 2] = "MODEL FILE NAME";
                        xlWorksheet.Cells[1, 3] = "HIERARCHY LEVEL";
                        xlWorksheet.Cells[1, 4] = "CATEGORY";
                        xlWorksheet.Cells[1, 5] = "ELEMENT NAME";
                        xlWorksheet.Cells[1, 6] = "ELEMENT GUID";

                        modelIdx++;
                        //REST BACK TO FIRST ROW FOR STORING ON NEXT WORKSHEET
                        rowNum = 2;
                        //MessageBox.Show($"{currItemDis}, {xlWorksheet.Index}, {xlWorksheet.Name}");
                    }
                   
                    //bool first = true;
                    colNum = 7;

                    //write properties to excel file
                    string cellDis = "A" + rowNum.ToString();
                    var rangeDis = xlWorksheet.get_Range(cellDis, cellDis);
                    rangeDis.Value2 = item.ExpDiscipline;

                    string cellModFile = "B" + rowNum.ToString();
                    var rangeModFile = xlWorksheet.get_Range(cellModFile, cellModFile);
                    rangeModFile.Value2 = item.ExpModFile;

                    string cellHiLvl = "C" + rowNum.ToString();
                    var rangeHiLvl = xlWorksheet.get_Range(cellHiLvl, cellHiLvl);
                    rangeHiLvl.Value2 = item.ExpHierLvl;
                    
                    string cellCat = "D" + rowNum.ToString();
                    var rangeCat = xlWorksheet.get_Range(cellCat, cellCat);
                    rangeCat.Value2 = item.ExpCategory;
                    
                    string cellName = "E" + rowNum.ToString();
                    var rangeName = xlWorksheet.get_Range(cellName, cellName);
                    rangeName.Value2 = item.ItemName;

                    string cellId = "F" + rowNum.ToString();
                    var rangeId = xlWorksheet.get_Range(cellId, cellId);
                    rangeId.Value2 = item.ExpGuid;

                    //----------------------------------------------------------------------------------------------

                    int indexMatch = ExportProperties.ExportItems.IndexOf(item);
                    var currRange = PropRange(indexMatch);
                    int idxMin = currRange.iMin;
                    int idxMax = currRange.iMax;
//                    // Curr1 = item.ExpDiscipline;
                    //Curr2 = idxMin.ToString();
                    //Curr3 = idxMax.ToString();
                    //Curr4 = indexMatch.ToString();
//                    //Check = "flag 3";
                    for (int i = idxMin; i <= idxMax; i++)
                    {
                        //var rangeProp = (Excel.Range)xlWorksheet.Cells[1, colNum]; //range using # (int) for column?
                        //rangeProp.Value2 = "Property - " + ExportProperties.ExportProp[i];

                        var rangeVal = (Excel.Range)xlWorksheet.Cells[rowNum, colNum]; //range using # (int) for column?
                        rangeVal.Value2 = $"PROPERTY: {ExportProperties.ExportProp[i]}_____VALUE: {ExportProperties.ExportVal[i]}";
//                      Check = "flag 5"; 
                        colNum++;
                    }
                   
                    match = false;
                    rowNum++;
                }
                
                    //Locate file save location
                    string exportYr = DateTime.Now.Year.ToString();
                    string exportMonth = DateTime.Now.Month.ToString();
                    string exportDay = DateTime.Now.Day.ToString();

                    if (exportMonth.Length == 1)
                    {
                        exportMonth = "0" + exportMonth;
                    }

                    if (exportDay.Length == 1)
                    {
                        exportDay = "0" + exportDay;
                    }

                    string exportDate = exportYr + exportMonth + exportDay;

                    SaveFileDialog saveModelProperties = new SaveFileDialog();

                    saveModelProperties.Title = "Save to...";
                    saveModelProperties.Filter = "Excel Workbook | *.xlsx|Excel 97-2003 Workbook | *.xls";
                    saveModelProperties.FileName = exportDate + "-System_Property_Data";

                    if (saveModelProperties.ShowDialog() == DialogResult.OK)
                    {
                        string path = saveModelProperties.FileName;
                        xlWorkbook.SaveCopyAs(path);
                        xlWorkbook.Saved = true;
                        xlWorkbook.Close(true, Missing.Value, Missing.Value);
                        xlApp.Quit();
                        
                    }

                    xlApp.Visible = false;
            }
            catch (Exception exception)
            {
//                MessageBox.Show($"{Curr1}, {Curr2}, {Curr3}, {Curr4}, {Curr5}, {Curr6}, {Check}");
                MessageBox.Show("Error! Check if clash test(s) exist or previously run.  Original Message: " + exception.Message);
            }
        }


        private static (int iMin, int iMax) PropRange(int indexMatch)
        {
            int iMin=-1;
            int iMax=-1;
            bool firstMatch = true;

            //FOR ALL OTHER CASES
            for (int i = IdxCounter; i < ExportProperties.ItemIdx.Count; i++)
            {
                    if (indexMatch == ExportProperties.ItemIdx[i])
                    {
                        if (firstMatch == true)
                        {
                            iMin = i;
                            iMax = i;
                            firstMatch = false;
                        }
                        else
                        {
                            if (i > iMax)
                            {
                                iMax = i;
                                IdxCounter = i;
                            }
                        }
                    }
//                //Curr5 = i.ToString();
//                //Curr6 = ExportProperties.ItemIdx[67700].ToString();
                }
            
            return (iMin, iMax);
        }


    }
}
