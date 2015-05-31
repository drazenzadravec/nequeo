/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.Forms.UI.Control
{
    /// <summary>
    /// Keyboard control.
    /// </summary>
    public partial class Keyboard : UserControl
    {
        /// <summary>
        /// Keyboard control.
        /// </summary>
        public Keyboard()
        {
            InitializeComponent();
        }

        private Boolean _shiftindicator = false;
        private Boolean _capslockindicator = false;
        private string _pvtKeyboardKeyPressed = "";
        private KeyboardType _pvtKeyboardType = KeyboardType.Standard; 

        /// <summary>
        /// Gets or sets the keyboard type.
        /// </summary>
        public KeyboardType KeyboardType
        {
            get
            {
                return _pvtKeyboardType;
            }
            set
            {
                _pvtKeyboardType = value;
                if (_shiftindicator) HandleShiftClick();
                if (_capslockindicator) HandleCapsLock();

                if (_pvtKeyboardType == KeyboardType.Standard)
                {
                    pictureBoxKeyboard.Image = Nequeo.Forms.Properties.Resources.keyboard_white;
                    pictureBoxCapsLockDown.Image = Nequeo.Forms.Properties.Resources.caps_down_white;
                    pictureBoxLeftShiftDown.Image = Nequeo.Forms.Properties.Resources.shift_down_white;
                    pictureBoxRightShiftDown.Image = Nequeo.Forms.Properties.Resources.shift_down_white;
                }
                else if (_pvtKeyboardType == KeyboardType.Alphabetical)
                {
                    pictureBoxKeyboard.Image = Nequeo.Forms.Properties.Resources.keyboard_alphabetical;
                    pictureBoxCapsLockDown.Image = Nequeo.Forms.Properties.Resources.caps_down_white;
                    pictureBoxLeftShiftDown.Image = Nequeo.Forms.Properties.Resources.shift_down_white;
                    pictureBoxRightShiftDown.Image = Nequeo.Forms.Properties.Resources.shift_down_white;
                }
                else   //   kids keyboard
                {
                    pictureBoxKeyboard.Image = Nequeo.Forms.Properties.Resources.keyboard_kids_lower;
                }
            }
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
        /// Mouse click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxKeyboard_MouseClick(object sender, MouseEventArgs e)
        {
            Single xpos = e.X;
            Single ypos = e.Y;

            xpos = 993 * (xpos / pictureBoxKeyboard.Width);
            ypos = 282 * (ypos / pictureBoxKeyboard.Height);

            if (_pvtKeyboardType == KeyboardType.Kids) _pvtKeyboardKeyPressed = HandleKidsMouseClick(xpos, ypos);
            else _pvtKeyboardKeyPressed = HandleTheMouseClick(xpos, ypos);

            KeyboardEventArgs dea = new KeyboardEventArgs(_pvtKeyboardKeyPressed);
            OnUserKeyPressed(dea);
        }

        /// <summary>
        /// Mouse click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxLeftShiftState_MouseClick(object sender, MouseEventArgs e)
        {
            HandleShiftClick();
        }

        /// <summary>
        /// Mouse click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxRightShiftState_MouseClick(object sender, MouseEventArgs e)
        {
            HandleShiftClick();
        }

        /// <summary>
        /// Mouse click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxCapsLockState_MouseClick(object sender, MouseEventArgs e)
        {
            HandleCapsLock();
        }

        /// <summary>
        /// Handle mouse click event.
        /// </summary>
        /// <param name="x">The current x position.</param>
        /// <param name="y">The current y position.</param>
        /// <returns>The key pressed.</returns>
        private string HandleKidsMouseClick(Single x, Single y)
        {
            string Keypressed = null;
            if (y < 73)
            {
                if (x < 79) Keypressed = "a";
                else if (x >= 79 && x < 147) Keypressed = "b";
                else if (x >= 147 && x < 214) Keypressed = "c";
                else if (x >= 214 && x < 281) Keypressed = "d";
                else if (x >= 281 && x < 348) Keypressed = "e";
                else if (x >= 348 && x < 416) Keypressed = "f";
                else if (x >= 416 && x < 491) Keypressed = "g";
                else if (x >= 491 && x < 672) Keypressed = "{BACKSPACE}";
                else if (x >= 672 && x < 764) Keypressed = ".";
                else if (x >= 764 && x < 845) Keypressed = "1";
                else if (x >= 845 && x < 912) Keypressed = "2";
                else if (x >= 912 && x < 989) Keypressed = "3";
            }
            else if (y >= 73 && y < 141)
            {
                if (x < 79) Keypressed = "h";
                else if (x >= 79 && x < 147) Keypressed = "i";
                else if (x >= 147 && x < 214) Keypressed = "j";
                else if (x >= 214 && x < 281) Keypressed = "k";
                else if (x >= 281 && x < 348) Keypressed = "l";
                else if (x >= 348 && x < 416) Keypressed = "m";
                else if (x >= 416 && x < 491) Keypressed = "n";
                else if (x >= 491 && x < 672) HandleCapsLock();
                else if (x >= 672 && x < 764) Keypressed = ",";
                else if (x >= 764 && x < 845) Keypressed = "4";
                else if (x >= 845 && x < 912) Keypressed = "5";
                else if (x >= 912 && x < 989) Keypressed = "6";
            }
            else if (y >= 141 && y < 209)
            {
                if (x < 79) Keypressed = "o";
                else if (x >= 79 && x < 147) Keypressed = "p";
                else if (x >= 147 && x < 214) Keypressed = "q";
                else if (x >= 214 && x < 281) Keypressed = "r";
                else if (x >= 281 && x < 348) Keypressed = "s";
                else if (x >= 348 && x < 416) Keypressed = "t";
                else if (x >= 416 && x < 491) Keypressed = "u";
                else if (x >= 491 && x < 672) Keypressed = "{ENTER}";
                else if (x >= 672 && x < 764) Keypressed = "?";
                else if (x >= 764 && x < 845) Keypressed = "7";
                else if (x >= 845 && x < 912) Keypressed = "8";
                else if (x >= 912 && x < 989) Keypressed = "9";
            }
            else if (y >= 209 && y < 277)
            {
                if (x >= 79 && x < 147) Keypressed = "v";
                else if (x >= 147 && x < 214) Keypressed = "w";
                else if (x >= 214 && x < 281) Keypressed = "x";
                else if (x >= 281 && x < 348) Keypressed = "y";
                else if (x >= 348 && x < 416) Keypressed = "z";
                else if (x >= 491 && x < 672) Keypressed = " ";
                else if (x >= 672 && x < 764) Keypressed = "!";
                else if (x >= 764 && x < 845) Keypressed = "{+}";
                else if (x >= 845 && x < 912) Keypressed = "0";
                else if (x >= 912 && x < 989) Keypressed = "-";
            }
            if (_capslockindicator && x < 491) return "+" + Keypressed;
            else return Keypressed;
        }

        /// <summary>
        /// Handle mouse click event.
        /// </summary>
        /// <param name="x">The current x position.</param>
        /// <param name="y">The current y position.</param>
        /// <returns>The key pressed.</returns>
        private string HandleTheMouseClick(Single x, Single y)
        {
            string Keypressed = null;
            if (x >= 4 && x < 815 && y >= 3 && y < 277)         //  keyboard section
            {
                if (y < 58)
                {
                    if (x >= 4 && x < 59) Keypressed = HandleShiftableKey("`");
                    else if (x >= 67 && x < 112) Keypressed = HandleShiftableKey("1");
                    else if (x >= 112 && x < 165) Keypressed = HandleShiftableKey("2");
                    else if (x >= 165 && x < 220) Keypressed = HandleShiftableKey("3");
                    else if (x >= 220 && x < 275) Keypressed = HandleShiftableKey("4");
                    else if (x >= 275 && x < 328) Keypressed = HandleShiftableKey("5");
                    else if (x >= 328 && x < 380) Keypressed = HandleShiftableKey("6");
                    else if (x >= 380 && x < 435) Keypressed = HandleShiftableKey("7");
                    else if (x >= 435 && x < 490) Keypressed = HandleShiftableKey("8");
                    else if (x >= 490 && x < 545) Keypressed = HandleShiftableKey("9");
                    else if (x >= 545 && x < 600) Keypressed = HandleShiftableKey("0");
                    else if (x >= 600 && x < 655) Keypressed = HandleShiftableKey("-");
                    else if (x >= 655 && x < 705) Keypressed = HandleShiftableKey("=");
                    else if (x >= 705 && x < 815) Keypressed = "{BACKSPACE}";
                    else Keypressed = null;
                }
                else if (y >= 58 && y < 114)
                {
                    if (x >= 85 && x < 140) Keypressed = HandleShiftableCaplockableKey("q");
                    else if (x >= 140 && x < 193) Keypressed = HandleShiftableCaplockableKey("w");
                    else if (x >= 193 && x < 247) Keypressed = HandleShiftableCaplockableKey("e");
                    else if (x >= 247 && x < 300) Keypressed = HandleShiftableCaplockableKey("r");
                    else if (x >= 300 && x < 355) Keypressed = HandleShiftableCaplockableKey("t");
                    else if (x >= 355 && x < 409) Keypressed = HandleShiftableCaplockableKey("y");
                    else if (x >= 409 && x < 463) Keypressed = HandleShiftableCaplockableKey("u");
                    else if (x >= 463 && x < 517) Keypressed = HandleShiftableCaplockableKey("i");
                    else if (x >= 517 && x < 571) Keypressed = HandleShiftableCaplockableKey("o");
                    else if (x >= 571 && x < 625) Keypressed = HandleShiftableCaplockableKey("p");
                    else if (x >= 625 && x < 680) Keypressed = HandleShiftableKey("{[}");
                    else if (x >= 680 && x < 733) Keypressed = HandleShiftableKey("{]}");
                    else Keypressed = null;
                }
                else if (y >= 114 && y < 168)
                {
                    if (x >= 4 && x < 113) HandleCapsLock();
                    else if (x >= 113 && x < 167) Keypressed = HandleShiftableCaplockableKey("a");
                    else if (x >= 167 && x < 221) Keypressed = HandleShiftableCaplockableKey("s");
                    else if (x >= 221 && x < 275) Keypressed = HandleShiftableCaplockableKey("d");
                    else if (x >= 275 && x < 330) Keypressed = HandleShiftableCaplockableKey("f");
                    else if (x >= 330 && x < 383) Keypressed = HandleShiftableCaplockableKey("g");
                    else if (x >= 383 && x < 437) Keypressed = HandleShiftableCaplockableKey("h");
                    else if (x >= 437 && x < 491) Keypressed = HandleShiftableCaplockableKey("j");
                    else if (x >= 491 && x < 545) Keypressed = HandleShiftableCaplockableKey("k");
                    else if (x >= 545 && x < 599) Keypressed = HandleShiftableCaplockableKey("l");
                    else if (x >= 599 && x < 653) Keypressed = HandleShiftableKey(";");
                    else if (x >= 653 && x < 706) Keypressed = HandleShiftableKey("'");
                    else if (x >= 706 && x < 815) Keypressed = "{ENTER}";
                    else Keypressed = null;
                }
                else if (y >= 168 && y < 221)
                {
                    if (x >= 4 && x < 140) HandleShiftClick();
                    else if (x >= 140 && x < 194) Keypressed = HandleShiftableCaplockableKey("z");
                    else if (x >= 194 && x < 248) Keypressed = HandleShiftableCaplockableKey("x");
                    else if (x >= 248 && x < 302) Keypressed = HandleShiftableCaplockableKey("c");
                    else if (x >= 302 && x < 356) Keypressed = HandleShiftableCaplockableKey("v");
                    else if (x >= 356 && x < 410) Keypressed = HandleShiftableCaplockableKey("b");
                    else if (x >= 410 && x < 464) Keypressed = HandleShiftableCaplockableKey("n");
                    else if (x >= 464 && x < 518) Keypressed = HandleShiftableCaplockableKey("m");
                    else if (x >= 518 && x < 572) Keypressed = HandleShiftableKey(",");
                    else if (x >= 572 && x < 626) Keypressed = HandleShiftableKey(".");
                    else if (x >= 626 && x < 680) Keypressed = HandleShiftableKey("/");
                    else if (x >= 680 && x < 815) HandleShiftClick();
                    else Keypressed = null;
                }
                else if (y >= 221 && y < 277)
                {
                    if (x >= 218 && x < 597) Keypressed = " ";
                    else Keypressed = null;
                }
            }
            else if (x >= 827 && x < 989 && y >= 27 && y < 193)   //  cursor keys
            {
                if (y < 83)
                {
                    if (x < 880) Keypressed = "{INSERT}";
                    else if (x >= 880 && x < 934) Keypressed = "{UP}";
                    else if (x >= 934) Keypressed = HandleShiftableKey("{HOME}");
                    else Keypressed = null;
                }
                else if (y >= 83 && y < 137)
                {
                    if (x < 880) Keypressed = "{LEFT}";
                    else if (x >= 934) Keypressed = "{RIGHT}";
                    else Keypressed = null;
                }
                else if (y >= 137)
                {
                    if (x < 880) Keypressed = "{DELETE}";
                    else if (x >= 880 && x < 934) Keypressed = "{DOWN}";
                    else if (x >= 934) Keypressed = HandleShiftableKey("{END}");
                    else Keypressed = null;
                }
                else Keypressed = null;
            }
            if (Keypressed != null)
            {
                if (_shiftindicator) HandleShiftClick();
                return Keypressed;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Handle shift table key.
        /// </summary>
        /// <param name="theKey">The current key.</param>
        /// <returns>The shift and key pressed.</returns>
        private string HandleShiftableKey(string theKey)
        {
            if (_shiftindicator)
            {
                return "+" + theKey;
            }
            else
            {
                return theKey;
            }
        }

        /// <summary>
        /// Handle shift table and caps lock key.
        /// </summary>
        /// <param name="theKey">The current key.</param>
        /// <returns>The shift caps lock and key pressed.</returns>
        private string HandleShiftableCaplockableKey(string theKey)
        {
            if (_pvtKeyboardType != KeyboardType.Standard)
            {
                switch (theKey)
                {
                    case ("q"):
                        theKey = "a";
                        break;
                    case ("w"):
                        theKey = "b";
                        break;
                    case ("e"):
                        theKey = "c";
                        break;
                    case ("r"):
                        theKey = "d";
                        break;
                    case ("t"):
                        theKey = "e";
                        break;
                    case ("y"):
                        theKey = "f";
                        break;
                    case ("u"):
                        theKey = "g";
                        break;
                    case ("i"):
                        theKey = "h";
                        break;
                    case ("o"):
                        theKey = "i";
                        break;
                    case ("p"):
                        theKey = "j";
                        break;
                    case ("a"):
                        theKey = "k";
                        break;
                    case ("s"):
                        theKey = "l";
                        break;
                    case ("d"):
                        theKey = "m";
                        break;
                    case ("f"):
                        theKey = "n";
                        break;
                    case ("g"):
                        theKey = "o";
                        break;
                    case ("h"):
                        theKey = "p";
                        break;
                    case ("j"):
                        theKey = "q";
                        break;
                    case ("k"):
                        theKey = "r";
                        break;
                    case ("l"):
                        theKey = "s";
                        break;
                    case ("z"):
                        theKey = "t";
                        break;
                    case ("x"):
                        theKey = "u";
                        break;
                    case ("c"):
                        theKey = "v";
                        break;
                    case ("v"):
                        theKey = "w";
                        break;
                    case ("b"):
                        theKey = "x";
                        break;
                    case ("n"):
                        theKey = "y";
                        break;
                    case ("m"):
                        theKey = "z";
                        break;
                }
            }
            if (_capslockindicator)
            {
                return "+" + theKey;
            }
            else if (_shiftindicator)
            {
                return "+" + theKey;
            }
            else
            {
                return theKey;
            }
        }

        /// <summary>
        /// Handle shift click.
        /// </summary>
        private void HandleShiftClick()
        {
            if (_shiftindicator)
            {
                _shiftindicator = false;
                pictureBoxLeftShiftDown.Visible = false;
                pictureBoxRightShiftDown.Visible = false;
            }
            else
            {
                _shiftindicator = true;
                pictureBoxLeftShiftDown.Visible = true;
                pictureBoxRightShiftDown.Visible = true;
            }
        }

        /// <summary>
        /// Handle caps lock click.
        /// </summary>
        private void HandleCapsLock()
        {
            if (_capslockindicator)
            {
                _capslockindicator = false;
                pictureBoxCapsLockDown.Visible = false;
                if (_pvtKeyboardType == KeyboardType.Kids) pictureBoxKeyboard.Image = Nequeo.Forms.Properties.Resources.keyboard_kids_lower;
            }
            else
            {
                _capslockindicator = true;
                if (_pvtKeyboardType == KeyboardType.Kids) pictureBoxKeyboard.Image = Nequeo.Forms.Properties.Resources.keyboard_kids_upper;
                else pictureBoxCapsLockDown.Visible = true;
            }
        }

        /// <summary>
        /// Keyboard size changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxKeyboard_SizeChanged(object sender, EventArgs e)
        {
            // position the capslock and shift down overlays
            pictureBoxCapsLockDown.Left = Convert.ToInt16(pictureBoxKeyboard.Width * 5 / 993);
            pictureBoxCapsLockDown.Top = Convert.ToInt16(pictureBoxKeyboard.Height * 115 / 282);
            pictureBoxLeftShiftDown.Left = Convert.ToInt16(pictureBoxKeyboard.Width * 5 / 993);
            pictureBoxLeftShiftDown.Top = Convert.ToInt16(pictureBoxKeyboard.Height * 169 / 282);
            pictureBoxRightShiftDown.Left = Convert.ToInt16(pictureBoxKeyboard.Width * 681 / 993);
            pictureBoxRightShiftDown.Top = pictureBoxLeftShiftDown.Top;


            // size the capslock and shift down overlays

            pictureBoxCapsLockDown.Width = Convert.ToInt16(pictureBoxKeyboard.Width * 110 / 993);
            pictureBoxCapsLockDown.Height = Convert.ToInt16(pictureBoxKeyboard.Height * 55 / 282);
            pictureBoxLeftShiftDown.Width = Convert.ToInt16(pictureBoxKeyboard.Width * 136 / 993);
            pictureBoxLeftShiftDown.Height = Convert.ToInt16(pictureBoxKeyboard.Height * 55 / 282);
            pictureBoxRightShiftDown.Width = Convert.ToInt16(pictureBoxKeyboard.Width * 135 / 993);
            pictureBoxRightShiftDown.Height = pictureBoxLeftShiftDown.Height;
        }
    }
}
