using System.Collections.Generic;

namespace D2VE
{
    public interface IOutput
    {
        void Create(OutputContext outputContext, List<ItemInstance> items);
    }
    public class OutputContext
    {
        /// <summary>Folder for output files.</summary>
        public string Folder { get; set; }
        /// <summary>Name for output file.</summary>
        public string SpreadsheetName { get; set; }
    }
}
