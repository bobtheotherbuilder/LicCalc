using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LicenseCalculator;
using System.Collections.Generic;
using System.Linq;

namespace AppLicenseCalculatorUnitTest
{
    [TestClass]
    public class AppLicenseCalculationTest
    {
        private MainWindow window;
        private List<AppInstallInfo> installs;

        [TestInitialize]
        public void Setup()
        {
            window = new MainWindow();
            installs = new List<AppInstallInfo>();
        }

        [TestCleanup]
        public void TearDown()
        {
            window.Close();
        }

        [TestMethod]
        public void TestFundamentalAlgorithmEqualTypes()
        {
            installs.Add(new AppInstallInfo { ComputerID = 1, UserID = 1, ComputerType = "LAPTOP" });
            installs.Add(new AppInstallInfo { ComputerID = 2, UserID = 1, ComputerType = "DESKTOP" });
            int numOfPC = installs.Count(d => d.ComputerType.ToLower() == "desktop");
            int numOfLaptop = installs.Count(d => d.ComputerType.ToLower() == "laptop");
            Assert.AreEqual(1, window.CountLicNum(numOfPC, numOfLaptop));
        }

        [TestMethod]
        public void TestFundamentalAlgorithmLessLaptop()
        {
            installs.Add(new AppInstallInfo { ComputerID = 1, UserID = 1, ComputerType = "LAPTOP" });
            installs.Add(new AppInstallInfo { ComputerID = 2, UserID = 1, ComputerType = "DESKTOP" });
            installs.Add(new AppInstallInfo { ComputerID = 3, UserID = 1, ComputerType = "DESKTOP" });
            int numOfPC = installs.Count(d => d.ComputerType.ToLower() == "desktop");
            int numOfLaptop = installs.Count(d => d.ComputerType.ToLower() == "laptop");
            Assert.AreEqual(2, window.CountLicNum(numOfPC, numOfLaptop));
        }

        [TestMethod]
        public void TestFundamentalAlgorithmEqualMoreLaptop()
        {
            installs.Add(new AppInstallInfo { ComputerID = 1, UserID = 1, ComputerType = "LAPTOP" });
            installs.Add(new AppInstallInfo { ComputerID = 3, UserID = 1, ComputerType = "LAPTOP" });
            installs.Add(new AppInstallInfo { ComputerID = 2, UserID = 1, ComputerType = "DESKTOP" });
            installs.Add(new AppInstallInfo { ComputerID = 4, UserID = 1, ComputerType = "LAPTOP" });
            installs.Add(new AppInstallInfo { ComputerID = 5, UserID = 1, ComputerType = "DESKTOP" });
            int numOfPC = installs.Count(d => d.ComputerType.ToLower() == "desktop");
            int numOfLaptop = installs.Count(d => d.ComputerType.ToLower() == "laptop");
            Assert.AreEqual(3, window.CountLicNum(numOfPC, numOfLaptop));
        }
    }
}

