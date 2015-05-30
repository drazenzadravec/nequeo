using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;

namespace ManagementTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Nequeo.Management.PowerShell.OperatingSystem vv = new Nequeo.Management.PowerShell.OperatingSystem();
            Nequeo.Management.Sys.Information hh = vv.Information;
        }

        [TestMethod]
        public void TestMethod2()
        {
            Nequeo.Management.Cpu.Usage usage = new Nequeo.Management.Cpu.Usage();
            List<PerformanceCounter> data = usage.Individual();
        }

        [TestMethod]
        public void TestMethod3()
        {
            Nequeo.Management.Cpu.Usage usage = new Nequeo.Management.Cpu.Usage();
            List<string> processorID = null;
            List<ulong> data = usage.Active(ref processorID);
        }
    }
}
