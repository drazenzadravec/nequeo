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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Nequeo.Wpf.Extension
{
    /// <summary>
    /// Rich text box assistant.
    /// </summary>
    public static class RichTextBoxAssistant
    {
        /// <summary>
        /// Create a dependency to the rich text box 'Text' property.
        /// </summary>
        public static readonly DependencyProperty BoundDocument =
           DependencyProperty.RegisterAttached("BoundDocument", typeof(string), typeof(RichTextBoxAssistant),
           new FrameworkPropertyMetadata(null,
               FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
               OnBoundDocumentChanged));

        /// <summary>
        /// Get bound document.
        /// </summary>
        /// <param name="dp">The dependency.</param>
        /// <returns>The new content data.</returns>
        public static string GetBoundDocument(DependencyObject dp)
        {
            var context = dp.GetValue(BoundDocument) as string;
            return context;
        }

        /// <summary>
        /// Set bound document.
        /// </summary>
        /// <param name="dp">The dependency.</param>
        /// <param name="value">The content data.</param>
        public static void SetBoundDocument(DependencyObject dp, string value)
        {
            var context = value;
            dp.SetValue(BoundDocument, context);
        }

        /// <summary>
        /// On Text changed event.
        /// </summary>
        /// <param name="d">The object.</param>
        /// <param name="e">The change event.</param>
        private static void OnBoundDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextBox box = d as RichTextBox;

            // If null.
            if (box == null)
                return;

            // Load the content.
            TextRange content = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);

            List<Hyperlink> results = new List<Hyperlink>();
            foreach (Match match in regexUrl.Matches(content.Text))
            {
                TextPointer p1 = box.Document.ContentStart;
                TextPointer p2 = box.Document.ContentEnd;
                TextRange tr = new TextRange(p1, p2);
                string URI = tr.Text;

                if (p1 != null && p2 != null)
                {
                    Hyperlink link = new Hyperlink(p1, p2);

                    link.IsEnabled = true;
                    link.NavigateUri = new Uri(URI);
                    link.RequestNavigate += Extension.RichTextBoxAssistant.OnUrlClickRequestNavigate;
                }
            }
        }

        /// <summary>
        /// Look for hyperlinks.
        /// </summary>
        private static readonly Regex regexUrl = new Regex(@"^(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~\/|\/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:\/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|\/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=?(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=?(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?$");

        /// <summary>
        /// To text pointer. Rich text box extension.
        /// </summary>
        /// <param name="rtb">The rich text box.</param>
        /// <param name="index">The intext.</param>
        /// <returns>The text pointer.</returns>
        public static TextPointer ToTextPointer(this RichTextBox rtb, int index)
        {
            int count = 0;

            // Get the current position.
            TextPointer position = rtb.Document.ContentStart.GetNextContextPosition(LogicalDirection.Forward).GetNextContextPosition(LogicalDirection.Forward);

            // If position is not null.
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);
                    int length = textRun.Length;
                    if (count + length > index)
                    {
                        // Return the position.
                        return position.GetPositionAtOffset(index - count);
                    }
                    count += length;
                }

                // Get the next position.
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }
            return null;
        }

        /// <summary>
        /// On hyperlink clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        internal static void OnUrlClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start((sender as Hyperlink).NavigateUri.AbsoluteUri);
        }

        /// <summary>
        /// On hyperlink request navigate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        internal static void OnUrlClickRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri.ToString());
        }
    }
}