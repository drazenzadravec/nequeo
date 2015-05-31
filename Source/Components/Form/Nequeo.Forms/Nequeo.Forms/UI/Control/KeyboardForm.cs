using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.Forms.UI.Control
{
    /// <summary>
    /// Keyboard form.
    /// </summary>
    public partial class KeyboardForm : Form
    {
        /// <summary>
        /// Keyboard form.
        /// </summary>
        public KeyboardForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// User key pressed event.
        /// </summary>
        [Category("Mouse"), Description("Return value of mouseclicked key")]
        public event KeyboardDelegate UserKeyPressed;

        /// <summary>
        /// On user key pressed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUserKeyPressed(KeyboardEventArgs e)
        {
            // If event used.
            if (UserKeyPressed != null)
                UserKeyPressed(this, e);
        }

        /// <summary>
        /// User key pressed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyboard_UserKeyPressed(object sender, KeyboardEventArgs e)
        {
            // Send the key pressed to the user.
            OnUserKeyPressed(e);
        }
    }
}
