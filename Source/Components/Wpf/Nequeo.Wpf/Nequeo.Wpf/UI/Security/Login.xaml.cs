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

namespace Nequeo.Wpf.UI.Security
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        /// <summary>
        /// Login window.
        /// </summary>
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The username.
        /// </summary>
        public string Username = "";

        /// <summary>
        /// The password.
        /// </summary>
        public string Password = "";

        /// <summary>
        /// The domain.
        /// </summary>
        public string Domain = "";

        /// <summary>
        /// Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Username = txtUserName.Text;
            Password = txtPassword.Password;
            Domain = txtDomain.Text;

            // If authenticated, then set DialogResult=true
            DialogResult = true;
        }

        /// <summary>
        /// On windows loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUserName.Text = Username;
            txtDomain.Text = Domain;
        }
    }
}
