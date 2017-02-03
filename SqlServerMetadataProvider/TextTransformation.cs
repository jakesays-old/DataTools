using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Std.Tools.Data.Metadata
{
	/// <summary>
	/// Base class for generated text transformations
	/// </summary>
	/// <remarks>
	/// Any class specified in an inherits directive must match this class in a duck-typing style.
	/// Note that this class therefore specifies an implicit contract with the transformation object.
	/// The object doesn't have to derive from any specific type or interface, but it must have
	/// a) A void Initialize() method.
	/// b) A string TransformText() method
	/// c) An Errors property that's duck-compatible with CompilerErrorCollection
	/// d) A GeneratonEnvironment property that's duck-compatible with StringBuilder.
	/// e) A void Write(string) method 
	/// Using any further features of T4 such as expression blocks will require the class to have further methods, such as ToStringHelper, but
	/// those will produce regular compiler errors at transform time that the base class author can address.
	/// These few methods together form a subset of the TextTransformation default base class' API.
	/// If you change this pseudo-contract to add more requirements, you should consider this a breaking change.
	/// It's OK, however, to change the contract to have fewer requirements.
	/// </remarks>
	public abstract class TextTransformation : IDisposable
	{
		private static CodeTypeMemberCollection baseClassMembers;

		private bool endsWithNewline;

		private CompilerErrorCollection errorsField;
		private StringBuilder generationEnvironmentField;

		private List<int> indentLengthsField;

		protected TextTransformation()
		{
		}

		/// <summary>
		/// Gets the current indent we use when adding lines to the output
		/// </summary>
		public string CurrentIndent { get; private set; } = "";

		/// <summary>
		/// The error collection for the generation process
		/// </summary>
		public CompilerErrorCollection Errors
		{
			[DebuggerStepThrough]
			get
			{
				if (errorsField == null)
				{
					errorsField = new CompilerErrorCollection();
				}
				return errorsField;
			}
		}

		/// <summary>
		/// The string builder that generation-time code is using to assemble generated output
		/// </summary>
		protected StringBuilder GenerationEnvironment
		{
			[DebuggerStepThrough]
			get
			{
				if (generationEnvironmentField == null)
				{
					generationEnvironmentField = new StringBuilder();
				}
				return generationEnvironmentField;
			}
			[DebuggerStepThrough]
			set { generationEnvironmentField = value; }
		}

		/// <summary>
		/// A list of the lengths of each indent that was added with PushIndent
		/// </summary>
		private List<int> indentLengths
		{
			get
			{
				if (indentLengthsField == null)
				{
					indentLengthsField = new List<int>();
				}
				return indentLengthsField;
			}
		}

		/// <summary>
		/// Current transformation session
		/// </summary>
		public virtual IDictionary<string, object> Session { get; set; }

		/// <summary>
		/// Disposes the state of this object.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Remove any indentation
		/// </summary>
		public void ClearIndent()
		{
			indentLengths.Clear();
			CurrentIndent = "";
		}

		/// <summary>
		/// Dispose implementation.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			generationEnvironmentField = null;
			errorsField = null;
		}

		/// <summary>
		/// Raise an error
		/// </summary>
		/// <param name="message"></param>
		public void Error(string message)
		{
			var compilerError = new CompilerError()
			{
				ErrorText = message
			};
			Errors.Add(compilerError);
		}

		/// <summary>
		/// Finaizlier.
		/// </summary>
		~TextTransformation()
		{
			Dispose(false);
		}

		/// <summary>
		/// Initialize the templating class
		/// </summary>
		/// <remarks>
		/// Derived classes are allowed to return errors from initialization 
		/// </remarks>
		public virtual void Initialize()
		{
		}

		/// <summary>
		/// Remove the last indent that was added with PushIndent
		/// </summary>
		/// <returns>The removed indent string</returns>
		public string PopIndent()
		{
			var str = "";
			if (indentLengths.Count > 0)
			{
				var item = indentLengths[indentLengths.Count - 1];
				indentLengths.RemoveAt(indentLengths.Count - 1);
				if (item > 0)
				{
					str = CurrentIndent.Substring(CurrentIndent.Length - item);
					CurrentIndent = CurrentIndent.Remove(CurrentIndent.Length - item);
				}
			}
			return str;
		}

		/// <summary>
		/// Increase the indent
		/// </summary>
		/// <param name="indent">indent string</param>
		public void PushIndent(string indent)
		{
			if (indent == null)
			{
				throw new ArgumentNullException("indent");
			}

			CurrentIndent = string.Concat(CurrentIndent, indent);
			indentLengths.Add(indent.Length);
		}

		/// <summary>
		/// Generate the output text of the transformation
		/// </summary>
		/// <returns></returns>
		public abstract string TransformText();

		/// <summary>
		/// Raise a warning
		/// </summary>
		/// <param name="message"></param>
		public void Warning(string message)
		{
			var compilerError = new CompilerError()
			{
				ErrorText = message,
				IsWarning = true
			};
			Errors.Add(compilerError);
		}

		/// <summary>
		/// Write text directly into the generated output
		/// </summary>
		/// <param name="textToAppend"></param>
		public void Write(string textToAppend)
		{
			if (string.IsNullOrEmpty(textToAppend))
			{
				return;
			}

			if (GenerationEnvironment.Length == 0 || endsWithNewline)
			{
				GenerationEnvironment.Append(CurrentIndent);
				endsWithNewline = false;
			}
			if (textToAppend.EndsWith(Environment.NewLine, StringComparison.CurrentCulture))
			{
				endsWithNewline = true;
			}
			if (CurrentIndent.Length == 0)
			{
				GenerationEnvironment.Append(textToAppend);
				return;
			}

			textToAppend = textToAppend.Replace(Environment.NewLine, string.Concat(Environment.NewLine, CurrentIndent));
			if (!endsWithNewline)
			{
				GenerationEnvironment.Append(textToAppend);
				return;
			}

			GenerationEnvironment.Append(textToAppend, 0, textToAppend.Length - CurrentIndent.Length);
		}

		/// <summary>
		/// Write formatted text directly into the generated output
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Write(string format,
		                  params object[] args)
		{
			Write(string.Format(CultureInfo.CurrentCulture, format, args));
		}

		/// <summary>
		/// Write text directly into the generated output
		/// </summary>
		/// <param name="textToAppend"></param>
		public void WriteLine(string textToAppend)
		{
			Write(textToAppend);
			GenerationEnvironment.AppendLine();
			endsWithNewline = true;
		}

		/// <summary>
		/// Write formatted text directly into the generated output
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void WriteLine(string format,
		                      params object[] args)
		{
			WriteLine(string.Format(CultureInfo.CurrentCulture, format, args));
		}
	}
}