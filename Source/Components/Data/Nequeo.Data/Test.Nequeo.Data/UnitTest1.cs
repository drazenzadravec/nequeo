using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Nequeo.Data
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Func<bool, bool, int> test = (one, two) => FuncMethod(one, two);
            int ret = test(true, false);
        }

        private Action<int, bool, string> Shit {get; set;}

        private int FuncMethod(bool one, bool two)
        {
            Shit = (a, b, c) => ActionMethod(a, b, c);
            Shit(5, true, "fgfg");
            return 5;
        }

        private void ActionMethod(int one, bool two, string three)
        {

        }
    }
}
