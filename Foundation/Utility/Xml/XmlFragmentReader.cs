using System;
using System.IO;

namespace Std.Utility.Xml
{
	/// <summary>
	/// Simple class to support reading multiple files of xml fragments
	/// as though they were a single file (or a single file that is an XML fragment).
	/// Useful with XLinq.
	/// </summary>
	public class XmlFragmentReader : TextReader
	{
		enum State
		{
			Unknown,
			RootBegin,
			Content,
			RootEnd,
			Complete
		}

		private readonly string[] _sourceFiles;
		private TextReader _currentReader;
		private int _sourceIndex;
		private State _state = State.Unknown;
		private readonly string _rootTag;

		public XmlFragmentReader(string sourceFile, string rootTag)
		{
			_sourceFiles = new string[]
		    {
		        sourceFile
		    };
			_rootTag = rootTag;
			_currentReader = new StringReader("<" + rootTag + ">");
			_state = State.RootBegin;
		}

		public XmlFragmentReader(string[] sourceFiles, string rootTag)
		{
			_sourceFiles = sourceFiles;
			_rootTag = rootTag;
			_currentReader = new StringReader("<" + rootTag + ">");
			_state = State.RootBegin;
		}

		private bool NextFile()
		{
			switch (_state)
			{
				case State.RootBegin:
				case State.Content:
					if (_sourceIndex >= _sourceFiles.Length)
					{
						_state = State.RootEnd;
						_currentReader = new StringReader("</" + _rootTag + ">");
					}
					else
					{
						_currentReader = new StreamReader(_sourceFiles[_sourceIndex++]);
						_state = State.Content;
					}
					return true;
				case State.RootEnd:
					_state = State.Complete;
					break;
				case State.Complete:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			_currentReader = null;

			return false;
		}

		public override int Peek()
		{
			if (_currentReader == null)
			{
				return -1;
			}

			var nextChar = _currentReader.Peek();

			if (nextChar == -1)
			{
				if (!NextFile())
				{
					return -1;
				}

				nextChar = _currentReader.Peek();
			}

			return nextChar;
		}

		public override int Read()
		{
			if (_currentReader == null)
			{
				return -1;
			}

			var nextChar = _currentReader.Read();

			if (nextChar == -1)
			{
				if (!NextFile())
				{
					return -1;
				}

				nextChar = _currentReader.Read();
			}

			return nextChar;
		}		
	}
}
