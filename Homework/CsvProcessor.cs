using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using CsvHelper;
using WpfApp1.configuration;
using System.Windows;
using System.Linq;

namespace CsvXmlProcessor
{
    public partial class MainWindow : Window
    {
        private void ProcessCsv()
        {
            ExportDataList = new List<ExportData>();

            using (TextFieldParser parser = new TextFieldParser(CsvFilePath))
            {
                parser.SetDelimiters(";");
                parser.HasFieldsEnclosedInQuotes = true;

                parser.ReadLine();

                int counter = 0;
                while (!parser.EndOfData)
                {
                    counter++;
                    string[] fields = parser.ReadFields();
                    if (fields.Length >= 17)
                    {
                        var address = fields[14];
                        if (address != "")
                        {
                            var district = GetDistrictName(fields[14]);

                            ExportData exportData = new ExportData
                            {
                                RegCode = fields[0],
                                Name = fields[2],
                                Address = fields[14],
                                District = district
                            };

                            FindAtvk(exportData, counter, string.Join(",", fields));
                        }
                        else
                        {
                            LogError("CSV Address is empty", string.Join(",", fields), counter);
                        }
                    }
                }
                WriteToCsv();
                MoveAndRenameFile(Config.ExportTempPath, "output");
            }
        }

        private void MoveAndRenameFile(string filePath, string type)
        {
            FileInfo fileInfo = new System.IO.FileInfo(filePath);
            string fileDirectory;

            if (fileInfo.Exists)
            {
                if (type == "output")
                {
                    fileDirectory = Path.Combine(Config.ExportPath, $"{type}_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.csv");
                    fileInfo.MoveTo(fileDirectory);
                    TxtCsvDirectory.Visibility = Visibility.Visible;
                    TxtCsvDirectory.Text = $"CSV File generated at: {fileDirectory}";
                }
                if (type == "log")
                {
                    fileDirectory = Path.Combine(Config.LogPath, $"{type}_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.txt");
                    fileInfo.MoveTo(fileDirectory);
                    TxtLogErrors.Visibility = Visibility.Visible;
                    TxtLogErrors.Text = $"There were some errors. See log: {fileDirectory}";
                }
            }
        }

        private void LogError(string message, string fields, int lineNumber)
        {
            lock (LockObject)
            {
                if (LogWriter == null || LogWriter.BaseStream == null || LogWriter.BaseStream.CanWrite == false)
                {
                    LogWriter?.Close();
                    LogWriter = new StreamWriter(Config.LogTempPath, true);
                }

                var line = $"ERROR: {message} at line {lineNumber}: {fields}";

                LogWriter.WriteLine(line);
                LogWriter.Flush();
            }
        }

        private void FindAtvk(ExportData exportData, int counter, string fields)
        {
            var matchingDistrict = DistrictInfoList.FirstOrDefault(d => d.DistrictName == exportData.District);

            if (matchingDistrict != null)
            {
                exportData.DistrictAtvk = matchingDistrict.DistrictAtvk;
                ExportDataList.Add(exportData);
            }
            else
            {
                LogError("CSV No matching address", fields, counter);
            }
        }

        private string GetDistrictName(string address)
        {
            var abbreviation = "nov.";

            var districtName = address.Trim('"');
            if (districtName.Contains(","))
            {
                districtName = districtName.Substring(0, address.IndexOf(","));
            }

            if (districtName.EndsWith(abbreviation))
            {
                districtName = districtName.Replace(abbreviation, "novads");
            }

            return districtName;
        }

        private void WriteToCsv()
        {
            using (var writer = new StreamWriter(Config.ExportTempPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<ExportData>();
                csv.NextRecord();
                csv.WriteRecords(ExportDataList);
            }
        }
    }
}