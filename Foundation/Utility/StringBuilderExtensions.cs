using System;
using System.Text;
using JetBrains.Annotations;

namespace Std.Utility
{
	public static class StringBuilderExtensions
	{
		[StringFormatMethod("message")]
		public static StringBuilder FormatLine(this StringBuilder sb, string message, object arg0)
		{
			if (sb == null)
			{
				throw new ArgumentNullException(nameof(sb));
			}

			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			return sb.AppendFormat(message, arg0).AppendLine();
		}

		[StringFormatMethod("message")]
		public static StringBuilder FormatLine(this StringBuilder sb, string message, object arg0, object arg1)
		{
			if (sb == null)
			{
				throw new ArgumentNullException(nameof(sb));
			}

			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			return sb.AppendFormat(message, arg0, arg1).AppendLine();
		}

		[StringFormatMethod("message")]
		public static StringBuilder FormatLine(this StringBuilder sb, string message, object arg0, object arg1, object arg2)
		{
			if (sb == null)
			{
				throw new ArgumentNullException(nameof(sb));
			}

			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			return sb.AppendFormat(message, arg0, arg1, arg2).AppendLine();
		}

		[StringFormatMethod("message")]
		public static StringBuilder FormatLine(this StringBuilder sb, string message, params object[] args)
		{
			if (sb == null)
			{
				throw new ArgumentNullException(nameof(sb));
			}

			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			return sb.AppendFormat(message, args).AppendLine();
		}
	}
}