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
using System.Windows;

namespace Nequeo.Wpf.Controls.Classes
{
    public static class SkinManager
    {
        private static SkinId skinId = SkinId.Windows7;

        public static event EventHandler SkinChanged;

        public static SkinId SkinId
        {
            get { return skinId; }
            set
            {
                if (skinId != value)
                {
                    skinId = value;
                    ApplySkin(value);
                }
            }
        }

        private static void ApplySkin(SkinId skin)
        {
            var dict = Application.Current.Resources.MergedDictionaries;

            switch (skin)
            {
                case SkinId.OfficeBlue:
                    dict.Remove(OfficeBlue);
                    break;

                case SkinId.OfficeBlack:
                    dict.Remove(OfficeBlack);
                    break;

                case SkinId.OfficeSilver:
                    dict.Remove(OfficeSilver);
                    break;

                case SkinId.Windows7:
                    dict.Remove(WindowsSeven);
                    break;

                case SkinId.Vista:
                    dict.Remove(Vista);
                    break;

                case SkinId.Custom:
                    dict.Remove(Custom);
                    break;
            }

            switch (skin)
            {
                case SkinId.OfficeBlue:
                    dict.Add(OfficeBlue);
                    break;

                case SkinId.OfficeBlack:
                    dict.Add(OfficeBlack);
                    break;

                case SkinId.OfficeSilver:
                    dict.Add(OfficeSilver);
                    break;

                case SkinId.Windows7:
                    dict.Add(WindowsSeven);
                    break;

                case SkinId.Vista:
                    dict.Add(Vista);
                    break;

                case SkinId.Custom:
                    dict.Add(Custom);
                    break;
            }
            OnSkinChanged();
        }

        private static void OnSkinChanged()
        {
            if (SkinChanged != null) SkinChanged(null, EventArgs.Empty);
        }

        private static ResourceDictionary officeBlue;
        public static ResourceDictionary OfficeBlue
        {
            get
            {
                if (officeBlue == null)
                {
                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri("/Nequeo.Wpf.Controls;Component/Skins/BlueSkin.xaml", UriKind.Relative);
                    officeBlue = dictionary;
                }
                return officeBlue;
            }
        }

        private static ResourceDictionary officeSilver;
        public  static ResourceDictionary OfficeSilver
        {
            get
            {
                if (officeSilver == null)
                {
                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri("/Nequeo.Wpf.Controls;Component/Skins/SilverSkin.xaml", UriKind.Relative);
                    officeSilver = dictionary;
                }
                return officeSilver;
            }
        }

        private static ResourceDictionary officeBlack;
        public static ResourceDictionary OfficeBlack
        {
            get
            {
                if (officeBlack == null)
                {
                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri("/Nequeo.Wpf.Controls;Component/Skins/BlackSkin.xaml", UriKind.Relative);
                    officeBlack = dictionary;
                }
                return officeBlack;
            }
        }

        private static ResourceDictionary windowSeven;
        public static ResourceDictionary WindowsSeven
        {
            get
            {
                if (windowSeven == null)
                {
                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri("/Nequeo.Wpf.Controls;Component/Skins/Win7Skin.xaml", UriKind.Relative);
                    windowSeven = dictionary;
                }
                return windowSeven;
            }
        }

        private static ResourceDictionary vista;
        public static ResourceDictionary Vista
        {
            get
            {
                if (vista == null)
                {
                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri("/Nequeo.Wpf.Controls;Component/Skins/VistaSkin.xaml", UriKind.Relative);
                    vista = dictionary;
                }
                return vista;
            }
        }

        private static ResourceDictionary custom;
        public static ResourceDictionary Custom
        {
            get
            {
                ResourceDictionary dictionary = new ResourceDictionary();
                dictionary.Source = customSkinResource;
                custom = dictionary;
                return custom;
            }
        }

        private static Uri customSkinResource;
        public static Uri CustomSkinResource
        {
            get { return customSkinResource; }
            set { customSkinResource = value; }
        }
    }
}
