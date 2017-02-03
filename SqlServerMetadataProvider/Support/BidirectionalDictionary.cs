//---------------------------------------------------------------------
// <copyright file="BidirectionalDictionary.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner       [....]
// @backupOwner [....]
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Std.Tools.Data.Metadata.Support
{
	/// <summary>
	/// This class provide service for both the singularization and pluralization, it takes the word pairs
	/// in the ctor following the rules that the first one is singular and the second one is plural.
	/// </summary>
	internal class BidirectionalDictionary<TFirst, TSecond>
	{
		private readonly Dictionary<TFirst, TSecond> _firstToSecondDictionary;
		private readonly Dictionary<TSecond, TFirst> _secondToFirstDictionary;

		internal BidirectionalDictionary()
		{
			_firstToSecondDictionary = new Dictionary<TFirst, TSecond>();
			_secondToFirstDictionary = new Dictionary<TSecond, TFirst>();
		}

		internal BidirectionalDictionary(Dictionary<TFirst, TSecond> firstToSecondDictionary)
			: this()
		{
			foreach (var key in firstToSecondDictionary.Keys)
			{
				AddValue(key, firstToSecondDictionary[key]);
			}
		}

		internal virtual bool ExistsInFirst(TFirst value)
		{
			if (_firstToSecondDictionary.ContainsKey(value))
			{
				return true;
			}

			return false;
		}

		internal virtual bool ExistsInSecond(TSecond value)
		{
			if (_secondToFirstDictionary.ContainsKey(value))
			{
				return true;
			}

			return false;
		}

		internal virtual TSecond GetSecondValue(TFirst value)
		{
			if (ExistsInFirst(value))
			{
				return _firstToSecondDictionary[value];
			}

			return default(TSecond);
		}

		internal virtual TFirst GetFirstValue(TSecond value)
		{
			if (ExistsInSecond(value))
			{
				return _secondToFirstDictionary[value];
			}

			return default(TFirst);
		}

		internal void AddValue(TFirst firstValue,
		                       TSecond secondValue)
		{
			_firstToSecondDictionary.Add(firstValue, secondValue);

			if (!_secondToFirstDictionary.ContainsKey(secondValue))
			{
				_secondToFirstDictionary.Add(secondValue, firstValue);
			}
		}
	}

	internal class StringBidirectionalDictionary : BidirectionalDictionary<string, string>
	{
		internal StringBidirectionalDictionary()
			: base()
		{
		}

		internal StringBidirectionalDictionary(Dictionary<string, string> firstToSecondDictionary)
			: base(firstToSecondDictionary)
		{
		}

		internal override bool ExistsInFirst(string value)
		{
			return base.ExistsInFirst(value.ToLowerInvariant());
		}

		internal override bool ExistsInSecond(string value)
		{
			return base.ExistsInSecond(value.ToLowerInvariant());
		}

		internal override string GetFirstValue(string value)
		{
			return base.GetFirstValue(value.ToLowerInvariant());
		}

		internal override string GetSecondValue(string value)
		{
			return base.GetSecondValue(value.ToLowerInvariant());
		}
	}
}