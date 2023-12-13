using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Win32;
using Microsoft.VisualBasic.FileIO;
using CsvHelper;
using System.Globalization;
using WpfApp1.configuration;

namespace CsvXmlProcessor
{
    public partial class MainWindow : Window
    {
        private List<DistrictInfo> DistrictInfoList;
        private List<ExportData> ExportDataList;
        private string CsvFilePath;
        private static readonly object LockObject = new object();
        private static StreamWriter LogWriter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpenCsv_Click(object sender, RoutedEventArgs e)
        {
            OpenCsvFileDialog();
        }

        private void BtnOpenXml_Click(object sender, RoutedEventArgs e)
        {
            OpenXmlFileDialog();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            ExportData();
        }

        private void OpenCsvFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CsvFilePath = openFileDialog.FileName;
                if (CsvFilePath != null)
                {
                    TxtCsvCheck.Visibility = Visibility.Visible;
                    TxtError.Visibility = Visibility.Hidden;
                }
            }
        }

        private void OpenXmlFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML files (*.xml)|*.xml"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ReadXml(openFileDialog.FileName);
            }
        }

        private void ExportData()
        {
            if (DistrictInfoList != null && CsvFilePath != null)
            {
                ProcessCsv();
                LogWriter.Close();
                MoveAndRenameFile(Config.LogTempPath, "log");
            }
            else
            {
                TxtError.Visibility = Visibility.Visible;
                if (DistrictInfoList == null)
                {
                    TxtError.Text = "XML File is not loaded";
                }
                if (CsvFilePath == null)
                {
                    TxtError.Text = "CSV file is not loaded";
                }
                if (DistrictInfoList == null && CsvFilePath == null)
                {
                    TxtError.Text = "CSV and XML files are not loaded";
                }
            }
        }
    }
}
