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

namespace Nequeo.Wpf.NequeoCompany
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Inject the data access services.
            invoice.CustomerDataSource = Service.NequeoCompany.ServiceLocator.Current.Resolve<Service.NequeoCompany.ICustomer>();
            customer.CustomerDataSource = Service.NequeoCompany.ServiceLocator.Current.Resolve<Service.NequeoCompany.ICustomer>();
            product.ProductDataSource = Service.NequeoCompany.ServiceLocator.Current.Resolve<Service.NequeoCompany.IProduct>();
            vendor.VendorDataSource = Service.NequeoCompany.ServiceLocator.Current.Resolve<Service.NequeoCompany.IVendor>();
            employee.EmployeeDataSource = Service.NequeoCompany.ServiceLocator.Current.Resolve<Service.NequeoCompany.IEmployee>();
            account.AccountDataSource = Service.NequeoCompany.ServiceLocator.Current.Resolve<Service.NequeoCompany.IAccount>();
            company.CompanyDataSource = Service.NequeoCompany.ServiceLocator.Current.Resolve<Service.NequeoCompany.ICompany>();
            asset.AssetDataSource = Service.NequeoCompany.ServiceLocator.Current.Resolve<Service.NequeoCompany.IAsset>();

            Service.NequeoCompany.IUser user = Service.NequeoCompany.ServiceLocator.Current.Resolve<Service.NequeoCompany.IUser>();
            management.User = user;
            management.ManageDataSource = Service.NequeoCompany.ServiceLocator.Current.Resolve<Service.NequeoCompany.IManage>();

            // Open the login page.
            UI.Security.Login login = new UI.Security.Login();
            login.ShowDialog();

            string username = login.Username;
            string domain = login.Domain;

            // Get the result
            if (login.DialogResult.HasValue && login.DialogResult.Value)
            {
                // Valaidate the user.
                DataAccess.NequeoCompany.Data.User userData = null;

                // While the user has not been validated.
                while (userData == null)
                {
                    // If data has been returned then valid.
                    userData = user.ValidateUser(login.Username, login.Password);
                    if (userData != null)
                    {
                        // Get the user type.
                        Common.Helper.UserType = (DataAccess.NequeoCompany.Enum.EnumUserType)
                            Enum.Parse(typeof(DataAccess.NequeoCompany.Enum.EnumUserType), userData.UserType);

                        // If the user has been suspended.
                        if (userData.Suspended)
                            tabControlNequeo.IsEnabled = false;
                        break;
                    }

                    login = new UI.Security.Login();
                    login.Username = username;
                    login.Domain = domain;
                    login.ShowDialog();

                    username = login.Username;
                    domain = login.Domain;

                    // If cancelled.
                    if (!login.DialogResult.HasValue || !login.DialogResult.Value)
                    {
                        tabControlNequeo.IsEnabled = false;
                        break;
                    }
                }
            }
            else
            {
                // Do not allow any interaction.
                tabControlNequeo.IsEnabled = false;
            }
        }

        /// <summary>
        /// On window application closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string details = string.Empty;

            // Get all data user controls
            Nequeo.IPropertyChanged invoiceChanged = invoice;
            Nequeo.IPropertyChanged customerChanged = customer;
            Nequeo.IPropertyChanged productChanged = product;
            Nequeo.IPropertyChanged vendorChanged = vendor;
            Nequeo.IPropertyChanged employeeChanged = employee;
            Nequeo.IPropertyChanged accountChanged = account;
            Nequeo.IPropertyChanged companyChanged = company;
            Nequeo.IPropertyChanged assetChanged = asset;
            Nequeo.IPropertyChanged managementChanged = management;

            // Test to see if any data user control has changed.
            details = GetDetails(invoiceChanged, details);
            details = GetDetails(customerChanged, details);
            details = GetDetails(productChanged, details);
            details = GetDetails(vendorChanged, details);
            details = GetDetails(employeeChanged, details);
            details = GetDetails(accountChanged, details);
            details = GetDetails(companyChanged, details);
            details = GetDetails(assetChanged, details);
            details = GetDetails(managementChanged, details);

            // If the data has been changed and not updated.
            if (!String.IsNullOrEmpty(details))
            {
                MessageBoxResult result = MessageBox.Show(details + "\r\n\r\n" +
                    "Disregard the changes and close (all changes will be lost)?", "Data Has Changed", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes)
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// Get all the deatils from the changes implementation.
        /// </summary>
        /// <param name="changes">The changes implementation.</param>
        /// <param name="currentDetails">The current deatils.</param>
        /// <returns>The current and new details text.</returns>
        private string GetDetails(Nequeo.IPropertyChanged changes, string currentDetails)
        {
            string details = currentDetails;
            List<string> list = changes.GetDetails();

            // If changes exist then add to the details.
            if (list.Count > 0)
                foreach (string detail in list)
                    details += detail + "\r\n";

            // Return the details.
            return details;
        }
    }
}
