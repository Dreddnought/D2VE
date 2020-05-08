using System.IO;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace D2VE
{
    public class ExcelOutput : IOutput
    {
        public ExcelOutput() { }
        public void Create(OutputContext outputContext, Dictionary<string, Category> data)
        {
            if (!Directory.Exists(outputContext.Folder))
                Directory.CreateDirectory(outputContext.Folder);
            string fileName = Path.Combine(outputContext.Folder, outputContext.SpreadsheetName + ".xlsx");
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook))
            {
                // Add a WorkbookPart to the document.
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Test Sheet" };
                sheets.Append(sheet);
                workbookPart.Workbook.Save();
            }
        }
    }
}
