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

namespace Nequeo.Math.ScientificCalculator.UI
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="result">The string to display in the result window.</param>
        /// <param name="label">The string to display in the label window.</param>
        public ResultWindow(string result, string label)
        {
            InitializeComponent();

            _result = result;
            _label = label;
        }

        private string _result = null;
        private string _label = null;

        /// <summary>
        /// Window loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            resultWindowText.Text = _result;
            resultWindowLabel.Text = _label;
        }
    }
}
