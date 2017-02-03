using System;
using System.Threading;

namespace Std.Utility.Threading
{
	public class WriteGuard : IDisposable
	{
		private readonly ReaderWriterLockSlim _rwlock;

		public WriteGuard(ReaderWriterLockSlim rwlock)
		{
			_rwlock = rwlock;
			_rwlock.EnterWriteLock();
		}

		public void Dispose()
		{
			if (_rwlock != null)
			{
				_rwlock.ExitWriteLock();
			}
		}
	}
}