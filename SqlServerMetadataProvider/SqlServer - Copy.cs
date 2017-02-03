using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.SchemaProvider;
using Std.Tools.Data.Metadata.Model;
using Std.Tools.Data.Metadata.Pluralization;
using AssociationType = Std.Tools.Data.Metadata.Model.AssociationType;
using Attribute = Std.Tools.Data.Metadata.Model.Attribute;

namespace Std.Tools.Data.Metadata
{
	public partial class SqlServer : SqlServerBase
	{

		/// <summary>
		/// Create the template output
		/// </summary>
		public virtual string TransformText()
		{
			Write("\r\n");


			{
				var beforeGenerateModel = BeforeGenerateModel;
				BeforeGenerateModel = () =>
				{
					GenerateTypesFromMetadata();
					beforeGenerateModel();
				};
			}



			if (BaseDataContextClass == null)
				BaseDataContextClass = "LinqToDB.Data.DataConnection";


			Write("\r\n");


			{
				var afterGenerateLinqToDBModel = AfterGenerateLinqToDBModel;
				AfterGenerateLinqToDBModel = () =>
				{
					afterGenerateLinqToDBModel();
					GenerateSqlServerTypes();
				};
			}


			Write("\r\n");


			{
				ToPlural = Pluralization.ToPlural;
				ToSingular = Pluralization.ToSingular;
			}


			Write("\r\n");


//	NamespaceName   = "DataContext";
//	DataContextName = "NorthwindDB";
			DatabaseName = null; //"Northwind";
			GenerateDatabaseName = true;
			OneToManyAssociationType = "List<{0}>";

//	BaseEntityClass = "object";

//	GenerateBackReferences = false;
//	GenerateAssociations = true;

//	GetSchemaOptions.GetProcedures = false;

			IncludeDefaultSchema = false;
			GenerateObsoleteAttributeForAliases = true;
			GenerateDataTypes = true;
			GenerateDbTypes = true;

			GenerateSchemaAsType = true;

			SchemaNameMapping.Add("TestSchema", "MySchema");

			LoadSqlServerMetadata(@"Server=DBHost\SQLSERVER2012;Database=Northwind;User Id=sa;Password=TestPassword");

			Tables["Order Details"].Columns["OrderID"].MemberName = "ID";

			GetTable("Categories").AliasPropertyName = "CATEG";
			GetTable("Categories").AliasTypeName = "CATEG";
			GetTable("Order Details").AliasPropertyName = "Order_Details";
			GetTable("Order Details").AliasTypeName = "ORD_DET";

			GenerateTypesFromMetadata();

			DataContextName = null;
			DataContextObject = null;

			DatabaseName = null; //"TestData";

			LoadSqlServerMetadata(@"Server=DBHost\SQLSERVER2008;Database=TestData;User Id=sa;Password=TestPassword;");
			GenerateModel();


			return GenerationEnvironment.ToString();
		}

		void GenerateSqlServerTypes()
		{
			Model.Usings.Add("System.Collections.Generic");
			Model.Usings.Add("System.Linq.Expressions");
			Model.Usings.Add("System.Reflection");
			Model.Usings.Add("LinqToDB");
			Model.Usings.Add("LinqToDB.DataProvider.SqlServer");

			DataContextObject.Members.Add(
				new MemberGroup
				{
					Region = "FreeTextTable",
					Members =
					{
						new Class("FreeTextKey<T>",
							new MemberGroup
							{
								IsCompact = true,
								Members =
								{
									new Field("T", "Key"),
									new Field("int", "Rank")
								}
							})
						{
							IsPartial = false
						},

						new Method("ITable<FreeTextKey<TKey>>", "FreeTextTable<TTable,TKey>",
							new[] {"string field", "string text"},
							new[]
							{
								"return this.GetTable<FreeTextKey<TKey>>(",
								"	this,",
								"	((MethodInfo)(MethodBase.GetCurrentMethod())).MakeGenericMethod(typeof(TTable), typeof(TKey)),",
								"	field,",
								"	text);",
							})
						{
							Attributes = {new Attribute("FreeTextTableExpression")}
						},
						new Method("ITable<FreeTextKey<TKey>>", "FreeTextTable<TTable,TKey>",
							new[] {"Expression<Func<TTable,string>> fieldSelector", "string text"},
							new[]
							{
								"return this.GetTable<FreeTextKey<TKey>>(",
								"	this,",
								"	((MethodInfo)(MethodBase.GetCurrentMethod())).MakeGenericMethod(typeof(TTable), typeof(TKey)),",
								"	fieldSelector,",
								"	text);",
							})
						{
							Attributes = {new Attribute("FreeTextTableExpression")}
						},
					}
				}
			);
		}

		DataConnection GetSqlServerConnection(string connectionString)
		{
			return LinqToDB.DataProvider.SqlServer.SqlServerTools.CreateDataConnection(connectionString);
		}

		DataConnection GetSqlServerConnection(string server, string database)
		{
			return
				GetSqlServerConnection($"Data Source={server};Database={database};Integrated Security=SSPI");
		}

		DataConnection GetSqlServerConnection(string server, string database, string user, string password)
		{
			return
				GetSqlServerConnection($"Server={server};Database={database};User Id={user};Password={password};");
		}

		void LoadSqlServerMetadata(string connectionString)
		{
			var dataConnection = GetSqlServerConnection(connectionString);
			LoadMetadata(dataConnection);
		}

		void LoadSqlServerMetadata(string server, string database)
		{
			var dataConnection = GetSqlServerConnection(server, database);
			LoadMetadata(dataConnection);
		}

		void LoadSqlServerMetadata(string server, string database, string user, string password)
		{
			var dataConnection = GetSqlServerConnection(server, database, user, password);
			LoadMetadata(dataConnection);
		}



		Action BeforeGenerateLinqToDBModel = () => { };
		Action AfterGenerateLinqToDBModel = () => { };

		Func<Table, MemberBase> GenerateProviderSpecificTable = t => null;

		bool GenerateObsoleteAttributeForAliases = false;
		bool GenerateFindExtensions = true;
		bool IsCompactColumns = true;
		bool IsCompactColumnAliases = true;
		bool GenerateDataTypes = false;
		bool? GenerateLengthProperty = null;
		bool? GeneratePrecisionProperty = null;
		bool? GenerateScaleProperty = null;
		bool GenerateDbTypes = false;
		bool GenerateSchemaAsType = false;
		bool GenerateViews = true;
		string SchemaNameSuffix = "Schema";
		string SchemaDataContextTypeName = "DataContext";

		Dictionary<string, string> SchemaNameMapping = new Dictionary<string, string>();

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
				group t by t.Schema
				into gr
				let typeName = SchemaNameMapping.TryGetValue(gr.Key, out schemaName) ? schemaName : gr.Key
				select new
				{
					Name = gr.Key,
					TypeName = typeName + SchemaNameSuffix,
					PropertyName = typeName,
					Props = new MemberGroup {IsCompact = true},
					Aliases = new MemberGroup {IsCompact = true, Region = "Alias members"},
					TableExtensions = new MemberGroup {Region = "Table Extensions"},
					Type = new Class(typeName + SchemaNameSuffix) {IsStatic = true},
					Tables = gr.ToList(),
					DataContext = new Class(SchemaDataContextTypeName),
					Procedures = new MemberGroup(),
					Functions = new MemberGroup(),
					TableFunctions = new MemberGroup {Region = "Table Functions"},
				}
			).ToDictionary(t => t.Name);

			var defProps = new MemberGroup {IsCompact = true};
			var defAliases = new MemberGroup {IsCompact = true, Region = "Alias members"};
			var defTableExtensions = new MemberGroup {};

			if (GenerateConstructors)
			{
				var body = new List<string>();

				if (schemas.Count > 0)
				{
					var schemaGroup = new MemberGroup {IsCompact = true, Region = "Schemas"};

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
					DataContextObject.Members.Add(new Method(null, DataContextObject.Name, new string[0], body)
					{
						AfterSignature = {": base(\"" + DefaultConfiguration + "\")"}
					});
				DataContextObject.Members.Add(new Method(null, DataContextObject.Name, new[] {"string configuration"}, body)
				{
					AfterSignature = {": base(configuration)"}
				});

				DataContextObject.Members.Add(new MemberGroup
				{
					IsCompact = true,
					Members = {new Method("void", "InitDataContext") {AccessModifier = AccessModifier.Partial}}
				});
			}

			if (Tables.Count > 0)
				DataContextObject.Members.Insert(0, defProps);

			foreach (var schema in schemas.Values)
			{
				schema.Type.Members.Add(schema.DataContext);
				schema.DataContext.Members.Insert(0, schema.Props);

				schema.DataContext.Members.Add(new Field("IDataContext", "_dataContext")
				{
					AccessModifier = AccessModifier.Private,
					IsReadonly = true
				});
				schema.DataContext.Members.Add(new Method(null, schema.DataContext.Name, new[] {"IDataContext dataContext"},
					new[] {"_dataContext = dataContext;"}));

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

				MemberBase dcProp = t.IsProviderSpecific
					? GenerateProviderSpecificTable(t)
					: new Property(
						$"ITable<{t.TypeName}>",
						t.DataContextPropertyName,
						new[] {string.Format((schema == null ? "this" : "_dataContext") + ".GetTable<{0}>()", t.TypeName)},
						null);

				if (dcProp == null) continue;

				t.DataContextProperty = dcProp;

				props.Members.Add(dcProp);

				Property aProp = null;

				if (t.AliasPropertyName != null && t.AliasPropertyName != t.DataContextPropertyName)
				{
					aProp = new Property(
						$"ITable<{t.TypeName}>",
						t.AliasPropertyName,
						new[] {t.DataContextPropertyName},
						null);

					if (GenerateObsoleteAttributeForAliases)
						aProp.Attributes.Add(new Attribute("Obsolete", "\"Use " + t.DataContextPropertyName + " instead.\""));

					aliases.Members.Add(aProp);
				}

				var tableAttrs = new List<string>();

				if (DatabaseName != null) tableAttrs.Add("Database=" + '"' + DatabaseName + '"');
				if (t.Schema != null) tableAttrs.Add("Schema=" + '"' + t.Schema + '"');

				tableAttrs.Add((tableAttrs.Count == 0 ? "" : "Name=") + '"' + t.TableName + '"');

				if (t.IsView)
					tableAttrs.Add("IsView=true");

				t.Attributes.Add(new Attribute("Table", tableAttrs.ToArray()) {IsSeparated = true});

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

				var columns = new MemberGroup {IsCompact = IsCompactColumns};
				var columnAliases = new MemberGroup {IsCompact = IsCompactColumnAliases, Region = "Alias members"};
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
						if (c.Precision != null) ca.Parameters.Add("Precision=" + c.Precision);
						canBeReplaced = false;
					}

					if (GenerateDataTypes && !GenerateScaleProperty.HasValue || GenerateScaleProperty == true)
					{
						if (c.Scale != null) ca.Parameters.Add("Scale=" + c.Scale);
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
							new[] {c.MemberName},
							new[] {c.MemberName + " = value;"});

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
						var associations = new MemberGroup {Region = "Associations"};

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

							aa.Parameters.Add("ThisKey=\"" + string.Join(", ", (from c in key.ThisColumns select c.MemberName).ToArray()) +
							                  "\"");
							aa.Parameters.Add("OtherKey=\"" + string.Join(", ", (from c in key.OtherColumns select c.MemberName).ToArray()) +
							                  "\"");
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
							new[] {($"this ITable<{t.TypeName}> table")}
								.Union(PKs.Select(c => c.Type + " " + c.MemberName)),
							new[] {"return table.FirstOrDefault(t =>"}
								.Union(PKs.SelectMany((c, i) =>
								{
									var ss = new List<string>();

									if (c.Conditional != null)
										ss.Add("#if " + c.Conditional);

									ss.Add(string.Format("\tt.{0}{1} == {0}{3}{2}",
										c.MemberName, LenDiff(maxNameLen1, c.MemberName), i == nPKs - 1 ? ");" : " &&",
										i == nPKs - 1 ? "" : LenDiff(maxNameLen2, c.MemberName)));

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
				var tabfs = new MemberGroup {Region = "Table Functions"};

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

					var proc = new MemberGroup {Region = p.Name};

					if (!p.IsFunction) addProcs(proc);
					else if (p.IsTableFunction) addTabfs(proc);
					else addFuncs(proc);

					if (p.ResultException != null)
					{
						proc.Errors.Add(p.ResultException.Message);
						continue;
					}

					proc.Members.Add(p);

					if (p.IsTableFunction)
					{
						var tableAttrs = new List<string>();

						if (DatabaseName != null) tableAttrs.Add("Database=" + '"' + DatabaseName + '"');
						if (p.Schema != null) tableAttrs.Add("Schema=" + '"' + p.Schema + '"');

						tableAttrs.Add("Name=" + '"' + p.ProcedureName + '"');

						p.Attributes.Add(new Attribute("Sql.TableFunction", tableAttrs.ToArray()));

						p.Type = "ITable<" + p.ResultTable.TypeName + ">";
					}
					else if (p.IsFunction)
					{
						p.IsStatic = true;
						p.Type = p.ProcParameters.Single(pr => pr.IsResult).ParameterType;
						p.Attributes.Add(new Attribute("Sql.Function", "Name=\"" + p.Schema + "." + p.ProcedureName + "\"",
							"ServerSideOnly=true"));
					}
					else
					{
						p.IsStatic = true;
						p.Type = p.ResultTable == null ? "int" : "IEnumerable<" + p.ResultTable.TypeName + ">";
						p.Parameters.Add("this DataConnection dataConnection");
					}

					foreach (var pr in p.ProcParameters.Where(par => !par.IsResult))
						p.Parameters.Add($"{(pr.IsOut ? pr.IsIn ? "ref " : "out " : "")}{pr.ParameterType} {pr.ParameterName}");

					if (p.IsTableFunction)
					{
						var body =
							string.Format("return " + thisDataContext + ".GetTable<{0}>(this, (MethodInfo)MethodBase.GetCurrentMethod()",
								p.ResultTable.TypeName);

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
									p.Body.Add(
										$"\t\t{c.MemberName}{LenDiff(maxNameLen, c.MemberName)} = Converter.ChangeTypeTo<{c.Type}>{LenDiff(maxTypeLen, c.Type)}(dataReader.GetValue({n++}), ms),");
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

							var str =
								$"\tnew DataParameter(\"{pr.SchemaName}\", {LenDiff(maxLenSchema, pr.SchemaName)}{pr.ParameterName}, {LenDiff(maxLenParam, pr.ParameterName)}{"DataType." + pr.DataType})";

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
								var str =
									$"{pr.ParameterName} {LenDiff(maxLenParam, pr.ParameterName)}= Converter.ChangeTypeTo<{pr.ParameterType}>{LenDiff(maxLenType, pr.ParameterType)}(((IDbDataParameter)dataConnection.Command.Parameters[\"{pr.SchemaName}\"]).{LenDiff(maxLenSchema, pr.SchemaName)}Value);";

								p.Body.Add(str);
							}

							p.Body.Add("");
							p.Body.Add("return " + retName + ";");
						}
					}

					if (p.ResultTable != null && p.ResultTable.DataContextPropertyName == null)
					{
						var columns = new MemberGroup {IsCompact = true};

						foreach (var c in p.ResultTable.Columns.Values)
						{
							if (c.MemberName != c.ColumnName)
								c.Attributes.Add(new Attribute("Column") {Parameters = {'"' + c.ColumnName + '"'}});
							columns.Members.Add(c);
						}

						p.ResultTable.Members.Add(columns);
						proc.Members.Add(p.ResultTable);
					}
				}

				if (procs.Members.Count > 0)
					Model.Types.Add(new Class(DataContextObject.Name + "StoredProcedures", procs) {IsStatic = true});

				if (funcs.Members.Count > 0)
					Model.Types.Add(new Class("SqlFunctions", funcs) {IsStatic = true});

				if (tabfs.Members.Count > 0)
					DataContextObject.Members.Add(tabfs);

				foreach (var schema in schemas.Values)
				{
					if (schema.Procedures.Members.Count > 0)
						schema.Type.Members.Add(new Class(DataContextObject.Name + "StoredProcedures", schema.Procedures)
						{
							IsStatic = true
						});

					if (schema.Functions.Members.Count > 0)
						schema.Type.Members.Add(new Class("SqlFunctions", schema.Functions) {IsStatic = true});

					if (schema.TableFunctions.Members.Count > 0)
						schema.DataContext.Members.Add(schema.TableFunctions);
				}
			}

			if (defTableExtensions.Members.Count > 0)
			{
				Model.Usings.Add("System.Linq");
				Model.Types.Add(new Class("TableExtensions", defTableExtensions) {IsStatic = true});
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

			Model.SetTree();

			AfterGenerateLinqToDBModel();
		}




		string NamespaceName
		{
			get { return Model.Namespace.Name; }
			set { Model.Namespace.Name = value; }
		}

		string DatabaseName = null;
		string DataContextName = null;
		string BaseDataContextClass = null;
		string BaseEntityClass = null;
		string OneToManyAssociationType = "IEnumerable<{0}>";

		bool GenerateDatabaseName = false;
		bool GenerateConstructors = true;
		string DefaultConfiguration = null;
		bool GenerateAssociations = true;
		bool GenerateBackReferences = true;
		bool ReplaceSimilarTables = true;
		bool IncludeDefaultSchema = true;

		Class DataContextObject;

		bool PluralizeClassNames = false;
		bool SingularizeClassNames = true;
		bool PluralizeDataContextPropertyNames = true;
		bool SingularizeDataContextPropertyNames = false;

		GetSchemaOptions GetSchemaOptions =
			new GetSchemaOptions();

		LinqToDB.SqlProvider.ISqlBuilder SqlBuilder;

		Func<TableSchema, Table> LoadProviderSpecificTable = tableSchema => null;

		void LoadServerMetadata(DataConnection dataConnection)
		{
			SqlBuilder = dataConnection.DataProvider.CreateSqlBuilder();

			var schemaProvider = dataConnection.DataProvider.GetSchemaProvider();
			var dbSchema = schemaProvider.GetSchema(dataConnection, GetSchemaOptions);

			if (DatabaseName == null && GenerateDatabaseName)
			{
				DatabaseName = dbSchema.Database;
			}

			if (DataContextName == null)
			{
				DataContextObject.Name = DataContextName = dbSchema.Database + "DB";
			}

			DataContextObject.Comment.Add("/ <summary>");
			DataContextObject.Comment.Add("/ Database       : " + dbSchema.Database);
			DataContextObject.Comment.Add("/ Data Source    : " + dbSchema.DataSource);
			DataContextObject.Comment.Add("/ Server Version : " + dbSchema.ServerVersion);
			DataContextObject.Comment.Add("/ </summary>");

			var tables = dbSchema.Tables
				.Where(table => !table.IsProviderSpecific)
				.Select(schema => new
				{
					t = schema,
					key = schema.IsDefaultSchema ? schema.TableName : schema.SchemaName + "." + schema.TableName,
					table = new Table
					{
						TableSchema = schema,
						Schema = (schema.IsDefaultSchema && !IncludeDefaultSchema) || string.IsNullOrEmpty(schema.SchemaName) ? null : schema.SchemaName,
						BaseClass = BaseEntityClass,
						TableName = schema.TableName,
						TypeName =
							PluralizeClassNames
								? ToPlural(schema.TypeName)
								: SingularizeClassNames ? ToSingular(schema.TypeName) : schema.TypeName,
						DataContextPropertyName =
							PluralizeDataContextPropertyNames
								? ToPlural(schema.TypeName)
								: SingularizeDataContextPropertyNames ? ToSingular(schema.TypeName) : schema.TypeName,
						IsView = schema.IsView,
						IsProviderSpecific = false,
						Description = schema.Description,
						Columns = schema.Columns.ToDictionary(
							c => c.ColumnName,
							c => new Column
							{
								ColumnName = c.ColumnName,
								ColumnType = c.ColumnType,
								DataType = "DataType." + c.DataType,
								Length = c.Length,
								Precision = c.Precision,
								Scale = c.Scale,
								IsNullable = c.IsNullable,
								IsIdentity = c.IsIdentity,
								IsPrimaryKey = c.IsPrimaryKey,
								PrimaryKeyOrder = c.PrimaryKeyOrder,
								MemberName = CheckType(c.SystemType, c.MemberName),
								Type = c.MemberType,
								SkipOnInsert = c.SkipOnInsert,
								SkipOnUpdate = c.SkipOnUpdate,
								Description = c.Description,
							})
					}
				})
				.ToList();

			tables.AddRange(dbSchema.Tables
				.Where(t => t.IsProviderSpecific)
				.Select(t => new
				{
					t,
					key = t.IsDefaultSchema ? t.TableName : t.SchemaName + "." + t.TableName,
					table = LoadProviderSpecificTable(t)
				})
				.Where(t => t.table != null));

			foreach (var t in tables)
			{
				Tables.Add(t.key, t.table);
			}

			var keys =
			(
				from t in tables
				from k in t.t.ForeignKeys
				let otherTable = tables.Where(tbl => tbl.t == k.OtherTable).Select(tbl => tbl.table).Single()
				select new
				{
					k,
					k.KeyName,
					t,
					foreignKey = new ForeignKey
					{
						KeyName = k.KeyName,
						OtherTable = otherTable,
						OtherColumns = k.OtherColumns.Select(c => otherTable.Columns[c.ColumnName]).ToList(),
						ThisColumns = k.ThisColumns.Select(c => t.table.Columns[c.ColumnName]).ToList(),
						CanBeNull = k.CanBeNull,
						MemberName = k.MemberName,
						AssociationType = (AssociationType) (int) k.AssociationType,
					}
				}
			).ToList();

			foreach (var key in keys)
			{
				key.t.table.ForeignKeys.Add(
					key.k.OtherTable.IsDefaultSchema ? key.KeyName : key.k.OtherTable.SchemaName + "." + key.KeyName,
					key.foreignKey);

				if (key.k.BackReference != null)
				{
					key.foreignKey.BackReference = keys.First(k => k.k == key.k.BackReference).foreignKey;
				}

				key.foreignKey.MemberName = key.foreignKey.MemberName.Replace(".", String.Empty);

				key.foreignKey.MemberName = key.foreignKey.AssociationType == AssociationType.OneToMany
					? ToPlural(key.foreignKey.MemberName)
					: ToSingular(key.foreignKey.MemberName);
			}

			var procedures = dbSchema.Procedures
				.Select(p => new
				{
					p,
					key = p.IsDefaultSchema ? p.ProcedureName : p.SchemaName + "." + p.ProcedureName,
					proc = new Procedure
					{
						Schema = (p.IsDefaultSchema && !IncludeDefaultSchema) || string.IsNullOrEmpty(p.SchemaName) ? null : p.SchemaName,
						ProcedureName = p.ProcedureName,
						Name = p.MemberName,
						IsFunction = p.IsFunction,
						IsTableFunction = p.IsTableFunction,
						IsDefaultSchema = p.IsDefaultSchema,
						IsLoaded = p.IsLoaded,
						ResultTable = p.ResultTable == null
							? null
							: new Table
							{
								TypeName =
									PluralizeClassNames
										? ToPlural(p.ResultTable.TypeName)
										: SingularizeClassNames ? ToSingular(p.ResultTable.TypeName) : p.ResultTable.TypeName,
								Columns = ToDictionary(
									p.ResultTable.Columns,
									c => c.ColumnName,
									c => new Column
									{
										ColumnName = c.ColumnName,
										ColumnType = c.ColumnType,
										IsNullable = c.IsNullable,
										IsIdentity = c.IsIdentity,
										IsPrimaryKey = c.IsPrimaryKey,
										PrimaryKeyOrder = c.PrimaryKeyOrder,
										MemberName = CheckColumnName(CheckType(c.SystemType, c.MemberName)),
										Type = c.MemberType,
										SkipOnInsert = c.SkipOnInsert,
										SkipOnUpdate = c.SkipOnUpdate,
										Description = c.Description,
									},
									(c, n) =>
									{
										c.IsDuplicateOrEmpty = true;
										return "$" + (c.MemberName = "Column" + n);
									})
							},
						ResultException = p.ResultException,
						SimilarTables = p.SimilarTables == null
							? new List<Table>()
							: p.SimilarTables
								.Select(t => tables.Single(tbl => tbl.t == t).table)
								.ToList(),
						ProcParameters = p.Parameters
							.Select(pr => new Parameter
							{
								SchemaName = pr.SchemaName,
								SchemaType = pr.SchemaType,
								IsIn = pr.IsIn,
								IsOut = pr.IsOut,
								IsResult = pr.IsResult,
								Size = pr.Size,
								ParameterName = CheckParameterName(pr.ParameterName),
								ParameterType = pr.ParameterType,
								SystemType = pr.SystemType,
								DataType = pr.DataType.ToString(),
							})
							.ToList(),
					}
				})
				.ToList();

			foreach (var p in procedures)
			{
				if (ReplaceSimilarTables)
					if (p.proc.SimilarTables.Count() == 1 || p.proc.SimilarTables.Count(t => !t.IsView) == 1)
						p.proc.ResultTable = p.proc.SimilarTables.Count() == 1
							? p.proc.SimilarTables[0]
							: p.proc.SimilarTables.First(t => !t.IsView);

				Procedures[p.key] = p.proc;
			}
		}

		Dictionary<string, TR> ToDictionary<T, TR>(IEnumerable<T> source, Func<T, string> keyGetter, Func<T, TR> objGetter,
			Func<TR, int, string> getKeyName)
		{
			var dic = new Dictionary<string, TR>();
			var current = 1;

			foreach (var item in source)
			{
				var key = keyGetter(item);
				var obj = objGetter(item);

				if (string.IsNullOrEmpty(key) || dic.ContainsKey(key))
					key = getKeyName(obj, current);

				dic.Add(key, obj);

				current++;
			}

			return dic;
		}

		string CheckType(Type type, string typeName)
		{
			if (!Model.Usings.Contains(type.Namespace))
				Model.Usings.Add(type.Namespace);
			return typeName;
		}

		string CheckColumnName(string memberName)
		{
			if (string.IsNullOrEmpty(memberName))
				memberName = "Empty";
			else
			{
				memberName = memberName.Replace("%", "Percent");
			}
			return memberName;
		}

		string CheckParameterName(string parameterName)
		{
			var invalidParameterNames = new List<string>
			{
				"@DataType"
			};

			var result = parameterName;
			while (invalidParameterNames.Contains(result))
			{
				result = result + "_";
			}
			return result;
		}

		Action AfterLoadMetadata = () => { };

		void LoadMetadata(DataConnection dataConnection)
		{
			if (DataContextObject == null)
			{
				DataContextObject = new Class(DataContextName)
				{
					BaseClass = BaseDataContextClass
				};

				Model.Types.Add(DataContextObject);
			}

			LoadServerMetadata(dataConnection);

			if (Tables.Values.SelectMany(_ => _.ForeignKeys.Values).Any(_ => _.AssociationType == AssociationType.OneToMany))
			{
				Model.Usings.Add("System.Collections.Generic");
			}

			var keyWords = new HashSet<string>
			{
				"abstract",
				"as",
				"base",
				"bool",
				"break",
				"byte",
				"case",
				"catch",
				"char",
				"checked",
				"class",
				"const",
				"continue",
				"decimal",
				"default",
				"delegate",
				"do",
				"double",
				"else",
				"enum",
				"event",
				"explicit",
				"extern",
				"false",
				"finally",
				"fixed",
				"float",
				"for",
				"foreach",
				"goto",
				"if",
				"implicit",
				"in",
				"int",
				"interface",
				"internal",
				"is",
				"lock",
				"long",
				"new",
				"null",
				"object",
				"operator",
				"out",
				"override",
				"params",
				"private",
				"protected",
				"public",
				"readonly",
				"ref",
				"return",
				"sbyte",
				"sealed",
				"short",
				"sizeof",
				"stackalloc",
				"static",
				"struct",
				"switch",
				"this",
				"throw",
				"true",
				"try",
				"typeof",
				"uint",
				"ulong",
				"unchecked",
				"unsafe",
				"ushort",
				"using",
				"virtual",
				"volatile",
				"void",
				"while"
			};

			foreach (var t in Tables.Values)
			{
				if (keyWords.Contains(t.TypeName))
				{
					t.TypeName = "@" + t.TypeName;
				}

				if (keyWords.Contains(t.DataContextPropertyName))
				{
					t.DataContextPropertyName = "@" + t.DataContextPropertyName;
				}

				t.TypeName = ConvertToCompilable(t.TypeName);
				t.DataContextPropertyName = ConvertToCompilable(t.DataContextPropertyName);

				foreach (var col in t.Columns.Values)
				{
					if (keyWords.Contains(col.MemberName))
					{
						col.MemberName = "@" + col.MemberName;
					}

					col.MemberName = ConvertToCompilable(col.MemberName);

					if (col.MemberName == t.TypeName)
					{
						col.MemberName += "_Column";
					}
				}
			}

			AfterLoadMetadata();
		}

		string ConvertToCompilable(string name)
		{
			var query =
				from c in name
				select char.IsLetterOrDigit(c) || c == '@' ? c : '_';

			return new string(query.ToArray());
		}

		Table GetTable(string name)
		{
			Table tbl;

			if (Tables.TryGetValue(name, out tbl))
				return tbl;

			WriteLine("#error Table '" + name + "' not found.");
			WriteLine("/*");
			WriteLine("\tExisting tables:");
			WriteLine("");

			foreach (var key in Tables.Keys)
				WriteLine("\t" + key);

			WriteLine(" */");

			throw new ArgumentException("Table '" + name + "' not found.");
		}

		Procedure GetProcedure(string name)
		{
			Procedure proc;

			if (Procedures.TryGetValue(name, out proc))
				return proc;

			WriteLine("#error Procedure '" + name + "' not found.");
			WriteLine("");
			WriteLine("/*");
			WriteLine("\tExisting procedures:");
			WriteLine("");

			foreach (var key in Procedures.Keys)
				WriteLine("\t" + key);

			WriteLine(" */");

			throw new ArgumentException("Procedure '" + name + "' not found.");
		}

		Column GetColumn(string tableName, string columnName)
		{
			var tbl = GetTable(tableName);

			Column col;

			if (tbl.Columns.TryGetValue(columnName, out col))
				return col;

			WriteLine("#error Column '" + tableName + "'.'" + columnName + "' not found.");
			WriteLine("");
			WriteLine("/*");
			WriteLine("\tExisting '" + tableName + "'columns:");
			WriteLine("");

			foreach (var key in tbl.Columns.Keys)
				WriteLine("\t" + key);

			WriteLine(" */");

			throw new ArgumentException("Column '" + tableName + "'.'" + columnName + "' not found.");
		}

		ForeignKey GetFK(string tableName, string fkName)
		{
			return GetForeignKey(tableName, fkName);
		}

		ForeignKey GetForeignKey(string tableName, string fkName)
		{
			var tbl = GetTable(tableName);

			ForeignKey col;

			if (tbl.ForeignKeys.TryGetValue(fkName, out col))
				return col;

			WriteLine("#error FK '" + tableName + "'.'" + fkName + "' not found.");
			WriteLine("");
			WriteLine("/*");
			WriteLine("\tExisting '" + tableName + "'FKs:");
			WriteLine("");

			foreach (var key in tbl.ForeignKeys.Keys)
				WriteLine("\t" + key);

			WriteLine(" */");

			throw new ArgumentException("FK '" + tableName + "'.'" + fkName + "' not found.");
		}


		public TableContext SetTable(string tableName,
			string TypeName = null,
			string DataContextPropertyName = null)
		{
			var ctx = new TableContext {Transformation = this, TableName = tableName};

			if (TypeName != null || DataContextPropertyName != null)
			{
				var t = GetTable(tableName);

				if (TypeName != null) t.TypeName = TypeName;
				if (DataContextPropertyName != null) t.DataContextPropertyName = DataContextPropertyName;
			}

			return ctx;
		}



		Dictionary<string, Table> Tables = new Dictionary<string, Table>();
		Dictionary<string, Procedure> Procedures = new Dictionary<string, Procedure>();



		static Action<GeneratedTextTransformation, string> WriteComment = (tt, s) => tt.WriteLine("//{0}", s);

		Action BeforeGenerateModel = () => { };

		bool GenerateProcedureErrors = true;

		void GenerateModel()
		{
			Model.SetTree();

			BeforeGenerateModel();

			if (GenerationEnvironment.Length > 0 && GenerationEnvironment.ToString().Trim().Length == 0)
				GenerationEnvironment.Length = 0;

			WriteComment(this,
				"---------------------------------------------------------------------------------------------------");
			WriteComment(this, " <auto-generated>");
			WriteComment(this, "    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).");
			WriteComment(this,
				"    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.");
			WriteComment(this, " </auto-generated>");
			WriteComment(this,
				"---------------------------------------------------------------------------------------------------");

			Model.Render(this);
		}

		void Trim()
		{
			var arr = new[] {'\r', '\n', ' '};
			while (GenerationEnvironment.Length > 0 && arr.Contains(GenerationEnvironment[GenerationEnvironment.Length - 1]))
				GenerationEnvironment.Length--;

			WriteLine("");
		}

// Base data types.
//

		ModelSource Model = new ModelSource();


// Helpers.
//

		Func<string, string> ToPlural = s => s + "s";
		Func<string, string> ToSingular = s => s;

		static string LenDiff(int max, string str)
		{
			var s = "";

			while (max-- > str.Length)
				s += " ";

			return s;
		}

		event Action<Property, string, object> SetPropertyValueAction;

		void SetPropertyValue(Property propertyObject, string propertyName, object value)
		{
			if (SetPropertyValueAction != null)
				SetPropertyValueAction(propertyObject, propertyName, value);
		}



		static class Pluralization
		{
			public static string CultureInfo = "en";

			static PluralizationService _service;

			public static Dictionary<string, string> Dictionary = new Dictionary<string, string>
			{
				{"access", "accesses"},
				{"afterlife", "afterlives"},
				{"alga", "algae"},
				{"alumna", "alumnae"},
				{"alumnus", "alumni"},
				{"analysis", "analyses"},
				{"antenna", "antennae"},
				{"appendix", "appendices"},
				{"axis", "axes"},
				{"bacillus", "bacilli"},
				{"basis", "bases"},
				{"Bedouin", "Bedouin"},
				{"cactus", "cacti"},
				{"calf", "calves"},
				{"cherub", "cherubim"},
				{"child", "children"},
				{"cod", "cod"},
				{"cookie", "cookies"},
				{"criterion", "criteria"},
				{"curriculum", "curricula"},
				{"data", "data"},
				{"deer", "deer"},
				{"diagnosis", "diagnoses"},
				{"die", "dice"},
				{"dormouse", "dormice"},
				{"elf", "elves"},
				{"elk", "elk"},
				{"erratum", "errata"},
				{"esophagus", "esophagi"},
				{"fauna", "faunae"},
				{"fish", "fish"},
				{"flora", "florae"},
				{"focus", "foci"},
				{"foot", "feet"},
				{"formula", "formulae"},
				{"fundus", "fundi"},
				{"fungus", "fungi"},
				{"genie", "genii"},
				{"genus", "genera"},
				{"goose", "geese"},
				{"grouse", "grouse"},
				{"hake", "hake"},
				{"half", "halves"},
				{"headquarters", "headquarters"},
				{"hippo", "hippos"},
				{"hippopotamus", "hippopotami"},
				{"hoof", "hooves"},
				{"housewife", "housewives"},
				{"hypothesis", "hypotheses"},
				{"index", "indices"},
				{"info", "info"},
				{"jackknife", "jackknives"},
				{"knife", "knives"},
				{"labium", "labia"},
				{"larva", "larvae"},
				{"leaf", "leaves"},
				{"life", "lives"},
				{"loaf", "loaves"},
				{"louse", "lice"},
				{"magus", "magi"},
				{"man", "men"},
				{"memorandum", "memoranda"},
				{"midwife", "midwives"},
				{"millennium", "millennia"},
				{"moose", "moose"},
				{"mouse", "mice"},
				{"nebula", "nebulae"},
				{"neurosis", "neuroses"},
				{"nova", "novas"},
				{"nucleus", "nuclei"},
				{"oesophagus", "oesophagi"},
				{"offspring", "offspring"},
				{"ovum", "ova"},
				{"ox", "oxen"},
				{"papyrus", "papyri"},
				{"passerby", "passersby"},
				{"penknife", "penknives"},
				{"person", "people"},
				{"phenomenon", "phenomena"},
				{"placenta", "placentae"},
				{"pocketknife", "pocketknives"},
				{"process", "processes"},
				{"pupa", "pupae"},
				{"radius", "radii"},
				{"reindeer", "reindeer"},
				{"retina", "retinae"},
				{"rhinoceros", "rhinoceros"},
				{"roe", "roe"},
				{"salmon", "salmon"},
				{"scarf", "scarves"},
				{"self", "selves"},
				{"seraph", "seraphim"},
				{"series", "series"},
				{"sheaf", "sheaves"},
				{"sheep", "sheep"},
				{"shelf", "shelves"},
				{"species", "species"},
				{"spectrum", "spectra"},
				{"status", "status"},
				{"stimulus", "stimuli"},
				{"stratum", "strata"},
				{"supernova", "supernovas"},
				{"swine", "swine"},
				{"terminus", "termini"},
				{"thesaurus", "thesauri"},
				{"thesis", "theses"},
				{"thief", "thieves"},
				{"trout", "trout"},
				{"vulva", "vulvae"},
				{"wife", "wives"},
				{"wildebeest", "wildebeest"},
				{"wolf", "wolves"},
				{"woman", "women"},
				{"yen", "yen"},
			};

			static string GetLastWord(string str)
			{
				if (string.IsNullOrWhiteSpace(str))
					return string.Empty;

				var i = str.Length - 1;
				var isLower = char.IsLower(str[i]);

				while (i > 0 && char.IsLower(str[i - 1]) == isLower)
					i--;

				return str.Substring(isLower && i > 0 ? i - 1 : i);
			}

			public static string ToPlural(string str)
			{
				if (_service == null)
					_service = PluralizationService.CreateService(System.Globalization.CultureInfo.GetCultureInfo(CultureInfo));

				var word = GetLastWord(str);

				string newWord;

				if (!Dictionary.TryGetValue(word.ToLower(), out newWord))
					newWord = _service.IsPlural(word) ? word : _service.Pluralize(word);

				if (string.Compare(word, newWord, true) != 0)
				{
					if (char.IsUpper(word[0]))
						newWord = char.ToUpper(newWord[0]) + newWord.Substring(1, newWord.Length - 1);

					return word == str ? newWord : str.Substring(0, str.Length - word.Length) + newWord;
				}

				return str;
			}

			public static string ToSingular(string str)
			{
				if (_service == null)
					_service = PluralizationService.CreateService(System.Globalization.CultureInfo.GetCultureInfo(CultureInfo));

				var word = GetLastWord(str);

				var newWord =
					Dictionary
						.Where(dic => string.Compare(dic.Value, word, true) == 0)
						.Select(dic => dic.Key)
						.FirstOrDefault()
					??
					(_service.IsSingular(word) ? word : _service.Singularize(word));

				if (string.Compare(word, newWord, true) != 0)
				{
					if (char.IsUpper(word[0]))
						newWord = char.ToUpper(newWord[0]) + newWord.Substring(1, newWord.Length - 1);

					return word == str ? newWord : str.Substring(0, str.Length - word.Length) + newWord;
				}

				return str;
			}
		}


	}
}