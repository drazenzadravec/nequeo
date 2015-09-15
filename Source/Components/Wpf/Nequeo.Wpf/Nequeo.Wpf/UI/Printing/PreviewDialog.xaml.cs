using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nequeo.Wpf.UI.Printing
{
    /// <summary>
    /// Print preview window.
    /// </summary>
    public partial class PreviewDialog : Window
    {
        /// <summary>
        /// Print preview window.
        /// </summary>
        /// <param name="document">The fixed document provider.</param>
        public PreviewDialog(IDocumentPaginatorSource document)
        {
            InitializeComponent();
            documentViewer.Document = document;
        }
    }
}
