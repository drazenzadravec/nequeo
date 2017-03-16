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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;
using System.Linq.Expressions;

namespace Nequeo.Math.ScientificCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Window loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Help.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void expressionHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open the hyperlink
                System.Diagnostics.Process.Start(Nequeo.Math.ScientificCalculator.Properties.Settings.Default.ScientificDocumentPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Help", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Add tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void expressionAddTab_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = new TabItem();
            tabItem.Header = "Scientific Calculator";
            tabItem.Content = new Nequeo.Math.ScientificCalculator.UI.ScientificCalculatorControl();
            expressionMainTab.Items.Add(tabItem);
        }
    }
}
