/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Printing;

namespace Nequeo.Forms.UI.Extender
{
    /// <summary>
    /// Html rich text box control.
    /// </summary>
    public class HtmlRichTextBox : RichTextBox
    {
        private int updating = 0;
        private int oldEventMask = 0;

        /// <summary>
        /// Maintains performance while updating.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is recommended to call this method before doing
        /// any major updates that you do not wish the user to
        /// see. Remember to call EndUpdate when you are finished
        /// with the update. Nested calls are supported.
        /// </para>
        /// <para>
        /// Calling this method will prevent redrawing. It will
        /// also setup the event mask of the underlying richedit
        /// control so that no events are sent.
        /// </para>
        /// </remarks>

        public void BeginUpdate()
        {
            // Deal with nested calls.
            ++updating;

            if (updating > 1)
                return;

            // Prevent the control from raising any events.
            oldEventMask = SendMessage(new HandleRef(this, Handle),
                EM_SETEVENTMASK, 0, 0);

            // Prevent the control from redrawing itself.
            SendMessage(new HandleRef(this, Handle),
                WM_SETREDRAW, 0, 0);
        }

        /// <summary>
        /// Resumes drawing and event handling.
        /// </summary>
        /// <remarks>
        /// This method should be called every time a call is made
        /// made to BeginUpdate. It resets the event mask to it's
        /// original value and enables redrawing of the control.
        /// </remarks>
        public void EndUpdate()
        {
            // Deal with nested calls.
            --updating;

            if (updating > 0)
                return;

            // Allow the control to redraw itself.
            SendMessage(new HandleRef(this, Handle),
                WM_SETREDRAW, 1, 0);

            // Allow the control to raise event messages.
            SendMessage(new HandleRef(this, Handle),
                EM_SETEVENTMASK, 0, oldEventMask);
        }

        /// <summary>
        /// Returns true when the control is performing some 
        /// internal updates, specially when is reading or writing
        /// HTML text
        /// </summary>
        public bool InternalUpdating
        {
            get
            {
                return (updating != 0);
            }
        }

        #region Win32 Apis

        // Constants from the Platform SDK.
        private const int EM_FORMATRANGE = 1081;

        private const int WM_USER = 0x0400;
        private const int EM_GETCHARFORMAT = WM_USER + 58;
        private const int EM_SETCHARFORMAT = WM_USER + 68;

        private const int EM_SETEVENTMASK = 1073;
        private const int EM_GETPARAFORMAT = 1085;
        private const int EM_SETPARAFORMAT = 1095;
        private const int EM_SETTYPOGRAPHYOPTIONS = 1226;
        private const int WM_SETREDRAW = 11;
        private const int TO_ADVANCEDTYPOGRAPHY = 1;


        // Defines for EM_SETCHARFORMAT/EM_GETCHARFORMAT
        private const Int32 SCF_SELECTION = 0x0001;
        private const Int32 SCF_WORD = 0x0002;
        private const Int32 SCF_ALL = 0x0004;

        public const int LF_FACESIZE = 32;

        // Defines for STRUCT_CHARFORMAT member dwMask
        public const UInt32 CFM_BOLD = 0x00000001;
        public const UInt32 CFM_ITALIC = 0x00000002;
        public const UInt32 CFM_UNDERLINE = 0x00000004;
        public const UInt32 CFM_STRIKEOUT = 0x00000008;
        public const UInt32 CFM_PROTECTED = 0x00000010;
        public const UInt32 CFM_LINK = 0x00000020;
        public const UInt32 CFM_SIZE = 0x80000000;
        public const UInt32 CFM_COLOR = 0x40000000;
        public const UInt32 CFM_FACE = 0x20000000;
        public const UInt32 CFM_OFFSET = 0x10000000;
        public const UInt32 CFM_CHARSET = 0x08000000;
        public const UInt32 CFM_SUBSCRIPT = CFE_SUBSCRIPT | CFE_SUPERSCRIPT;
        public const UInt32 CFM_SUPERSCRIPT = CFM_SUBSCRIPT;

        // Defines for STRUCT_CHARFORMAT member dwEffects
        public const UInt32 CFE_BOLD = 0x00000001;
        public const UInt32 CFE_ITALIC = 0x00000002;
        public const UInt32 CFE_UNDERLINE = 0x00000004;
        public const UInt32 CFE_STRIKEOUT = 0x00000008;
        public const UInt32 CFE_PROTECTED = 0x00000010;
        public const UInt32 CFE_LINK = 0x00000020;
        public const UInt32 CFE_AUTOCOLOR = 0x40000000;
        public const UInt32 CFE_SUBSCRIPT = 0x00010000;     /* Superscript and subscript are */
        public const UInt32 CFE_SUPERSCRIPT = 0x00020000;       /*  mutually exclusive			 */

        public const byte CFU_UNDERLINENONE = 0x00;
        public const byte CFU_UNDERLINE = 0x01;
        public const byte CFU_UNDERLINEWORD = 0x02; /* (*) displayed as ordinary underline	*/
        public const byte CFU_UNDERLINEDOUBLE = 0x03; /* (*) displayed as ordinary underline	*/
        public const byte CFU_UNDERLINEDOTTED = 0x04;
        public const byte CFU_UNDERLINEDASH = 0x05;
        public const byte CFU_UNDERLINEDASHDOT = 0x06;
        public const byte CFU_UNDERLINEDASHDOTDOT = 0x07;
        public const byte CFU_UNDERLINEWAVE = 0x08;
        public const byte CFU_UNDERLINETHICK = 0x09;
        public const byte CFU_UNDERLINEHAIRLINE = 0x0A; /* (*) displayed as ordinary underline	*/

        public const int CFM_SMALLCAPS = 0x0040;            /* (*)	*/
        public const int CFM_ALLCAPS = 0x0080;          /* Displayed by 3.0	*/
        public const int CFM_HIDDEN = 0x0100;           /* Hidden by 3.0 */
        public const int CFM_OUTLINE = 0x0200;          /* (*)	*/
        public const int CFM_SHADOW = 0x0400;           /* (*)	*/
        public const int CFM_EMBOSS = 0x0800;           /* (*)	*/
        public const int CFM_IMPRINT = 0x1000;          /* (*)	*/
        public const int CFM_DISABLED = 0x2000;
        public const int CFM_REVISED = 0x4000;

        public const int CFM_BACKCOLOR = 0x04000000;
        public const int CFM_LCID = 0x02000000;
        public const int CFM_UNDERLINETYPE = 0x00800000;        /* Many displayed by 3.0 */
        public const int CFM_WEIGHT = 0x00400000;
        public const int CFM_SPACING = 0x00200000;      /* Displayed by 3.0	*/
        public const int CFM_KERNING = 0x00100000;      /* (*)	*/
        public const int CFM_STYLE = 0x00080000;        /* (*)	*/
        public const int CFM_ANIMATION = 0x00040000;        /* (*)	*/
        public const int CFM_REVAUTHOR = 0x00008000;

        // Font Weights
        public const short FW_DONTCARE = 0;
        public const short FW_THIN = 100;
        public const short FW_EXTRALIGHT = 200;
        public const short FW_LIGHT = 300;
        public const short FW_NORMAL = 400;
        public const short FW_MEDIUM = 500;
        public const short FW_SEMIBOLD = 600;
        public const short FW_BOLD = 700;
        public const short FW_EXTRABOLD = 800;
        public const short FW_HEAVY = 900;

        public const short FW_ULTRALIGHT = FW_EXTRALIGHT;
        public const short FW_REGULAR = FW_NORMAL;
        public const short FW_DEMIBOLD = FW_SEMIBOLD;
        public const short FW_ULTRABOLD = FW_EXTRABOLD;
        public const short FW_BLACK = FW_HEAVY;

        // PARAFORMAT mask values
        public const UInt32 PFM_STARTINDENT = 0x00000001;
        public const UInt32 PFM_RIGHTINDENT = 0x00000002;
        public const UInt32 PFM_OFFSET = 0x00000004;
        public const UInt32 PFM_ALIGNMENT = 0x00000008;
        public const UInt32 PFM_TABSTOPS = 0x00000010;
        public const UInt32 PFM_NUMBERING = 0x00000020;
        public const UInt32 PFM_OFFSETINDENT = 0x80000000;

        // PARAFORMAT numbering options
        public const UInt16 PFN_BULLET = 0x0001;

        // PARAFORMAT alignment options
        public const UInt16 PFA_LEFT = 0x0001;
        public const UInt16 PFA_RIGHT = 0x0002;
        public const UInt16 PFA_CENTER = 0x0003;



        // It makes no difference if we use PARAFORMAT or
        // PARAFORMAT2 here, so I have opted for PARAFORMAT2.
        [StructLayout(LayoutKind.Sequential)]
        public struct PARAFORMAT
        {
            public int cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;

            // PARAFORMAT2 from here onwards.
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CHARFORMAT
        {
            public int cbSize;
            public UInt32 dwMask;
            public UInt32 dwEffects;
            public Int32 yHeight;
            public Int32 yOffset;
            public Int32 crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szFaceName;

            // CHARFORMAT2 from here onwards.
            public short wWeight;
            public short sSpacing;
            public Int32 crBackColor;
            public uint lcid;
            public uint dwReserved;
            public short sStyle;
            public short wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
            public byte bReserved1;
        }

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd,
            int msg,
            int wParam,
            int lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd,
            int msg,
            int wParam,
            ref PARAFORMAT lp);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd,
            int msg,
            int wParam,
            ref CHARFORMAT lp);

        #endregion

        //----------------------------
        // New, version 1.1
        public void SetSuperScript(bool bSet)
        {
            CHARFORMAT cf = this.CharFormat;

            if (bSet)
            {
                cf.dwMask |= CFM_SUPERSCRIPT;
                cf.dwEffects |= CFE_SUPERSCRIPT;
            }
            else
            {
                cf.dwEffects &= ~CFE_SUPERSCRIPT;
            }

            this.CharFormat = cf;
        }

        public void SetSubScript(bool bSet)
        {
            CHARFORMAT cf = this.CharFormat;

            if (bSet)
            {
                cf.dwMask |= CFM_SUBSCRIPT;
                cf.dwEffects |= CFE_SUBSCRIPT;
            }
            else
            {
                cf.dwEffects &= ~CFE_SUBSCRIPT;
            }

            this.CharFormat = cf;
        }

        public bool IsSuperScript()
        {
            CHARFORMAT cf = this.CharFormat;
            return ((cf.dwEffects & CFE_SUPERSCRIPT) == CFE_SUPERSCRIPT);
        }

        public bool IsSubScript()
        {
            CHARFORMAT cf = this.CharFormat;
            return ((cf.dwEffects & CFE_SUBSCRIPT) == CFE_SUBSCRIPT);
        }
        //----------------------------

        public PARAFORMAT ParaFormat
        {
            get
            {
                PARAFORMAT pf = new PARAFORMAT();
                pf.cbSize = Marshal.SizeOf(pf);

                // Get the alignment.
                SendMessage(new HandleRef(this, Handle),
                    EM_GETPARAFORMAT,
                    SCF_SELECTION, ref pf);

                return pf;
            }

            set
            {
                PARAFORMAT pf = value;
                pf.cbSize = Marshal.SizeOf(pf);

                // Set the alignment.
                SendMessage(new HandleRef(this, Handle),
                    EM_SETPARAFORMAT,
                    SCF_SELECTION, ref pf);
            }
        }

        public PARAFORMAT DefaultParaFormat
        {
            get
            {
                PARAFORMAT pf = new PARAFORMAT();
                pf.cbSize = Marshal.SizeOf(pf);

                // Get the alignment.
                SendMessage(new HandleRef(this, Handle),
                    EM_GETPARAFORMAT,
                    SCF_ALL, ref pf);

                return pf;
            }

            set
            {
                PARAFORMAT pf = value;
                pf.cbSize = Marshal.SizeOf(pf);

                // Set the alignment.
                SendMessage(new HandleRef(this, Handle),
                    EM_SETPARAFORMAT,
                    SCF_ALL, ref pf);
            }
        }

        public CHARFORMAT CharFormat
        {
            get
            {
                CHARFORMAT cf = new CHARFORMAT();
                cf.cbSize = Marshal.SizeOf(cf);

                // Get the alignment.
                SendMessage(new HandleRef(this, Handle),
                    EM_GETCHARFORMAT,
                    SCF_SELECTION, ref cf);

                return cf;
            }

            set
            {
                CHARFORMAT cf = value;
                cf.cbSize = Marshal.SizeOf(cf);

                // Set the alignment.
                SendMessage(new HandleRef(this, Handle),
                    EM_SETCHARFORMAT,
                    SCF_SELECTION, ref cf);
            }
        }

        public CHARFORMAT DefaultCharFormat
        {
            get
            {
                CHARFORMAT cf = new CHARFORMAT();
                cf.cbSize = Marshal.SizeOf(cf);

                // Get the alignment.
                SendMessage(new HandleRef(this, Handle),
                    EM_GETCHARFORMAT,
                    SCF_ALL, ref cf);

                return cf;
            }

            set
            {
                CHARFORMAT cf = value;
                cf.cbSize = Marshal.SizeOf(cf);

                // Set the alignment.
                SendMessage(new HandleRef(this, Handle),
                    EM_SETCHARFORMAT,
                    SCF_ALL, ref cf);
            }
        }

        #region COLORREF helper functions

        // convert COLORREF to Color
        private Color GetColor(int crColor)
        {
            byte r = (byte)(crColor);
            byte g = (byte)(crColor >> 8);
            byte b = (byte)(crColor >> 16);

            return Color.FromArgb(r, g, b);
        }

        // convert COLORREF to Color
        private int GetCOLORREF(int r, int g, int b)
        {
            int r2 = r;
            int g2 = (g << 8);
            int b2 = (b << 16);

            int result = r2 | g2 | b2;

            return result;
        }

        private int GetCOLORREF(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            return GetCOLORREF(r, g, b);
        }
        #endregion

        #region "Get HTML text"

        // format states
        private enum ctformatStates
        {
            nctNone = 0, // none format applied
            nctNew = 1, // new format
            nctContinue = 2, // continue with previous format
            nctReset = 3 // reset format (close this tag)
        }

        private enum uMyREType
        {
            U_MYRE_TYPE_TAG,
            U_MYRE_TYPE_EMO,
            U_MYRE_TYPE_ENTITY,
        }

        private struct cMyREFormat
        {
            public uMyREType nType;
            public int nLen;
            public int nPos;
            public string strValue;
        }

        public string GetHTML(bool bHTML, bool bParaFormat)
        {
            //------------------------
            // working variables
            CHARFORMAT cf;
            PARAFORMAT pf;

            ctformatStates bold = ctformatStates.nctNone;
            ctformatStates bitalic = ctformatStates.nctNone;
            ctformatStates bstrikeout = ctformatStates.nctNone;
            ctformatStates bunderline = ctformatStates.nctNone;
            ctformatStates super = ctformatStates.nctNone;
            ctformatStates sub = ctformatStates.nctNone;

            ctformatStates bacenter = ctformatStates.nctNone;
            ctformatStates baleft = ctformatStates.nctNone;
            ctformatStates baright = ctformatStates.nctNone;
            ctformatStates bnumbering = ctformatStates.nctNone;

            string strFont = "";
            Int32 crFont = 0;
            Color color = new Color();
            int yHeight = 0;
            int i = 0;
            //-------------------------

            //-------------------------
            // to store formatting
            ArrayList colFormat = new ArrayList();

            cMyREFormat mfr;
            //-------------------------

            //-------------------------
            // ok, lets go
            int nStart, nEnd;
            string strHTML = "";

            this.HideSelection = true;
            this.BeginUpdate();

            nStart = this.SelectionStart;
            nEnd = this.SelectionLength;

            try
            {
                //--------------------------------
                // replace entities
                if (bHTML)
                {

                    char[] ch = { '&', '<', '>', '"', '\'' };
                    string[] strreplace = { "&amp;", "&lt;", "&gt;", "&quot;", "&apos;" };

                    for (i = 0; i < ch.Length; i++)
                    {
                        char[] ch2 = { ch[i] };

                        int n = this.Find(ch2, 0);
                        while (n != -1)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = n;
                            mfr.nLen = 1;
                            mfr.nType = uMyREType.U_MYRE_TYPE_ENTITY;
                            mfr.strValue = strreplace[i];

                            colFormat.Add(mfr);

                            n = this.Find(ch2, n + 1);
                        }
                    }
                }
                //--------------------------------

                string strT = "";

                int k = this.TextLength;
                char[] chtrim = { ' ', '\x0000' };

                //--------------------------------
                // this is an inefficient method to get text format
                // but RichTextBox doesn't provide another method to
                // get something like an array of charformat and paraformat
                //--------------------------------
                for (i = 0; i < k; i++)
                {
                    // select one character
                    this.Select(i, 1);
                    string strChar = this.SelectedText;

                    if (bHTML)
                    {
                        //-------------------------
                        // get format for this character
                        cf = this.CharFormat;
                        pf = this.ParaFormat;

                        string strfname = new string(cf.szFaceName);
                        strfname = strfname.Trim(chtrim);
                        //-------------------------


                        //-------------------------
                        // new font format ?
                        if ((strFont != strfname) || (crFont != cf.crTextColor) || (yHeight != cf.yHeight))
                        {
                            if (strFont != "")
                            {
                                // close previous <font> tag

                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "</font>";

                                colFormat.Add(mfr);
                            }

                            //-------------------------
                            // save this for cache
                            strFont = strfname;
                            crFont = cf.crTextColor;
                            yHeight = cf.yHeight;
                            //-------------------------

                            //-------------------------
                            // font size should be translate to 
                            // html size (Approximately)
                            int fsize = yHeight / (20 * 5);
                            //-------------------------

                            //-------------------------
                            // color object from COLORREF
                            color = GetColor(crFont);
                            //-------------------------

                            //-------------------------
                            // add <font> tag
                            mfr = new cMyREFormat();

                            string strcolor = string.Concat("#", (color.ToArgb() & 0x00FFFFFF).ToString("X6"));

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "<font face=\"" + strFont + "\" color=\"" + strcolor + "\" size=\"" + fsize + "\">";

                            colFormat.Add(mfr);
                            //-------------------------
                        }

                        //-------------------------
                        // are we in another line ?
                        if ((strChar == "\r") || (strChar == "\n"))
                        {
                            // yes?
                            // then, we need to reset paragraph format
                            // and character format
                            if (bParaFormat)
                            {
                                bnumbering = ctformatStates.nctNone;
                                baleft = ctformatStates.nctNone;
                                baright = ctformatStates.nctNone;
                                bacenter = ctformatStates.nctNone;
                            }

                            // close previous tags

                            // is italic? => close it
                            if (bitalic != ctformatStates.nctNone)
                            {
                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "</i>";

                                colFormat.Add(mfr);

                                bitalic = ctformatStates.nctNone;
                            }

                            // is bold? => close it
                            if (bold != ctformatStates.nctNone)
                            {
                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "</b>";

                                colFormat.Add(mfr);

                                bold = ctformatStates.nctNone;
                            }

                            // is underline? => close it
                            if (bunderline != ctformatStates.nctNone)
                            {
                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "</u>";

                                colFormat.Add(mfr);

                                bunderline = ctformatStates.nctNone;
                            }

                            // is strikeout? => close it
                            if (bstrikeout != ctformatStates.nctNone)
                            {
                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "</s>";

                                colFormat.Add(mfr);

                                bstrikeout = ctformatStates.nctNone;
                            }

                            // is super? => close it
                            if (super != ctformatStates.nctNone)
                            {
                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "</sup>";

                                colFormat.Add(mfr);

                                super = ctformatStates.nctNone;
                            }

                            // is sub? => close it
                            if (sub != ctformatStates.nctNone)
                            {
                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "</sub>";

                                colFormat.Add(mfr);

                                sub = ctformatStates.nctNone;
                            }
                        }

                        // now, process the paragraph format,
                        // managing states: none, new, continue {with previous}, reset
                        if (bParaFormat)
                        {
                            // align to center?
                            if (pf.wAlignment == PFA_CENTER)
                            {
                                if (bacenter == ctformatStates.nctNone)
                                    bacenter = ctformatStates.nctNew;
                                else
                                    bacenter = ctformatStates.nctContinue;
                            }
                            else
                            {
                                if (bacenter != ctformatStates.nctNone)
                                    bacenter = ctformatStates.nctReset;
                            }

                            if (bacenter == ctformatStates.nctNew)
                            {
                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "<p align=\"center\">";

                                colFormat.Add(mfr);
                            }
                            else if (bacenter == ctformatStates.nctReset)
                                bacenter = ctformatStates.nctNone;
                            //---------------------

                            //---------------------
                            // align to left ?
                            if (pf.wAlignment == PFA_LEFT)
                            {
                                if (baleft == ctformatStates.nctNone)
                                    baleft = ctformatStates.nctNew;
                                else
                                    baleft = ctformatStates.nctContinue;
                            }
                            else
                            {
                                if (baleft != ctformatStates.nctNone)
                                    baleft = ctformatStates.nctReset;
                            }

                            if (baleft == ctformatStates.nctNew)
                            {
                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "<p align=\"left\">";

                                colFormat.Add(mfr);
                            }
                            else if (baleft == ctformatStates.nctReset)
                                baleft = ctformatStates.nctNone;
                            //---------------------

                            //---------------------
                            // align to right ?
                            if (pf.wAlignment == PFA_RIGHT)
                            {
                                if (baright == ctformatStates.nctNone)
                                    baright = ctformatStates.nctNew;
                                else
                                    baright = ctformatStates.nctContinue;
                            }
                            else
                            {
                                if (baright != ctformatStates.nctNone)
                                    baright = ctformatStates.nctReset;
                            }

                            if (baright == ctformatStates.nctNew)
                            {
                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "<p align=\"right\">";

                                colFormat.Add(mfr);
                            }
                            else if (baright == ctformatStates.nctReset)
                                baright = ctformatStates.nctNone;
                            //---------------------

                            //---------------------
                            // bullet ?
                            if (pf.wNumbering == PFN_BULLET)
                            {
                                if (bnumbering == ctformatStates.nctNone)
                                    bnumbering = ctformatStates.nctNew;
                                else
                                    bnumbering = ctformatStates.nctContinue;
                            }
                            else
                            {
                                if (bnumbering != ctformatStates.nctNone)
                                    bnumbering = ctformatStates.nctReset;
                            }

                            if (bnumbering == ctformatStates.nctNew)
                            {
                                mfr = new cMyREFormat();

                                mfr.nPos = i;
                                mfr.nLen = 0;
                                mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                                mfr.strValue = "<li>";

                                colFormat.Add(mfr);
                            }
                            else if (bnumbering == ctformatStates.nctReset)
                                bnumbering = ctformatStates.nctNone;
                            //---------------------
                        }

                        //---------------------
                        // bold ?
                        if ((cf.dwEffects & CFE_BOLD) == CFE_BOLD)
                        {
                            if (bold == ctformatStates.nctNone)
                                bold = ctformatStates.nctNew;
                            else
                                bold = ctformatStates.nctContinue;
                        }
                        else
                        {
                            if (bold != ctformatStates.nctNone)
                                bold = ctformatStates.nctReset;
                        }

                        if (bold == ctformatStates.nctNew)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "<b>";

                            colFormat.Add(mfr);
                        }
                        else if (bold == ctformatStates.nctReset)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "</b>";

                            colFormat.Add(mfr);

                            bold = ctformatStates.nctNone;
                        }
                        //---------------------

                        //---------------------
                        // Italic
                        if ((cf.dwEffects & CFE_ITALIC) == CFE_ITALIC)
                        {
                            if (bitalic == ctformatStates.nctNone)
                                bitalic = ctformatStates.nctNew;
                            else
                                bitalic = ctformatStates.nctContinue;
                        }
                        else
                        {
                            if (bitalic != ctformatStates.nctNone)
                                bitalic = ctformatStates.nctReset;
                        }

                        if (bitalic == ctformatStates.nctNew)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "<i>";

                            colFormat.Add(mfr);
                        }
                        else if (bitalic == ctformatStates.nctReset)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "</i>";

                            colFormat.Add(mfr);

                            bitalic = ctformatStates.nctNone;
                        }
                        //---------------------

                        //---------------------
                        // strikeout
                        if ((cf.dwEffects & CFM_STRIKEOUT) == CFM_STRIKEOUT)
                        {
                            if (bstrikeout == ctformatStates.nctNone)
                                bstrikeout = ctformatStates.nctNew;
                            else
                                bstrikeout = ctformatStates.nctContinue;
                        }
                        else
                        {
                            if (bstrikeout != ctformatStates.nctNone)
                                bstrikeout = ctformatStates.nctReset;
                        }

                        if (bstrikeout == ctformatStates.nctNew)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "<s>";

                            colFormat.Add(mfr);
                        }
                        else if (bstrikeout == ctformatStates.nctReset)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "</s>";

                            colFormat.Add(mfr);

                            bstrikeout = ctformatStates.nctNone;
                        }
                        //---------------------

                        //---------------------
                        // underline ?
                        if ((cf.dwEffects & CFE_UNDERLINE) == CFE_UNDERLINE)
                        {
                            if (bunderline == ctformatStates.nctNone)
                                bunderline = ctformatStates.nctNew;
                            else
                                bunderline = ctformatStates.nctContinue;
                        }
                        else
                        {
                            if (bunderline != ctformatStates.nctNone)
                                bunderline = ctformatStates.nctReset;
                        }

                        if (bunderline == ctformatStates.nctNew)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "<u>";

                            colFormat.Add(mfr);
                        }
                        else if (bunderline == ctformatStates.nctReset)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "</u>";

                            colFormat.Add(mfr);

                            bunderline = ctformatStates.nctNone;
                        }
                        //---------------------

                        //---------------------
                        // superscript ?
                        if ((cf.dwEffects & CFE_SUPERSCRIPT) == CFE_SUPERSCRIPT)
                        {
                            if (super == ctformatStates.nctNone)
                                super = ctformatStates.nctNew;
                            else
                                super = ctformatStates.nctContinue;
                        }
                        else
                        {
                            if (super != ctformatStates.nctNone)
                                super = ctformatStates.nctReset;
                        }

                        if (super == ctformatStates.nctNew)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "<sup>";

                            colFormat.Add(mfr);
                        }
                        else if (super == ctformatStates.nctReset)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "</sup>";

                            colFormat.Add(mfr);

                            super = ctformatStates.nctNone;
                        }
                        //---------------------

                        //---------------------
                        // subscript ?
                        if ((cf.dwEffects & CFE_SUBSCRIPT) == CFE_SUBSCRIPT)
                        {
                            if (sub == ctformatStates.nctNone)
                                sub = ctformatStates.nctNew;
                            else
                                sub = ctformatStates.nctContinue;
                        }
                        else
                        {
                            if (sub != ctformatStates.nctNone)
                                sub = ctformatStates.nctReset;
                        }

                        if (sub == ctformatStates.nctNew)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "<sub>";

                            colFormat.Add(mfr);
                        }
                        else if (sub == ctformatStates.nctReset)
                        {
                            mfr = new cMyREFormat();

                            mfr.nPos = i;
                            mfr.nLen = 0;
                            mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                            mfr.strValue = "</sub>";

                            colFormat.Add(mfr);

                            sub = ctformatStates.nctNone;
                        }
                        //---------------------
                    }

                    strT += strChar;
                }

                if (bHTML)
                {
                    // close pending tags
                    if (bold != ctformatStates.nctNone)
                    {
                        mfr = new cMyREFormat();

                        mfr.nPos = i;
                        mfr.nLen = 0;
                        mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                        mfr.strValue = "</b>";

                        colFormat.Add(mfr);
                        //strT += "</b>";
                    }

                    if (bitalic != ctformatStates.nctNone)
                    {
                        mfr = new cMyREFormat();

                        mfr.nPos = i;
                        mfr.nLen = 0;
                        mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                        mfr.strValue = "</i>";

                        colFormat.Add(mfr);
                        //strT += "</i>";
                    }

                    if (bstrikeout != ctformatStates.nctNone)
                    {
                        mfr = new cMyREFormat();

                        mfr.nPos = i;
                        mfr.nLen = 0;
                        mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                        mfr.strValue = "</s>";

                        colFormat.Add(mfr);

                        //strT += "</s>";
                    }

                    if (bunderline != ctformatStates.nctNone)
                    {
                        mfr = new cMyREFormat();

                        mfr.nPos = i;
                        mfr.nLen = 0;
                        mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                        mfr.strValue = "</u>";

                        colFormat.Add(mfr);

                        //strT += "</u>";
                    }

                    if (super != ctformatStates.nctNone)
                    {
                        mfr = new cMyREFormat();

                        mfr.nPos = i;
                        mfr.nLen = 0;
                        mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                        mfr.strValue = "</sup>";

                        colFormat.Add(mfr);
                        //strT += "</sup>";
                    }

                    if (sub != ctformatStates.nctNone)
                    {
                        mfr = new cMyREFormat();

                        mfr.nPos = i;
                        mfr.nLen = 0;
                        mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                        mfr.strValue = "</sub>";

                        colFormat.Add(mfr);
                        //strT += "</sub>";
                    }

                    if (strFont != "")
                    {
                        // close pending font format
                        mfr = new cMyREFormat();

                        mfr.nPos = i;
                        mfr.nLen = 0;
                        mfr.nType = uMyREType.U_MYRE_TYPE_TAG;
                        mfr.strValue = "</font>";

                        colFormat.Add(mfr);
                    }
                }

                //--------------------------
                // now, reorder the formatting array
                k = colFormat.Count;
                for (i = 0; i < k - 1; i++)
                {
                    for (int j = i + 1; j < k; j++)
                    {
                        mfr = (cMyREFormat)colFormat[i];
                        cMyREFormat mfr2 = (cMyREFormat)colFormat[j];

                        if (mfr2.nPos < mfr.nPos)
                        {
                            colFormat.RemoveAt(j);
                            colFormat.Insert(i, mfr2);
                            j--;
                        }
                        else if ((mfr2.nPos == mfr.nPos) && (mfr2.nLen < mfr.nLen))
                        {
                            colFormat.RemoveAt(j);
                            colFormat.Insert(i, mfr2);
                            j--;
                        }
                    }
                }
                //--------------------------


                //--------------------------
                // apply format by replacing and inserting HTML tags
                // stored in the Format Array
                int nAcum = 0;
                for (i = 0; i < k; i++)
                {
                    mfr = (cMyREFormat)colFormat[i];

                    strHTML += strT.Substring(nAcum, mfr.nPos - nAcum) + mfr.strValue;
                    nAcum = mfr.nPos + mfr.nLen;
                }

                if (nAcum < strT.Length)
                    strHTML += strT.Substring(nAcum);
                //--------------------------
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                //--------------------------
                // finish, restore
                this.SelectionStart = nStart;
                this.SelectionLength = nEnd;

                this.EndUpdate();
                this.HideSelection = false;
                //--------------------------
            }

            return strHTML;
        }
        #endregion

        #region "Add HTML text"
        public void AddHTML(string strHTML)
        {
            CHARFORMAT cf;
            PARAFORMAT pf;

            cf = this.DefaultCharFormat; // to apply character formatting
            pf = this.DefaultParaFormat; // to apply paragraph formatting

            char[] chtrim = { ' ', '\x0000' };

            this.HideSelection = true;
            this.BeginUpdate();

            try
            {
                // process text
                while (strHTML.Length > 0)
                {
                    string strData = strHTML;

                    reinit:

                    // looking for start tags
                    int nStart = strHTML.IndexOf('<');
                    if (nStart >= 0)
                    {
                        if (nStart > 0)
                        {
                            // tag is not the first character, so
                            // we need to add text to control and continue
                            // looking for tags at the begining of the text
                            strData = strHTML.Substring(0, nStart);
                            strHTML = strHTML.Substring(nStart);
                        }
                        else
                        {
                            // ok, get tag value
                            int nEnd = strHTML.IndexOf('>', nStart);
                            if (nEnd > nStart)
                            {
                                if ((nEnd - nStart) > 0)
                                {
                                    string strTag = strHTML.Substring(nStart, nEnd - nStart + 1);
                                    strTag = strTag.ToLower();

                                    if (strTag == "<b>")
                                    {
                                        cf.dwMask |= CFM_WEIGHT | CFM_BOLD;
                                        cf.dwEffects |= CFE_BOLD;
                                        cf.wWeight = FW_BOLD;
                                    }
                                    else if (strTag == "<i>")
                                    {
                                        cf.dwMask |= CFM_ITALIC;
                                        cf.dwEffects |= CFE_ITALIC;
                                    }
                                    else if (strTag == "<u>")
                                    {
                                        cf.dwMask |= CFM_UNDERLINE | CFM_UNDERLINETYPE;
                                        cf.dwEffects |= CFE_UNDERLINE;
                                        cf.bUnderlineType = CFU_UNDERLINE;
                                    }
                                    else if (strTag == "<s>")
                                    {
                                        cf.dwMask |= CFM_STRIKEOUT;
                                        cf.dwEffects |= CFE_STRIKEOUT;
                                    }
                                    else if (strTag == "<sup>")
                                    {
                                        cf.dwMask |= CFM_SUPERSCRIPT;
                                        cf.dwEffects |= CFE_SUPERSCRIPT;
                                    }
                                    else if (strTag == "<sub>")
                                    {
                                        cf.dwMask |= CFM_SUBSCRIPT;
                                        cf.dwEffects |= CFE_SUBSCRIPT;
                                    }
                                    else if ((strTag.Length > 2) && (strTag.Substring(0, 2) == "<p"))
                                    {
                                        if (strTag.IndexOf("align=\"left\"") > 0)
                                        {
                                            pf.dwMask |= PFM_ALIGNMENT;
                                            pf.wAlignment = (short)PFA_LEFT;
                                        }
                                        else if (strTag.IndexOf("align=\"right\"") > 0)
                                        {
                                            pf.dwMask |= PFM_ALIGNMENT;
                                            pf.wAlignment = (short)PFA_RIGHT;
                                        }
                                        else if (strTag.IndexOf("align=\"center\"") > 0)
                                        {
                                            pf.dwMask |= PFM_ALIGNMENT;
                                            pf.wAlignment = (short)PFA_CENTER;
                                        }
                                    }
                                    else if ((strTag.Length > 5) && (strTag.Substring(0, 5) == "<font"))
                                    {
                                        string strFont = new string(cf.szFaceName);
                                        strFont = strFont.Trim(chtrim);
                                        int crFont = cf.crTextColor;
                                        int yHeight = cf.yHeight;

                                        int nFace = strTag.IndexOf("face=");
                                        if (nFace > 0)
                                        {
                                            int nFaceEnd = strTag.IndexOf('\"', nFace + 6);
                                            if (nFaceEnd > nFace)
                                                strFont = strTag.Substring(nFace + 6, nFaceEnd - nFace - 6);
                                        }

                                        int nSize = strTag.IndexOf("size=");
                                        if (nSize > 0)
                                        {
                                            int nSizeEnd = strTag.IndexOf('\"', nSize + 6);
                                            if (nSizeEnd > nSize)
                                            {
                                                yHeight = int.Parse(strTag.Substring(nSize + 6, nSizeEnd - nSize - 6));
                                                yHeight *= (20 * 5);
                                            }
                                        }

                                        int nColor = strTag.IndexOf("color=");
                                        if (nColor > 0)
                                        {
                                            int nColorEnd = strTag.IndexOf('\"', nColor + 7);
                                            if (nColorEnd > nColor)
                                            {
                                                if (strTag.Substring(nColor + 7, 1) == "#")
                                                {
                                                    string strCr = strTag.Substring(nColor + 8, nColorEnd - nColor - 8);
                                                    int nCr = Convert.ToInt32(strCr, 16);

                                                    Color color = Color.FromArgb(nCr);

                                                    crFont = GetCOLORREF(color);
                                                }
                                                else
                                                {
                                                    crFont = int.Parse(strTag.Substring(nColor + 7, nColorEnd - nColor - 7));
                                                }
                                            }
                                        }

                                        cf.szFaceName = new char[LF_FACESIZE];
                                        strFont.CopyTo(0, cf.szFaceName, 0, Math.Min(LF_FACESIZE - 1, strFont.Length));
                                        //cf.szFaceName = strFont.ToCharArray(0, Math.Min(strFont.Length, LF_FACESIZE));
                                        cf.crTextColor = crFont;
                                        cf.yHeight = yHeight;

                                        cf.dwMask |= CFM_COLOR | CFM_SIZE | CFM_FACE;
                                        cf.dwEffects &= ~CFE_AUTOCOLOR;
                                    }
                                    else if (strTag == "<li>")
                                    {
                                        if (pf.wNumbering != PFN_BULLET)
                                        {
                                            pf.dwMask |= PFM_NUMBERING;
                                            pf.wNumbering = (short)PFN_BULLET;
                                        }
                                    }
                                    else if (strTag == "</b>")
                                    {
                                        cf.dwEffects &= ~CFE_BOLD;
                                        cf.wWeight = FW_NORMAL;
                                    }
                                    else if (strTag == "</i>")
                                    {
                                        cf.dwEffects &= ~CFE_ITALIC;
                                    }
                                    else if (strTag == "</u>")
                                    {
                                        cf.dwEffects &= ~CFE_UNDERLINE;
                                    }
                                    else if (strTag == "</s>")
                                    {
                                        cf.dwEffects &= ~CFM_STRIKEOUT;
                                    }
                                    else if (strTag == "</sup>")
                                    {
                                        cf.dwEffects &= ~CFE_SUPERSCRIPT;
                                    }
                                    else if (strTag == "</sub>")
                                    {
                                        cf.dwEffects &= ~CFE_SUBSCRIPT;
                                    }
                                    else if (strTag == "</font>")
                                    {
                                    }
                                    else if (strTag == "</p>")
                                    {
                                    }
                                    else if (strTag == "</li>")
                                    {
                                    }

                                    //-------------------------------
                                    // now, remove tag from HTML
                                    int nStart2 = strHTML.IndexOf("<", nEnd + 1);
                                    if (nStart2 > 0)
                                    {
                                        // extract partial data
                                        strData = strHTML.Substring(nEnd + 1, nStart2 - nEnd - 1);
                                        strHTML = strHTML.Substring(nStart2);
                                    }
                                    else
                                    {
                                        // get remain text and finish
                                        if ((nEnd + 1) < strHTML.Length)
                                            strData = strHTML.Substring(nEnd + 1);
                                        else
                                            strData = "";

                                        strHTML = "";
                                    }
                                    //-------------------------------s


                                    //-------------------------------
                                    // have we any continuos tag ?
                                    if (strData.Length > 0)
                                    {
                                        // yes, ok, goto to reinit
                                        if (strData[0] == '<')
                                            goto reinit;
                                    }
                                    //-------------------------------
                                }
                                else
                                {
                                    // we have not found any valid tag
                                    strHTML = "";
                                }
                            }
                            else
                            {
                                // we have not found any valid tag
                                strHTML = "";
                            }
                        }
                    }
                    else
                    {
                        // we have not found any tag
                        strHTML = "";
                    }

                    if (strData.Length > 0)
                    {
                        //-------------------------------
                        // replace entities
                        strData = strData.Replace("&amp;", "&");
                        strData = strData.Replace("&lt;", "<");
                        strData = strData.Replace("&gt;", ">");
                        strData = strData.Replace("&apos;", "'");
                        strData = strData.Replace("&quot;", "\"");
                        //-------------------------------

                        string strAux = strData; // use another copy

                        while (strAux.Length > 0)
                        {
                            //-----------------------
                            int nLen = strAux.Length;
                            //-----------------------

                            //-------------------------------
                            // now, add text to control
                            int nStartCache = this.SelectionStart;
                            string strt = strAux.Substring(0, nLen);

                            this.SelectedText = strt;
                            strAux = strAux.Remove(0, nLen);

                            this.SelectionStart = nStartCache;
                            this.SelectionLength = strt.Length;
                            //-------------------------------

                            //-------------------------------
                            // apply format
                            this.ParaFormat = pf;
                            this.CharFormat = cf;
                            //-------------------------------


                            // reposition to final
                            this.SelectionStart = this.TextLength + 1;
                            this.SelectionLength = 0;
                        }

                        // reposition to final
                        this.SelectionStart = this.TextLength + 1;
                        this.SelectionLength = 0;

                        //-------------------------------
                        // new paragraph requires to reset alignment
                        if ((strData.IndexOf("\r\n", 0) >= 0) || (strData.IndexOf("\n", 0) >= 0))
                        {
                            pf.dwMask = PFM_ALIGNMENT | PFM_NUMBERING;
                            pf.wAlignment = (short)PFA_LEFT;
                            pf.wNumbering = 0;
                        }
                        //-------------------------------
                    }
                } // while (strHTML.Length > 0)
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // reposition to final
                this.SelectionStart = this.TextLength + 1;
                this.SelectionLength = 0;

                this.EndUpdate();
                this.HideSelection = false;
            }
        }
        #endregion
    }
}
