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

using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

using Nequeo.IO.Audio.Capture;

namespace Nequeo.IO.Audio.Directx
{
    /// <summary>
    /// 
    /// </summary>
    public class Device : IDevice, IEquatable<Device>
	{
        /// <summary>
        /// 
        /// </summary>
        public Device()
        {
            if (_devices.Count > 0)
                _description = _devices[0].Description;

            if (_devices.Count > 0)
                _driverGuid = _devices[0].DriverGuid;

            if (_devices.Count > 0)
                _moduleName = _devices[0].ModuleName;
        }

        private CaptureDevicesCollection _devices = new CaptureDevicesCollection();

        private string _description = string.Empty;
        private Guid _driverGuid;
        private string _moduleName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public CaptureDevicesCollection Devices
        {
            get { return _devices; }
        }

        /// <summary>
        /// Gets a textual description of the Microsoft DirectSound device.
        /// </summary>
        public string Description 
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets the globally unique identifier (GUID) of a Microsoft DirectSound
        /// driver.
        /// </summary>
        public Guid DriverGuid 
        {
            get { return _driverGuid; }
        }

        /// <summary>
        /// Gets the module name of the Microsoft DirectSound driver corresponding
        /// to this device.
        /// </summary>
        public string ModuleName 
        {
            get { return _moduleName; }
        }

        /// <summary>
        /// Lists Sound Capture Devices which are currently available in the OS.
        /// </summary>
        public IEnumerable<Device> Available
        {
            get
            {
                foreach (DeviceInformation deviceInfo in _devices)
                {
                    Device currentDevice = new Device();
                    currentDevice._description = deviceInfo.Description;
                    currentDevice._driverGuid = deviceInfo.DriverGuid;
                    currentDevice._moduleName = deviceInfo.ModuleName;
                    yield return currentDevice;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DeviceInformation GetDevice(int index)
        {
            if ((index < 0) && (index >= _devices.Count))
                throw new IndexOutOfRangeException("Index bust be between '0' and '" + (_devices.Count - 1) + "'");

            return _devices[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SetDevice(int index)
        {
            if ((index < 0) && (index >= _devices.Count))
                throw new IndexOutOfRangeException("Index bust be between '0' and '" + (_devices.Count - 1) + "'");

            _description = _devices[index].Description;
            _driverGuid = _devices[index].DriverGuid;
            _moduleName = _devices[index].ModuleName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device1"></param>
        /// <param name="device2"></param>
        /// <returns></returns>
        public static bool operator ==(Device device1, Device device2)
        {
            return device1.Equals(device2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device1"></param>
        /// <param name="device2"></param>
        /// <returns></returns>
        public static bool operator !=(Device device1, Device device2)
        {
            return !(device1 == device2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Device))
                return false;

            return base.Equals((Device)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _driverGuid.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Device other)
        {
            return _driverGuid == other._driverGuid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
