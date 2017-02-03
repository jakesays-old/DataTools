using System;
using System.IO;
using System.Threading.Tasks;

namespace Std.Utility.IO
{
	public class FileChangeWatcher
	{
		private readonly string _targetPath;
		private readonly Action _changeNotifyCallback;
		private FileSystemWatcher _changeWatcher;
		private DateTime _timeOfLastNotification = DateTime.MinValue;

		public FileChangeWatcher(string targetPath, Action changeNotifyCallback)
		{
			_targetPath = targetPath;
			_changeNotifyCallback = changeNotifyCallback;
		}

		public bool IsActive
		{
			get { return _changeWatcher != null; }
		}

		private const double ChangeNotificationDebounceDelay =
#if DEBUG
			//delay longer in debug mode to work around stepping delays
			20.0
#else
 2.0
#endif
;
		public void BeginWatching()
		{
			EndWatching();

			var extension = Path.GetExtension(_targetPath);
			if (extension == "")
			{
				extension = ".*";
			}
			else
			{
				extension = "*" + extension;
			}

			_changeWatcher = new FileSystemWatcher(Path.GetDirectoryName(_targetPath), extension)
			{
				NotifyFilter = NotifyFilters.LastWrite
			};

			_changeWatcher.Changed += ChangeHandler;

			_changeWatcher.EnableRaisingEvents = true;
		}

		public void EndWatching()
		{
			_timeOfLastNotification = DateTime.MinValue;

			if (_changeWatcher != null)
			{
				_changeWatcher.EnableRaisingEvents = false;
				_changeWatcher.Changed -= ChangeHandler;

				_changeWatcher = null;
			}
		}

		private void ChangeHandler(object source, FileSystemEventArgs e)
		{
			var timeReceived = DateTime.Now;

			//make sure this is the file we're interested in
			if (e.Name != Path.GetFileName(_targetPath))
			{
				//ignore the event if its not our target file
				return;
			}

			if (_timeOfLastNotification != DateTime.MinValue)
			{
				var diff = timeReceived - _timeOfLastNotification;

				if (diff.TotalSeconds < ChangeNotificationDebounceDelay)
				{
					//ignore if notification received less than ChangeNotificationDebounceDelay seconds from previous one
					return;
				}
			}

			_timeOfLastNotification = timeReceived;

			if (_changeNotifyCallback != null)
			{
				Task.Factory.StartNew(_changeNotifyCallback);
			}
		}
	}
}