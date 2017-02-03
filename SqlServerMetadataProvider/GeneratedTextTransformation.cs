using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.SchemaProvider;
using LinqToDB.SqlProvider;
using Std.Tools.Data.Metadata.Model;
using AssociationType = Std.Tools.Data.Metadata.Model.AssociationType;
using Attribute = Std.Tools.Data.Metadata.Model.Attribute;
using Type = System.Type;

namespace Std.Tools.Data.Metadata
{
	public class GeneratedTextTransformation : TextTransformation
	{
		private Action BeforeGenerateLinqToDBModel = () => {
		};

		private Action AfterGenerateLinqToDBModel = () => {
		};

		private Func<Table, MemberBase> GenerateProviderSpecificTable = t => null;

		private bool GenerateObsoleteAttributeForAliases = false;

		private bool GenerateFindExtensions = true;

		private bool IsCompactColumns = true;

		private bool IsCompactColumnAliases = true;

		private bool GenerateDataTypes = false;

		private bool? GenerateLengthProperty = null;

		private bool? GeneratePrecisionProperty = null;

		private bool? GenerateScaleProperty = null;

		private bool GenerateDbTypes = false;

		private bool GenerateSchemaAsType = false;

		private bool GenerateViews = true;

		private string SchemaNameSuffix = "Schema";

		private string SchemaDataContextTypeName = "DataContext";

		private Dictionary<string, string> SchemaNameMapping = new Dictionary<string, string>();

		private string DatabaseName = null;

		private string DataContextName = null;

		private string BaseDataContextClass = null;

		private string BaseEntityClass = null;

		private string OneToManyAssociationType = "IEnumerable<{0}>";

		private bool GenerateDatabaseName = false;

		private bool GenerateConstructors = true;

		private string DefaultConfiguration = null;

		private bool GenerateAssociations = true;

		private bool GenerateBackReferences = true;

		private bool ReplaceSimilarTables = true;

		private bool IncludeDefaultSchema = true;

		private Class DataContextObject;

		private bool PluralizeClassNames = false;

		private bool SingularizeClassNames = true;

		private bool PluralizeDataContextPropertyNames = true;

		private bool SingularizeDataContextPropertyNames = false;

		private GetSchemaOptions GetSchemaOptions = new GetSchemaOptions();

		private ISqlBuilder SqlBuilder;

		private Func<TableSchema, Table> LoadProviderSpecificTable = tableSchema => null;

		private Action AfterLoadMetadata = () => {
		};

		private Dictionary<string, Table> Tables = new Dictionary<string, Table>();

		private Dictionary<string, Procedure> Procedures = new Dictionary<string, Procedure>();

		private static Action<GeneratedTextTransformation, string> WriteComment;

		private Action BeforeGenerateModel = () => {
		};

		private bool GenerateProcedureErrors = true;

		private static Action<GeneratedTextTransformation, string> WriteUsing;

		private ModelSource Model = new ModelSource();

		private static Action<GeneratedTextTransformation, string> WriteBeginNamespace;

		private static Action<GeneratedTextTransformation> WriteEndNamespace;

		private static Action<GeneratedTextTransformation, Class> WriteBeginClass;

		private static Action<GeneratedTextTransformation> WriteEndClass;

		private static Action<GeneratedTextTransformation, string> BeginRegion;

		private static Action<GeneratedTextTransformation> EndRegion;

		private static Action<GeneratedTextTransformation, Field> WriteField;

		private static Action<GeneratedTextTransformation, Event> WriteEvent;

		private static Action<GeneratedTextTransformation, Property, bool> WriteProperty;

		private static Action<GeneratedTextTransformation, Method, bool> WriteMethod;

		private static Action<GeneratedTextTransformation, Attribute> WriteAttribute;

		private Func<string, string> ToPlural = s => string.Concat(s, "s");

		private Func<string, string> ToSingular = s => s;

		private string NamespaceName
		{
			get
			{
				return Model.Namespace.Name;
			}
			set
			{
				Model.Namespace.Name = value;
			}
		}

		static GeneratedTextTransformation()
		{
			WriteComment = (tt, s) => tt.WriteLine("//{0}", new object[] { s });
			WriteUsing = (tt, s) => tt.WriteLine("using {0};", new object[] { s });
			WriteBeginNamespace = (tt, s) => {
				tt.WriteLine("namespace {0}", new object[] { s });
				tt.WriteLine("{");
			};
			WriteEndNamespace = tt => tt.WriteLine("}");
			WriteBeginClass = (tt, cl) => {
				tt.Write(string.Concat(cl.AccessModifier.ToString().ToLower(), " "));
				if (cl.IsStatic)
				{
					tt.Write("static ");
				}
				if (cl.IsPartial)
				{
					tt.Write("partial ", new object[] { cl.Name });
				}
				tt.Write("class {0}", new object[] { cl.Name });
				if ((!string.IsNullOrEmpty(cl.BaseClass) ? true : cl.Interfaces.Count > 0))
				{
					string[] arr = (
						from n in (new string[] { cl.BaseClass }).Concat<string>(cl.Interfaces)
						where n != null
						select n).ToArray<string>();
					tt.Write(" : ");
					tt.Write(string.Join(", ", arr));
				}
				tt.WriteLine("");
				tt.WriteLine("{");
			};
			WriteEndClass = tt => tt.WriteLine("}");
			BeginRegion = (tt, s) => tt.WriteLine("#region {0}", new object[] { s });
			EndRegion = tt => tt.WriteLine("#endregion");
			WriteField = (tt, f) => {
				string am = f.AccessModifier.ToString().ToLower();
				string mdf = string.Concat((f.IsStatic ? " static" : ""), (f.IsReadonly ? " readonly" : ""));
				tt.Write("{0}{1}{2}{3} {4}{5} {6}", new object[] { am, LenDiff(f.AccessModifierLen, am), mdf, LenDiff(f.ModifierLen, mdf), f.Type, LenDiff(f.TypeLen, f.Type), f.Name });
				if (f.InitValue != null)
				{
					tt.Write(" = {0}", new object[] { f.InitValue });
				}
				tt.Write(";");
				if (string.IsNullOrEmpty(f.EndLineComment))
				{
					tt.WriteLine("");
				}
				else
				{
					tt.WriteSpaces(f.NameLen - f.Name.Length + f.BodyLen + f.ParamLen - 1);
					tt.Write(" ");
					WriteComment(tt, string.Concat(" ", f.EndLineComment));
				}
			};
			WriteEvent = (tt, m) => {
				string am = m.AccessModifier.ToString().ToLower();
				string mdf = string.Concat((m.IsStatic ? " static" : ""), (m.IsVirtual ? " virtual" : ""), " event");
				tt.Write("{0}{1}{2}{3} {4}{5} {6};", new object[] { am, LenDiff(m.AccessModifierLen, am), mdf, LenDiff(m.ModifierLen, mdf), m.Type, LenDiff(m.TypeLen, m.Type), m.Name });
				if (string.IsNullOrEmpty(m.EndLineComment))
				{
					tt.WriteLine("");
				}
				else
				{
					tt.WriteSpaces(m.NameLen - m.Name.Length + m.BodyLen + m.ParamLen - 1);
					tt.Write(" ");
					WriteComment(tt, string.Concat(" ", m.EndLineComment));
				}
			};
			WriteProperty = (tt, p, compact) => {
				string am = p.AccessModifier.ToString().ToLower();
				string mdf = (p.IsAbstract ? " abstract" : (p.IsVirtual ? " virtual" : (p.IsOverride ? " override" : (p.IsStatic ? " static" : ""))));
				tt.Write("{0}{1}{2}{3} {4}{5} {6}", new object[] { am, LenDiff(p.AccessModifierLen, am), mdf, LenDiff(p.ModifierLen, mdf), p.Type, LenDiff(p.TypeLen, p.Type), p.Name });
				Action writeComment = () => {
					if (string.IsNullOrEmpty(p.EndLineComment))
					{
						tt.WriteLine("");
					}
					else
					{
						tt.Write(" ");
						WriteComment(tt, string.Concat(" ", p.EndLineComment));
					}
				};
				if (p.IsAuto)
				{
					tt.Write(LenDiff(p.NameLen + p.ParamLen, p.Name));
					int len = tt.GenerationEnvironment.Length;
					tt.Write(" { ");
					if (!p.HasGetter)
					{
						tt.Write("private ");
					}
					else if (p.GetterLen == 13)
					{
						tt.Write("        ");
					}
					tt.Write("get; ");
					if (!p.HasSetter)
					{
						tt.Write("private ");
					}
					else if (p.SetterLen == 13)
					{
						tt.Write("        ");
					}
					tt.Write("set; ");
					tt.Write("}");
					if (!string.IsNullOrEmpty(p.EndLineComment))
					{
						tt.WriteSpaces(p.BodyLen - (tt.GenerationEnvironment.Length - len));
					}
					writeComment();
				}
				else if (!compact)
				{
					writeComment();
					tt.WriteLine("{");
					tt.PushIndent("\t");
					if (p.HasGetter)
					{
						if (p.GetBody.Count != 1)
						{
							tt.WriteLine("get");
							tt.WriteLine("{");
							tt.PushIndent("\t");
							foreach (string t in p.GetBody)
							{
								tt.WriteLine(t);
							}
							tt.PopIndent();
							tt.WriteLine("}");
						}
						else
						{
							tt.WriteLine("get {{ {0} }}", new object[] { p.GetBody[0] });
						}
					}
					if (p.HasSetter)
					{
						if (p.SetBody.Count != 1)
						{
							tt.WriteLine("set");
							tt.WriteLine("{");
							tt.PushIndent("\t");
							foreach (string t in p.SetBody)
							{
								tt.WriteLine(t);
							}
							tt.PopIndent();
							tt.WriteLine("}");
						}
						else
						{
							tt.WriteLine("set {{ {0} }}", new object[] { p.SetBody[0] });
						}
					}
					tt.PopIndent();
					tt.WriteLine("}");
				}
				else
				{
					tt.Write(LenDiff(p.NameLen + p.ParamLen, p.Name));
					int len = tt.GenerationEnvironment.Length;
					tt.Write(" { ");
					if (p.HasGetter)
					{
						tt.Write("get { ");
						foreach (string t in p.GetBody)
						{
							tt.Write("{0} ", new object[] { t });
						}
						tt.Write("} ");
					}
					if (p.HasSetter)
					{
						tt.Write("set { ");
						foreach (string t in p.SetBody)
						{
							tt.Write("{0} ", new object[] { t });
						}
						tt.Write("} ");
					}
					tt.Write("}");
					if (!string.IsNullOrEmpty(p.EndLineComment))
					{
						tt.WriteSpaces(p.BodyLen - (tt.GenerationEnvironment.Length - len));
					}
					writeComment();
				}
			};
			WriteMethod = (tt, m, compact) => {
				string am1 = m.AccessModifier.ToString().ToLower();
				int len1 = m.AccessModifierLen;
				string am2 = "";
				int len2 = 0;
				string mdf = (m.IsAbstract ? " abstract" : (m.IsVirtual ? " virtual" : (m.IsOverride ? " override" : (m.IsStatic ? " static" : ""))));
				int mlen = m.ModifierLen;
				if ((am1 != "partial" ? false : mdf.Length > 0))
				{
					am2 = string.Concat(" ", am1);
					len2 = len1 + 1;
					am1 = "";
					len1 = 0;
					mdf = mdf.Trim();
					mlen--;
				}
				GeneratedTextTransformation generatedTextTransformation = tt;
				object[] type = new object[] { am1, LenDiff(len1, am1), mdf, LenDiff(mlen, mdf), am2, LenDiff(len2, am2), null, null, null, null };
				type[6] = (m.Type == null ? "" : " ");
				type[7] = m.Type;
				type[8] = LenDiff(m.TypeLen, m.Type ?? "");
				type[9] = m.Name;
				generatedTextTransformation.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8} {9}", type);
				Action writeComment = () => {
					if (string.IsNullOrEmpty(m.EndLineComment))
					{
						tt.WriteLine("");
					}
					else
					{
						tt.Write(" ");
						WriteComment(tt, string.Concat(" ", m.EndLineComment));
					}
				};
				Action writeParams = () => {
					tt.Write("(");
					for (int i = 0; i < m.Parameters.Count; i++)
					{
						if (i > 0)
						{
							tt.Write(", ");
						}
						tt.Write(m.Parameters[i]);
					}
					tt.Write(")");
				};
				if (!compact)
				{
					writeParams();
					writeComment();
					tt.PushIndent("\t");
					foreach (string str in m.AfterSignature)
					{
						tt.WriteLine(str);
					}
					tt.PopIndent();
					tt.WriteLine("{");
					tt.PushIndent("\t");
					foreach (string t in m.Body)
					{
						if ((t.Length <= 1 ? false : t[0] == '#'))
						{
							tt.RemoveSpace();
						}
						tt.WriteLine(t);
					}
					tt.PopIndent();
					tt.WriteLine("}");
				}
				else
				{
					tt.Write(LenDiff(m.NameLen, m.Name));
					int len = tt.GenerationEnvironment.Length;
					writeParams();
					foreach (string str1 in m.AfterSignature)
					{
						tt.Write(" ");
						tt.Write(str1);
					}
					len = tt.GenerationEnvironment.Length - len;
					if ((m.IsAbstract ? false : m.AccessModifier != AccessModifier.Partial))
					{
						tt.WriteSpaces(m.ParamLen - len);
						len = tt.GenerationEnvironment.Length;
						tt.Write(" {");
						foreach (string t in m.Body)
						{
							tt.Write(" {0}", new object[] { t });
						}
						tt.Write(" }");
					}
					else
					{
						tt.Write(";");
						len = 0;
					}
					if (!string.IsNullOrEmpty(m.EndLineComment))
					{
						tt.WriteSpaces(m.BodyLen - (tt.GenerationEnvironment.Length - len));
					}
					writeComment();
				}
			};
			WriteAttribute = (tt, a) => {
				tt.Write(a.Name);
				if (a.Parameters.Count > 0)
				{
					tt.Write("(");
					for (int i = 0; i < a.Parameters.Count; i++)
					{
						if (i > 0)
						{
							if (!a.Parameters[i - 1].All<char>(c => c == ' '))
							{
								SkipSpacesAndInsert(tt, ", ");
							}
							else
							{
								tt.Write("  ");
							}
						}
						tt.Write(a.Parameters[i]);
					}
					SkipSpacesAndInsert(tt, ")");
				}
			};
		}

		public GeneratedTextTransformation()
		{
		}

		private string CheckColumnName(string memberName)
		{
			if (!string.IsNullOrEmpty(memberName))
			{
				memberName = memberName.Replace("%", "Percent");
			}
			else
			{
				memberName = "Empty";
			}
			return memberName;
		}

		private string CheckParameterName(string parameterName)
		{
			List<string> invalidParameterNames = new List<string>()
			{
				"@DataType"
			};
			string result = parameterName;
			while (invalidParameterNames.Contains(result))
			{
				result = string.Concat(result, "_");
			}
			return result;
		}

		private string CheckType(Type type, string typeName)
		{
			if (!Model.Usings.Contains(type.Namespace))
			{
				Model.Usings.Add(type.Namespace);
			}
			return typeName;
		}

		private string ConvertToCompilable(string name)
		{
			IEnumerable<char> query = 
				from c in name
				select (char.IsLetterOrDigit(c) || c == '@' ? c : '\u005F');
			return new string(query.ToArray<char>());
		}

		public static INode FindNode(INode parent, Func<INode, bool> func)
		{
			INode node;
			foreach (INode sibling in parent.Children)
			{
				if (!func(sibling))
				{
					INode n = FindNode(sibling, func);
					if (n != null)
					{
						node = n;
						return node;
					}
				}
				else
				{
					node = sibling;
					return node;
				}
			}
			node = null;
			return node;
		}

		private void GenerateModel()
		{
			Model.SetParent();
			BeforeGenerateModel();
			if ((base.GenerationEnvironment.Length <= 0 ? false : base.GenerationEnvironment.ToString().Trim().Length == 0))
			{
				base.GenerationEnvironment.Length = 0;
			}
			WriteComment(this, "---------------------------------------------------------------------------------------------------");
			WriteComment(this, " <auto-generated>");
			WriteComment(this, "    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).");
			WriteComment(this, "    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.");
			WriteComment(this, " </auto-generated>");
			WriteComment(this, "---------------------------------------------------------------------------------------------------");
			Model.Accept(this);
		}

		private void GenerateSqlServerTypes()
		{
			Model.Usings.Add("System.Collections.Generic");
			Model.Usings.Add("System.Linq.Expressions");
			Model.Usings.Add("System.Reflection");
			Model.Usings.Add("LinqToDB");
			Model.Usings.Add("LinqToDB.DataProvider.SqlServer");
			List<IClassMember> members = DataContextObject.Members;
			MemberGroup memberGroup = new MemberGroup()
			{
				Region = "FreeTextTable"
			};
			List<IClassMember> classMembers = memberGroup.Members;
			IClassMember[] classMemberArray = new IClassMember[1];
			MemberGroup memberGroup1 = new MemberGroup()
			{
				IsCompact = true
			};
			memberGroup1.Members.Add(new Field("T", "Key"));
			memberGroup1.Members.Add(new Field("int", "Rank"));
			classMemberArray[0] = memberGroup1;
			classMembers.Add(new Class("FreeTextKey<T>", classMemberArray)
			{
				IsPartial = false
			});
			List<IClassMember> members1 = memberGroup.Members;
			Method method = new Method("ITable<FreeTextKey<TKey>>", "FreeTextTable<TTable,TKey>", new string[] { "string field", "string text" }, new string[] { "return this.GetTable<FreeTextKey<TKey>>(", "\tthis,", "\t((MethodInfo)(MethodBase.GetCurrentMethod())).MakeGenericMethod(typeof(TTable), typeof(TKey)),", "\tfield,", "\ttext);" });
			method.Attributes.Add(new Attribute("FreeTextTableExpression", Array.Empty<string>()));
			members1.Add(method);
			List<IClassMember> classMembers1 = memberGroup.Members;
			Method method1 = new Method("ITable<FreeTextKey<TKey>>", "FreeTextTable<TTable,TKey>", new string[] { "Expression<Func<TTable,string>> fieldSelector", "string text" }, new string[] { "return this.GetTable<FreeTextKey<TKey>>(", "\tthis,", "\t((MethodInfo)(MethodBase.GetCurrentMethod())).MakeGenericMethod(typeof(TTable), typeof(TKey)),", "\tfieldSelector,", "\ttext);" });
			method1.Attributes.Add(new Attribute("FreeTextTableExpression", Array.Empty<string>()));
			classMembers1.Add(method1);
			members.Add(memberGroup);
		}

		void GenerateTypesFromMetadata()
		{
			BeforeGenerateLinqToDBModel();

			Model.Usings.Add("LinqToDB");
			Model.Usings.Add("LinqToDB.Mapping");

			if (NamespaceName == null)
				NamespaceName = "DataModel";

			string schemaName;

			var schemas =
			(
				from t in Tables.Values
				where GenerateSchemaAsType && t.Schema != null && !t.TableSchema.IsDefaultSchema
				group t by t.Schema into gr
				let typeName = SchemaNameMapping.TryGetValue(gr.Key, out schemaName) ? schemaName : gr.Key
				select new
				{
					Name = gr.Key,
					TypeName = typeName + SchemaNameSuffix,
					PropertyName = typeName,
					Props = new MemberGroup { IsCompact = true },
					Aliases = new MemberGroup { IsCompact = true, Region = "Alias members" },
					TableExtensions = new MemberGroup { Region = "Table Extensions" },
					Type = new Class(typeName + SchemaNameSuffix) { IsStatic = true },
					Tables = gr.ToList(),
					DataContext = new Class(SchemaDataContextTypeName),
					Procedures = new MemberGroup(),
					Functions = new MemberGroup(),
					TableFunctions = new MemberGroup { Region = "Table Functions" },
				}
			).ToDictionary(t => t.Name);

			var defProps = new MemberGroup { IsCompact = true };
			var defAliases = new MemberGroup { IsCompact = true, Region = "Alias members" };
			var defTableExtensions = new MemberGroup { };

			if (GenerateConstructors)
			{
				var body = new List<string>();

				if (schemas.Count > 0)
				{
					var schemaGroup = new MemberGroup { IsCompact = true, Region = "Schemas" };

					foreach (var schema in schemas.Values)
					{
						schemaGroup.Members.Add(new Property(schema.TypeName + "." + SchemaDataContextTypeName, schema.PropertyName));
						body.Add(schema.PropertyName + " = new " + schema.TypeName + "." + SchemaDataContextTypeName + "(this);");
					}

					body.Add("");

					DataContextObject.Members.Add(schemaGroup);
				}

				body.Add("InitDataContext();");

				if (DefaultConfiguration == null)
					DataContextObject.Members.Add(new Method(null, DataContextObject.Name, new string[0], body));
				else
					DataContextObject.Members.Add(new Method(null, DataContextObject.Name, new string[0], body) { AfterSignature = { ": base(\"" + DefaultConfiguration + "\")" } });
				DataContextObject.Members.Add(new Method(null, DataContextObject.Name, new[] { "string configuration" }, body) { AfterSignature = { ": base(configuration)" } });

				DataContextObject.Members.Add(new MemberGroup
				{
					IsCompact = true,
					Members = { new Method("void", "InitDataContext") { AccessModifier = AccessModifier.Partial } }
				});
			}

			if (Tables.Count > 0)
				DataContextObject.Members.Insert(0, defProps);

			foreach (var schema in schemas.Values)
			{
				schema.Type.Members.Add(schema.DataContext);
				schema.DataContext.Members.Insert(0, schema.Props);

				schema.DataContext.Members.Add(new Field("IDataContext", "_dataContext") { AccessModifier = AccessModifier.Private, IsReadonly = true });
				schema.DataContext.Members.Add(new Method(null, schema.DataContext.Name, new[] { "IDataContext dataContext" }, new[] { "_dataContext = dataContext;" }));

				foreach (var t in schema.Tables)
				{
					t.TypePrefix = schema.TypeName + ".";
				}
			}

			foreach (var t in Tables.Values.OrderBy(tbl => tbl.IsProviderSpecific).ThenBy(tbl => tbl.TypeName))
			{
				Action<Class> addType = tp => Model.Types.Add(tp);
				var props = defProps;
				var aliases = defAliases;
				var tableExtensions = defTableExtensions;

				if (t.IsView && !GenerateViews)
				{
					continue;
				}

				var schema = t.Schema != null && schemas.ContainsKey(t.Schema) ? schemas[t.Schema] : null;

				if (schema != null)
				{
					var si = schemas[t.Schema];

					addType = tp => si.Type.Members.Add(tp);
					props = si.Props;
					aliases = si.Aliases;
					tableExtensions = si.TableExtensions;
				}

				MemberBase dcProp = t.IsProviderSpecific ?
					GenerateProviderSpecificTable(t) :
					new Property(
						string.Format("ITable<{0}>", t.TypeName),
						t.DataContextPropertyName,
						new[] { string.Format((schema == null ? "this" : "_dataContext") + ".GetTable<{0}>()", t.TypeName) },
						null);

				if (dcProp == null)
					continue;

				t.DataContextProperty = dcProp;

				props.Members.Add(dcProp);

				Property aProp = null;

				if (t.AliasPropertyName != null && t.AliasPropertyName != t.DataContextPropertyName)
				{
					aProp = new Property(
						string.Format("ITable<{0}>", t.TypeName),
						t.AliasPropertyName,
						new[] { t.DataContextPropertyName },
						null);

					if (GenerateObsoleteAttributeForAliases)
						aProp.Attributes.Add(new Attribute("Obsolete", "\"Use " + t.DataContextPropertyName + " instead.\""));

					aliases.Members.Add(aProp);
				}

				var tableAttrs = new List<string>();

				if (DatabaseName != null)
					tableAttrs.Add("Database=" + '"' + DatabaseName + '"');
				if (t.Schema != null)
					tableAttrs.Add("Schema=" + '"' + t.Schema + '"');

				tableAttrs.Add((tableAttrs.Count == 0 ? "" : "Name=") + '"' + t.TableName + '"');

				if (t.IsView)
					tableAttrs.Add("IsView=true");

				t.Attributes.Add(new Attribute("Table", tableAttrs.ToArray()) { IsSeparated = true });

				var comments = new List<string>();

				if (!string.IsNullOrWhiteSpace(t.Description))
				{
					comments.Add("/ <summary>");
					foreach (var line in t.Description.Split('\n'))
						comments.Add("/ " + line.TrimEnd());
					comments.Add("/ </summary>");
				}

				if (comments.Count > 0)
				{
					t.Comment.AddRange(comments);
					dcProp.Comment.AddRange(comments);

					if (aProp != null)
						aProp.Comment.AddRange(comments);
				}

				var columns = new MemberGroup { IsCompact = IsCompactColumns };
				var columnAliases = new MemberGroup { IsCompact = IsCompactColumnAliases, Region = "Alias members" };
				var nPKs = t.Columns.Values.Count(c => c.IsPrimaryKey);
				var allNullable = t.Columns.Values.All(c => c.IsNullable || c.IsIdentity);
				var nameMaxLen = t.Columns.Values.Max(c => (int?) (c.MemberName == c.ColumnName ? 0 : c.ColumnName.Length)) ?? 0;
				var dbTypeMaxLen = t.Columns.Values.Max(c => (int?) (c.ColumnType.Length)) ?? 0;
				var dataTypeMaxLen = t.Columns.Values.Where(c => c.DataType != null).Max(c => (int?) (c.DataType.Length)) ?? 0;
				var dataTypePrefix = t.Columns.Values.Any(c => c.MemberName == "DataType") ? "LinqToDB." : "";

				foreach (var c in t.Columns.Values)
				{
					// Column.
					//
					var ca = new Attribute("Column");
					var canBeReplaced = true;

					if (c.MemberName != c.ColumnName)
					{
						var space = new string(' ', nameMaxLen - c.ColumnName.Length);

						ca.Parameters.Add('"' + c.ColumnName + '"' + space);
						canBeReplaced = false;
					}
					else if (nameMaxLen > 0)
					{
						ca.Parameters.Add(new string(' ', nameMaxLen + 2));
						canBeReplaced = false;
					}

					if (GenerateDbTypes)
					{
						var space = new string(' ', dbTypeMaxLen - c.ColumnType.Length);

						ca.Parameters.Add("DbType=\"" + c.ColumnType + '"' + space);
						canBeReplaced = false;
					}

					if (GenerateDataTypes)
					{
						var space = new string(' ', dataTypeMaxLen - c.DataType.Length);
						ca.Parameters.Add("DataType=" + dataTypePrefix + c.DataType + space);
						canBeReplaced = false;
					}

					if (GenerateDataTypes && !GenerateLengthProperty.HasValue || GenerateLengthProperty == true)
					{
						if (c.Length != null)
							ca.Parameters.Add("Length=" + (c.Length == int.MaxValue ? "int.MaxValue" : c.Length.ToString()));
						canBeReplaced = false;
					}

					if (GenerateDataTypes && !GeneratePrecisionProperty.HasValue || GeneratePrecisionProperty == true)
					{
						if (c.Precision != null)
							ca.Parameters.Add("Precision=" + c.Precision);
						canBeReplaced = false;
					}

					if (GenerateDataTypes && !GenerateScaleProperty.HasValue || GenerateScaleProperty == true)
					{
						if (c.Scale != null)
							ca.Parameters.Add("Scale=" + c.Scale);
						canBeReplaced = false;
					}

					if (c.SkipOnInsert && !c.IsIdentity)
					{
						ca.Parameters.Add("SkipOnInsert=true");
						canBeReplaced = false;
					}

					if (c.SkipOnUpdate && !c.IsIdentity)
					{
						ca.Parameters.Add("SkipOnUpdate=true");
						canBeReplaced = false;
					}

					if (c.IsDiscriminator)
					{
						ca.Parameters.Add("IsDiscriminator=true");
						canBeReplaced = false;
					}

					c.Attributes.Add(ca);

					// PK.
					//
					if (c.IsPrimaryKey)
					{
						var pka = new Attribute("PrimaryKey");

						if (nPKs > 1)
							pka.Parameters.Add(c.PrimaryKeyOrder.ToString());

						if (canBeReplaced)
							c.Attributes[0] = pka;
						else
							c.Attributes.Add(pka);

						canBeReplaced = false;
					}

					// Identity.
					//
					if (c.IsIdentity)
					{
						var ida = new Attribute("Identity");

						if (canBeReplaced)
							c.Attributes[0] = ida;
						else
							c.Attributes.Add(ida);

						canBeReplaced = false;
					}

					// Nullable.
					//
					if (c.IsNullable)
						c.Attributes.Add(new Attribute((allNullable ? "" : "   ") + "Nullable"));
					else if (!c.IsIdentity)
						c.Attributes.Add(new Attribute("NotNull"));

					var columnComments = new List<string>();

					if (!string.IsNullOrWhiteSpace(c.Description))
					{
						columnComments.Add("/ <summary>");
						foreach (var line in c.Description.Split('\n'))
							columnComments.Add("/ " + line.TrimEnd());
						columnComments.Add("/ </summary>");
					}

					if (columnComments.Count > 0)
						c.Comment.AddRange(columnComments);

					// End line comment.
					//
					c.EndLineComment = c.ColumnType;

					SetPropertyValue(c, "IsNotifying", true);
					SetPropertyValue(c, "IsEditable", true);

					columns.Members.Add(c);

					// Alias.
					//
					if (c.AliasName != null && c.AliasName != c.MemberName)
					{
						var caProp = new Property(
							c.Type,
							c.AliasName,
							new[] { c.MemberName },
							new[] { c.MemberName + " = value;" });

						caProp.Comment.AddRange(columnComments);

						if (GenerateObsoleteAttributeForAliases)
							caProp.Attributes.Add(new Attribute("Obsolete", "\"Use " + c.MemberName + " instead.\""));

						caProp.Attributes.Add(new Attribute("ColumnAlias", "\"" + c.MemberName + "\""));

						columnAliases.Members.Add(caProp);
					}
				}

				t.Members.Add(columns);

				if (columnAliases.Members.Count > 0)
					t.Members.Add(columnAliases);

				if (GenerateAssociations)
				{
					var keys = t.ForeignKeys.Values.ToList();

					if (!GenerateBackReferences)
						keys = keys.Where(k => k.BackReference != null).ToList();

					if (keys.Count > 0)
					{
						var associations = new MemberGroup { Region = "Associations" };

						foreach (var key in keys)
						{
							key.Comment.Add("/ <summary>");
							key.Comment.Add("/ " + key.KeyName);
							key.Comment.Add("/ </summary>");

							if (key.AssociationType == AssociationType.OneToMany)
								key.Type = string.Format(OneToManyAssociationType, key.OtherTable.TypePrefix + key.OtherTable.TypeName);
							else
								key.Type = key.OtherTable.TypePrefix + key.OtherTable.TypeName;

							var aa = new Attribute("Association");

							aa.Parameters.Add("ThisKey=\"" + string.Join(", ", (from c in key.ThisColumns select c.MemberName).ToArray()) + "\"");
							aa.Parameters.Add("OtherKey=\"" + string.Join(", ", (from c in key.OtherColumns select c.MemberName).ToArray()) + "\"");
							aa.Parameters.Add("CanBeNull=" + (key.CanBeNull ? "true" : "false"));

							switch (key.AssociationType)
							{
								case AssociationType.OneToOne:
									aa.Parameters.Add("Relationship=Relationship.OneToOne");
									break;
								case AssociationType.OneToMany:
									aa.Parameters.Add("Relationship=Relationship.OneToMany");
									break;
								case AssociationType.ManyToOne:
									aa.Parameters.Add("Relationship=Relationship.ManyToOne");
									break;
							}

							if (key.BackReference != null)
							{
								if (!string.IsNullOrEmpty(key.KeyName))
									aa.Parameters.Add("KeyName=\"" + key.KeyName + "\"");
								if (GenerateBackReferences && !string.IsNullOrEmpty(key.BackReference.KeyName))
									aa.Parameters.Add("BackReferenceName=\"" + key.BackReference.MemberName + "\"");
							}
							else
							{
								aa.Parameters.Add("IsBackReference=true");
							}

							key.Attributes.Add(aa);

							SetPropertyValue(key, "IsNotifying", true);
							SetPropertyValue(key, "IsEditable", true);

							associations.Members.Add(key);
						}

						t.Members.Add(associations);
					}
				}

				if (GenerateFindExtensions && nPKs > 0)
				{
					var PKs = t.Columns.Values.Where(c => c.IsPrimaryKey).ToList();
					var maxNameLen1 = PKs.Max(c => (int?) c.MemberName.Length) ?? 0;
					var maxNameLen2 = PKs.Take(nPKs - 1).Max(c => (int?) c.MemberName.Length) ?? 0;

					tableExtensions.Members.Add(
						new Method(
							t.TypeName,
							"Find",
							new[] { (string.Format("this ITable<{0}> table", t.TypeName)) }
								.Union(PKs.Select(c => c.Type + " " + c.MemberName)),
							new[] { "return table.FirstOrDefault(t =>" }
								.Union(PKs.SelectMany((c, i) =>
								{
									var ss = new List<string>();

									if (c.Conditional != null)
										ss.Add("#if " + c.Conditional);

									ss.Add(string.Format("\tt.{0}{1} == {0}{3}{2}",
										c.MemberName, LenDiff(maxNameLen1, c.MemberName), i == nPKs - 1 ? ");" : " &&", i == nPKs - 1 ? "" : LenDiff(maxNameLen2, c.MemberName)));

									if (c.Conditional != null)
									{
										if (ss[1].EndsWith(");"))
										{
											ss[1] = ss[1].Substring(0, ss[1].Length - 2);
											ss.Add("#endif");
											ss.Add("\t\t);");
										}
										else
										{
											ss.Add("#endif");
										}
									}

									return ss;
								})))
						{
							IsStatic = true
						});
				}

				addType(t);

				if (!string.IsNullOrWhiteSpace(t.AliasTypeName))
				{
					var aClass = new Class(t.AliasTypeName)
					{
						BaseClass = t.TypeName
					};

					if (comments.Count > 0)
						aClass.Comment.AddRange(comments);

					if (GenerateObsoleteAttributeForAliases)
						aClass.Attributes.Add(new Attribute("Obsolete", "\"Use " + t.TypeName + " instead.\""));

					Model.Types.Add(aClass);
				}
			}

			if (defAliases.Members.Count > 0)
				DataContextObject.Members.Add(defAliases);

			foreach (var schema in schemas.Values)
				if (schema.Aliases.Members.Count > 0)
					schema.Type.Members.Add(defAliases);

			if (Procedures.Count > 0)
			{
				Model.Usings.Add("System.Collections.Generic");
				Model.Usings.Add("System.Data");
				Model.Usings.Add("LinqToDB.Data");
				Model.Usings.Add("LinqToDB.Common");

				var procs = new MemberGroup();
				var funcs = new MemberGroup();
				var tabfs = new MemberGroup { Region = "Table Functions" };

				foreach (var p in Procedures.Values.Where(
					proc => proc.IsLoaded || proc.IsFunction && !proc.IsTableFunction ||
					proc.IsTableFunction && proc.ResultException != null
					))
				{
					Action<MemberGroup> addProcs = tp => procs.Members.Add(tp);
					Action<MemberGroup> addFuncs = tp => funcs.Members.Add(tp);
					Action<MemberGroup> addTabfs = tp => tabfs.Members.Add(tp);

					var thisDataContext = "this";

					var schema = p.Schema != null && schemas.ContainsKey(p.Schema) ? schemas[p.Schema] : null;

					if (schema != null)
					{
						var si = schemas[p.Schema];

						addProcs = tp => si.Procedures.Members.Add(tp);
						addFuncs = tp => si.Functions.Members.Add(tp);
						addTabfs = tp => si.TableFunctions.Members.Add(tp);
						thisDataContext = "_dataContext";
					}

					var proc = new MemberGroup { Region = p.Name };

					if (!p.IsFunction)
						addProcs(proc);
					else if (p.IsTableFunction)
						addTabfs(proc);
					else
						addFuncs(proc);

					if (p.ResultException != null)
					{
						proc.Errors.Add(p.ResultException.Message);
						continue;
					}

					proc.Members.Add(p);

					if (p.IsTableFunction)
					{
						var tableAttrs = new List<string>();

						if (DatabaseName != null)
							tableAttrs.Add("Database=" + '"' + DatabaseName + '"');
						if (p.Schema != null)
							tableAttrs.Add("Schema=" + '"' + p.Schema + '"');

						tableAttrs.Add("Name=" + '"' + p.ProcedureName + '"');

						p.Attributes.Add(new Attribute("Sql.TableFunction", tableAttrs.ToArray()));

						p.Type = "ITable<" + p.ResultTable.TypeName + ">";
					}
					else if (p.IsFunction)
					{
						p.IsStatic = true;
						p.Type = p.ProcParameters.Single(pr => pr.IsResult).ParameterType;
						p.Attributes.Add(new Attribute("Sql.Function", "Name=\"" + p.Schema + "." + p.ProcedureName + "\"", "ServerSideOnly=true"));
					}
					else
					{
						p.IsStatic = true;
						p.Type = p.ResultTable == null ? "int" : "IEnumerable<" + p.ResultTable.TypeName + ">";
						p.Parameters.Add("this DataConnection dataConnection");
					}

					foreach (var pr in p.ProcParameters.Where(par => !par.IsResult))
						p.Parameters.Add(string.Format("{0}{1} {2}",
							pr.IsOut ? pr.IsIn ? "ref " : "out " : "", pr.ParameterType, pr.ParameterName));

					if (p.IsTableFunction)
					{
						var body = string.Format("return " + thisDataContext + ".GetTable<{0}>(this, (MethodInfo)MethodBase.GetCurrentMethod()", p.ResultTable.TypeName);

						body += p.ProcParameters.Count == 0 ? ");" : ",";

						p.Body.Add(body);

						for (var i = 0; i < p.ProcParameters.Count; i++)
							p.Body.Add("\t" + p.ProcParameters[i].ParameterName + (i + 1 == p.ProcParameters.Count ? ");" : ","));
					}
					else if (p.IsFunction)
					{
						p.Body.Add("throw new InvalidOperationException();");
					}
					else
					{
						var spName =
							SqlBuilder.BuildTableName(
								new System.Text.StringBuilder(),
								(string) SqlBuilder.Convert(DatabaseName, LinqToDB.SqlProvider.ConvertType.NameToDatabase),
								(string) SqlBuilder.Convert(p.Schema, LinqToDB.SqlProvider.ConvertType.NameToOwner),
								(string) SqlBuilder.Convert(p.ProcedureName, LinqToDB.SqlProvider.ConvertType.NameToQueryTable)
							).ToString();

						spName = "\"" + spName.Replace("\"", "\\\"") + "\"";

						var inputParameters = p.ProcParameters.Where(pp => pp.IsIn).ToList();
						var outputParameters = p.ProcParameters.Where(pp => pp.IsOut).ToList();

						spName += inputParameters.Count == 0 ? ");" : ",";

						var retName = "ret";
						var retNo = 0;

						while (p.ProcParameters.Any(pp => pp.ParameterName == retName))
							retName = "ret" + ++retNo;

						var hasOut = outputParameters.Any(pr => pr.IsOut);
						var prefix = hasOut ? "var " + retName + " = " : "return ";

						if (p.ResultTable == null)
							p.Body.Add(prefix + "dataConnection.ExecuteProc(" + spName);
						else
						{
							if (p.ResultTable.Columns.Values.Any(c => c.IsDuplicateOrEmpty))
							{
								p.Body.Add("var ms = dataConnection.MappingSchema;");
								p.Body.Add("");
								p.Body.Add(prefix + "dataConnection.QueryProc(dataReader =>");
								p.Body.Add("\tnew " + p.ResultTable.TypeName);
								p.Body.Add("\t{");

								var n = 0;
								var maxNameLen = p.ResultTable.Columns.Values.Max(c => (int?) c.MemberName.Length) ?? 0;
								var maxTypeLen = p.ResultTable.Columns.Values.Max(c => (int?) c.Type.Length) ?? 0;

								foreach (var c in p.ResultTable.Columns.Values)
								{
									p.Body.Add(string.Format("\t\t{0}{1} = Converter.ChangeTypeTo<{2}>{3}(dataReader.GetValue({4}), ms),",
										c.MemberName, LenDiff(maxNameLen, c.MemberName), c.Type, LenDiff(maxTypeLen, c.Type), n++));
								}

								p.Body.Add("\t},");
								p.Body.Add("\t" + spName);
							}
							else
							{
								p.Body.Add(prefix + "dataConnection.QueryProc<" + p.ResultTable.TypeName + ">(" + spName);
							}
						}

						var maxLenSchema = inputParameters.Max(pr => (int?) pr.SchemaName.Length) ?? 0;
						var maxLenParam = inputParameters.Max(pr => (int?) pr.ParameterName.Length) ?? 0;
						var maxLenType = inputParameters.Max(pr => (int?) ("DataType." + pr.DataType).Length) ?? 0;

						for (var i = 0; i < inputParameters.Count; i++)
						{
							var pr = inputParameters[i];

							var str = string.Format("\tnew DataParameter(\"{0}\", {1}{2}, {3}{4})",
								pr.SchemaName,
								LenDiff(maxLenSchema, pr.SchemaName),
								pr.ParameterName,
								LenDiff(maxLenParam, pr.ParameterName),
								"DataType." + pr.DataType);

							if (pr.IsOut)
							{
								str += LenDiff(maxLenType, "DataType." + pr.DataType);
								str += " { Direction = " + (pr.IsIn ? "ParameterDirection.InputOutput" : "ParameterDirection.Output");

								if (pr.Size != null && pr.Size.Value != 0)
									str += ", Size = " + pr.Size.Value;

								str += " }";
							}

							str += i + 1 == inputParameters.Count ? ");" : ",";

							p.Body.Add(str);
						}

						if (hasOut)
						{
							maxLenSchema = outputParameters.Max(pr => (int?) pr.SchemaName.Length) ?? 0;
							maxLenParam = outputParameters.Max(pr => (int?) pr.ParameterName.Length) ?? 0;
							maxLenType = outputParameters.Max(pr => (int?) pr.ParameterType.Length) ?? 0;

							p.Body.Add("");

							foreach (var pr in p.ProcParameters.Where(_ => _.IsOut))
							{
								var str = string.Format("{0} {1}= Converter.ChangeTypeTo<{2}>{3}(((IDbDataParameter)dataConnection.Command.Parameters[\"{4}\"]).{5}Value);",
									pr.ParameterName,
									LenDiff(maxLenParam, pr.ParameterName),
									pr.ParameterType,
									LenDiff(maxLenType, pr.ParameterType),
									pr.SchemaName,
									LenDiff(maxLenSchema, pr.SchemaName));

								p.Body.Add(str);
							}

							p.Body.Add("");
							p.Body.Add("return " + retName + ";");
						}
					}

					if (p.ResultTable != null && p.ResultTable.DataContextPropertyName == null)
					{
						var columns = new MemberGroup { IsCompact = true };

						foreach (var c in p.ResultTable.Columns.Values)
						{
							if (c.MemberName != c.ColumnName)
								c.Attributes.Add(new Attribute("Column") { Parameters = { '"' + c.ColumnName + '"' } });
							columns.Members.Add(c);
						}

						p.ResultTable.Members.Add(columns);
						proc.Members.Add(p.ResultTable);
					}
				}

				if (procs.Members.Count > 0)
					Model.Types.Add(new Class(DataContextObject.Name + "StoredProcedures", procs) { IsStatic = true });

				if (funcs.Members.Count > 0)
					Model.Types.Add(new Class("SqlFunctions", funcs) { IsStatic = true });

				if (tabfs.Members.Count > 0)
					DataContextObject.Members.Add(tabfs);

				foreach (var schema in schemas.Values)
				{
					if (schema.Procedures.Members.Count > 0)
						schema.Type.Members.Add(new Class(DataContextObject.Name + "StoredProcedures", schema.Procedures) { IsStatic = true });

					if (schema.Functions.Members.Count > 0)
						schema.Type.Members.Add(new Class("SqlFunctions", schema.Functions) { IsStatic = true });

					if (schema.TableFunctions.Members.Count > 0)
						schema.DataContext.Members.Add(schema.TableFunctions);
				}
			}

			if (defTableExtensions.Members.Count > 0)
			{
				Model.Usings.Add("System.Linq");
				Model.Types.Add(new Class("TableExtensions", defTableExtensions) { IsStatic = true });
			}

			foreach (var schema in schemas.Values)
			{
				Model.Types.Add(schema.Type);

				if (schema.TableExtensions.Members.Count > 0)
				{
					Model.Usings.Add("System.Linq");
					schema.Type.Members.Add(schema.TableExtensions);
				}
			}

			Tables.Clear();
			Procedures.Clear();

			Model.SetParent();

			AfterGenerateLinqToDBModel();
		}

		private Column GetColumn(string tableName, string columnName)
		{
			Column col;
			Table tbl = GetTable(tableName);
			if (!tbl.Columns.TryGetValue(columnName, out col))
			{
				base.WriteLine(string.Concat(new string[] { "#error Column '", tableName, "'.'", columnName, "' not found." }));
				base.WriteLine("");
				base.WriteLine("/*");
				base.WriteLine(string.Concat("\tExisting '", tableName, "'columns:"));
				base.WriteLine("");
				foreach (string key in tbl.Columns.Keys)
				{
					base.WriteLine(string.Concat("\t", key));
				}
				base.WriteLine(" */");
				throw new ArgumentException(string.Concat(new string[] { "Column '", tableName, "'.'", columnName, "' not found." }));
			}
			return col;
		}

		private ForeignKey GetFK(string tableName, string fkName)
		{
			return GetForeignKey(tableName, fkName);
		}

		private ForeignKey GetForeignKey(string tableName, string fkName)
		{
			ForeignKey col;
			Table tbl = GetTable(tableName);
			if (!tbl.ForeignKeys.TryGetValue(fkName, out col))
			{
				base.WriteLine(string.Concat(new string[] { "#error FK '", tableName, "'.'", fkName, "' not found." }));
				base.WriteLine("");
				base.WriteLine("/*");
				base.WriteLine(string.Concat("\tExisting '", tableName, "'FKs:"));
				base.WriteLine("");
				foreach (string key in tbl.ForeignKeys.Keys)
				{
					base.WriteLine(string.Concat("\t", key));
				}
				base.WriteLine(" */");
				throw new ArgumentException(string.Concat(new string[] { "FK '", tableName, "'.'", fkName, "' not found." }));
			}
			return col;
		}

		private Procedure GetProcedure(string name)
		{
			Procedure proc;
			if (!Procedures.TryGetValue(name, out proc))
			{
				base.WriteLine(string.Concat("#error Procedure '", name, "' not found."));
				base.WriteLine("");
				base.WriteLine("/*");
				base.WriteLine("\tExisting procedures:");
				base.WriteLine("");
				foreach (string key in Procedures.Keys)
				{
					base.WriteLine(string.Concat("\t", key));
				}
				base.WriteLine(" */");
				throw new ArgumentException(string.Concat("Procedure '", name, "' not found."));
			}
			return proc;
		}

		private Table GetTable(string name)
		{
			Table tbl;
			if (!Tables.TryGetValue(name, out tbl))
			{
				base.WriteLine(string.Concat("#error Table '", name, "' not found."));
				base.WriteLine("/*");
				base.WriteLine("\tExisting tables:");
				base.WriteLine("");
				foreach (string key in Tables.Keys)
				{
					base.WriteLine(string.Concat("\t", key));
				}
				base.WriteLine(" */");
				throw new ArgumentException(string.Concat("Table '", name, "' not found."));
			}
			return tbl;
		}

		public static IEnumerable<INode> GetTreeNodes(INode parent)
		{
			foreach (INode tree in parent.Children)
			{
				yield return tree;
				foreach (INode treeNode in GetTreeNodes(tree))
				{
					yield return treeNode;
				}
			}
		}

		private static string LenDiff(int max, string str)
		{
			string s = "";
			while (true)
			{
				int num = max;
				max = num - 1;
				if (num <= str.Length)
				{
					break;
				}
				s = string.Concat(s, " ");
			}
			return s;
		}

		private void RemoveSpace()
		{
			bool flag;
			base.Write(" ");
			while (true)
			{
				if (base.GenerationEnvironment.Length <= 0)
				{
					flag = false;
				}
				else
				{
					flag = (base.GenerationEnvironment[base.GenerationEnvironment.Length - 1] == ' ' ? true : base.GenerationEnvironment[base.GenerationEnvironment.Length - 1] == '\t');
				}
				if (!flag)
				{
					break;
				}
				StringBuilder generationEnvironment = base.GenerationEnvironment;
				generationEnvironment.Length = generationEnvironment.Length - 1;
			}
		}

		private void RenderUsings(List<string> usings)
		{
			IEnumerable<IGrouping<string, string>> q = 
				from ns in usings.Distinct<string>()
				group ns by ns.Split(new char[] { '.' })[0];
			IEnumerable<IGrouping<string, string>> groups = (
				from ns in q
				where ns.Key == "System"
				select ns).Concat<IGrouping<string, string>>(
				from ns in q
				where ns.Key != "System"
				orderby ns.Key
				select ns);
			foreach (IGrouping<string, string> gr in groups)
			{
				foreach (string str in 
					from s in gr
					orderby s
					select s)
				{
					WriteUsing(this, str);
				}
				base.WriteLine("");
			}
			Trim();
		}

		private void SetPropertyValue(Property propertyObject, string propertyName, object value)
		{
			if (SetPropertyValueAction != null)
			{
				SetPropertyValueAction(propertyObject, propertyName, value);
			}
		}

		public TableContext SetTable(string tableName, string TypeName = null, string DataContextPropertyName = null)
		{
			TableContext ctx = new TableContext()
			{
				Transformation = this,
				TableName = tableName
			};
			if ((TypeName != null ? true : DataContextPropertyName != null))
			{
				Table t = GetTable(tableName);
				if (TypeName != null)
				{
					t.TypeName = TypeName;
				}
				if (DataContextPropertyName != null)
				{
					t.DataContextPropertyName = DataContextPropertyName;
				}
			}
			return ctx;
		}

		private static void SkipSpacesAndInsert(GeneratedTextTransformation tt, string value)
		{
			int l = tt.GenerationEnvironment.Length;
			while (true)
			{
				if ((l <= 0 ? true : tt.GenerationEnvironment[l - 1] != ' '))
				{
					break;
				}
				l--;
			}
			tt.GenerationEnvironment.Insert(l, value);
		}

		private string ToCamelCase(string name)
		{
			string lower;
			int n = 0;
			string str = name;
			int num = 0;
			while (num < str.Length)
			{
				if (!char.IsUpper(str[num]))
				{
					break;
				}
				else
				{
					n++;
					num++;
				}
			}
			if (n == 0)
			{
				lower = name;
			}
			else if (n != name.Length)
			{
				n = Math.Max(1, n - 1);
				lower = string.Concat(name.Substring(0, n).ToLower(), name.Substring(n));
			}
			else
			{
				lower = name.ToLower();
			}
			return lower;
		}

		private Dictionary<string, TR> ToDictionary<T, TR>(IEnumerable<T> source, Func<T, string> keyGetter, Func<T, TR> objGetter, Func<TR, int, string> getKeyName)
		{
			Dictionary<string, TR> dic = new Dictionary<string, TR>();
			int current = 1;
			foreach (T item in source)
			{
				string key = keyGetter(item);
				TR obj = objGetter(item);
				if ((string.IsNullOrEmpty(key) ? true : dic.ContainsKey(key)))
				{
					key = getKeyName(obj, current);
				}
				dic.Add(key, obj);
				current++;
			}
			return dic;
		}

		public override string TransformText()
		{
			try
			{
				base.Write("\r\n");
				Action beforeGenerateModel = BeforeGenerateModel;
				BeforeGenerateModel = () => {
					GenerateTypesFromMetadata();
					beforeGenerateModel();
				};
				if (BaseDataContextClass == null)
				{
					BaseDataContextClass = "LinqToDB.Data.DataConnection";
				}
				base.Write("\r\n");
				Action afterGenerateLinqToDBModel = AfterGenerateLinqToDBModel;
				AfterGenerateLinqToDBModel = () => {
					afterGenerateLinqToDBModel();
					GenerateSqlServerTypes();
				};
				base.Write("\r\n");
				ToPlural = Pluralization.ToPlural;
				ToSingular = Pluralization.ToSingular;
				base.Write("\r\n");
				DatabaseName = null;
				GenerateDatabaseName = true;
				OneToManyAssociationType = "List<{0}>";
				IncludeDefaultSchema = false;
				GenerateObsoleteAttributeForAliases = true;
				GenerateDataTypes = true;
				GenerateDbTypes = true;
				GenerateSchemaAsType = true;
				SchemaNameMapping.Add("TestSchema", "MySchema");
				//LoadSqlServerMetadata("Server=DBHost\\SQLSERVER2012;Database=Northwind;User Id=sa;Password=TestPassword");
				Tables["Order Details"].Columns["OrderID"].MemberName = "ID";
				GetTable("Categories").AliasPropertyName = "CATEG";
				GetTable("Categories").AliasTypeName = "CATEG";
				GetTable("Order Details").AliasPropertyName = "Order_Details";
				GetTable("Order Details").AliasTypeName = "ORD_DET";
				GenerateTypesFromMetadata();
				DataContextName = null;
				DataContextObject = null;
				DatabaseName = null;
				//LoadSqlServerMetadata("Server=DBHost\\SQLSERVER2008;Database=TestData;User Id=sa;Password=TestPassword;");
				GenerateModel();
			}
			catch (Exception exception)
			{
				Exception e = exception;
				e.Data["TextTemplatingProgress"] = base.GenerationEnvironment.ToString();
				throw new Exception("Template runtime error", e);
			}
			return base.GenerationEnvironment.ToString();
		}

		private void Trim()
		{
			char[] arr = new char[] { '\r', '\n', ' ' };
			while (true)
			{
				if ((base.GenerationEnvironment.Length <= 0 ? true : !arr.Contains<char>(base.GenerationEnvironment[base.GenerationEnvironment.Length - 1])))
				{
					break;
				}
				StringBuilder generationEnvironment = base.GenerationEnvironment;
				generationEnvironment.Length = generationEnvironment.Length - 1;
			}
			base.WriteLine("");
		}

		public void WriteSpaces(int len)
		{
			while (true)
			{
				int num = len;
				len = num - 1;
				if (num <= 0)
				{
					break;
				}
				base.Write(" ");
			}
		}

		private event Action<Property, string, object> SetPropertyValueAction;
	}
}