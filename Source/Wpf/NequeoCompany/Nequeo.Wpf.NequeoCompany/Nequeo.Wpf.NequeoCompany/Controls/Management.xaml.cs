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

namespace Nequeo.Wpf.NequeoCompany.Controls
{
    /// <summary>
    /// Interaction logic for Management.xaml
    /// </summary>
    public partial class Management : UserControl, Nequeo.IPropertyChanged
    {
        /// <summary>
        /// Management control.
        /// </summary>
        public Management()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IManage _manage;
        private bool _hasChanged = false;

        /// <summary>
        /// The User service.
        /// </summary>
        public Service.NequeoCompany.IUser User;

        /// <summary>
        /// Gets sets, the user injected interface.
        /// </summary>
        public Service.NequeoCompany.IManage ManageDataSource
        {
            get { return _manage; }
            set
            {
                _manage = value;

                // Assign each dependency data source
                userType.ManageDataSource = _manage;
                user.UserDataSource = User;
                transactionType.ManageDataSource = _manage;
                state.ManageDataSource = _manage;
                purchaseType.ManageDataSource = _manage;
                productCategory.ManageDataSource = _manage;
                productSubCategory.ManageDataSource = _manage;
                productStatus.ManageDataSource = _manage;
                paymentType.ManageDataSource = _manage;
                payIntervalType.ManageDataSource = _manage;
                incomeType.ManageDataSource = _manage;
                gstIncomeType.ManageDataSource = _manage;
                genericData.ManageDataSource = _manage;
                expenseType.ManageDataSource = _manage;
                accountType.ManageDataSource = _manage;
                assetCategory.ManageDataSource = _manage;
                dataMember.ManageDataSource = _manage;
                dataMemberTable.ManageDataSource = _manage;
            }
        }

        /// <summary>
        /// Gets sets, has the property changed.
        /// </summary>
        public bool PropertyChanged
        {
            get { return _hasChanged; }
            set { _hasChanged = value; }
        }

        /// <summary>
        /// Get the details of the interface.
        /// </summary>
        /// <returns>The implementation details.</returns>
        public List<string> GetDetails()
        {
            List<string> details = new List<string>();

            // If has changed
            if (userType.GetDetails().Count > 0)
                details.AddRange(userType.GetDetails().ToArray());

            // If has changed
            if (user.GetDetails().Count > 0)
                details.AddRange(user.GetDetails().ToArray());

            // If has changed
            if (transactionType.GetDetails().Count > 0)
                details.AddRange(transactionType.GetDetails().ToArray());

            // If has changed
            if (state.GetDetails().Count > 0)
                details.AddRange(state.GetDetails().ToArray());

            // If has changed
            if (purchaseType.GetDetails().Count > 0)
                details.AddRange(purchaseType.GetDetails().ToArray());

            // If has changed
            if (productCategory.GetDetails().Count > 0)
                details.AddRange(productCategory.GetDetails().ToArray());

            // If has changed
            if (productSubCategory.GetDetails().Count > 0)
                details.AddRange(productSubCategory.GetDetails().ToArray());

            // If has changed
            if (productStatus.GetDetails().Count > 0)
                details.AddRange(productStatus.GetDetails().ToArray());

            // If has changed
            if (paymentType.GetDetails().Count > 0)
                details.AddRange(paymentType.GetDetails().ToArray());

            // If has changed
            if (payIntervalType.GetDetails().Count > 0)
                details.AddRange(payIntervalType.GetDetails().ToArray());

            // If has changed
            if (incomeType.GetDetails().Count > 0)
                details.AddRange(incomeType.GetDetails().ToArray());

            // If has changed
            if (gstIncomeType.GetDetails().Count > 0)
                details.AddRange(gstIncomeType.GetDetails().ToArray());

            // If has changed
            if (genericData.GetDetails().Count > 0)
                details.AddRange(genericData.GetDetails().ToArray());

            // If has changed
            if (expenseType.GetDetails().Count > 0)
                details.AddRange(expenseType.GetDetails().ToArray());

            // If has changed
            if (accountType.GetDetails().Count > 0)
                details.AddRange(accountType.GetDetails().ToArray());

            // If has changed
            if (assetCategory.GetDetails().Count > 0)
                details.AddRange(assetCategory.GetDetails().ToArray());

            // If has changed
            if (dataMember.GetDetails().Count > 0)
                details.AddRange(dataMember.GetDetails().ToArray());

            // If has changed
            if (dataMemberTable.GetDetails().Count > 0)
                details.AddRange(dataMemberTable.GetDetails().ToArray());

            return details;
        }

        /// <summary>
        /// User control loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            // Set permission.
            switch (Common.Helper.UserType)
            {
                case DataAccess.NequeoCompany.Enum.EnumUserType.Administrator:
                    this.IsEnabled = true;
                    break;
            }
        }
    }
}
