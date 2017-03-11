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

using Nequeo.Science.Math.LinearAlgebra;
using Nequeo.Science.Math.LinearAlgebra.Solvers;
using Nequeo.Science.Math.LinearAlgebra.Single;
using Nequeo.Science.Math.LinearAlgebra.Single.Solvers;

using Nequeo.Science.Math.Providers.LinearAlgebra;
using Nequeo.Science.Math.Providers.LinearAlgebra.OpenBlas;

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
            customExceuteOp.Items.Add("None");
            customExceuteOp.Items.Add("Integrate");
            customExceuteOp.Items.Add("Derivative");
            customExceuteOp.Items.Add("Linear Algebra");
        }

        /// <summary>
        /// Variable Name(s).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void variableNamesText_TextChanged(object sender, TextChangedEventArgs e) { }

        /// <summary>
        /// Selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customExceuteOp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If something is selected.
            if (customExceuteOp.SelectedItem != null)
            {
                // Select the index.
                switch (customExceuteOp.SelectedIndex)
                {
                    case 3:
                        // Show Linear Algebra.
                        gridExtractOpsIntegrate.Visibility = Visibility.Hidden;
                        gridExtractOpsDerivative.Visibility = Visibility.Hidden;
                        gridExtractOpsLinearAlgebra.Visibility = Visibility.Visible;
                        break;

                    case 2:
                        // Show Derivative.
                        gridExtractOpsIntegrate.Visibility = Visibility.Hidden;
                        gridExtractOpsDerivative.Visibility = Visibility.Visible;
                        gridExtractOpsLinearAlgebra.Visibility = Visibility.Hidden;
                        break;

                    case 1:
                        // Show Integrate.
                        gridExtractOpsIntegrate.Visibility = Visibility.Visible;
                        gridExtractOpsDerivative.Visibility = Visibility.Hidden;
                        gridExtractOpsLinearAlgebra.Visibility = Visibility.Hidden;
                        break;

                    case 0:
                    default:
                        // Hide.
                        gridExtractOpsIntegrate.Visibility = Visibility.Hidden;
                        gridExtractOpsDerivative.Visibility = Visibility.Hidden;
                        gridExtractOpsLinearAlgebra.Visibility = Visibility.Hidden;
                        break;
                }
            }
            else
            {
                // Hide.
                gridExtractOpsIntegrate.Visibility = Visibility.Hidden;
                gridExtractOpsDerivative.Visibility = Visibility.Hidden;
                gridExtractOpsLinearAlgebra.Visibility = Visibility.Hidden;
            }
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
        /// Variable Value(s).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void variableValuesText_TextChanged(object sender, TextChangedEventArgs e) { }

        /// <summary>
        /// Execute the expression.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void executeExpression_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                uint variableSize = 0U;
                double result = 0.0;
                Dictionary<string, double> variables = new Dictionary<string, double>();

                // Execute the expression.
                string expression = expressionWindowText.Text;
                string expressionReplace = expression;
                string expressionResult = "";

                // Create the math object.
                Nequeo.Math.MathGenerics<bool> math = new Nequeo.Math.MathGenerics<bool>();

                string[] names = null;
                string[] values = null;

                // Get the variables.
                if (!String.IsNullOrEmpty(variableNamesText.Text) &&
                    !String.IsNullOrEmpty(variableValuesText.Text))
                {
                    names = variableNamesText.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    values = variableValuesText.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    // For each variable.
                    for (int i = 0; i < names.Length; i++)
                    {
                        try
                        {
                            // Add variables.
                            variables.Add(names[i].Trim(), Double.Parse(values[i].Trim()));

                            // Replace variable with value.
                            expressionReplace = expressionReplace.Replace(names[i].Trim(), "(" + values[i].Trim() + ")");
                        }
                        catch { }
                    }

                    // If multi variable.
                    if (variables.Count > 1)
                        variableSize = 2U;
                    else
                        variableSize = 1U;
                }

                // Select the extra operation index.
                switch (customExceuteOp.SelectedIndex)
                {
                    case 3:
                        // Linear Algebra.
                        // Use managed provider to solver system.
                        ILinearAlgebraProvider<double> provider = new ManagedLinearAlgebraProvider();

                        // Variable names.
                        names = variableNamesText.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        // Get row and column lengths.
                        string[] aRowColumn = aColumnsRowsLinearAlgebra.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        int aRow = Int32.Parse(aRowColumn[0].Trim());
                        int aColumn = Int32.Parse(aRowColumn[1].Trim());
                        int bColumn = Int32.Parse(bColumnsLinearAlgebra.Text.Trim());

                        // Variables.
                        double[] a = new double[aRow * aColumn];
                        double[] b = new double[aRow * bColumn];

                        // Get the equation.
                        string[] equations = expression.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int eq = 0; eq < equations.Length; eq++)
                        {
                            // Current equation.
                            string[] eqAB = equations[eq].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                            // Assign B vector.
                            b[eq] = Double.Parse(eqAB[1].Trim());

                            // Get each variable.
                            string[] eqA = eqAB[0].Replace("-", "+ -").Replace("+ - ", "+ -").Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int equa = 0; equa < eqA.Length; equa++)
                            {
                                // Get current value.
                                string currentRowColumnValue = eqA[equa];

                                // Replace variable names.
                                for (int v = 0; v < names.Length; v++)
                                {
                                    // Remove variable name.
                                    currentRowColumnValue = currentRowColumnValue.Replace(names[v].Trim(), "");
                                }

                                // Convert to double.
                                double aRCValue = Double.Parse(
                                    (String.IsNullOrEmpty(currentRowColumnValue.Trim()) ? "1" :
                                        (currentRowColumnValue.Trim() == "-" ? "-1" : currentRowColumnValue.Trim()))
                                    );

                                // Assign the A matrix.
                                a[(eq * aRow) + equa] = aRCValue;
                            }
                        }

                        // Re-arrage A.
                        double[] ax = new double[aRow * aColumn];
                        for (int row = 0; row < aRow; row++)
                        {
                            for (int column = 0; column < aColumn; column++)
                            {
                                // Assign
                                ax[(row * aRow) + column] = a[row + (column * aColumn)];
                            }
                        }

                        // The result.
                        double[] x = new double[names.Length];

                        // Solver the system.
                        provider.SvdSolve(ax, aRow, aColumn, b, bColumn, x);

                        // The result.
                        for (int z = 0; z < x.Length; z++)
                        {
                            // Result string.
                            expressionResult += names[z].Trim() + " = " + x[z].ToString() + "\r\n";
                        }
                        expressionReplace = "System Linear Equation Solver : \r\n" + expressionReplace;
                        break;

                    case 2:
                        // Derivative.
                        result = math.ExpressionDerivative<double>(expression, Double.Parse(xVariableDerivative.Text), "x", variables);
                        expressionReplace = "Derivative at (" + xVariableDerivative.Text + ") : " + expressionReplace;
                        expressionResult = result.ToString();
                        break;

                    case 1:
                        // Integrate.
                        result = math.ExpressionIntegrate<double>(expression, 0.0, "x", Double.Parse(aVariableIntegrate.Text), Double.Parse(bVariableIntegrate.Text), variables);
                        expressionReplace = "Integral at (x) interval [" + aVariableIntegrate.Text + ", " + bVariableIntegrate.Text + "] : " + expressionReplace;
                        expressionResult = result.ToString();
                        break;

                    case 0:
                    default:
                        // Select the variable number.
                        switch (variableSize)
                        {
                            case 2U:
                                result = math.ExpressionMulti<double>(expression, variables);
                                expressionResult = result.ToString();
                                break;

                            case 1U:
                                result = math.Expression<double>(expression, variables.Values.First(), variables.Keys.First());
                                expressionResult = result.ToString();
                                break;

                            case 0U:
                            default:
                                result = math.Expression<double>(expression);
                                expressionResult = result.ToString();
                                break;
                        }
                        break;
                }

                // Display the result.
                UI.ResultWindow resultWindow = new UI.ResultWindow(expressionResult, expressionReplace);
                resultWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Scientific Calculator", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void expressionWindowText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(expressionWindowText.Text))
                executeExpression.IsEnabled = true;
            else
                executeExpression.IsEnabled = false;
        }
    }
}
