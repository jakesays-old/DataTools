//---------------------------------------------------------------------
// <copyright file="EnglishPluralizationService.cs" company="Microsoft">
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
using System.Text;
using System.Text.RegularExpressions;

namespace Std.Tools.Data.Metadata.Support
{
	internal partial class EnglishPluralizationService : PluralizationService, ICustomPluralizationMapping
	{
		private readonly StringBidirectionalDictionary _assimilatedClassicalInflectionPluralizationService;
		private readonly StringBidirectionalDictionary _classicalInflectionPluralizationService;
		private readonly StringBidirectionalDictionary _irregularPluralsPluralizationService;

		private readonly List<string> _knownPluralWords;

		private readonly List<string> _knownSingluarWords;
		private readonly StringBidirectionalDictionary _oSuffixPluralizationService;
		private readonly BidirectionalDictionary<string, string> _userDictionary;
		private readonly StringBidirectionalDictionary _wordsEndingWithInxAnxYnxPluralizationService;
		private readonly StringBidirectionalDictionary _wordsEndingWithSePluralizationService;
		private readonly StringBidirectionalDictionary _wordsEndingWithSisPluralizationService;
		private readonly StringBidirectionalDictionary _wordsEndingWithSusPluralizationService;


		internal EnglishPluralizationService()
		{
			Culture = new CultureInfo("en");

			_userDictionary = new BidirectionalDictionary<string, string>();

			_irregularPluralsPluralizationService =
				new StringBidirectionalDictionary(_irregularPluralsDictionary);
			_assimilatedClassicalInflectionPluralizationService =
				new StringBidirectionalDictionary(_assimilatedClassicalInflectionDictionary);
			_oSuffixPluralizationService =
				new StringBidirectionalDictionary(_oSuffixDictionary);
			_classicalInflectionPluralizationService =
				new StringBidirectionalDictionary(_classicalInflectionDictionary);
			_wordsEndingWithSePluralizationService =
				new StringBidirectionalDictionary(_wordsEndingWithSeDictionary);
			_wordsEndingWithSisPluralizationService =
				new StringBidirectionalDictionary(_wordsEndingWithSisDictionary);
			_wordsEndingWithSusPluralizationService =
				new StringBidirectionalDictionary(_wordsEndingWithSusDictionary);
			_wordsEndingWithInxAnxYnxPluralizationService =
				new StringBidirectionalDictionary(_wordsEndingWithInxAnxYnxDictionary);

			// verb
			_irregularVerbPluralizationService =
				new StringBidirectionalDictionary(_irregularVerbList);

			_knownSingluarWords = new List<string>(
			                                       _irregularPluralsDictionary.Keys
			                                                                  .Concat(
			                                                                          _assimilatedClassicalInflectionDictionary
				                                                                          .Keys)
			                                                                  .Concat(_oSuffixDictionary.Keys)
			                                                                  .Concat(_classicalInflectionDictionary.Keys)
			                                                                  .Concat(_irregularVerbList.Keys)
			                                                                  .Concat(_irregularPluralsDictionary.Keys)
			                                                                  .Concat(_wordsEndingWithSeDictionary.Keys)
			                                                                  .Concat(_wordsEndingWithSisDictionary.Keys)
			                                                                  .Concat(_wordsEndingWithSusDictionary.Keys)
			                                                                  .Concat(_wordsEndingWithInxAnxYnxDictionary.Keys)
			                                                                  .Concat(_uninflectiveWordList)
			                                                                  .Except(_knownConflictingPluralList));

				// see the _knowConflictingPluralList comment above

			_knownPluralWords = new List<string>(
			                                     _irregularPluralsDictionary.Values
			                                                                .Concat(
			                                                                        _assimilatedClassicalInflectionDictionary
				                                                                        .Values)
			                                                                .Concat(_oSuffixDictionary.Values)
			                                                                .Concat(_classicalInflectionDictionary.Values)
			                                                                .Concat(_irregularVerbList.Values)
			                                                                .Concat(_irregularPluralsDictionary.Values)
			                                                                .Concat(_wordsEndingWithSeDictionary.Values)
			                                                                .Concat(_wordsEndingWithSisDictionary.Values)
			                                                                .Concat(_wordsEndingWithSusDictionary.Values)
			                                                                .Concat(_wordsEndingWithInxAnxYnxDictionary.Values)
			                                                                .Concat(_uninflectiveWordList));
		}

		/// <summary>
		/// This method allow you to add word to internal PluralizationService of English.
		/// If the singluar or the plural value was already added by this method, then an ArgumentException will be thrown.
		/// </summary>
		/// <param name="singular"></param>
		/// <param name="plural"></param>
		public void AddWord(string singular,
		                    string plural)
		{
			EDesignUtil.CheckArgumentNull(singular, "singular");
			EDesignUtil.CheckArgumentNull(plural, "plural");

			if (_userDictionary.ExistsInSecond(plural))
			{
				throw new ArgumentException($"Duplicate entry in user dictionary: plural '{plural}'");
			}
			if (_userDictionary.ExistsInFirst(singular))
			{
				throw new ArgumentException($"Duplicate entry in user dictionary: singular '{singular}'");
			}

			_userDictionary.AddValue(singular, plural);
		}

		public override bool IsPlural(string word)
		{
			EDesignUtil.CheckArgumentNull(word, "word");

			if (_userDictionary.ExistsInSecond(word))
			{
				return true;
			}
			if (_userDictionary.ExistsInFirst(word))
			{
				return false;
			}

			if (IsUninflective(word) ||
			    _knownPluralWords.Contains(word.ToLower(Culture)))
			{
				return true;
			}
			if (Singularize(word).Equals(word))
			{
				return false;
			}

			return true;
		}

		public override bool IsSingular(string word)
		{
			EDesignUtil.CheckArgumentNull(word, "word");

			if (_userDictionary.ExistsInFirst(word))
			{
				return true;
			}
			if (_userDictionary.ExistsInSecond(word))
			{
				return false;
			}

			if (IsUninflective(word) ||
			    _knownSingluarWords.Contains(word.ToLower(Culture)))
			{
				return true;
			}
			if (!IsNoOpWord(word) &&
			    Singularize(word).Equals(word))
			{
				return true;
			}

			return false;
		}

		// 
		public override string Pluralize(string word)
		{
			EDesignUtil.CheckArgumentNull(word, "word");

			return Capitalize(word, InternalPluralize);
		}

		private string InternalPluralize(string word)
		{
			// words that we know of
			if (_userDictionary.ExistsInFirst(word))
			{
				return _userDictionary.GetSecondValue(word);
			}

			if (IsNoOpWord(word))
			{
				return word;
			}

			string prefixWord;
			var suffixWord = GetSuffixWord(word, out prefixWord);

			// by me -> by me
			if (IsNoOpWord(suffixWord))
			{
				return prefixWord + suffixWord;
			}

			// handle the word that do not inflect in the plural form
			if (IsUninflective(suffixWord))
			{
				return prefixWord + suffixWord;
			}

			// if word is one of the known plural forms, then just return
			if (_knownPluralWords.Contains(suffixWord.ToLowerInvariant()) ||
			    IsPlural(suffixWord))
			{
				return prefixWord + suffixWord;
			}

			// handle irregular plurals, e.g. "ox" -> "oxen"
			if (_irregularPluralsPluralizationService.ExistsInFirst(suffixWord))
			{
				return prefixWord + _irregularPluralsPluralizationService.GetSecondValue(suffixWord);
			}

			string newSuffixWord;
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"man"},
			                                                      (s) => s.Remove(s.Length - 2, 2) + "en", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// handle irregular inflections for common suffixes, e.g. "mouse" -> "mice"
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"louse", "mouse"},
			                                                      (s) => s.Remove(s.Length - 4, 4) + "ice", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"tooth"},
			                                                      (s) => s.Remove(s.Length - 4, 4) + "eeth", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"goose"},
			                                                      (s) => s.Remove(s.Length - 4, 4) + "eese", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"foot"},
			                                                      (s) => s.Remove(s.Length - 3, 3) + "eet", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"zoon"},
			                                                      (s) => s.Remove(s.Length - 3, 3) + "oa", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"cis", "sis", "xis"},
			                                                      (s) => s.Remove(s.Length - 2, 2) + "es", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// handle assimilated classical inflections, e.g. vertebra -> vertebrae
			if (_assimilatedClassicalInflectionPluralizationService.ExistsInFirst(suffixWord))
			{
				return prefixWord + _assimilatedClassicalInflectionPluralizationService.GetSecondValue(suffixWord);
			}

			// Handle the classical variants of modern inflections
			// 
			if (_classicalInflectionPluralizationService.ExistsInFirst(suffixWord))
			{
				return prefixWord + _classicalInflectionPluralizationService.GetSecondValue(suffixWord);
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"trix"},
			                                                      (s) => s.Remove(s.Length - 1, 1) + "ces", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"eau", "ieu"},
			                                                      (s) => s + "x", Culture, out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (_wordsEndingWithInxAnxYnxPluralizationService.ExistsInFirst(suffixWord))
			{
				return prefixWord + _wordsEndingWithInxAnxYnxPluralizationService.GetSecondValue(suffixWord);
			}

			// [cs]h and ss that take es as plural form
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, new List<string>() {"ch", "sh", "ss"},
			                                                      (s) => s + "es", Culture, out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// f, fe that take ves as plural form
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"alf", "elf", "olf", "eaf", "arf"},
			                                                      (s) => s.EndsWith("deaf", true, Culture)
				                                                      ? s
				                                                      : s.Remove(s.Length - 1, 1) + "ves", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"nife", "life", "wife"},
			                                                      (s) => s.Remove(s.Length - 2, 2) + "ves", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// y takes ys as plural form if preceded by a vowel, but ies if preceded by a consonant, e.g. stays, skies
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"ay", "ey", "iy", "oy", "uy"},
			                                                      (s) => s + "s", Culture, out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// 

			if (suffixWord.EndsWith("y", true, Culture))
			{
				return prefixWord + suffixWord.Remove(suffixWord.Length - 1, 1) + "ies";
			}

			// handle some of the words o -> os, and [vowel]o -> os, and the rest are o->oes
			if (_oSuffixPluralizationService.ExistsInFirst(suffixWord))
			{
				return prefixWord + _oSuffixPluralizationService.GetSecondValue(suffixWord);
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"ao", "eo", "io", "oo", "uo"},
			                                                      (s) => s + "s", Culture, out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (suffixWord.EndsWith("o", true, Culture) ||
			    suffixWord.EndsWith("s", true, Culture))
			{
				return prefixWord + suffixWord + "es";
			}

			if (suffixWord.EndsWith("x", true, Culture))
			{
				return prefixWord + suffixWord + "es";
			}

			// cats, bags, hats, speakers
			return prefixWord + suffixWord + "s";
		}

		public override string Singularize(string word)
		{
			EDesignUtil.CheckArgumentNull(word, "word");

			return Capitalize(word, InternalSingularize);
		}

		private string InternalSingularize(string word)
		{
			// words that we know of
			if (_userDictionary.ExistsInSecond(word))
			{
				return _userDictionary.GetFirstValue(word);
			}

			if (IsNoOpWord(word))
			{
				return word;
			}

			string prefixWord;
			var suffixWord = GetSuffixWord(word, out prefixWord);

			if (IsNoOpWord(suffixWord))
			{
				return prefixWord + suffixWord;
			}

			// handle the word that is the same as the plural form
			if (IsUninflective(suffixWord))
			{
				return prefixWord + suffixWord;
			}

			// if word is one of the known singular words, then just return

			if (_knownSingluarWords.Contains(suffixWord.ToLowerInvariant()))
			{
				return prefixWord + suffixWord;
			}

			// handle simple irregular verbs, e.g. was -> were
			if (_irregularVerbPluralizationService.ExistsInSecond(suffixWord))
			{
				return prefixWord + _irregularVerbPluralizationService.GetFirstValue(suffixWord);
			}

			// handle irregular plurals, e.g. "ox" -> "oxen"
			if (_irregularPluralsPluralizationService.ExistsInSecond(suffixWord))
			{
				return prefixWord + _irregularPluralsPluralizationService.GetFirstValue(suffixWord);
			}

			// handle singluarization for words ending with sis and pluralized to ses, 
			// e.g. "ses" -> "sis"
			if (_wordsEndingWithSisPluralizationService.ExistsInSecond(suffixWord))
			{
				return prefixWord + _wordsEndingWithSisPluralizationService.GetFirstValue(suffixWord);
			}

			// handle words ending with se, e.g. "ses" -> "se"
			if (_wordsEndingWithSePluralizationService.ExistsInSecond(suffixWord))
			{
				return prefixWord + _wordsEndingWithSePluralizationService.GetFirstValue(suffixWord);
			}

			// handle words ending with sus, e.g. "suses" -> "sus"
			if (_wordsEndingWithSusPluralizationService.ExistsInSecond(suffixWord))
			{
				return prefixWord + _wordsEndingWithSusPluralizationService.GetFirstValue(suffixWord);
			}

			string newSuffixWord;
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"men"},
			                                                      (s) => s.Remove(s.Length - 2, 2) + "an", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// handle irregular inflections for common suffixes, e.g. "mouse" -> "mice"
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"lice", "mice"},
			                                                      (s) => s.Remove(s.Length - 3, 3) + "ouse", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"teeth"},
			                                                      (s) => s.Remove(s.Length - 4, 4) + "ooth", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"geese"},
			                                                      (s) => s.Remove(s.Length - 4, 4) + "oose", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"feet"},
			                                                      (s) => s.Remove(s.Length - 3, 3) + "oot", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"zoa"},
			                                                      (s) => s.Remove(s.Length - 2, 2) + "oon", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// [cs]h and ss that take es as plural form, this is being moved up since the sses will be override by the ses
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"ches", "shes", "sses"},
			                                                      (s) => s.Remove(s.Length - 2, 2), Culture, out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// handle assimilated classical inflections, e.g. vertebra -> vertebrae
			if (_assimilatedClassicalInflectionPluralizationService.ExistsInSecond(suffixWord))
			{
				return prefixWord + _assimilatedClassicalInflectionPluralizationService.GetFirstValue(suffixWord);
			}

			// Handle the classical variants of modern inflections
			// 
			if (_classicalInflectionPluralizationService.ExistsInSecond(suffixWord))
			{
				return prefixWord + _classicalInflectionPluralizationService.GetFirstValue(suffixWord);
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"trices"},
			                                                      (s) => s.Remove(s.Length - 3, 3) + "x", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"eaux", "ieux"},
			                                                      (s) => s.Remove(s.Length - 1, 1), Culture, out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (_wordsEndingWithInxAnxYnxPluralizationService.ExistsInSecond(suffixWord))
			{
				return prefixWord + _wordsEndingWithInxAnxYnxPluralizationService.GetFirstValue(suffixWord);
			}

			// f, fe that take ves as plural form
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>()
			                                                      {
				                                                      "alves",
				                                                      "elves",
				                                                      "olves",
				                                                      "eaves",
				                                                      "arves"
			                                                      },
			                                                      (s) => s.Remove(s.Length - 3, 3) + "f", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"nives", "lives", "wives"},
			                                                      (s) => s.Remove(s.Length - 3, 3) + "fe", Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// y takes ys as plural form if preceded by a vowel, but ies if preceded by a consonant, e.g. stays, skies
			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"ays", "eys", "iys", "oys", "uys"},
			                                                      (s) => s.Remove(s.Length - 1, 1), Culture, out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// 

			if (suffixWord.EndsWith("ies", true, Culture))
			{
				return prefixWord + suffixWord.Remove(suffixWord.Length - 3, 3) + "y";
			}

			// handle some of the words o -> os, and [vowel]o -> os, and the rest are o->oes
			if (_oSuffixPluralizationService.ExistsInSecond(suffixWord))
			{
				return prefixWord + _oSuffixPluralizationService.GetFirstValue(suffixWord);
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"aos", "eos", "ios", "oos", "uos"},
			                                                      (s) => suffixWord.Remove(suffixWord.Length - 1, 1), Culture,
			                                                      out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			// 

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"ces"},
			                                                      (s) => s.Remove(s.Length - 1, 1), Culture, out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord,
			                                                      new List<string>() {"ces", "ses", "xes"},
			                                                      (s) => s.Remove(s.Length - 2, 2), Culture, out newSuffixWord))
			{
				return prefixWord + newSuffixWord;
			}

			if (suffixWord.EndsWith("oes", true, Culture))
			{
				return prefixWord + suffixWord.Remove(suffixWord.Length - 2, 2);
			}

			if (suffixWord.EndsWith("ss", true, Culture))
			{
				return prefixWord + suffixWord;
			}

			if (suffixWord.EndsWith("s", true, Culture))
			{
				return prefixWord + suffixWord.Remove(suffixWord.Length - 1, 1);
			}

			// word is a singlar
			return prefixWord + suffixWord;
		}

		/// <summary>
		/// captalize the return word if the parameter is capitalized
		/// if word is "Table", then return "Tables"
		/// </summary>
		/// <param name="word"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		private string Capitalize(string word,
		                          Func<string, string> action)
		{
			var result = action(word);

			if (IsCapitalized(word))
			{
				if (result.Length == 0)
				{
					return result;
				}

				var sb = new StringBuilder(result.Length);

				sb.Append(char.ToUpperInvariant(result[0]));
				sb.Append(result.Substring(1));
				return sb.ToString();
			}

			return result;
		}

		/// <summary>
		/// separate one combine word in to two parts, prefix word and the last word(suffix word)
		/// </summary>
		/// <param name="word"></param>
		/// <param name="prefixWord"></param>
		/// <returns></returns>
		private string GetSuffixWord(string word,
		                             out string prefixWord)
		{
			// use the last space to separate the words
			var lastSpaceIndex = word.LastIndexOf(' ');
			prefixWord = word.Substring(0, lastSpaceIndex + 1);
			return word.Substring(lastSpaceIndex + 1);

			// 
		}

		private bool IsCapitalized(string word)
		{
			return string.IsNullOrEmpty(word)
				? false
				: char.IsUpper(word, 0);
		}

		private bool IsAlphabets(string word)
		{
			// return false when the word is "[\s]*" or leading or tailing with spaces
			// or contains non alphabetical characters
			if (string.IsNullOrEmpty(word.Trim()) ||
			    !word.Equals(word.Trim()) ||
			    Regex.IsMatch(word, "[^a-zA-Z\\s]"))
			{
				return false;
			}

			return true;
		}

		private bool IsUninflective(string word)
		{
			EDesignUtil.CheckArgumentNull(word, "word");
			if (PluralizationServiceUtil.DoesWordContainSuffix(word, _uninflectiveSuffixList, Culture)
			    ||
			    (!word.ToLower(Culture).Equals(word) && word.EndsWith("ese", false, Culture))
			    ||
			    _uninflectiveWordList.Contains(word.ToLowerInvariant()))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// return true when the word is "[\s]*" or leading or tailing with spaces
		/// or contains non alphabetical characters
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		private bool IsNoOpWord(string word)
		{
			if (!IsAlphabets(word) ||
			    word.Length <= 1 ||
			    _pronounList.Contains(word.ToLowerInvariant()))
			{
				return true;
			}

			return false;
		}
	}
}