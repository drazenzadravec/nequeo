using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Firewall API type library.
using NetFwTypeLib;

namespace Nequeo.Firewall
{
    /// <summary>
    /// Windows firewall provider.
    /// </summary>
	public class WindowsAdapter
	{
        private INetFwProfile fwProfile = null;

        /// <summary>
        /// Creates a new port entry in the firewall collection.
        /// </summary>
        /// <param name="port">The port number.</param>
        /// <param name="name">The name of the port.</param>
        /// <param name="protocol">The protocol used.</param>
        /// <param name="scope">The scope of the control.</param>
        public void OpenFirewallPort(int port, string name,
            NET_FW_IP_PROTOCOL_ protocol, NET_FW_SCOPE_ scope)
        {
            // Set the current access profile.
            SetProfile();

            // Get the current globall
            // open port profile control.
            INetFwOpenPorts openPorts = fwProfile.GloballyOpenPorts;

            // Create a new instance of the
            // open new port type.
            INetFwOpenPort openPort = (INetFwOpenPort)GetInstance("INetOpenPort");

            // Assign the port specifications.
            openPort.Port = port;
            openPort.Name = name;
            openPort.Scope = scope;
            openPort.Protocol = protocol;

            // Add the new port to the
            // collection of ports.
            openPorts.Add(openPort);
            openPorts = null;
        }

        /// <summary>
        /// Removes the port entry from the firewall collection.
        /// </summary>
        /// <param name="port">The port number.</param>
        /// <param name="protocol">The protocol used.</param>
        public void CloseFirewallPort(int port, NET_FW_IP_PROTOCOL_ protocol)
        {
            // Set the current access profile.
            SetProfile();

            // Get the current globall
            // open port profile control.
            INetFwOpenPorts openPorts = fwProfile.GloballyOpenPorts;

            // Remove the specified port from the collection.
            openPorts.Remove(port, protocol);
            openPorts = null;
        }

        /// <summary>
        /// Creates a new authorized application entry in the firewall collection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="applicationPath">The authorized application path.</param>
        /// <param name="scope">The scope of the control.</param>
        public void OpenFirewallAuthorizedApplication(string name,
            string applicationPath, NET_FW_SCOPE_ scope)
        {
            // Set the current access profile.
            SetProfile();

            // Get the collection of applications
            // with the firewall control.
            INetFwAuthorizedApplications openApplications = fwProfile.AuthorizedApplications;

            // Create a new instance of the
            // open new authorized application type.
            INetFwAuthorizedApplication openAuthApp = (INetFwAuthorizedApplication)GetInstance("INetAuthApp");

            // Assign the authorized application specifications.
            openAuthApp.Name = name;
            openAuthApp.Scope = scope;
            openAuthApp.ProcessImageFileName = applicationPath;

            // Add the new application to the
            // collection of authorized applications.
            openApplications.Add(openAuthApp);
            openApplications = null;
        }

        /// <summary>
        /// Removes the authorized application entry from the firewall collection.
        /// </summary>
        /// <param name="applicationPath">The authorized application path.</param>
        public void CloseFirewallAuthorizedApplication(string applicationPath)
        {
            // Set the current access profile.
            SetProfile();

            // Get the collection of applications
            // with the firewall control.
            INetFwAuthorizedApplications openApplications = fwProfile.AuthorizedApplications;

            // Remove the specified application from the collection.
            openApplications.Remove(applicationPath);
            openApplications = null;
        }

        /// <summary>
        /// Create anew instance of the firewall type.
        /// </summary>
        /// <param name="typeName">The name of the type to create.</param>
        /// <returns>The object instance of the type.</returns>
        private Object GetInstance(String typeName)
        {
            // Get the type of firewall control
            if (typeName == "INetFwMgr")
            {
                // Firewall managment.
                Type type = Type.GetTypeFromCLSID(new Guid("{304CE942-6E39-40D8-943A-B913C40C9CD4}"));
                return Activator.CreateInstance(type);
            }
            else if (typeName == "INetAuthApp")
            {
                // Firewall application.
                Type type = Type.GetTypeFromCLSID(new Guid("{EC9846B3-2762-4A6B-A214-6ACB603462D2}"));
                return Activator.CreateInstance(type);
            }
            else if (typeName == "INetOpenPort")
            {
                // Firewall open port.
                Type type = Type.GetTypeFromCLSID(new Guid("{0CA545C6-37AD-4A6C-BF92-9F7610067EF5}"));
                return Activator.CreateInstance(type);
            }
            else return null;
        }

        /// <summary>
        /// Set the current managment profile.
        /// </summary>
        private void SetProfile()
        {
            // Access INetFwMgr.
            INetFwMgr fwMgr = (INetFwMgr)GetInstance("INetFwMgr");
            INetFwPolicy fwPolicy = fwMgr.LocalPolicy;

            // Get the current application profile.
            fwProfile = fwPolicy.CurrentProfile;
            fwMgr = null;
            fwPolicy = null;
        }
    }
}

