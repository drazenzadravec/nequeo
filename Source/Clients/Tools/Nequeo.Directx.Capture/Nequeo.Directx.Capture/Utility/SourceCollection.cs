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
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.Text;

using DirectShowLib;
using DirectShowLib.BDA;
using DirectShowLib.DES;
using DirectShowLib.DMO;
using DirectShowLib.Dvd;
using DirectShowLib.MultimediaStreaming;
using DirectShowLib.SBE;

namespace Nequeo.Directx.Utility
{
    /// <summary>
    /// A collection of sources (or physical connectors) on an 
    /// audio or video device. This is used by the <see cref="Capture"/>
    /// class to provide a list of available sources on the currently
    /// selected audio and video devices. This class cannot be created
    /// directly.  This class assumes there is only 1 video and 1 audio
    /// crossbar and all input pins route to a single output pin on each 
    /// crossbar.
    /// </summary>
    public class SourceCollection : CollectionBase, IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal SourceCollection()
        {
            InnerList.Capacity = 1;
        }

        /// <summary>
        /// Initialize collection with sources from graph.
        /// </summary>
        /// <param name="graphBuilder">The capture graph</param>
        /// <param name="deviceFilter">The device filter</param>
        /// <param name="isVideoDevice">Is video device</param>
        internal SourceCollection(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter, bool isVideoDevice)
        {
            AddFromGraph(graphBuilder, deviceFilter, isVideoDevice);
        }

        /// <summary>
        /// Get the source at the specified index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The source</returns>
        public Source this[int index]
        {
            get { return ((Source)InnerList[index]); }
        }

        /// <summary>
        ///  Gets or sets the source/physical connector currently in use.
        ///  This is marked internal so that the Capture class can control
        ///  how and when the source is changed.
        /// </summary>
        internal Source CurrentSource
        {
            get
            {
                // Loop through each source and find the first
                // enabled source.
                foreach (Source s in InnerList)
                {
                    if (s.Enabled)
                        return (s);
                }

                return (null);

            }
            set
            {
                if (value == null)
                {
                    // Disable all sources
                    foreach (Source s in InnerList)
                        s.Enabled = false;
                }
                else if (value is CrossbarSource)
                {
                    // Enable this source
                    // (this will automatically disable all other sources)
                    value.Enabled = true;
                }
                else
                {
                    // Disable all sources
                    // Enable selected source
                    foreach (Source s in InnerList)
                        s.Enabled = false;
                    value.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Empty the collection.
        /// </summary>
        public new void Clear()
        {
            for (int c = 0; c < InnerList.Count; c++)
                this[c].Dispose();
            InnerList.Clear();
        }

        /// <summary>
        /// Populate the collection from a filter graph.
        /// </summary>
        /// <param name="graphBuilder">The capture graph</param>
        /// <param name="deviceFilter">The device filter</param>
        /// <param name="isVideoDevice">Is video device</param>
        private void AddFromGraph(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter, bool isVideoDevice)
        {
            ArrayList crossbars = FindCrossbars(graphBuilder, deviceFilter);
            foreach (IAMCrossbar crossbar in crossbars)
            {
                ArrayList sources = FindCrossbarSources(graphBuilder, crossbar, isVideoDevice);
                InnerList.AddRange(sources);
            }

            if (!isVideoDevice)
            {
                if (InnerList.Count == 0)
                {
                    ArrayList sources = FindAudioSources(graphBuilder, deviceFilter);
                    InnerList.AddRange(sources);

                }
            }
        }

        /// <summary>
        ///  Retrieve a list of crossbar filters in the graph.
        ///  Most hardware devices should have a maximum of 2 crossbars, 
        ///  one for video and another for audio.
        /// </summary>
        /// <param name="graphBuilder">The capture graph</param>
        /// <param name="deviceFilter">The device filter</param>
        /// <returns>The cross bars list</returns>
        private ArrayList FindCrossbars(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter)
        {
            ArrayList crossbars = new ArrayList();

            Guid category = FindDirection.UpstreamOnly;
            Guid type = new Guid();
            Guid riid = typeof(IAMCrossbar).GUID;
            int hr;

            object comObj = null;
            object comObjNext = null;

            // Find the first interface, look upstream from the selected device
            hr = graphBuilder.FindInterface(category, type, deviceFilter, riid, out comObj);
            while ((hr == 0) && (comObj != null))
            {
                // If found, add to the list
                if (comObj is IAMCrossbar)
                {
                    crossbars.Add(comObj as IAMCrossbar);

                    // Find the second interface, look upstream from the next found crossbar
                    hr = graphBuilder.FindInterface(category, type, comObj as IBaseFilter, riid, out comObjNext);
                    comObj = comObjNext;
                }
                else
                    comObj = null;
            }

            return (crossbars);
        }

        /// <summary>
        /// Populate the internal InnerList with sources/physical connectors
        ///  found on the crossbars. Each instance of this class is limited
        ///  to video only or audio only sources ( specified by the isVideoDevice
        ///  parameter on the constructor) so we check each source before adding
        ///  it to the list.
        /// </summary>
        /// <param name="graphBuilder">The capture graph</param>
        /// <param name="crossbar">The cross bar</param>
        /// <param name="isVideoDevice">Is video device</param>
        /// <returns>The cross bars list</returns>
        private ArrayList FindCrossbarSources(ICaptureGraphBuilder2 graphBuilder, IAMCrossbar crossbar, bool isVideoDevice)
        {
            ArrayList sources = new ArrayList();
            int hr;
            int numOutPins;
            int numInPins;
            hr = crossbar.get_PinCounts(out numOutPins, out numInPins);
            if (hr < 0)
                Marshal.ThrowExceptionForHR(hr);

            // We loop through every combination of output and input pin
            // to see which combinations match.

            // Loop through output pins
            for (int cOut = 0; cOut < numOutPins; cOut++)
            {
                // Loop through input pins
                for (int cIn = 0; cIn < numInPins; cIn++)
                {
                    // Can this combination be routed?
                    hr = crossbar.CanRoute(cOut, cIn);
                    if (hr == 0)
                    {
                        // Yes, this can be routed
                        int relatedPin;
                        PhysicalConnectorType connectorType;
                        hr = crossbar.get_CrossbarPinInfo(true, cIn, out relatedPin, out connectorType);
                        if (hr < 0)
                            Marshal.ThrowExceptionForHR(hr);

                        // Is this the correct type?, If so add to the InnerList
                        CrossbarSource source = new CrossbarSource(crossbar, cOut, cIn, connectorType);
                        if (connectorType < PhysicalConnectorType.Audio_Tuner)
                            if (isVideoDevice)
                                sources.Add(source);
                            else
                                if (!isVideoDevice)
                                    sources.Add(source);
                    }
                }
            }

            // Some silly drivers (*cough* Nvidia *cough*) add crossbars
            // with no real choices. Every input can only be routed to
            // one output. Loop through every Source and see if there
            // at least one other Source with the same output pin.
            int refIndex = 0;
            while (refIndex < sources.Count)
            {
                bool found = false;
                CrossbarSource refSource = (CrossbarSource)sources[refIndex];
                for (int c = 0; c < sources.Count; c++)
                {
                    CrossbarSource s = (CrossbarSource)sources[c];
                    if ((refSource.OutputPin == s.OutputPin) && (refIndex != c))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    refIndex++;
                else
                    sources.RemoveAt(refIndex);
            }

            return (sources);
        }

        /// <summary>
        /// Find the audio source
        /// </summary>
        /// <param name="graphBuilder">The capture graph</param>
        /// <param name="deviceFilter">The device filter</param>
        /// <returns>The audio list.</returns>
        private ArrayList FindAudioSources(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter)
        {
            ArrayList sources = new ArrayList();
            IAMAudioInputMixer audioInputMixer = deviceFilter as IAMAudioInputMixer;
            if (audioInputMixer != null)
            {
                // Get a pin enumerator off the filter
                IEnumPins pinEnum;
                int hr = deviceFilter.EnumPins(out pinEnum);
                pinEnum.Reset();
                if ((hr == 0) && (pinEnum != null))
                {
                    // Loop through each pin
                    IPin[] pins = new IPin[1];
                    IntPtr f = IntPtr.Zero;
                    do
                    {
                        // Get the next pin
                        hr = pinEnum.Next(1, pins, f);
                        if ((hr == 0) && (pins[0] != null))
                        {
                            // Is this an input pin?
                            PinDirection dir = PinDirection.Output;
                            hr = pins[0].QueryDirection(out dir);
                            if ((hr == 0) && (dir == (PinDirection.Input)))
                            {
                                // Add the input pin to the sources list
                                AudioSource source = new AudioSource(pins[0]);
                                sources.Add(source);
                            }
                            pins[0] = null;
                        }
                    }
                    while (hr == 0);

                    Marshal.ReleaseComObject(pinEnum); pinEnum = null;
                }
            }

            // If there is only one source, don't return it
            // because there is nothing for the user to choose.
            // (Hopefully that single source is already enabled).
            if (sources.Count == 1)
                sources.Clear();

            return (sources);
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                Clear();
                InnerList.Capacity = 1;

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SourceCollection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
