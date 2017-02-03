namespace Std.Utility.Collections.Generic
{
	internal static class Errors
	{
		internal static string CopyTo_ArgumentsTooSmall
		{
			get
			{
				return "Destination array is not long enough to copy all the items in the collection. Check array index and length.";
			}
		}

		internal static string Create_TValueCollectionReadOnly
		{
			get
			{
				return
					"The specified TValueCollection creates collections that have IsReadOnly set to true by default. TValueCollection must be a mutable ICollection.";
			}
		}

		internal static string Enumerator_AfterCurrent
		{
			get
			{
				return "Enumeration has already completed.";
			}
		}

		internal static string Enumerator_BeforeCurrent
		{
			get
			{
				return "Enumeration has not started. Call MoveNext() before Current.";
			}
		}

		internal static string Enumerator_Modification
		{
			get
			{
				return "Collection was modified; enumeration operation may not execute";
			}
		}

		internal static string KeyNotFound
		{
			get
			{
				return "The given key was not present.";
			}
		}

		internal static string ReadOnly_Modification
		{
			get
			{
				return "The collection is read-only";
			}
		}
	}
}