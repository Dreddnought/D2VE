using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D2VE
{
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
                // Add header row.
                StringBuilder csv = new StringBuilder();
                csv.AppendLine(string.Join(",", category.ColumnNames));
                foreach (object[] row in category.Rows)
                {
                    foreach (object o in row)
                    {
                        if (o != null)
                            if (o.GetType() == typeof(long))
                                csv.Append(o);
                            else  // string
                            {
                                csv.Append("\"");
                                csv.Append(o);
                                csv.Append("\"");
                            }
                        csv.Append(",");
                    }
                    csv.Remove(csv.Length - 1, 1);  // Remove trailing comma
                    csv.AppendLine();
                }
                string fileName = Path.Combine(folderName, category.Name + ".csv");
                File.WriteAllText(fileName, csv.ToString());
            }
        }
    }
}
