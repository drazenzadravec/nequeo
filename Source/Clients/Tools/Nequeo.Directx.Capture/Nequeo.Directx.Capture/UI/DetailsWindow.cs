using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.Directx.UI
{
    /// <summary>
    /// Details window.
    /// </summary>
    public partial class DetailsWindow : Form
    {
        /// <summary>
        /// Details window.
        /// </summary>
        public DetailsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the details data.
        /// </summary>
        /// <param name="details">The details data to set.</param>
        public void SetDetailData(string details)
        {
            this.richTextBoxDetails.Text = details;
        }
    }
}
