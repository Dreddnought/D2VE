using System.Collections.Generic;

namespace D2VE
{
    public interface IOutput
    {
        void Create(OutputContext outputContext, Dictionary<string, Category> data);
    }
    public class OutputContext
    {
        /// <summary>Folder for output files.</summary>
        public string Folder { get; set; }
        /// <summary>Name for output file.</summary>
        public string SpreadsheetName { get; set; }
    }
    public class Category
    {
        public Category(string name) { Name = name; }
        public string Name { get; }
        public List<string> ColumnNames { get; } = new List<string>();
        public int ColumnIndex(string name)
        {
            int index = ColumnNames.IndexOf(name);
            if (index == -1)  // New column so add it
                ColumnNames.Add(name);
            return ColumnNames.IndexOf(name);
        }
        public List<object[]> Rows { get; } = new List<object[]>();
    }
}
