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

namespace Nequeo.Wpf.Common
{
    /// <summary>
    /// Helper class for platform detection.
    /// </summary>
    internal static class DesignerLibrary
    {
        internal static DesignerPlatformLibrary DetectedDesignerLibrary
        {
            get
            {
                if(_detectedDesignerPlatformLibrary == null)
                {
                    _detectedDesignerPlatformLibrary = GetCurrentPlatform();
                }
                return _detectedDesignerPlatformLibrary.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static DesignerPlatformLibrary GetCurrentPlatform()
        {
            // We check Silverlight first because when in the VS designer, the .NET libraries will resolve
            // If we can resolve the SL libs, then we're in SL or WP
            // Then we check .NET because .NET will load the WinRT library (even though it can't really run it)
            // When running in WinRT, it will not load the PresentationFramework lib

            // Check Silverlight
            var dm = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
            if (dm != null)
            {
                return DesignerPlatformLibrary.Silverlight;
            }

            // Check .NET 
            var cmdm = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            if (cmdm != null) // loaded the assembly, could be .net 
            {
                return DesignerPlatformLibrary.Net;
            }

            // check WinRT next
            var wadm = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");
            if (wadm != null)
            {
                return DesignerPlatformLibrary.WinRt;
            }

            return DesignerPlatformLibrary.Unknown;
        }

        /// <summary>
        /// 
        /// </summary>
        private static DesignerPlatformLibrary? _detectedDesignerPlatformLibrary;
    }

    /// <summary>
    /// 
    /// </summary>
    internal enum DesignerPlatformLibrary
    {
        Unknown,
        Net,
        WinRt,
        Silverlight
    }
}
