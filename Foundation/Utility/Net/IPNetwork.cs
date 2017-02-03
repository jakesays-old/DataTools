using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

//Borrowed from http://ipnetwork.codeplex.com/ 

namespace Std.Utility.Net
{
	/// <summary>
	/// IP Network utility class. 
	/// Use Parse to create instances.
	/// </summary>
	public class IPNetwork : IComparable<IPNetwork>, IEquatable<IPNetwork>
	{
		internal IPNetwork(uint ipaddress, byte cidr)
		{
			if (cidr > 32)
			{
				throw new ArgumentOutOfRangeException("cidr");
			}

			IpAddress = ipaddress;
			Cidr = cidr;
			Netmask = cidr == 0 ? 0 : 0xffffffff << (32 - cidr);
			Network = IpAddress & Netmask;
			BroadcastAddress = Network + ~Netmask;
			NetmaskCidr = BitsSet(Netmask);
			UsableAddresses = (NetmaskCidr > 30) ? 0 : ((0xffffffff >> NetmaskCidr) - 1);
			FirstUsableAddress = (UsableAddresses <= 0) ? Network : Network + 1;
			LastUsableAddress = (UsableAddresses <= 0) ? Network : BroadcastAddress - 1;
		}

		/// <summary>
		/// Original IP address used to create this network
		/// </summary>
		public uint IpAddress { get; private set; }

		/// <summary>
		/// Network address
		/// </summary>
		public uint Network { get; private set; }

		/// <summary>
		/// Netmask
		/// </summary>
		public uint Netmask { get; private set; }

		/// <summary>
		/// Netmask as CIDR
		/// </summary>
		public int NetmaskCidr { get; private set; }

		/// <summary>
		/// Broadcast address
		/// </summary>
		public uint BroadcastAddress { get; private set; }

		/// <summary>
		/// First usable IP adress in Network
		/// </summary>
		public uint FirstUsableAddress { get; private set; }

		/// <summary>
		/// Last usable IP adress in Network
		/// </summary>
		public uint LastUsableAddress { get; private set; }

		/// <summary>
		/// Number of usable IP adress in Network
		/// </summary>
		public uint UsableAddresses { get; private set; }

		/// <summary>
		/// The CIDR netmask notation
		/// </summary>
		public byte Cidr { get; private set; }

		/// <summary>
		/// 192.168.168.100 - 255.255.255.0
		/// 
		/// Network   : 192.168.168.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.168.1
		/// End       : 192.168.168.254
		/// Broadcast : 192.168.168.255
		/// </summary>
		public static IPNetwork Parse(string ipaddress, string netmask)
		{
			IPNetwork ipnetwork;
			InternalParse(false, ipaddress, netmask, out ipnetwork);

			return ipnetwork;
		}

		/// <summary>
		/// 192.168.168.100/24
		/// 
		/// Network   : 192.168.168.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.168.1
		/// End       : 192.168.168.254
		/// Broadcast : 192.168.168.255
		/// </summary>
		public static IPNetwork Parse(string ipaddress, byte cidr)
		{
			IPNetwork ipnetwork;
			InternalParse(false, ipaddress, cidr, out ipnetwork);

			return ipnetwork;
		}

		/// <summary>
		/// 192.168.168.100 255.255.255.0
		/// 
		/// Network   : 192.168.168.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.168.1
		/// End       : 192.168.168.254
		/// Broadcast : 192.168.168.255
		/// </summary>
		public static IPNetwork Parse(IPAddress ipaddress, IPAddress netmask)
		{
			IPNetwork ipnetwork;
			InternalParse(false, ipaddress, netmask, out ipnetwork);

			return ipnetwork;
		}

		/// <summary>
		/// 192.168.0.1/24
		/// 192.168.0.1 255.255.255.0
		/// 
		/// Network   : 192.168.0.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.0.1
		/// End       : 192.168.0.254
		/// Broadcast : 192.168.0.255
		/// </summary>
		public static IPNetwork Parse(string network)
		{
			IPNetwork ipnetwork;
			InternalParse(false, network, out ipnetwork);

			return ipnetwork;
		}


		/// <summary>
		/// 192.168.168.100 - 255.255.255.0
		/// 
		/// Network   : 192.168.168.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.168.1
		/// End       : 192.168.168.254
		/// Broadcast : 192.168.168.255
		/// </summary>
		public static bool TryParse(string ipaddress, string netmask, out IPNetwork ipnetwork)
		{
			IPNetwork ipnetwork2;
			InternalParse(true, ipaddress, netmask, out ipnetwork2);
			var parsed = (ipnetwork2 != null);
			ipnetwork = ipnetwork2;

			return parsed;
		}

		/// <summary>
		/// 192.168.168.100/24
		/// 
		/// Network   : 192.168.168.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.168.1
		/// End       : 192.168.168.254
		/// Broadcast : 192.168.168.255
		/// </summary>
		public static bool TryParse(string ipaddress, byte cidr, out IPNetwork ipnetwork)
		{
			IPNetwork ipnetwork2;
			InternalParse(true, ipaddress, cidr, out ipnetwork2);
			var parsed = (ipnetwork2 != null);
			ipnetwork = ipnetwork2;

			return parsed;
		}

		/// <summary>
		/// 192.168.0.1/24
		/// 192.168.0.1 255.255.255.0
		/// 
		/// Network   : 192.168.0.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.0.1
		/// End       : 192.168.0.254
		/// Broadcast : 192.168.0.255
		/// </summary>
		/// <param name="network"></param>
		/// <param name="ipnetwork"></param>
		/// <returns></returns>
		public static bool TryParse(string network, out IPNetwork ipnetwork)
		{
			IPNetwork ipnetwork2;
			InternalParse(true, network, out ipnetwork2);
			var parsed = (ipnetwork2 != null);
			ipnetwork = ipnetwork2;

			return parsed;
		}

		/// <summary>
		/// 192.168.0.1/24
		/// 192.168.0.1 255.255.255.0
		/// 
		/// Network   : 192.168.0.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.0.1
		/// End       : 192.168.0.254
		/// Broadcast : 192.168.0.255
		/// </summary>
		/// <param name="ipaddress"></param>
		/// <param name="netmask"></param>
		/// <param name="ipnetwork"></param>
		/// <returns></returns>
		public static bool TryParse(IPAddress ipaddress, IPAddress netmask, out IPNetwork ipnetwork)
		{
			IPNetwork ipnetwork2;
			InternalParse(true, ipaddress, netmask, out ipnetwork2);
			var parsed = (ipnetwork2 != null);
			ipnetwork = ipnetwork2;

			return parsed;
		}

		/// <summary>
		/// 192.168.168.100 - 255.255.255.0
		/// 
		/// Network   : 192.168.168.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.168.1
		/// End       : 192.168.168.254
		/// Broadcast : 192.168.168.255
		/// </summary>
		private static void InternalParse(bool tryParse, string ipaddress, string netmask, out IPNetwork ipnetwork)
		{
			ipnetwork = null;

			if (string.IsNullOrEmpty(ipaddress))
			{
				if (tryParse == false)
				{
					throw new ArgumentNullException("ipaddress");
				}
				return;
			}

			if (string.IsNullOrEmpty(netmask))
			{
				if (tryParse == false)
				{
					throw new ArgumentNullException("netmask");
				}

				return;
			}

			IPAddress ip;
			var ipaddressParsed = IPAddress.TryParse(ipaddress, out ip);
			if (ipaddressParsed == false)
			{
				if (tryParse == false)
				{
					throw new ArgumentException("ipaddress");
				}
				return;
			}

			IPAddress mask;
			var netmaskParsed = IPAddress.TryParse(netmask, out mask);
			if (netmaskParsed == false)
			{
				if (tryParse == false)
				{
					throw new ArgumentException("netmask");
				}
				return;
			}

			InternalParse(tryParse, ip, mask, out ipnetwork);
		}

		private static void InternalParse(bool tryParse, string network, out IPNetwork ipnetwork)
		{
			ipnetwork = null;

			if (string.IsNullOrEmpty(network))
			{
				if (tryParse == false)
				{
					throw new ArgumentNullException("network");
				}
				return;
			}

			network = Regex.Replace(network, @"[^0-9\.\/\s]+", "");
			network = Regex.Replace(network, @"\s{2,}", " ");
			network = network.Trim();
			var args = network.Split(new[] { ' ', '/' });
			byte cidr;

			if (args.Length == 1)
			{

				if (TryGuessCidr(args[0], out cidr))
				{
					InternalParse(tryParse, args[0], cidr, out ipnetwork);
					return;
				}

				if (tryParse == false)
				{
					throw new ArgumentException("network");
				}
				return;
			}

			if (byte.TryParse(args[1], out cidr))
			{
				InternalParse(tryParse, args[0], cidr, out ipnetwork);
				return;
			}

			InternalParse(tryParse, args[0], args[1], out ipnetwork);
		}

		/// <summary>
		/// 192.168.168.100 255.255.255.0
		/// 
		/// Network   : 192.168.168.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.168.1
		/// End       : 192.168.168.254
		/// Broadcast : 192.168.168.255
		/// </summary>
		private static void InternalParse(bool tryParse, IPAddress ipaddress, IPAddress netmask, out IPNetwork ipnetwork)
		{
			if (ipaddress == null)
			{
				if (tryParse == false)
				{
					throw new ArgumentNullException("ipaddress");
				}
				ipnetwork = null;
				return;
			}

			if (netmask == null)
			{
				if (tryParse == false)
				{
					throw new ArgumentNullException("netmask");
				}
				ipnetwork = null;
				return;
			}

			var uintIpAddress = ToUint(ipaddress);
			byte? cidr2;
			var parsed = TryToCidr(netmask, out cidr2);
			if (parsed == false)
			{
				if (tryParse == false)
				{
					throw new ArgumentException("netmask");
				}
				ipnetwork = null;
				return;
			}
			var cidr = (byte) cidr2;

			var ipnet = new IPNetwork(uintIpAddress, cidr);
			ipnetwork = ipnet;

			return;
		}

		/// <summary>
		/// 192.168.168.100/24
		/// 
		/// Network   : 192.168.168.0
		/// Netmask   : 255.255.255.0
		/// Cidr      : 24
		/// Start     : 192.168.168.1
		/// End       : 192.168.168.254
		/// Broadcast : 192.168.168.255
		/// </summary>
		private static void InternalParse(bool tryParse, string ipaddress, byte cidr, out IPNetwork ipnetwork)
		{

			if (string.IsNullOrEmpty(ipaddress))
			{
				if (tryParse == false)
				{
					throw new ArgumentNullException("ipaddress");
				}
				ipnetwork = null;
				return;
			}

			IPAddress ip;
			var ipaddressParsed = IPAddress.TryParse(ipaddress, out ip);

			if (ipaddressParsed == false)
			{
				if (tryParse == false)
				{
					throw new ArgumentException("ipaddress");
				}
				
				ipnetwork = null;
				
				return;
			}

			IPAddress mask;
			var parsedNetmask = TryToNetmask(cidr, out mask);

			if (parsedNetmask == false)
			{
				if (tryParse == false)
				{
					throw new ArgumentException("cidr");
				}

				ipnetwork = null;

				return;
			}

			InternalParse(tryParse, ip, mask, out ipnetwork);
		}

		/// <summary>
		/// Convert an ipadress to decimal
		/// 0.0.0.0 -> 0
		/// 0.0.1.0 -> 256
		/// </summary>
		public static uint ToUint(IPAddress ipaddress)
		{
			uint? uintIpAddress;
			InternalToUint(false, ipaddress, out uintIpAddress);

			return (uint) uintIpAddress;
		}

		/// <summary>
		/// Convert an ipadress to decimal
		/// 0.0.0.0 -> 0
		/// 0.0.1.0 -> 256
		/// </summary>
		public static bool TryToUint(IPAddress ipaddress, out uint? uintIpAddress)
		{
			uint? uintIpAddress2;
			InternalToUint(true, ipaddress, out uintIpAddress2);
			var parsed = (uintIpAddress2 != null);
			uintIpAddress = uintIpAddress2;

			return parsed;
		}

		private static void InternalToUint(bool tryParse, IPAddress ipaddress, out uint? uintIpAddress)
		{
			if (ipaddress == null)
			{
				if (tryParse == false)
				{
					throw new ArgumentNullException("ipaddress");
				}
				uintIpAddress = null;
				return;
			}

			var bytes = ipaddress.GetAddressBytes();
			if (bytes.Length != 4)
			{
				if (tryParse == false)
				{
					throw new ArgumentException("bytes");
				}
				uintIpAddress = null;
				return;

			}

			Array.Reverse(bytes);
			var value = BitConverter.ToUInt32(bytes, 0);
			uintIpAddress = value;

			return;
		}

		/// <summary>
		/// Convert a cidr to uint netmask
		/// </summary>
		/// <param name="cidr"></param>
		public static uint ToUint(byte cidr)
		{
			uint? uintNetmask;
			InternalToUint(false, cidr, out uintNetmask);
		
			return (uint) uintNetmask;
		}


		/// <summary>
		/// Convert a cidr to uint netmask
		/// </summary>
		/// <param name="cidr"></param>
		/// <returns></returns>
		public static bool TryToUint(byte cidr, out uint? uintNetmask)
		{
			uint? uintNetmask2;
			InternalToUint(true, cidr, out uintNetmask2);
			var parsed = (uintNetmask2 != null);
			uintNetmask = uintNetmask2;

			return parsed;
		}

		/// <summary>
		/// Convert a cidr to uint netmask
		/// </summary>
		private static void InternalToUint(bool tryParse, byte cidr, out uint? uintNetmask)
		{
			if (cidr > 32)
			{
				if (tryParse == false)
				{
					throw new ArgumentOutOfRangeException("cidr");
				}
				uintNetmask = null;
				return;
			}
			var uintNetmask2 = cidr == 0 ? 0 : 0xffffffff << (32 - cidr);
			uintNetmask = uintNetmask2;
		}

		/// <summary>
		/// Convert netmask to CIDR
		///  255.255.255.0 -> 24
		///  255.255.0.0   -> 16
		///  255.0.0.0     -> 8
		/// </summary>
		private static byte ToCidr(uint netmask)
		{
			byte? cidr;
			InternalToCidr(false, netmask, out cidr);

			return (byte) cidr;
		}

		/// <summary>
		/// Convert netmask to CIDR
		///  255.255.255.0 -> 24
		///  255.255.0.0   -> 16
		///  255.0.0.0     -> 8
		/// </summary>
		private static void InternalToCidr(bool tryParse, uint netmask, out byte? cidr)
		{

			if (!ValidNetmask(netmask))
			{
				if (tryParse == false)
				{
					throw new ArgumentException("netmask");
				}
				cidr = null;
				return;
			}

			var cidr2 = BitsSet(netmask);
			cidr = cidr2;
			return;

		}

		/// <summary>
		/// Convert netmask to CIDR
		///  255.255.255.0 -> 24
		///  255.255.0.0   -> 16
		///  255.0.0.0     -> 8
		/// </summary>
		public static byte ToCidr(IPAddress netmask)
		{
			byte? cidr;
			InternalToCidr(false, netmask, out cidr);

			return (byte) cidr;
		}

		/// <summary>
		/// Convert netmask to CIDR
		///  255.255.255.0 -> 24
		///  255.255.0.0   -> 16
		///  255.0.0.0     -> 8
		/// </summary>
		/// <param name="netmask"></param>
		/// <returns></returns>
		public static bool TryToCidr(IPAddress netmask, out byte? cidr)
		{
			byte? cidr2;
			InternalToCidr(true, netmask, out cidr2);
			var parsed = (cidr2 != null);
			cidr = cidr2;

			return parsed;
		}

		private static void InternalToCidr(bool tryParse, IPAddress netmask, out byte? cidr)
		{

			if (netmask == null)
			{
				if (tryParse == false)
				{
					throw new ArgumentNullException("netmask");
				}
				cidr = null;
				return;
			}
			uint? uintNetmask2;
			var parsed = TryToUint(netmask, out uintNetmask2);
			if (parsed == false)
			{
				if (tryParse == false)
				{
					throw new ArgumentException("netmask");
				}
				cidr = null;
				return;
			}
			var uintNetmask = (uint) uintNetmask2;

			byte? cidr2 = null;
			InternalToCidr(tryParse, uintNetmask, out cidr2);
			cidr = cidr2;

			return;

		}

		/// <summary>
		/// Convert CIDR to netmask
		///  24 -> 255.255.255.0
		///  16 -> 255.255.0.0
		///  8 -> 255.0.0.0
		/// </summary>
		/// <see cref="http://snipplr.com/view/15557/cidr-class-for-ipv4/"/>
		public static IPAddress ToNetmask(byte cidr)
		{
			IPAddress netmask;
			InternalToNetmask(false, cidr, out netmask);

			return netmask;
		}

		/// <summary>
		/// Convert CIDR to netmask
		///  24 -> 255.255.255.0
		///  16 -> 255.255.0.0
		///  8 -> 255.0.0.0
		/// </summary>
		public static bool TryToNetmask(byte cidr, out IPAddress netmask)
		{
			IPAddress netmask2;
			InternalToNetmask(true, cidr, out netmask2);
			var parsed = (netmask2 != null);
			netmask = netmask2;

			return parsed;
		}


		private static void InternalToNetmask(bool tryParse, byte cidr, out IPAddress netmask)
		{
			if (cidr < 0 || cidr > 32)
			{
				if (tryParse == false)
				{
					throw new ArgumentOutOfRangeException("cidr");
				}
				netmask = null;
				return;
			}

			var mask = ToUint(cidr);
			var netmask2 = ToIPAddress(mask);
			netmask = netmask2;

			return;
		}

		/// <summary>
		/// Count bits set to 1 in netmask
		/// </summary>
		/// <see cref="http://stackoverflow.com/questions/109023/best-algorithm-to-count-the-number-of-set-bits-in-a-32-bit-integer"/>
		/// <param name="netmask"></param>
		/// <returns></returns>
		private static byte BitsSet(uint netmask)
		{
			var i = netmask;
			i = i - ((i >> 1) & 0x55555555);
			i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
			i = ((i + (i >> 4) & 0xf0f0f0f) * 0x1010101) >> 24;

			return (byte) i;
		}

		/// <summary>
		/// Count bits set to 1 in netmask
		/// </summary>
		/// <param name="netmask"></param>
		/// <returns></returns>
		public static byte BitsSet(IPAddress netmask)
		{
			var uintNetmask = ToUint(netmask);
			var bits = BitsSet(uintNetmask);

			return bits;
		}

		/// <summary>
		/// return true if netmask is a valid netmask
		/// 255.255.255.0, 255.0.0.0, 255.255.240.0, ...
		/// </summary>
		/// <see cref="http://www.actionsnip.com/snippets/tomo_atlacatl/calculate-if-a-netmask-is-valid--as2-"/>
		/// <param name="netmask"></param>
		/// <returns></returns>
		public static bool ValidNetmask(IPAddress netmask)
		{
			if (netmask == null)
			{
				throw new ArgumentNullException("netmask");
			}
			var uintNetmask = ToUint(netmask);
			var valid = ValidNetmask(uintNetmask);

			return valid;
		}

		private static bool ValidNetmask(uint netmask)
		{
			var neg = ((~(int) netmask) & 0xffffffff);
			var isNetmask = ((neg + 1) & neg) == 0;

			return isNetmask;
		}

		/// <summary>
		/// Transform a uint ipaddress into IPAddress object
		/// </summary>
		/// <param name="ipaddress"></param>
		/// <returns></returns>
		public static IPAddress ToIPAddress(uint ipaddress)
		{
			var bytes = BitConverter.GetBytes(ipaddress);
			Array.Reverse(bytes);
			var ip = new IPAddress(bytes);

			return ip;
		}

		public bool Contains(uint ipaddress)
		{
			var contains = (ipaddress >= Network)
				&& (ipaddress <= BroadcastAddress);

			return contains;
		}

		/// <summary>
		/// return true if ipaddress is contained in network
		/// </summary>
		/// <param name="network"></param>
		/// <param name="ipaddress"></param>
		/// <returns></returns>
		public static bool Contains(IPNetwork network, uint ipaddress)
		{
			if (network == null)
			{
				throw new ArgumentNullException("network");
			}

			var contains = (ipaddress >= network.Network)
				&& (ipaddress <= network.BroadcastAddress);

			return contains;
		}

		/// <summary>
		/// return true is network2 is fully contained in network
		/// </summary>
		/// <param name="network"></param>
		/// <param name="network2"></param>
		/// <returns></returns>
		public static bool Contains(IPNetwork network, IPNetwork network2)
		{
			if (network == null)
			{
				throw new ArgumentNullException("network");
			}

			if (network2 == null)
			{
				throw new ArgumentNullException("network2");
			}

			var contains = (network2.Network >= network.Network)
				&& (network2.BroadcastAddress <= network.BroadcastAddress);

			return contains;
		}

		/// <summary>
		/// return true is network2 overlap network
		/// </summary>
		/// <param name="network"></param>
		/// <param name="network2"></param>
		/// <returns></returns>
		public static bool Overlap(IPNetwork network, IPNetwork network2)
		{
			if (network == null)
			{
				throw new ArgumentNullException("network");
			}

			if (network2 == null)
			{
				throw new ArgumentNullException("network2");
			}

			var uintNetwork = network.Network;
			var uintBroadcast = network.BroadcastAddress;

			var uintFirst = network2.Network;
			var uintLast = network2.BroadcastAddress;

			var overlap =
				(uintFirst >= uintNetwork && uintFirst <= uintBroadcast)
				|| (uintLast >= uintNetwork && uintLast <= uintBroadcast)
				|| (uintFirst <= uintNetwork && uintLast >= uintBroadcast)
				|| (uintFirst >= uintNetwork && uintLast <= uintBroadcast);

			return overlap;
		}

		public override string ToString()
		{
			return string.Format("{0}/{1}", Network, Cidr);
		}

		static IPNetwork()
		{
			ReservedCBlock = Parse("192.168.0.0/16");
			ReservedBBlock = Parse("172.16.0.0/12");
			ReservedABlock = Parse("10.0.0.0/8");
		}

		/// <summary>
		/// 10.0.0.0/8
		/// </summary>
		/// <returns></returns>
		public static IPNetwork ReservedABlock { get; private set; }

		/// <summary>
		/// 172.12.0.0/12
		/// </summary>
		/// <returns></returns>
		public static IPNetwork ReservedBBlock { get; private set; }

		/// <summary>
		/// 192.168.0.0/16
		/// </summary>
		/// <returns></returns>
		public static IPNetwork ReservedCBlock { get; private set; }

		/// <summary>
		/// return true if ipaddress is contained in 
		/// ReservedABlock, ReservedBBlock, ReservedCBlock
		/// </summary>
		/// <param name="ipaddress"></param>
		/// <returns></returns>
		//public static bool IsInReservedBlock(IPAddress ipaddress)
		//{
		//    if (ipaddress == null)
		//    {
		//        throw new ArgumentNullException("ipaddress");
		//    }

		//    return Contains(ReservedABlock, ipaddress)
		//        || Contains(ReservedBBlock, ipaddress)
		//        || Contains(ReservedCBlock, ipaddress);
		//}

		/// <summary>
		/// return true if ipnetwork is contained in 
		/// ReservedABlock, ReservedBBlock, ReservedCBlock
		/// </summary>
		/// <param name="ipnetwork"></param>
		/// <returns></returns>
		public static bool IsInReservedBlock(IPNetwork ipnetwork)
		{
			if (ipnetwork == null)
			{
				throw new ArgumentNullException("ipnetwork");
			}

			return Contains(ReservedABlock, ipnetwork)
				|| Contains(ReservedBBlock, ipnetwork)
				|| Contains(ReservedCBlock, ipnetwork);
		}

		/// <summary>
		/// Subnet a network into multiple nets of cidr mask
		/// Subnet 192.168.0.0/24 into cidr 25 gives 192.168.0.0/25, 192.168.0.128/25
		/// Subnet 10.0.0.0/8 into cidr 9 gives 10.0.0.0/9, 10.128.0.0/9
		/// </summary>
		/// <param name="network"></param>
		/// <param name="cidr"></param>
		/// <returns></returns>
		public static IPNetworkCollection Subnet(IPNetwork network, byte cidr)
		{
			IPNetworkCollection ipnetworkCollection;
			InternalSubnet(false, network, cidr, out ipnetworkCollection);

			return ipnetworkCollection;
		}

		/// <summary>
		/// Subnet a network into multiple nets of cidr mask
		/// Subnet 192.168.0.0/24 into cidr 25 gives 192.168.0.0/25, 192.168.0.128/25
		/// Subnet 10.0.0.0/8 into cidr 9 gives 10.0.0.0/9, 10.128.0.0/9
		/// </summary>
		public static bool TrySubnet(IPNetwork network, byte cidr, out IPNetworkCollection ipnetworkCollection)
		{
			IPNetworkCollection inc;
			InternalSubnet(true, network, cidr, out inc);
			if (inc == null)
			{
				ipnetworkCollection = null;
				return false;
			}

			ipnetworkCollection = inc;
			return true;
		}

		private static void InternalSubnet(bool trySubnet, IPNetwork network, byte cidr,
			out IPNetworkCollection ipnetworkCollection)
		{
			if (network == null)
			{
				if (trySubnet == false)
				{
					throw new ArgumentNullException("network");
				}
				ipnetworkCollection = null;
				return;
			}

			if (cidr > 32)
			{
				if (trySubnet == false)
				{
					throw new ArgumentOutOfRangeException("cidr");
				}
				ipnetworkCollection = null;
				return;
			}

			if (cidr < network.Cidr)
			{
				if (trySubnet == false)
				{
					throw new ArgumentException("cidr");
				}
				ipnetworkCollection = null;
				return;
			}

			ipnetworkCollection = new IPNetworkCollection(network, cidr);

			return;
		}

		/// <summary>
		/// Supernet two consecutive cidr equal subnet into a single one
		/// 192.168.0.0/24 + 192.168.1.0/24 = 192.168.0.0/23 
		/// 10.1.0.0/16 + 10.0.0.0/16 = 10.0.0.0/15
		/// 192.168.0.0/24 + 192.168.0.0/25 = 192.168.0.0/24 
		/// </summary>
		/// <param name="network1"></param>
		/// <param name="network2"></param>
		/// <returns></returns>
		public static IPNetwork Supernet(IPNetwork network1, IPNetwork network2)
		{
			IPNetwork supernet;
			InternalSupernet(false, network1, network2, out supernet);

			return supernet;
		}

		/// <summary>
		/// Try to supernet two consecutive cidr equal subnet into a single one
		/// 192.168.0.0/24 + 192.168.1.0/24 = 192.168.0.0/23 
		/// 10.1.0.0/16 + 10.0.0.0/16 = 10.0.0.0/15
		/// 192.168.0.0/24 + 192.168.0.0/25 = 192.168.0.0/24 
		/// </summary>
		/// <param name="network1"></param>
		/// <param name="network2"></param>
		/// <param name="supernet"></param>
		/// <returns></returns>
		public static bool TrySupernet(IPNetwork network1, IPNetwork network2, out IPNetwork supernet)
		{
			IPNetwork outSupernet;
			InternalSupernet(true, network1, network2, out outSupernet);
			var parsed = (outSupernet != null);
			supernet = outSupernet;

			return parsed;
		}

		private static void InternalSupernet(bool trySupernet, IPNetwork network1, IPNetwork network2,
			out IPNetwork supernet)
		{
			if (network1 == null)
			{
				if (trySupernet == false)
				{
					throw new ArgumentNullException("network1");
				}
				supernet = null;
				return;
			}

			if (network2 == null)
			{
				if (trySupernet == false)
				{
					throw new ArgumentNullException("network2");
				}
				supernet = null;
				return;
			}

			if (Contains(network1, network2))
			{
				supernet = new IPNetwork(network1.Network, network1.Cidr);
				return;
			}

			if (Contains(network2, network1))
			{
				supernet = new IPNetwork(network2.Network, network2.Cidr);
				return;
			}

			if (network1.Cidr != network2.Cidr)
			{
				if (trySupernet == false)
				{
					throw new ArgumentException("cidr");
				}
				supernet = null;
				return;
			}

			var first = (network1.Network < network2.Network) ? network1 : network2;
			var last = (network1.Network > network2.Network) ? network1 : network2;

			// Starting from here :
			// network1 and network2 have the same cidr,
			// network1 does not contain network2,
			// network2 does not contain network1,
			// first is the lower subnet
			// last is the higher subnet

			if ((first.BroadcastAddress + 1) != last.Network)
			{
				if (trySupernet == false)
				{
					throw new ArgumentOutOfRangeException("network");
				}

				supernet = null;

				return;
			}

			var uintSupernet = first.Network;
			var cidrSupernet = (byte) (first.Cidr - 1);

			var networkSupernet = new IPNetwork(uintSupernet, cidrSupernet);
			if (networkSupernet.Network != first.Network)
			{
				if (trySupernet == false)
				{
					throw new ArgumentException("network");
				}
				supernet = null;
				return;
			}
			supernet = networkSupernet;

			return;
		}

		/// <summary>
		/// Supernet a list of subnet
		/// 192.168.0.0/24 + 192.168.1.0/24 = 192.168.0.0/23
		/// 192.168.0.0/24 + 192.168.1.0/24 + 192.168.2.0/24 + 192.168.3.0/24 = 192.168.0.0/22
		/// </summary>
		public static IPNetwork[] Supernet(IPNetwork[] ipnetworks)
		{
			IPNetwork[] supernet;
			InternalSupernet(false, ipnetworks, out supernet);

			return supernet;
		}

		/// <summary>
		/// Supernet a list of subnet
		/// 192.168.0.0/24 + 192.168.1.0/24 = 192.168.0.0/23
		/// 192.168.0.0/24 + 192.168.1.0/24 + 192.168.2.0/24 + 192.168.3.0/24 = 192.168.0.0/22
		/// </summary>
		/// <param name="ipnetworks"></param>
		/// <param name="supernet"></param>
		/// <returns></returns>
		public static bool TrySupernet(IPNetwork[] ipnetworks, out IPNetwork[] supernet)
		{
			var supernetted = InternalSupernet(true, ipnetworks, out supernet);

			return supernetted;
		}

		public static bool InternalSupernet(bool trySupernet, IPNetwork[] ipnetworks, out IPNetwork[] supernet)
		{
			if (ipnetworks == null)
			{
				if (trySupernet == false)
				{
					throw new ArgumentNullException("ipnetworks");
				}
				supernet = null;
				return false;
			}

			if (ipnetworks.Length <= 0)
			{
				supernet = new IPNetwork[0];
				return true;
			}

			var supernetted = new List<IPNetwork>();
			var ipns = Array2List(ipnetworks);
			var current = List2Stack(ipns);
			var previousCount = 0;
			var currentCount = current.Count;

			while (previousCount != currentCount)
			{

				supernetted.Clear();
				while (current.Count > 1)
				{
					var ipn1 = current.Pop();
					var ipn2 = current.Peek();

					IPNetwork outNetwork;
					var success = TrySupernet(ipn1, ipn2, out outNetwork);

					if (success)
					{
						current.Pop();
						current.Push(outNetwork);
					}
					else
					{
						supernetted.Add(ipn1);
					}
				}
				if (current.Count == 1)
				{
					supernetted.Add(current.Pop());
				}

				previousCount = currentCount;
				currentCount = supernetted.Count;
				current = List2Stack(supernetted);

			}

			supernet = supernetted.ToArray();

			return true;
		}

		private static Stack<IPNetwork> List2Stack(List<IPNetwork> list)
		{
			var stack = new Stack<IPNetwork>();
			list.ForEach(stack.Push);
			
			return stack;
		}

		private static List<IPNetwork> Array2List(IPNetwork[] array)
		{
			var ipns = new List<IPNetwork>();
			ipns.AddRange(array);
			RemoveNull(ipns);

			ipns.Sort(delegate(IPNetwork ipn1, IPNetwork ipn2)
			{
				var networkCompare = ipn1.Network.CompareTo(ipn2.Network);
				if (networkCompare == 0)
				{
					var cidrCompare = ipn1.Cidr.CompareTo(ipn2.Cidr);
					return cidrCompare;
				}
				return networkCompare;
			});
			ipns.Reverse();

			return ipns;
		}

		private static void RemoveNull(List<IPNetwork> ipns)
		{
			ipns.RemoveAll(delegate(IPNetwork ipn)
			{
				if (ipn == null)
				{
					return true;
				}
				return false;
			});
		}

		//public static IPNetwork WideSubnet(string start, string end)
		//{
		//    if (string.IsNullOrEmpty(start))
		//    {
		//        throw new ArgumentNullException("start");
		//    }

		//    if (string.IsNullOrEmpty(end))
		//    {
		//        throw new ArgumentNullException("end");
		//    }

		//    IPAddress startIP;
		//    if (!IPAddress.TryParse(start, out startIP))
		//    {
		//        throw new ArgumentException("start");
		//    }

		//    IPAddress endIP;
		//    if (!IPAddress.TryParse(end, out endIP))
		//    {
		//        throw new ArgumentException("end");
		//    }

		//    var ipnetwork = new IPNetwork(0, 0);
		//    for (byte cidr = 32; cidr >= 0; cidr--)
		//    {
		//        var wideSubnet = Parse(start, cidr);
		//        if (Contains(wideSubnet, endIP))
		//        {
		//            ipnetwork = wideSubnet;
		//            break;
		//        }
		//    }
		//    return ipnetwork;

		//}

		public static bool TryWideSubnet(IPNetwork[] ipnetworks, out IPNetwork ipnetwork)
		{
			IPNetwork ipn;
			InternalWideSubnet(true, ipnetworks, out ipn);
			if (ipn == null)
			{
				ipnetwork = null;
				return false;
			}
			ipnetwork = ipn;
			return true;
		}

		public static IPNetwork WideSubnet(IPNetwork[] ipnetworks)
		{
			IPNetwork ipn = null;
			InternalWideSubnet(false, ipnetworks, out ipn);
			return ipn;
		}

		private static void InternalWideSubnet(bool tryWide, IPNetwork[] ipnetworks, out IPNetwork ipnetwork)
		{

			if (ipnetworks == null)
			{
				if (tryWide == false)
				{
					throw new ArgumentNullException("ipnetworks");
				}
				ipnetwork = null;
				return;
			}

			var nnin = Array.FindAll(ipnetworks, ipnet => ipnet != null);

			if (nnin.Length <= 0)
			{
				if (tryWide == false)
				{
					throw new ArgumentException("ipnetworks");
				}
				ipnetwork = null;
				return;
			}

			if (nnin.Length == 1)
			{
				var ipn0 = nnin[0];
				ipnetwork = ipn0;
				return;
			}

			Array.Sort(nnin);
			var nnin0 = nnin[0];
			var uintNnin0 = nnin0.IpAddress;

			var nninX = nnin[nnin.Length - 1];
			var ipaddressX = nninX.BroadcastAddress;

			var ipn = new IPNetwork(0, 0);
			for (var cidr = nnin0.Cidr; cidr >= 0; cidr--)
			{
				var wideSubnet = new IPNetwork(uintNnin0, cidr);
				if (Contains(wideSubnet, ipaddressX))
				{
					ipn = wideSubnet;
					break;
				}
			}

			ipnetwork = ipn;
			return;
		}

		/// <summary>
		/// Print an ipnetwork in a clear representation string
		/// </summary>
		/// <param name="ipnetwork"></param>
		/// <returns></returns>
		public static string Print(IPNetwork ipnetwork)
		{
			if (ipnetwork == null)
			{
				throw new ArgumentNullException("ipnetwork");
			}

			var sw = new StringWriter();

			sw.WriteLine("IPNetwork   : {0}", ipnetwork);
			sw.WriteLine("Network     : {0}", ipnetwork.Network);
			sw.WriteLine("Netmask     : {0}", ipnetwork.Netmask);
			sw.WriteLine("Cidr        : {0}", ipnetwork.Cidr);
			sw.WriteLine("Broadcast   : {0}", ipnetwork.BroadcastAddress);
			sw.WriteLine("FirstUsable : {0}", ipnetwork.FirstUsableAddress);
			sw.WriteLine("LastUsable  : {0}", ipnetwork.LastUsableAddress);
			sw.WriteLine("Usable      : {0}", ipnetwork.UsableAddresses);

			return sw.ToString();
		}

		/// <summary>
		/// 
		/// Class              Leading bits    Default netmask
		///     A (CIDR /8)	       00           255.0.0.0
		///     A (CIDR /8)	       01           255.0.0.0
		///     B (CIDR /16)	   10           255.255.0.0
		///     C (CIDR /24)       11 	        255.255.255.0
		///  
		/// </summary>
		/// <param name="ip"></param>
		/// <param name="cidr"></param>
		/// <returns></returns>
		public static bool TryGuessCidr(string ip, out byte cidr)
		{
			IPAddress ipaddress;
			var parsed = IPAddress.TryParse(string.Format("{0}", ip), out ipaddress);
			if (parsed == false)
			{
				cidr = 0;
				return false;
			}

			var uintIPAddress = ToUint(ipaddress);
			uintIPAddress = uintIPAddress >> 29;
			if (uintIPAddress <= 3)
			{
				cidr = 8;
				return true;
			}

			if (uintIPAddress <= 5)
			{
				cidr = 16;
				return true;
			}

			if (uintIPAddress <= 6)
			{
				cidr = 24;
				return true;
			}

			cidr = 0;

			return false;

		}

		/// <summary>
		/// Try to parse cidr. Have to be >= 0 and <= 32
		/// </summary>
		/// <param name="sidr"></param>
		/// <param name="cidr"></param>
		/// <returns></returns>
		public static bool TryParseCidr(string sidr, out byte? cidr)
		{

			byte b = 0;
			if (!byte.TryParse(sidr, out b))
			{
				cidr = null;
				return false;
			}

			IPAddress netmask;
			if (!TryToNetmask(b, out netmask))
			{
				cidr = null;
				return false;
			}

			cidr = b;
			return true;
		}

		public static IPAddressCollection ListIPAddress(IPNetwork ipnetwork)
		{
			return new IPAddressCollection(ipnetwork);
		}

		public int CompareTo(IPNetwork other)
		{
			var network = Network.CompareTo(other.Network);
			if (network != 0)
			{
				return network;
			}

			var cidr = Cidr.CompareTo(other.Cidr);

			return cidr;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(IPNetwork))
			{
				return false;
			}
			return Equals((IPNetwork) obj);
		}

		public static bool operator ==(IPNetwork lhs, IPNetwork rhs)
		{
			if (ReferenceEquals(null, lhs))
			{
				return false;
			}

			if (ReferenceEquals(null, rhs))
			{
				return false;
			}

			if (ReferenceEquals(lhs, rhs))
			{
				return true;
			}

			return lhs.Network == rhs.Network && lhs.Netmask == rhs.Netmask;
		}

		public static bool operator !=(IPNetwork lhs, IPNetwork rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(IPNetwork other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return other.Network == Network && other.Netmask == Netmask;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Network.GetHashCode() * 397) ^ Netmask.GetHashCode();
			}
		}
	}
}