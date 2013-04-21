using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XmlParserLib.Tools
{
    public class FolderReader
    {
        public string FolderPath { get; set; }
        public List<string> Files { get; set; }

        public FolderReader(string folderPath)
        {
            FolderPath = folderPath;
            Files = Directory.GetFiles(FolderPath, "*.xml").ToList<string>();
        }
    }
}
