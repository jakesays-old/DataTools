using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Std.Utility.Net
{
	public static class NetworkHelpers
	{
		private static IPAddress _hostAddress;

		/// <summary>
		/// Gets the default host address.
		/// </summary>
		/// <returns>The IP-4 address of the machine hosting the current process. Returns 127.0.0.1 if
		/// an address can't be determined.</returns>
		[DebuggerStepThrough]
		public static IPAddress	GetDefaultHostAddress()
		{
			if (_hostAddress == null)
			{
				foreach (var hostAddress in Dns.GetHostAddresses(""))
				{
					if (hostAddress != IPAddress.Loopback &&
						hostAddress.AddressFamily != AddressFamily.InterNetworkV6)
					{
						_hostAddress = hostAddress;
						break;
					}
				}

				if (_hostAddress == null)
				{
					_hostAddress = IPAddress.Loopback;
				}
			}

			return _hostAddress;
		}		
	}
}