using System.Text;

namespace D2VE;

public class CsvOutput : IOutput
{
    public void Create(OutputContext outputContext, Dictionary<string, Category> data)
    {
        if (!Directory.Exists(outputContext.Folder))
            Directory.CreateDirectory(outputContext.Folder);
        string folderName = Path.Combine(outputContext.Folder, outputContext.SpreadsheetName);
        Directory.CreateDirectory(folderName);
        foreach (Category category in data.Values)
        {
            string fileName = Path.Combine(folderName, category.Name + ".csv");
            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                // Add header row.
                streamWriter.WriteLine(string.Join(",", category.ColumnNames));
                StringBuilder csvLine = new StringBuilder();
                foreach (object[] row in category.Rows)
                {
                    foreach (object o in row)
                    {
                        if (o != null)
                            if (o.GetType() == typeof(long))
                                csvLine.Append(o);
                            else  // string
                            {
                                csvLine.Append("\"");
                                csvLine.Append(o);
                                csvLine.Append("\"");
                            }
                        csvLine.Append(",");
                    }
                    csvLine.Remove(csvLine.Length - 1, 1);  // Remove trailing comma
                    streamWriter.WriteLine(csvLine.ToString());
                    csvLine.Clear();
                }
            }
        }
    }
}
