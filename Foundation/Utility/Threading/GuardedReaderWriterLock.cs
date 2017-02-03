using System;
using System.Threading;

namespace Std.Utility.Threading
{
	/// <summary>
	/// Simple ReaderWriterLockShim wrapper that exposes read and write
	/// guards.
	/// </summary>
	public class GuardedReaderWriterLock
	{
		private readonly ReaderWriterLockSlim _lock;
		private readonly AutoGuard _readGuard;
		private readonly AutoGuard _writeGuard;
		private readonly UpgradeGuard _upgradeGuard;

		class AutoGuard : IDisposable
		{
			private readonly ReaderWriterLockSlim _lock;
			private readonly bool _isWriteGuard;
			
			public AutoGuard(ReaderWriterLockSlim @lock, bool isWriteGuard)
			{
				_lock = @lock;
				_isWriteGuard = isWriteGuard;
			}
			
			public IDisposable Enter()
			{
				if (_isWriteGuard)
				{
					_lock.EnterWriteLock();
				}
				else
				{
					_lock.EnterReadLock();
				}
				
				return this;
			}

			public void Dispose()
			{
				if (_isWriteGuard)
				{
					_lock.ExitWriteLock();
				}
				else
				{
					_lock.ExitReadLock();
				}				
			}
		}

		class UpgradeGuard : IDisposable
		{
			private readonly ReaderWriterLockSlim _lock;

			public UpgradeGuard(ReaderWriterLockSlim @lock)
			{
				_lock = @lock;
			}

			public IDisposable Enter()
			{
				_lock.EnterUpgradeableReadLock();

				return this;
			}

			public void Dispose()
			{
				_lock.ExitUpgradeableReadLock();
			}
		}

		public ReaderWriterLockSlim UnderlyingLock
		{
			get { return _lock; }
		}

		public GuardedReaderWriterLock()
		{
			_lock = new ReaderWriterLockSlim();
			_readGuard = new AutoGuard(_lock, false);
			_writeGuard = new AutoGuard(_lock, true);
			_upgradeGuard = new UpgradeGuard(_lock);
		}

		public GuardedReaderWriterLock(LockRecursionPolicy recursionPolicy)
		{
			_lock = new ReaderWriterLockSlim(recursionPolicy);
			_readGuard = new AutoGuard(_lock, false);
			_writeGuard = new AutoGuard(_lock, true);
			_upgradeGuard = new UpgradeGuard(_lock);
		}

		public IDisposable Read
		{
			get { return _readGuard.Enter(); }
		}

		public IDisposable ReadUpgradable
		{
			get { return _upgradeGuard.Enter(); }
		}

		public IDisposable Write
		{
			get { return _writeGuard.Enter(); }
		}
	}
}
