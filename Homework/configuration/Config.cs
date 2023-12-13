using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.configuration
{
    public sealed class Config
    {
        public static string SolutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        public static string LogTempPath = $"{SolutionDirectory}\\output\\temp\\log.txt";
        public static string ExportTempPath = $"{SolutionDirectory}\\output\\temp\\output.csv";
        public static string LogPath = $"{SolutionDirectory}\\output\\logs\\";
        public static string ExportPath = $"{SolutionDirectory}\\output\\";
    }
}
