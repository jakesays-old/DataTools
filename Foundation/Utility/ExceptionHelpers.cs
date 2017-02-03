using System;

namespace Std.Utility
{
	public static class ExceptionHelpers
	{
		public static TExceptionType CreateException<TExceptionType>(string message)
			where TExceptionType : Exception
		{
			var ex = (TExceptionType) Activator.CreateInstance(typeof(TExceptionType),
				new object[] { message });

			return ex;
		}

		public static TExceptionType CreateException<TExceptionType>(string message, params object[] args)
			where TExceptionType : Exception
		{
			return CreateException<TExceptionType>(string.Format(message, args));
		}

		public static Exception CreateException(Func<Exception> initializer)
		{
			var ex = initializer();

			return ex;
		}

		public static ArgumentException CreateArgumentException(string argName)
		{
			var ex = new ArgumentNullException(argName);
			return ex;
		}

		public static ArgumentException CreateArgumentException(string argName, string message, params object[] messageArgs)
		{
			var msg = string.Format(message, messageArgs);
			var ex = new ArgumentNullException(msg, argName);
			return ex;
		}

		public static ArgumentNullException CreateArgumentNullException(string argName)
		{
			var ex = new ArgumentNullException(argName);
			return ex;
		}

		public static ArgumentNullException CreateArgumentNullException(string argName, string message, params object[] messageArgs)
		{
			var msg = string.Format(message, messageArgs);
			var ex = new ArgumentNullException(msg, argName);

			return ex;
		}

		public static InvalidOperationException CreateInvalidOperationException(string message)
		{
			var ex = new InvalidOperationException(message);

			return ex;
		}

		public static InvalidOperationException CreateInvalidOperationException(string message, params object[] messageArgs)
		{
			var msg = string.Format(message, messageArgs);
			var ex = new InvalidOperationException(msg);

			return ex;
		}
	}
}