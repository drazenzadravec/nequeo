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

using Nequeo.Science.Math.LinearAlgebra;
using Nequeo.Science.Math.LinearAlgebra.Solvers;
using Nequeo.Science.Math.LinearAlgebra.Single;
using Nequeo.Science.Math.LinearAlgebra.Single.Solvers;

using Nequeo.Science.Math.Providers.LinearAlgebra;
using Nequeo.Science.Math.Providers.LinearAlgebra.OpenBlas;

using Nequeo.Extension;
using Nequeo.Linq.Extension;

namespace Nequeo.Math.ScientificCalculator.UI
{
    /// <summary>
    /// Interaction logic for ScientificCalculatorControl.xaml
    /// </summary>
    public partial class ScientificCalculatorControl : UserControl
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        public ScientificCalculatorControl()
        {
            InitializeComponent();
        }

        private bool _hasLoaded = false;

        /// <summary>
        /// Window loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // If not loaded.
            if (!_hasLoaded)
            {
                _hasLoaded = true;

                customExceuteOp.Items.Add("None");
                customExceuteOp.Items.Add("Integrate");
                customExceuteOp.Items.Add("Derivative");
                customExceuteOp.Items.Add("Linear Algebra");
                customExceuteOp.Items.Add("Equation Root f(x) = 0");
                customExceuteOp.Items.Add("Quadratic Roots a*x^2 + b*x + c = 0");
                customExceuteOp.Items.Add("Cubic Roots a*x^3 + b*x^2 + c*x + d = 0");
                customExceuteOp.Items.Add("Factorial n!");
                customExceuteOp.Items.Add("Binomial n choose k");
                customExceuteOp.Items.Add("Regularized Lower Incomplete Beta Function");
                customExceuteOp.Items.Add("Exponential Integral Function");
            }
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
                    case 4:
                        // Show Equation Root f(x) = 0.
                        gridExtractOpsIntegrate.Visibility = Visibility.Hidden;
                        gridExtractOpsDerivative.Visibility = Visibility.Hidden;
                        gridExtractOpsLinearAlgebra.Visibility = Visibility.Hidden;
                        gridExtractOpsEquationRoot.Visibility = Visibility.Visible;
                        break;

                    case 3:
                        // Show Linear Algebra.
                        gridExtractOpsIntegrate.Visibility = Visibility.Hidden;
                        gridExtractOpsDerivative.Visibility = Visibility.Hidden;
                        gridExtractOpsLinearAlgebra.Visibility = Visibility.Visible;
                        gridExtractOpsEquationRoot.Visibility = Visibility.Hidden;
                        break;

                    case 2:
                        // Show Derivative.
                        gridExtractOpsIntegrate.Visibility = Visibility.Hidden;
                        gridExtractOpsDerivative.Visibility = Visibility.Visible;
                        gridExtractOpsLinearAlgebra.Visibility = Visibility.Hidden;
                        gridExtractOpsEquationRoot.Visibility = Visibility.Hidden;
                        break;

                    case 1:
                        // Show Integrate.
                        gridExtractOpsIntegrate.Visibility = Visibility.Visible;
                        gridExtractOpsDerivative.Visibility = Visibility.Hidden;
                        gridExtractOpsLinearAlgebra.Visibility = Visibility.Hidden;
                        gridExtractOpsEquationRoot.Visibility = Visibility.Hidden;
                        break;

                    case 0:
                    default:
                        // Hide.
                        gridExtractOpsIntegrate.Visibility = Visibility.Hidden;
                        gridExtractOpsDerivative.Visibility = Visibility.Hidden;
                        gridExtractOpsLinearAlgebra.Visibility = Visibility.Hidden;
                        gridExtractOpsEquationRoot.Visibility = Visibility.Hidden;
                        break;
                }
            }
            else
            {
                // Hide.
                gridExtractOpsIntegrate.Visibility = Visibility.Hidden;
                gridExtractOpsDerivative.Visibility = Visibility.Hidden;
                gridExtractOpsLinearAlgebra.Visibility = Visibility.Hidden;
                gridExtractOpsEquationRoot.Visibility = Visibility.Hidden;
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
                            //expressionReplace = expressionReplace.Replace(names[i].Trim(), "(" + values[i].Trim() + ")");
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
                    case 10:
                        // Exponential Integral Function
                        string[] exponential = expression.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        double xExp = Double.Parse(exponential[0].ToString().Trim());
                        int nExp = Int32.Parse(exponential[1].ToString().Trim());

                        result = Nequeo.Science.Math.SpecialFunctions.ExponentialIntegral(xExp, nExp);
                        expressionResult = result.ToString();
                        expressionReplace = "Exponential Integral Function : \r\n" +
                            "x = " + xExp.ToString() + ", n = " + nExp.ToString();
                        break;

                    case 9:
                        // Regularized Lower Incomplete Beta Function.
                        // I_x(a,b) = 1/ Beta(a,b) * int(t^(a-1)*(1-t)^(b-1),t=0..x) for real a > 0, b > 0, 1 >= x >= 0.
                        string[] beta = expression.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        double aBeta = Double.Parse(beta[0].ToString().Trim());
                        double bBeta = Double.Parse(beta[1].ToString().Trim());
                        double xBeta = Double.Parse(beta[2].ToString().Trim());

                        result = Nequeo.Science.Math.SpecialFunctions.BetaRegularized(aBeta, bBeta, xBeta);
                        expressionResult = result.ToString();
                        expressionReplace = "Regularized Lower Incomplete Beta Function : \r\n" +
                            "I_x(a,b) = 1/ Beta(a,b) * int(t^(a-1)*(1-t)^(b-1),t=0..x) for real a > 0, b > 0, 1 >= x >= 0\r\n" + 
                            "a = " + aBeta.ToString() + ", b = " + bBeta.ToString() + ", x = " + xBeta.ToString();
                        break;

                    case 8:
                        // Binomial n choose k
                        string[] binomial = expression.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        int nB = Int32.Parse(binomial[0].ToString().Trim());
                        int kB = Int32.Parse(binomial[1].ToString().Trim());

                        result = Nequeo.Science.Math.SpecialFunctions.Binomial(nB, kB);
                        expressionResult = result.ToString();
                        expressionReplace = "Binomial n choose k : \r\n" + nB.ToString() + " choose " + kB.ToString();
                        break;

                    case 7:
                        // Factorial n!
                        System.Numerics.BigInteger nF = new System.Numerics.BigInteger(Double.Parse(expression.Trim()));
                        System.Numerics.BigInteger bitInt = Nequeo.Science.Math.SpecialFunctions.Factorial(nF);
                        expressionResult = bitInt.ToString();
                        expressionReplace = "Factorial n! : \r\n" + nF.ToString() + "!";
                        break;

                    case 6:
                        // Cubic Roots a*x^3 + b*x^2 + c*x + d = 0.
                        Tuple<Complex, Complex, Complex> cubicRoots = Nequeo.Science.Math.FindRoots.Cubic(variables["b"], variables["c"], variables["b"], variables["a"]);
                        expressionResult = "Real = " + cubicRoots.Item1.Real.ToString() + ", Imaginary = " + cubicRoots.Item1.Imaginary.ToString() + "\r\n";
                        expressionResult += "Real = " + cubicRoots.Item2.Real.ToString() + ", Imaginary = " + cubicRoots.Item2.Imaginary.ToString() + "\r\n";
                        expressionResult += "Real = " + cubicRoots.Item3.Real.ToString() + ", Imaginary = " + cubicRoots.Item3.Imaginary.ToString();
                        expressionReplace = "Cubic Roots a*x^3 + b*x^2 + c*x + d = 0 : \r\n" + expressionReplace;
                        break;

                    case 5:
                        // Quadratic Roots a*x^2 + b*x + c = 0.
                        Tuple<Complex, Complex> quadraticRoots = Nequeo.Science.Math.FindRoots.Quadratic(variables["c"], variables["b"], variables["a"]);
                        expressionResult = "Real = " + quadraticRoots.Item1.Real.ToString() + ", Imaginary = " + quadraticRoots.Item1.Imaginary.ToString() + "\r\n";
                        expressionResult += "Real = " + quadraticRoots.Item2.Real.ToString() + ", Imaginary = " + quadraticRoots.Item2.Imaginary.ToString();
                        expressionReplace = "Quadratic Roots a*x^2 + b*x + c = 0 : \r\n" + expressionReplace;
                        break;

                    case 4:
                        // Equation Root f(x) = 0.
                        object[] expressionParameters = (variables.Count > 0 ? new object[variables.Count] : null);
                        int varIndex = 0;

                        // Replace the expression.
                        string hidedExpression = ReplaceExpressionHide(expression.Trim());

                        // For each variable.
                        foreach (KeyValuePair<string, double> item in variables)
                        {
                            // Replace the variable names.
                            hidedExpression = hidedExpression.Replace(item.Key, "@" + varIndex++.ToString());

                            // Add the variable parameter.
                            expressionParameters[varIndex - 1] = item.Value;
                        }

                        // Replace the expression.
                        string replacedExpression = ReplaceExpression(hidedExpression.Trim());

                        // A parameter for the lambda expression.
                        System.Linq.Expressions.ParameterExpression paramExpr = System.Linq.Expressions.Expression.Parameter(typeof(double), "x");
                        ParameterExpression[] pe = new List<ParameterExpression>() { paramExpr }.ToArray();
                        LambdaExpression lambda = Nequeo.Linq.DynamicExpression.ParseLambda(pe, typeof(double), replacedExpression.Trim(), expressionParameters);

                        // Get the function.
                        Func<double, double> function = (Func<double, double>)lambda.Compile();
                        result = Nequeo.Science.Math.FindRoots.OfFunction(function, Double.Parse(aVariableEquationRoot.Text), Double.Parse(bVariableEquationRoot.Text));

                        expressionReplace = "Equation Root f(x) = 0 : \r\n" + expressionReplace + " = 0";
                        expressionResult = result.ToString();
                        break;

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

        /// <summary>
        /// Replace the expression hiding.
        /// </summary>
        /// <param name="expression">The currrent expression.</param>
        /// <returns>The replaced expression.</returns>
        private string ReplaceExpressionHide(string expression)
        {
            return expression.
                Replace("abs", "!!").
                Replace("acos", "@@").
                Replace("asin", "##").
                Replace("atan2", "$$").
                Replace("atan", "%%").
                Replace("cosh", "::").
                Replace("cos", ";;").
                Replace("exp", ",,").
                Replace("log10", "//").
                Replace("log", "``").
                Replace("pow", "~~").
                Replace("sinh", "??").
                Replace("sin", "__").
                Replace("sqrt", "''").
                Replace("tanh", "^^").
                Replace("tan", "**");
        }

        /// <summary>
        /// Replace the expression.
        /// </summary>
        /// <param name="expression">The currrent expression.</param>
        /// <returns>The replaced expression.</returns>
        private string ReplaceExpression(string expression)
        {
            return expression.
                Replace("!!", "Math.Abs").
                Replace("@@", "Math.Acos").
                Replace("##", "Math.Asin").
                Replace("$$", "Math.Atan2").
                Replace("%%", "Math.Atan").
                Replace("::", "Math.Cosh").
                Replace(";;", "Math.Cos").
                Replace(",,", "Math.Exp").
                Replace("//", "Math.Log10").
                Replace("``", "Math.Log").
                Replace("~~", "Math.Pow").
                Replace("??", "Math.Sinh").
                Replace("__", "Math.Sin").
                Replace("''", "Math.Sqrt").
                Replace("^^", "Math.Tanh").
                Replace("**", "Math.Tan");
        }
    }
}
