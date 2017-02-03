using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

//Borrowed from http://ipnetwork.codeplex.com/ 

namespace Std.Utility.Net
{
	public class IPAddressCollection : IEnumerable<IPAddress>, IEnumerator<IPAddress>
	{

		private readonly IPNetwork _ipnetwork;
		private double _enumerator;

		internal IPAddressCollection(IPNetwork ipnetwork)
		{
			_ipnetwork = ipnetwork;
			_enumerator = -1;
		}

		public double Count
		{
			get
			{
				return _ipnetwork.UsableAddresses + 2;
			}
		}

		public IPAddress this[double i]
		{
			get
			{
				if (i >= Count)
				{
					throw new ArgumentOutOfRangeException("i");
				}

				var ipn = IPNetwork.Subnet(_ipnetwork, 32);
				return IPNetwork.ToIPAddress(ipn[i].Network);
			}
		}

		IEnumerator<IPAddress> IEnumerable<IPAddress>.GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}

		public IPAddress Current
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