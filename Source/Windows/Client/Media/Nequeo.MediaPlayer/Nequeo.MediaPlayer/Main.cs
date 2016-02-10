using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.MediaPlayer
{
    /// <summary>
    /// Media player.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// Media player.
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (mediaPlayerControl != null)
                {
                    // Close the media player.
                    this.mediaPlayerControl.CloseMedia();
                }
            }
            catch { }
        }
    }
}
