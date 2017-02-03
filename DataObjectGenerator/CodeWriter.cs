using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using Std.Utility;

namespace Std.CodeTools.CodeGeneratorSupport
{
	public class CodeWriter : ICodeWriter
	{
		private readonly StringBuilder _codeBuilder = new StringBuilder();
		private readonly List<int> _indentLengthsField = new List<int>();

		private string _currentIndentField = "";
		private bool _endsWithNewline;

		private readonly HashSet<string> _actionMethods = new HashSet<string>(); 

		public bool GeneratePartialClasses { get; set; }

		public void AddAction(string methodName)
		{
			if (!_actionMethods.Contains(methodName))
			{
				_actionMethods.Add(methodName);
			}
		}

		public List<string> ActionMethods
		{
			get { return _actionMethods.ToList(); }
		}

		public void Reset()
		{
			_codeBuilder.Clear();
			_endsWithNewline = false;
			_indentLengthsField.Clear();
			_currentIndentField = "";
			_actionMethods.Clear();
		}

		public string CodeOutput
		{
			get
			{
				return _codeBuilder.ToString();
			}
		}

		private List<int> IndentLengths
		{
			get
			{
				return _indentLengthsField;
			}
		}

		public string CurrentIndent
		{
			get { return _currentIndentField; }
		}

		public void Write(char ch)
		{
			Write(ch.ToString(CultureInfo.InvariantCulture));
		}

		public void Write(string textToAppend)
		{
			if (string.IsNullOrEmpty(textToAppend))
			{
				return;
			}
			// If we're starting off, or if the previous text ended with a newline,
			// we have to append the current indent first.
			if (((CodeOutput.Length == 0)
				|| _endsWithNewline))
			{
				_codeBuilder.Append(_currentIndentField);
				_endsWithNewline = false;
			}
			// Check if the current text ends with a newline
			if (textToAppend.EndsWith(Environment.NewLine, StringComparison.CurrentCulture))
			{
				_endsWithNewline = true;
			}
			// This is an optimization. If the current indent is "", then we don't have to do any
			// of the more complex stuff further down.
			if ((_currentIndentField.Length == 0))
			{
				_codeBuilder.Append(textToAppend);
				return;
			}
			// Everywhere there is a newline in the text, add an indent after it
			textToAppend = textToAppend.Replace(Environment.NewLine, (Environment.NewLine + _currentIndentField));
			// If the text ends with a newline, then we should strip off the indent added at the very end
			// because the appropriate indent will be added when the next time Write() is called
			if (_endsWithNewline)
			{
				_codeBuilder.Append(textToAppend, 0, (textToAppend.Length - _currentIndentField.Length));
			}
			else
			{
				_codeBuilder.Append(textToAppend);
			}
		}

		public void OpenScope(string scopeTag = "{")
		{
			WriteLine(scopeTag);
			PushIndent();
		}

		public void CloseScope(string scopeTag = "}")
		{
			PopIndent();
			WriteLine(scopeTag);
		}

		public void WriteLine(string textToAppend = null)
		{
			if (!textToAppend.IsNullOrEmpty())
			{
				Write(textToAppend);
			}
			_codeBuilder.AppendLine();
			_endsWithNewline = true;
		}

		[StringFormatMethod("format")]
		public void Write(string format, params object[] args)
		{
			Write(string.Format(CultureInfo.CurrentCulture, format, args));
		}

		[StringFormatMethod("format")]
		public void WriteLine(string format, params object[] args)
		{
			WriteLine(string.Format(CultureInfo.CurrentCulture, format, args));
		}

		public void PushIndent(string indent = "\t")
		{
			if ((indent == null))
			{
				throw new ArgumentNullException("indent");
			}
			_currentIndentField = (_currentIndentField + indent);
			IndentLengths.Add(indent.Length);
		}

		public void WriteBlock(params string[] lines)
		{
			foreach (var line in lines)
			{
				if (line == "")
				{
					WriteLine();
					continue;
				}

				foreach (var ch in line)
				{
					if (ch == '\a')
					{
						PushIndent();
						continue;
					}

					if (ch == '\b')
					{
						PopIndent();
						continue;
					}

					if (ch == '\n')
					{
						WriteLine();
						continue;
					}

					if (ch == '{')
					{
						if (!_endsWithNewline)
						{
							WriteLine();
						}
						OpenScope();
						continue;
					}

					if (ch == '}')
					{
						if (!_endsWithNewline)
						{
							WriteLine();
						}
						CloseScope();
						continue;
					}

					Write(ch);
				}
			}
		}

		public string PopIndent()
		{
			var returnValue = "";
			if ((IndentLengths.Count > 0))
			{
				var indentLength = IndentLengths[(IndentLengths.Count - 1)];
				IndentLengths.RemoveAt((IndentLengths.Count - 1));
				if ((indentLength > 0))
				{
					returnValue = _currentIndentField.Substring((_currentIndentField.Length - indentLength));
					_currentIndentField = _currentIndentField.Remove((_currentIndentField.Length - indentLength));
				}
			}
			return returnValue;
		}

		public void ClearIndent()
		{
			IndentLengths.Clear();
			_currentIndentField = "";
		}


		public virtual void WriteComment(string s)
		{
			WriteLine("//{0}", s);
		}

		const int MaxSummaryLineLength = 40;

		public virtual void WriteSummary(string s)
		{
			WriteSummary(s, MaxSummaryLineLength);
		}

		public virtual void WriteSummary(string s, int lineLength)
		{
			if (string.IsNullOrWhiteSpace(s))
			{
				return;
			}

			var lines = new List<string>();
			var sourceLines = s.Split('\n');
			foreach (var line in sourceLines)
			{
				if (line.Length <= lineLength)
				{
					lines.Add(line);
					continue;
				}
				var brokeLines = StringExtensions.BreakIntoLines(line, lineLength);
				lines.AddRange(brokeLines);
			}

			var text = "\n" + string.Join("\n", lines) + "\n";

			var xml = new XElement("summary", text);
			
			using (var reader = new StringReader(xml.ToString()))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					WriteLine("/// " + line);
				}
			}
		}

		public int UsingCount { get; private set; }

		public virtual void WriteUsing(string s)
		{
			WriteLine("using {0};", s);
			UsingCount += 1;
		}

		public virtual void WriteBeginNamespace(string s)
		{
			WriteLine("namespace {0}", s);
			WriteLine("{");
			PushIndent("\t");
		}

		public virtual void WriteEndNamespace()
		{
			PopIndent();
			WriteLine("}");
		}

		public virtual void WritePublicEnum(string enumName, params string[] tags)
		{
			WriteBeginEnum(enumName, true);

			var lastTag = tags.Last();
			foreach (var tag in tags)
			{
				WriteLine("{0}{1}",
					tag,
					tag == lastTag ? "" : ",");
			}

			CloseScope();
		}

		public virtual void WriteBeginEnum(string enumName, bool ispublic)
		{
			Write("{0} {1}", ispublic ? "public" : "internal", enumName);
			OpenScope();
		}

		public virtual void WriteEndEnum()
		{
			CloseScope();
		}

		public virtual void WriteBeginClass(string className, bool isstatic = false, bool ispublic = false)
		{
			WriteBeginClass(className, null, isstatic, ispublic);
		}

		public virtual void WriteBeginClass(string className, string baseClassName,
			bool isstatic = false, bool ispublic = false, bool ispartial = false,
			bool issealed = false)
		{
			Write("{3}{2} {0}class {1}",
				(GeneratePartialClasses || ispartial) ? "partial " : "",
				className,
				isstatic ? " static" : (issealed ? " sealed" : ""),
				ispublic ? "public" : "internal");

			if (!string.IsNullOrEmpty(baseClassName))
			{
				Write(" : {0}", baseClassName);
			}
			WriteLine("");
			WriteLine("{");
			PushIndent("\t");
		}

		public virtual void WriteEndClass()
		{
			PopIndent();
			WriteLine("}");
		}

		public virtual void WriteAttribute(string a)
		{
			Write("[{0}]", a);
		}

		public virtual void WriteAttributeLine()
		{
			WriteLine("");
		}

		public virtual void WriteBeginInterface(string interfaceName, bool ispublic = false)
		{
			WriteBeginInterface(interfaceName, null, ispublic);
		}

		public void WriteReadonlyProperty(string type, string name, string value, bool ispublic = false)
		{
			WriteLine("{0} {1} {2}",
				GetAccessability(ispublic),
				type,
				name);
			OpenScope();
			WriteLine("get {{ return {0}; }}", value);
			CloseScope();
		}

		public void WriteAutoProperty(string type, string name, bool ispublic = false)
		{
			WriteLine("{0} {1} {2} {{ get; set; }}",
				GetAccessability(ispublic),
				type,
				name);
		}

		protected string GetAccessability(bool ispublic)
		{
			return ispublic ? "public" : "internal";
		}

		public virtual void WriteBeginInterface(string interfaceName, string baseInterfaceName,
			bool ispublic = false)
		{
			Write("{0} interface {1}",
				GetAccessability(ispublic),
				interfaceName);

			if (!string.IsNullOrEmpty(baseInterfaceName))
			{
				Write(" : {0}", baseInterfaceName);
			}
			WriteLine("");
			WriteLine("{");
			PushIndent("\t");
		}

		public virtual void WriteEndInterface()
		{
			PopIndent();
			WriteLine("}");
		}

		public void Save(string outputPath)
		{
			var path = Path.GetDirectoryName(outputPath);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			File.WriteAllText(outputPath, CodeOutput);
		}
	}
}