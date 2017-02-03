using System;
using System.Threading;

namespace Std.Utility.Threading
{
	public class ReadGuard : IDisposable
	{
		private readonly ReaderWriterLockSlim _rwlock;

		public ReadGuard(ReaderWriterLockSlim rwlock)
		{
			_rwlock = rwlock;
			_rwlock.EnterReadLock();
		}

		public void Dispose()
		{
			if (_rwlock != null)
			{
				_rwlock.ExitReadLock();
			}
		}
	}
}