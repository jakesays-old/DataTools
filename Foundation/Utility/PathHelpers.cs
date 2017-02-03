using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Std.Utility.Process;

namespace Std.Utility
{
	public static class PathHelpers
	{

		private static string _executingDirectory;

		[NotNull]
		public static string ExecutingDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(_executingDirectory))
				{
					if (ProcessHelpers.IsTestHostProcess)
					{
						_executingDirectory = Directory.GetCurrentDirectory();
					}
					else
					{
						//first try the executable assembly location. it appears this isn't always available.
						//seems to be the case when running under the vs test host process.

						var assy = Assembly.GetEntryAssembly();
						if (assy != null && !string.IsNullOrEmpty(assy.Location))
						{
							_executingDirectory = Path.GetDirectoryName(assy.Location);
						}
						else
						{
							//fall back to the process location
							_executingDirectory = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
						}
					}
				}

				return _executingDirectory;
			}
		}

		private static readonly char _dirSeparatorChar = Path.DirectorySeparatorChar;
		private static readonly string _dirSeparatorString = Path.DirectorySeparatorChar.ToString();
		private const string CurrentDirSingledot = ".";
		private const string ParentDirDoubledot = "..";

		//
		//  Valid path and coherent path mode verification
		//

		// What we call InnerSpecialDir is when at least one '.' or '..' directory is after a valid directory
		// For example tehse paths all contains inner special dir
		// C:\..  
		// .\..\Dir2\.\Dir3
		// .\..\..\Dir2\..\Dir3
		public static bool ContainsInnerSpecialDir(string path)
		{
			// These cases should have been handled by the calling method and cannot be handled
			Debug.Assert(path != null);
			Debug.Assert(path.Length != 0);
			Debug.Assert(path == NormalizePathInternal(path));

			// Analyze if a /./ or a /../ donc come after a valid DirectoryName
			var pathDirs = path.Split(_dirSeparatorChar);
			var bNextDoubleDotParentDirIsInnerSpecial = false;
			var bNextSingleDotCurrentDirIsInnerSpecial = false;
			foreach (var pathDir in pathDirs)
			{
				if (pathDir == CurrentDirSingledot)
				{
					if (bNextSingleDotCurrentDirIsInnerSpecial)
					{
						return true;
					}
				}
				else if (pathDir == ParentDirDoubledot)
				{
					if (bNextDoubleDotParentDirIsInnerSpecial)
					{
						return true;
					}
				}
				else
				{
					bNextDoubleDotParentDirIsInnerSpecial = true;
				}
				bNextSingleDotCurrentDirIsInnerSpecial = true;
			}

			return false;
		}

		public static string AddSeparator(string path)
		{
			if (!path.EndsWith(@"\"))
			{
				path += @"\";
			}

			return path;
		}

		public static string NormalizePath(string denormalizedPath, bool includeSeparator)
		{
			denormalizedPath = NormalizePathInternal(denormalizedPath);

			if (!ContainsInnerSpecialDir(denormalizedPath))
			{
				return includeSeparator ? AddSeparator(denormalizedPath) : denormalizedPath;
			}

			string normalizedPath;
			string unused;

			if (!TryResolveInnerSpecialDir(denormalizedPath, out normalizedPath, out unused))
			{
				return null;
			}

			return includeSeparator ? AddSeparator(normalizedPath) : normalizedPath;
		}

		public static bool HasWildcard(string path)
		{
			return path.IndexOf('*') != -1 || path.IndexOf('?') != -1;
		}

		public static bool TryResolveInnerSpecialDir(string path, out string pathResolved, out string failureReason)
		{
			// These cases should have been handled by the calling method and cannot be handled
			Debug.Assert(path != null);
			Debug.Assert(path.Length != 0);
			Debug.Assert(path == NormalizePathInternal(path));

			failureReason = string.Empty; // <- failureReason empty by default 
			pathResolved = string.Empty; // <- pathResolved is empty by default

			// TryResolveInnerSpecialDir() is never called without calling first ContainsInnerSpecialDir()
			Debug.Assert(ContainsInnerSpecialDir(path));

			var bPathIsAbolute = !CanOnlyBeARelativePath(path);


			var pathDirs = path.Split(_dirSeparatorChar);
			Debug.Assert(pathDirs.Length > 0);
			var dirStack = new Stack<string>();
			var bNextDoubleDotParentDirIsInnerSpecial = false;
			var bNextSingleDotCurrentDirIsInnerSpecial = false;
			foreach (var dir in pathDirs)
			{
				if (dir == CurrentDirSingledot)
				{
					if (bNextSingleDotCurrentDirIsInnerSpecial)
					{
						// Just ignore InnerSpecial SingleDot
						continue;
					}
					else
					{
						dirStack.Push(dir);
					}
				}
				else if (dir == ParentDirDoubledot)
				{
					if (bNextDoubleDotParentDirIsInnerSpecial)
					{
						// This condition can't be reached because of next conditions
						/*if (dirStack.Count == 0) {
				 failureReason = @"The path {" + path + @"} references a non-existing parent dir \..\, it cannot be resolved";
				 return false;
			  }*/
						if (bPathIsAbolute && dirStack.Count == 1)
						{
							failureReason = @"The path {" + path + @"} references the parent dir \..\ of the root dir {" + pathDirs[0] +
								"}, it cannot be resolved";
							return false;
						}
						var dirToRemove = dirStack.Peek();
						if (dirToRemove == CurrentDirSingledot)
						{
							Debug.Assert(dirStack.Count == 1);
							failureReason = @"The path {" + path +
								@"} references the parent dir \..\ of the current root dir .\, it cannot be resolved";
							return false;
						}
						if (dirToRemove == ParentDirDoubledot)
						{
							failureReason = @"The path {" + path +
								@"} references the parent dir \..\ of a parent dir \..\, it cannot be resolved";
							return false;
						}
						dirStack.Pop();
					}
					else
					{
						dirStack.Push(dir);
					}
				}
				else
				{
					dirStack.Push(dir);
					bNextDoubleDotParentDirIsInnerSpecial = true;
				}
				bNextSingleDotCurrentDirIsInnerSpecial = true;
			}

			// Concatenate the dirs
			var stringBuilder = new StringBuilder(path.Length);

			// Notice that the dirs are reverse ordered, that's why we use Insert(0,
			foreach (var dir in dirStack)
			{
				stringBuilder.Insert(0, _dirSeparatorString);
				stringBuilder.Insert(0, dir);
			}
			// Remove the last DIR_SEPARATOR
			stringBuilder.Length = stringBuilder.Length - 1;
			pathResolved = stringBuilder.ToString();
			Debug.Assert(pathResolved == NormalizePathInternal(pathResolved));
			return true;
		}


		public static bool CanOnlyBeARelativePath(string path)
		{
			Debug.Assert(path != null);
			Debug.Assert(path.Length >= 1);
			return path[0] == '.';
		}


		//
		//  Path normalization
		//
		private static string NormalizePathInternal(string path)
		{
			Debug.Assert(path != null && path.Length > 0);
			path = path.Replace('/', _dirSeparatorChar);

			// EventuallyRemoveConsecutiveDirSeparator
			var consecutiveDirSeparator = _dirSeparatorString + _dirSeparatorString;
			while (path.IndexOf(consecutiveDirSeparator) != -1)
			{
				path = path.Replace(consecutiveDirSeparator, _dirSeparatorString);
			}


			// EventuallyRemoveEndingDirSeparator
			while (true)
			{
				Debug.Assert(path.Length > 0);
				var lastChar = path[path.Length - 1];
				if (lastChar != _dirSeparatorChar)
				{
					break;
				}
				path = path.Substring(0, path.Length - 1);
			}

			return path;
		}

		public static bool HasParentDir(string path)
		{
			return path.Contains(_dirSeparatorString);
		}

		public static string GetParentDirectory(string path)
		{
			if (!HasParentDir(path))
			{
				throw new InvalidOperationException(@"Can't get the parent dir from path """ + path + @"""");
			}
			var index = path.LastIndexOf(_dirSeparatorChar);
			return path.Substring(0, index);
		}

		public static string GetLastName(string path)
		{
			if (!HasParentDir(path))
			{
				return string.Empty;
			}
			var index = path.LastIndexOf(_dirSeparatorChar);
			Debug.Assert(index != path.Length - 1);
			return path.Substring(index + 1, path.Length - index - 1);
		}

		public static string GetExtension(string path)
		{
			var fileName = GetLastName(path);
			var index = fileName.LastIndexOf('.');
			if (index == -1)
			{
				return string.Empty;
			}
			if (index == fileName.Length - 1)
			{
				return string.Empty;
			}
			return fileName.Substring(index, fileName.Length - index);
		}


		//
		//  GetPathRelativeTo
		//
		public static string GetPathRelativeTo(string pathFrom, string pathTo)
		{
			// Don't return .\ but just . to remain compliant
			if (string.Compare(pathFrom, pathTo, true) == 0)
			{
				return CurrentDirSingledot;
			}

			var relativeDirs = new List<string>();
			var pathFromDirs = pathFrom.Split(_dirSeparatorChar);
			var pathToDirs = pathTo.Split(_dirSeparatorChar);
			var length = Math.Min(pathFromDirs.Length, pathToDirs.Length);
			var lastCommonRoot = -1;

			// find common root
			for (var i = 0; i < length; i++)
			{
				if (string.Compare(pathFromDirs[i], pathToDirs[i], true) != 0)
				{
					break;
				}
				lastCommonRoot = i;
			}

			// The lastCommon root problem is handled by the calling method and cannot be tested
			Debug.Assert(lastCommonRoot != -1);

			// add relative folders in from path
			for (var i = lastCommonRoot + 1; i < pathFromDirs.Length; i++)
			{
				if (pathFromDirs[i].Length > 0)
				{
					relativeDirs.Add("..");
				}
			}
			if (relativeDirs.Count == 0)
			{
				relativeDirs.Add(CurrentDirSingledot);
			}
			// add to folders to path
			for (var i = lastCommonRoot + 1; i < pathToDirs.Length; i++)
			{
				relativeDirs.Add(pathToDirs[i]);
			}

			// create relative path
			var relativeParts = new string[relativeDirs.Count];
			relativeDirs.CopyTo(relativeParts);
			var relativePath = string.Join(_dirSeparatorString, relativeParts);
			return relativePath;
		}


		//
		//  GetAbsolutePath
		//
		public static string GetAbsolutePath(string pathFrom, string pathTo)
		{
			//			Debug.Assert(pathTo[0] == '.');

			var pathFromDirs = pathFrom.Split(_dirSeparatorChar);
			var pathToDirs = pathTo.Split(_dirSeparatorChar);

			// Compute nbParentDirToGoBackInPathFrom
			var nbParentDirToGoBackInPathFrom = 0;
			var nbSpecialDirToGoUpInPathTo = 0;
			for (var i = 0; i < pathToDirs.Length; i++)
			{
				if (pathToDirs[i] == ParentDirDoubledot)
				{
					nbParentDirToGoBackInPathFrom++;
					nbSpecialDirToGoUpInPathTo++;
				}
				else if (pathToDirs[i] == CurrentDirSingledot)
				{
					nbSpecialDirToGoUpInPathTo++;
				}
				else
				{
					break;
				}
			}

			// check nbParentDirToGoBackInPathFrom is valid
			if (nbParentDirToGoBackInPathFrom >= pathFromDirs.Length)
			{
				throw new ArgumentException(
					@"Cannot infer pathTo.GetAbsolutePath(pathFrom) because there are too many parent dirs in pathTo:
PathFrom = """ +
						pathFrom + @"""
PathTo   = """ + pathTo + @"""");
			}

			// Apply nbParentDirToGoBackInPathFrom to extract part from pathFrom
			var dirsExtractedFromPathFrom = new string[(pathFromDirs.Length - nbParentDirToGoBackInPathFrom)];
			for (var i = 0; i < pathFromDirs.Length - nbParentDirToGoBackInPathFrom; i++)
			{
				dirsExtractedFromPathFrom[i] = pathFromDirs[i];
			}
			var partExtractedFromPathFrom = string.Join(_dirSeparatorString, dirsExtractedFromPathFrom);

			// Apply nbParentDirToGoBackInPathFrom to extract part from pathTo
			var dirsExtractedFromPathTo = new string[(pathToDirs.Length - nbSpecialDirToGoUpInPathTo)];
			for (var i = 0; i < pathToDirs.Length - nbSpecialDirToGoUpInPathTo; i++)
			{
				dirsExtractedFromPathTo[i] = pathToDirs[i + nbSpecialDirToGoUpInPathTo];
			}
			var partExtractedFromPathTo = string.Join(_dirSeparatorString, dirsExtractedFromPathTo);

			// Concatenate the 2 parts extracted from pathFrom and pathTo
			return partExtractedFromPathFrom + _dirSeparatorString + partExtractedFromPathTo;
		}

		//
		//  Uniform Ressource Locator 
		//
		public static bool IsUniformRessourceLocatorPath(string path)
		{
			var urnSchemes = new[]
			{
				"ftp",
				"http",
				"gopher",
				"mailto",
				"news",
				"nntp",
				"telnet",
				"wais",
				"file",
				"prospero"
			};

			foreach (var scheme in urnSchemes)
			{
				if (path.Length > scheme.Length &&
					string.Compare(path.Substring(0, scheme.Length),
						scheme, true) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public static bool BeginsWithASingleLetterDrive(string path)
		{
			if (path.Length < 2)
			{
				return false;
			}
			if (!char.IsLetter(path[0]))
			{
				return false;
			}
			if (path[1] != ':')
			{
				return false;
			}
			return true;
		}

		public static string ResolvePath(string basePath, string path, bool includeSeparator)
		{
			if (Path.IsPathRooted(path))
			{
				return includeSeparator ? AddSeparator(path) : path;
			}

			var resolvedPath = NormalizePath(Path.Combine(basePath, path), includeSeparator);

			return resolvedPath;
		}

		public static string ResolvePath(string basePath, string path)
		{
			return ResolvePath(basePath, path, false);
		}

		public static string ResolvePath(string path)
		{
			return ResolvePath(Directory.GetCurrentDirectory(), path, false);
		}

		public static string ResolvePath(string path, bool includeSeparator)
		{
			return ResolvePath(Directory.GetCurrentDirectory(), path, includeSeparator);
		}
	}
}