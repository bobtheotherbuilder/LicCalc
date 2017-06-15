using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using System;
using System.Windows.Threading;
using System.Linq;
using System.ComponentModel;

namespace LicenseCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Background worker is responsible for reading selected file and calculation  
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int targetAppId = 374;
        private static List<AppInstallInfo> targetAppList;
        private static DispatcherTimer t;
        private static BackgroundWorker worker;
        private static DateTime readStartTime;
        string filePath, errorMsg;

        public MainWindow()
        {
            InitializeComponent();
            targetAppList = new List<AppInstallInfo>();

            worker = new BackgroundWorker();
            worker.DoWork += ReadFile;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void ReadFile(object o, EventArgs args)
        {
            targetAppList = new List<AppInstallInfo>();
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string headerline = reader.ReadLine();
                    int appIdIndex = GetAppIdIndexFromHeaderLine(headerline);

                    while (!reader.EndOfStream)
                    {
                        parseLine(appIdIndex, targetAppList, reader.ReadLine());
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
        }

        // Calculate License or show error if any
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            t.Stop();
            btnBrowse.IsEnabled = true;

            if (string.IsNullOrEmpty(errorMsg))
            {
                var newList = targetAppList.GroupBy(x => x.UserID)
                    .Select(g =>
                                new {
                                    ID = g.First().UserID,
                                    Count = CountLicNum(g.Count(y => y.ComputerType == "computer"), g.Count(y => y.ComputerType == "laptop"))
                                });

                txtResult.Text = $"Total license needed: {newList.Sum(x => x.Count).ToString()}";
            }
            else
            {
                txtResult.Text = errorMsg;
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            errorMsg = string.Empty;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "CSV file (.csv)|*.csv";

            if (dlg.ShowDialog() == true)
            {
                txtFileName.Text = dlg.FileName;
                txtResult.Text = string.Empty;
                filePath = txtFileName.Text;
                btnRead.IsEnabled = true;
            }
        }

        private void btnRead_ClickAsync(object sender, RoutedEventArgs e)
        {
            btnRead.IsEnabled = false;
            btnBrowse.IsEnabled = false;

            if (!File.Exists(txtFileName.Text))
            {
                ResetWithMessage("File does not exist.");
                return;
            }

            t = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal,
                ShowProcessTime, Dispatcher.CurrentDispatcher);
            readStartTime = DateTime.Now;

            worker.RunWorkerAsync();
        }


        /// <summary>
        /// Logic to calculate total licenses required for a given user
        /// </summary>
        /// <param name="numOfPC"></param>
        /// <param name="numOfLaptop"></param>
        /// <returns></returns>
        public int CountLicNum(int numOfPC, int numOfLaptop)
        {
            if (numOfLaptop <= numOfPC)
                return numOfPC;

            return numOfPC + (int)Math.Ceiling((decimal)(numOfLaptop - numOfPC) / 2);
        }

        // this can actually be ommitted 
        private static int GetAppIdIndexFromHeaderLine(string headerline)
        {
            string[] headers = headerline.Split(',');

            int appIdIndex = 0;
            while (appIdIndex < headers.Length)
            {
                if (headers[appIdIndex].ToLower() == "applicationid")
                {
                    break;
                }
                appIdIndex++;
            }

            return appIdIndex;
        }

        // assume the headers are in the same order as given in the sample data file 
        private void parseLine(int appIdIndex, List<AppInstallInfo> installs, string line)
        {
            int appId;
            string[] lineData = line.Split(',');
            int.TryParse(lineData[appIdIndex], out appId);

            if (appId == targetAppId)
            {
                try
                {
                    var install = new AppInstallInfo()
                    {
                        ComputerID = int.Parse(lineData[0]),
                        UserID = int.Parse(lineData[1]),
                        ComputerType = lineData[3].Trim().ToLower(),
                    };
                    if (!installs.Contains(install))
                    {
                        installs.Add(install);
                    }
                }
                catch (Exception)
                {
                    // skip invalid row
                }
            }
        }

        private void ResetWithMessage(string message)
        {
            txtResult.Text = message;
            txtFileName.Text = "";
            btnBrowse.IsEnabled = true;
            btnRead.IsEnabled = false;
        }

        private void ShowProcessTime(object sender, EventArgs e)
        {
            txtResult.Text = ShowTime();
        }

        private string ShowTime()
        {
            return $"Busy Processing: { DateTime.Now.Subtract(readStartTime).ToString(@"hh\:mm\:ss")}";
        }

    }
}
