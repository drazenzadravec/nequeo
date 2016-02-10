using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.MediaCapture
{
    /// <summary>
    /// Main
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// Main
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if(captureControl1 != null)
                {
                    captureControl1.CloseMedia();
                }
            }
            catch { }
        }
    }
}
