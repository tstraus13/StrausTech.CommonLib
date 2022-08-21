using System.Reflection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace StrausTech.CommonLib;

public class Document
{
    /// <summary>
    /// Creates a xlsx document based on the provided data.
    /// </summary>
    /// <typeparam name="T">The type of the data object</typeparam>
    /// <param name="rows">An IEnumerable collection of data</param>
    /// <param name="filepath">The full output path, with extension included.</param>
    /// <param name="sheetName">The name of the excel sheet in the workbook.</param>
    /// <param name="columnMappings">Key = Property Name; Value = Column Header.</param>
    /// <returns></returns>
    public static string CreateExcelFile<T>(List<T> rows, string filepath, string sheetName, Dictionary<string, string> columnMappings = null)
    {
        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);
        // Add a WorkbookPart to the document.
        WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
        workbookpart.Workbook = new Workbook();

        // Add a WorksheetPart to the WorkbookPart.
        WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());

        // Add Sheets to the Workbook.
        Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
            AppendChild<Sheets>(new Sheets());

        // Append a new worksheet and associate it with the workbook.
        Sheet sheet = new Sheet()
        {
            Id = spreadsheetDocument.WorkbookPart.
            GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = sheetName
        };
        sheets.Append(sheet);

        Worksheet worksheet = worksheetPart.Worksheet;
        SheetData sheetData = worksheet.GetFirstChild<SheetData>();

        int c = 0;
        uint r = 1;
        if (!columnMappings.Any()) //Populate columnMappings from the given type, if no mapping was supplied.
        {
            columnMappings = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p.Name);
        }

        var eRow = new Row() { RowIndex = r };
        //Build the header row
        foreach (var columnMapping in columnMappings)
        {
            var cell = new Cell();
            cell.CellValue = new CellValue(columnMapping.Value);
            cell.DataType = new EnumValue<CellValues>(CellValues.String);
            eRow.InsertAt(cell, c);
            c++;
        }
        sheetData.Append(eRow);

        ////Insert and populate each cell from the data
        r = 2; 
        foreach (var row in rows)
        {
            eRow = new Row() { RowIndex = r };

            c = 0;
            foreach (var columnMapping in columnMappings)
            {
                var cell = new Cell();
                var type = row.GetType().GetProperty(columnMapping.Key);

                //Check if property is a collection.Strings are considered Ienumerable
                if (row.GetType().GetProperty(columnMapping.Key).GetValue(row, null) is System.Collections.IList list && list.GetType().Name != "String")
                {
                    List<string> vals = new List<string>();
                    foreach (var item in list)
                    {
                        vals.Add(item.ToString());
                    }
                    cell.CellValue = new CellValue(string.Join(",", vals));
                }
                else
                {
                    cell.CellValue = new CellValue(row.GetType().GetProperty(columnMapping.Key).GetValue(row, null)?.ToString() ?? "");
                }
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                eRow.InsertAt(cell, c);
                c++;
            }
            sheetData.Append(eRow);
            r++;
        }

        worksheetPart.Worksheet.Save();
        spreadsheetDocument.Close();
        return filepath;
    }

    public static string CreateExcelFile<T>(IEnumerable<T> rows, string filepath, string sheetName, Dictionary<string, string> columnMappings = null)
    {
        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);
        // Add a WorkbookPart to the document.
        WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
        workbookpart.Workbook = new Workbook();

        // Add a WorksheetPart to the WorkbookPart.
        WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());

        // Add Sheets to the Workbook.
        Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
            AppendChild<Sheets>(new Sheets());

        // Append a new worksheet and associate it with the workbook.
        Sheet sheet = new Sheet()
        {
            Id = spreadsheetDocument.WorkbookPart.
            GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = sheetName
        };
        sheets.Append(sheet);

        Worksheet worksheet = worksheetPart.Worksheet;
        SheetData sheetData = worksheet.GetFirstChild<SheetData>();

        int c = 0;
        uint r = 1;
        if (!columnMappings.Any()) //Populate columnMappings from the given type, if no mapping was supplied.
        {
            columnMappings = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p.Name);
        }

        var eRow = new Row() { RowIndex = r };
        //Build the header row
        foreach (var columnMapping in columnMappings)
        {
            var cell = new Cell();
            cell.CellValue = new CellValue(columnMapping.Value);
            cell.DataType = new EnumValue<CellValues>(CellValues.String);
            eRow.InsertAt(cell, c);
            c++;
        }
        sheetData.Append(eRow);

        ////Insert and populate each cell from the data
        r = 2;
        foreach (var row in rows)
        {
            eRow = new Row() { RowIndex = r };

            c = 0;
            foreach (var columnMapping in columnMappings)
            {
                var cell = new Cell();
                var type = row.GetType().GetProperty(columnMapping.Key);

                //Check if property is a collection.Strings are considered Ienumerable
                if (row.GetType().GetProperty(columnMapping.Key).GetValue(row, null) is System.Collections.IList list && list.GetType().Name != "String")
                {
                    List<string> vals = new List<string>();
                    foreach (var item in list)
                    {
                        vals.Add(item.ToString());
                    }
                    cell.CellValue = new CellValue(string.Join(",", vals));
                }
                else
                {
                    cell.CellValue = new CellValue(row.GetType().GetProperty(columnMapping.Key).GetValue(row, null)?.ToString() ?? "");
                }
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                eRow.InsertAt(cell, c);
                c++;
            }
            sheetData.Append(eRow);
            r++;
        }

        worksheetPart.Worksheet.Save();
        spreadsheetDocument.Close();
        return filepath;
    }

    public static string CreateCsvFile<T>(List<T> items, string filePath)
    {
        Type itemType = typeof(T);

        var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine(string.Join(",", props.Select(p => p.Name)));

            foreach (var item in items)
                writer.WriteLine(string.Join(",", props.Select(p => p.GetValue(item, null))));
        }

        return filePath;
    }

    public static string CreateCsvFile<T>(IEnumerable<T> items, string filePath)
    {
        Type itemType = typeof(T);

        var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine(string.Join(",", props.Select(p => p.Name)));

            foreach (var item in items)
                writer.WriteLine(string.Join(",", props.Select(p => p.GetValue(item, null))));
        }

        return filePath;
    }
}