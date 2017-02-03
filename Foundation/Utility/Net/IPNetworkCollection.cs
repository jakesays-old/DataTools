using System;
using System.Collections;
using System.Collections.Generic;

//Borrowed from http://ipnetwork.codeplex.com/ 

namespace Std.Utility.Net
{
	public class IPNetworkCollection : IEnumerable<IPNetwork>, IEnumerator<IPNetwork>
	{
		private double _enumerator;
		private readonly byte _cidrSubnet;
		private readonly IPNetwork _ipnetwork;

		private byte Cidr
		{
			get { return _ipnetwork.Cidr; }
		}

		private uint BroadcastAddress
		{
			get { return _ipnetwork.BroadcastAddress; }
		}

		private uint Network
		{
			get { return _ipnetwork.Network; }
		}

		internal IPNetworkCollection(IPNetwork ipnetwork, byte cidrSubnet)
		{
			if (cidrSubnet > 32)
			{
				throw new ArgumentOutOfRangeException("cidrSubnet");
			}

			if (cidrSubnet < ipnetwork.Cidr)
			{
				throw new ArgumentException("cidr");
			}

			_cidrSubnet = cidrSubnet;
			_ipnetwork = ipnetwork;
			_enumerator = -1;
		}

		public double Count
		{
			get
			{
				double count = Math.Pow(2, _cidrSubnet - Cidr);
				return count;
			}
		}

		public IPNetwork this[double i]
		{
			get
			{
				if (i >= Count)
				{
					throw new ArgumentOutOfRangeException("i");
				}
				var size = Count;
				var increment = (int) ((BroadcastAddress - Network) / size);
				var uintNetwork = (uint) (Network + ((increment + 1) * i));
				var ipn = new IPNetwork(uintNetwork, _cidrSubnet);
				return ipn;
			}
		}

		IEnumerator<IPNetwork> IEnumerable<IPNetwork>.GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}

		public IPNetwork Current
		{
			get { return this[_enumerator]; }
		}

		public void Dispose()
		{
			// nothing to dispose
			return;
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public bool MoveNext()
		{
			_enumerator++;
			if (_enumerator >= Count)
			{
				return false;
			}
			return true;

		}

		public void Reset()
		{
			_enumerator = -1;
		}
	}
}