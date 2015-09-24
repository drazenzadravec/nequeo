using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents.Serialization;
using System.Resources;
using System.Reflection;
using System.Printing;
using System.Windows.Xps;
using System.Diagnostics;
using System.ComponentModel;

namespace Nequeo.Wpf.UI.Printing
{
    /// <summary>
    /// Print progress dialog.
    /// </summary>
    public partial class PrintProgressWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Print progress dialog.
        /// </summary>
        /// <param name="docWriter">Xps document writer.</param>
        public PrintProgressWindow(XpsDocumentWriter docWriter)
        {
            InitializeComponent();
            _docWriter = docWriter;
        }

        private int _pageNumber = -1;
        private int _writingProgressPercentage = -1;
        private XpsDocumentWriter _docWriter = null;

        /// <summary>
        /// Writing progress active event.
        /// </summary>
        public event EventHandler WritingProgressActivate;

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// On writing progress changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWritingProgressChanged(object sender, WritingProgressChangedEventArgs e)
        {
            Debug.WriteLine("OnWritingProgressChanged ProgressPercentage = " + ((Double)e.ProgressPercentage).ToString());
            Debug.WriteLine("OnWritingProgressChanged Number             = " + e.Number.ToString());
            WritingProgressPercentage = e.ProgressPercentage;
            WritingProgressPercentage = e.Number;
            switch (e.WritingLevel)
            {
                case WritingProgressChangeLevel.FixedPageWritingProgress:
                    PageNumber = e.Number;
                    break;
                case WritingProgressChangeLevel.FixedDocumentWritingProgress:
                    this.Title = "Printing Complete";
                    break;
            }
        }

        /// <summary>
        /// On writing complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWritingCompleted(object sender, WritingCompletedEventArgs e)
        {
            Debug.WriteLine("OnWritingCompleted fired. Cancelled = " + e.Cancelled.ToString());
            this.Close();
            Application.Current.MainWindow.Opacity = 1.0;
        }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                if (_pageNumber.Equals(value) == false)
                {
                    _pageNumber = value;
                    // Call OnPropertyChanged whenever the property is updated
                    OnPropertyChanged("PageNumber");
                }
            }
        }
        
        /// <summary>
        /// On property changed.
        /// </summary>
        /// <param name="info">The property name.</param>
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        /// <summary>
        /// Writing progress percentage.
        /// </summary>
        public int WritingProgressPercentage
        {
            get
            {
                return _writingProgressPercentage;
            }
            set
            {
                _writingProgressPercentage = value;
                _writingProgressBar.Value = _writingProgressPercentage;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("WritingProgressPercentage");
            }
        }

        /// <summary>
        /// Cancel button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CancelClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.Write("CancelAsync: ");
                _docWriter.CancelAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed");
                MessageBox.Show("Exception Cancelling Async printing: " + ex.ToString());
                return;
            }
            Debug.WriteLine("Successful");
            DialogResult = false;
            this.Close();
            Debug.WriteLine("Successfully closed PrintProgressWindow");
            Application.Current.MainWindow.Opacity = 1.0;
        }
    }
}
