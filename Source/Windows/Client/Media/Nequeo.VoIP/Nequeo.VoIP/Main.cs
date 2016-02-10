using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.VoIP
{
    /// <summary>
    /// Main.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// Main.
        /// </summary>
        public Main()
        {
            InitializeComponent();
            voIPControl1.Initialize();
        }

        /// <summary>
        /// Closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        /// <summary>
        /// Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            voIPControl1.DisposeCall();
        }
    }
}
