//---------------------------------------------------------------------
// <copyright file="PluralizationServiceUtil.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner       [....]
// @backupOwner [....]
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Std.Tools.Data.Metadata.Support
{
	internal static class PluralizationServiceUtil
	{
		internal static bool DoesWordContainSuffix(string word,
		                                           IEnumerable<string> suffixes,
		                                           CultureInfo culture)
		{
			return suffixes.Any(s => word.EndsWith(s, true, culture));
		}

		internal static bool TryInflectOnSuffixInWord(string word,
		                                              IEnumerable<string> suffixes,
		                                              Func<string, string> operationOnWord,
		                                              CultureInfo culture,
		                                              out string newWord)
		{
			newWord = null;

			if (DoesWordContainSuffix(word, suffixes, culture))
			{
				newWord = operationOnWord(word);
				return true;
			}

			return false;
		}
	}
}