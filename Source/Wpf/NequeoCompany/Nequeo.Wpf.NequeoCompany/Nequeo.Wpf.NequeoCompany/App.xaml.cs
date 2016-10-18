using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
using System.Windows.Markup;
using System.Globalization;

namespace Nequeo.Wpf.NequeoCompany
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public App()
        {
            AppStartingUp();
        }

        /// <summary>
        /// Application startup
        /// </summary>
        private void AppStartingUp()
        {
            // Set the current language culture for the application.
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            // Initialise all services
            Nequeo.Wpf.NequeoCompany.Common.ServiceLocator.InitialiseService();
        }
    }
}
