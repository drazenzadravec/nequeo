/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nequeo.ComponentModel.Design
{
    /// <summary>
    /// Code generation custome tool configuration
    /// </summary>
    public partial class CodeGeneration : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CodeGeneration()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="customToolXmlName">the custom too xml name is CustomToolXml resource type.</param>
        public CodeGeneration(string customToolXmlName)
        {
            InitializeComponent();

            switch (customToolXmlName)
            {
                case "EnumXml":
                    this.Text = "Enum Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.EnumCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.EnumXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.EnumHelp;
                    break;

                case "DataDatabaseXml":
                    this.Text = "Data Database Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataDatabaseCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataDatabaseXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataDatabaseHelp;
                    break;

                case "DataDatabaseCodeXml":
                    this.Text = "Data Database Model Xml Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataDatabaseCodeXmlCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataDatabaseCodeXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataDatabaseCodeXmlHelp;
                    break;

                case "DataDatabaseContextXml":
                    this.Text = "Data Database Context Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataDatabaseContextCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataDatabaseContextXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataDatabaseContextHelp;
                    break;

                case "DataContextExtensionXml":
                    this.Text = "Data Extension Context Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataContextExtensionCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataContextExtensionXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataContextExtensionHelp;
                    break;

                case "DataExtensionXml":
                    this.Text = "Data Extension Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataExtensionCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataExtensionXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataExtensionHelp;
                    break;

                case "DataExtendedXml":
                    this.Text = "Data Extended Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataExtendedCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataExtendedXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataExtendedHelp;
                    break;

                case "DataStoredProcedureXml":
                    this.Text = "Data Stored Procedure Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataStoredProcedureCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataStoredProcedureXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataStoredProcedureHelp;
                    break;

                case "DataTableFunctionXml":
                    this.Text = "Data Table Function Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataTableFunctionCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataTableFunctionXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataTableFunctionHelp;
                    break;

                case "DataScalarFunctionXml":
                    this.Text = "Data Scalar Function Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataScalarFunctionCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataScalarFunctionXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataScalarFunctionHelp;
                    break;

                case "DataSetExtensionXml":
                    this.Text = "DataSet Extension Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataSetExtensionCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataSetExtensionXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataSetExtensionHelp;
                    break;

                case "DataSetContextExtensionXml":
                    this.Text = "DataSet Context Extension Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.DataSetContextExtensionCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.DataSetContextExtensionXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.DataSetContextExtensionHelp;
                    break;

                case "EdmExtensionXml":
                    this.Text = "Edm Extension Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.EdmExtensionCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.EdmExtensionXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.EdmExtensionHelp;
                    break;

                case "EdmContextExtensionXml":
                    this.Text = "Edm Context Extension Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.EdmContextExtensionCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.EdmContextExtensionXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.EdmContextExtensionHelp;
                    break;

                case "LinqExtensionXml":
                    this.Text = "Linq Extension Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.LinqExtensionCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.LinqExtensionXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.LinqExtensionHelp;
                    break;

                case "LinqContextExtensionXml":
                    this.Text = "Linq Context Extension Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.LinqContextExtensionCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.LinqContextExtensionXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.LinqContextExtensionHelp;
                    break;

                case "LinqExtendedXml":
                    this.Text = "Linq Extended Code Generation";
                    txtCustomTool.Text = Nequeo.Resource.CustomToolXml.LinqExtendedCustomTool;
                    txtXmlConfigurationData.Text = Nequeo.Resource.CustomToolXml.LinqExtendedXml;
                    txtHelp.Text = Nequeo.Resource.CustomToolXml.LinqExtendedHelp;
                    break;
            }
        }
    }
}
