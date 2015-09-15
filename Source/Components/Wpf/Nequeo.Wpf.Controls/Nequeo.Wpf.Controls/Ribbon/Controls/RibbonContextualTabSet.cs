/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Nequeo.Wpf.Controls.Ribbon.Interfaces;
using System.Windows;
using System.Windows.Markup;

namespace Nequeo.Wpf.Controls
{
    [ContentProperty("Tabs")]
    public class RibbonContextualTabSet:Control,IRibbonControl
    {
        static RibbonContextualTabSet()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonContextualTabSet), new FrameworkPropertyMetadata(typeof(RibbonContextualTabSet)));
        }


        private ObservableCollection<RibbonTabItem> tabs;

        /// <summary>
        /// Gets the collection of RibbonTabItems.
        /// </summary>
        public Collection<RibbonTabItem> Tabs
        {
            get
            {
                if (tabs == null)
                {
                    tabs = new ObservableCollection<RibbonTabItem>();
                    tabs.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnTabItemCollectionChanged);
                }
                return tabs;
            }
        }

        /// <summary>
        /// Occurs when the TabItems collection has changed.
        /// </summary>
        protected virtual void OnTabItemCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (RibbonTabItem tab in e.NewItems)
                    {
                        tab.tabSet = this;
                        tab.RibbonBar = RibbonBar;
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (RibbonTabItem tab in e.OldItems)
                    {
                        tab.tabSet = null;
                        tab.RibbonBar = null;
                    }
                    break;
            }
        }

        private RibbonBar ribbonBar;
        public RibbonBar RibbonBar
        {
            get {return ribbonBar;}
            internal set
            {
                if (ribbonBar != value)
                {
                    ribbonBar = value;
                    foreach (var tab in Tabs) tab.RibbonBar = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets the color for the ContextualTabSet.
        /// This is a dependency property.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(RibbonContextualTabSet), new UIPropertyMetadata(Color.FromArgb(64, 255, 255, 255)));




        /// <summary>
        /// Gets or sets the title for the ContextualTabSet.
        /// This is a dependency property.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RibbonContextualTabSet), new UIPropertyMetadata(""));


        internal void SetSelectedTabItem(int index)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                RibbonTabItem item = Tabs[i];
                item.IsSelected = i == index;
            }
        }



        /// <summary>
        /// Gets whether the Tabset is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedPropertyKey, value); }
        }



        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey IsSelectedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsSelected", typeof(bool), typeof(RibbonContextualTabSet), new UIPropertyMetadata(false));

        private static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

        /// <summary>
        /// Select the first tab in the RibbonBar of the contextual tab set when the control is clicked:
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (this.tabs != null && this.tabs.Count > 0)
            {
                RibbonBar.SelectedTabItem = tabs[0];
            }
        }

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                return Tabs.GetEnumerator();
            }
        }

    }
}
