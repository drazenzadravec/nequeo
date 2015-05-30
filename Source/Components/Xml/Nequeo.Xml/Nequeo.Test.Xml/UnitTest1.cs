using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nequeo.Test.Xml
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(@"C:\Temp\Form.xml");
            IEnumerable<System.Xml.Linq.XNode> nodes = doc.Nodes();
            System.Xml.Linq.XNode next = nodes.Last();
            System.Xml.Linq.XDocument docx = new System.Xml.Linq.XDocument(next);
            System.Xml.Linq.XName xname = System.Xml.Linq.XName.Get("statusReport", "http://schemas.microsoft.com/office/infopath/2003/myXSD/2005-09-22T20:42:56");
            System.Xml.Linq.XName xnames = System.Xml.Linq.XName.Get("reportDate", "http://schemas.microsoft.com/office/infopath/2003/myXSD/2005-09-22T20:42:56");
            System.Xml.Linq.XElement hh = docx.Element(xname).Element(xnames);
            string data = hh.Value;
        }
    }
}
