using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.SchemaProvider;
using LinqToDB.SqlProvider;
using Std.Tools.Data.Metadata.Model;
using Std.Utility;
using Std.Utility.Linq;
using AssociationType = Std.Tools.Data.Metadata.Model.AssociationType;
using Type = System.Type;
using Attribute = Std.Tools.Data.Metadata.Model.Attribute;

namespace Std.Tools.Data.Metadata
{
	public class SqlServerMetadataCollector : ISqlBuilderProvider
	{
		private readonly GetSchemaOptions _getSchemaOptions =
			new GetSchemaOptions();

		private readonly SchemaModel _model = new SchemaModel();
		private readonly Action AfterGenerateLinqToDBModel = () => { };

		private readonly Action BeforeGenerateLinqToDBModel = () => { };

		private readonly Func<Table, MemberBase> GenerateProviderSpecificTable = t => null;

		private readonly Func<string, string> ToPlural = s => s + "s";
		private readonly Func<string, string> ToSingular = s => s;

		public ISqlBuilder SqlBuilder { get; private set; }
		public Dictionary<string, Table> Tables { get; } = new Dictionary<string, Table>();
		public Dictionary<string, Procedure> Procedures { get; } = new Dictionary<string, Procedure>();

		public string DataContextName { get; set; }
		public string BaseDataContextClass { get; set; }
		public string BaseEntityClass { get; set; }
		private bool ReplaceSimilarTables { get; } = true;
		private bool IncludeDefaultSchema { get; } = true;
		public Class DataContextObject { get; set; }
		//public bool PluralizeClassNames { get; set; }
		//public bool SingularizeClassNames { get; set; } = true;
		//public bool PluralizeDataContextPropertyNames { get; set; } = true;
		//public bool SingularizeDataContextPropertyNames { get; set; }

		public bool GenerateSchemaAsType { get; set; } = true;

		public bool GenerateObsoleteAttributeForAliases { get; set; } = false;
		public bool GenerateFindExtensions { get; set; } = true;
		public bool IsCompactColumns { get; set; } = true;
		public bool IsCompactColumnAliases { get; set; } = true;
		public bool GenerateDataTypes { get; set; } = false;
		public bool? GenerateLengthProperty { get; set; } = null;
		public bool? GeneratePrecisionProperty { get; set; } = null;
		public bool? GenerateScaleProperty { get; set; } = null;
		public bool GenerateDbTypes { get; set; } = false;
		public bool GenerateViews { get; set; } = true;
		public string SchemaNameSuffix { get; set; } = "Schema";
		public string SchemaDataContextTypeName { get; set; } = "DataContext";

		public string OneToManyAssociationType { get; set; } = "List<{0}>";

		public bool GenerateConstructors { get; set; } = true;
		public string DefaultConfiguration { get; set; } = null;
		public bool GenerateAssociations { get; set; } = true;
		public bool GenerateBackReferences { get; set; } = true;

		public SchemaModel Model { get; } = new SchemaModel();

		public string NamespaceName { get; set; } = "DataContext";
		public string DatabaseName { get; set; }
		public bool GenerateDatabaseName { get; set; } = true;

		public Dictionary<string, string> SchemaNameMapping { get; } = new Dictionary<string, string>();

		public Func<TableSchema, Table> LoadProviderSpecificTable { get; set; } = tableSchema => null;

		public SqlServerMetadataCollector()
		{
		}

		public void LoadMetadata(string connectionString)
		{
			var dataConnection = GetSqlServerConnection(connectionString);
			LoadMetadata(dataConnection);
		}

		public void LoadMetadata(string server,
		                         string database)
		{
			var dataConnection = GetSqlServerConnection(server, database);
			LoadMetadata(dataConnection);
		}

		public void LoadMetadata(string server,
		                         string database,
		                         string user,
		                         string password)
		{
			var dataConnection = GetSqlServerConnection(server, database, user, password);
			LoadMetadata(dataConnection);
		}

		public void LoadMetadata(DataConnection dataConnection)
		{
			if (DataContextObject == null)
			{
				DataContextObject = new Class(DataContextName)
				{
					BaseClass = BaseDataContextClass
				};

				_model.Types.Add(DataContextObject);
			}

			LoadServerMetadata(dataConnection);

			if (Tables.Values.SelectMany(_ => _.ForeignKeys.Values).Any(_ => _.AssociationType == AssociationType.OneToMany))
			{
				_model.Usings.Add("System.Collections.Generic");
			}

			foreach (var t in Tables.Values)
			{
				if (KeywordList.KeyWords.Contains(t.TypeName))
				{
					t.TypeName = "@" + t.TypeName;
				}

				if (KeywordList.KeyWords.Contains(t.DataContextPropertyName))
				{
					t.DataContextPropertyName = "@" + t.DataContextPropertyName;
				}

				t.TypeName = ConvertToCompilable(t.TypeName);
				t.DataContextPropertyName = ConvertToCompilable(t.DataContextPropertyName);

				foreach (var col in t.Columns.Values)
				{
					if (KeywordList.KeyWords.Contains(col.MemberName))
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
		}

		private DataConnection GetSqlServerConnection(string connectionString)
		{
			return SqlServerTools.CreateDataConnection(connectionString);
		}

		private DataConnection GetSqlServerConnection(string server,
		                                              string database)
		{
			return
				GetSqlServerConnection($"Data Source={server};Database={database};Integrated Security=SSPI");
		}

		private DataConnection GetSqlServerConnection(string server,
		                                              string database,
		                                              string user,
		                                              string password)
		{
			return
				GetSqlServerConnection($"Server={server};Database={database};User Id={user};Password={password};");
		}

		private void LoadServerMetadata(DataConnection dataConnection)
		{
			SqlBuilder = dataConnection.DataProvider.CreateSqlBuilder();

			var schemaProvider = dataConnection.DataProvider.GetSchemaProvider();
			var dbSchema = schemaProvider.GetSchema(dataConnection, _getSchemaOptions);

			if (DataContextName == null)
			{
				DataContextObject.Name = DataContextName = dbSchema.Database + "DB";
			}

			DataContextObject.Comment.Add("/ <summary>");
			DataContextObject.Comment.Add("/ Database       : " + dbSchema.Database);
			DataContextObject.Comment.Add("/ Data Source    : " + dbSchema.DataSource);
			DataContextObject.Comment.Add("/ Server Version : " + dbSchema.ServerVersion);
			DataContextObject.Comment.Add("/ </summary>");

			var schemaTables = dbSchema.Tables
				.DistinctBy(t => t.TableName)
				.ToList();

			var tables = schemaTables
				.Where(table => !table.IsProviderSpecific)
				.Select(schema => new
				        {
					        t = schema,
					        key = schema.IsDefaultSchema
						        ? schema.TableName
						        : schema.SchemaName + "." + schema.TableName,
					        table = MakeTable(schema)
				        })
				.ToList();

			tables.AddRange(schemaTables
								.Where(t => t.IsProviderSpecific)
				                .Select(t => new
				                        {
					                        t,
					                        key = t.IsDefaultSchema
						                        ? t.TableName
						                        : t.SchemaName + "." + t.TableName,
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
					key.k.OtherTable.IsDefaultSchema
						? key.KeyName
						: key.k.OtherTable.SchemaName + "." + key.KeyName,
					key.foreignKey);

				if (key.k.BackReference != null)
				{
					key.foreignKey.BackReference = keys.First(k => k.k == key.k.BackReference).foreignKey;
				}

				key.foreignKey.MemberName = key.foreignKey.MemberName.Replace(".", string.Empty);

				key.foreignKey.MemberName = key.foreignKey.AssociationType == AssociationType.OneToMany
					? ToPlural(key.foreignKey.MemberName)
					: ToSingular(key.foreignKey.MemberName);
			}

			var procedures = dbSchema.Procedures
				.Select(delegate(ProcedureSchema procSchema)
				        {
					        return new
					        {
						        key =
						        procSchema.IsDefaultSchema
							        ? procSchema.ProcedureName
							        : procSchema.SchemaName + "." + procSchema.ProcedureName,
						        proc = new Procedure
						        {
									Schema = procSchema,
							        SchemaName =
								        (procSchema.IsDefaultSchema && !IncludeDefaultSchema) ||
								        string.IsNullOrEmpty(procSchema.SchemaName)
									        ? null
									        : procSchema.SchemaName,
							        ProcedureName = procSchema.ProcedureName,
									MethodName = procSchema.ProcedureName.ToPascalCase(),
									Name = procSchema.MemberName,
							        IsFunction = procSchema.IsFunction,
							        IsTableFunction = procSchema.IsTableFunction,
							        IsDefaultSchema = procSchema.IsDefaultSchema,
							        IsLoaded = procSchema.IsLoaded,
							        ResultTable = procSchema.ResultTable == null
								        ? null
								        : MakeResultTable(procSchema),
							        ResultException = procSchema.ResultException,
							        SimilarTables = procSchema.SimilarTables == null
								        ? new List<Table>()
								        : procSchema.SimilarTables
									        .Select(t => tables.Single(tbl => tbl.t == t).table)
									        .ToList(),
							        ProcParameters = procSchema.Parameters
								        .Select(pr => new Parameter
								                {
									                SchemaName = pr.SchemaName,
									                SchemaType = pr.SchemaType,
									                IsIn = pr.IsIn,
									                IsOut = pr.IsOut,
									                IsResult = pr.IsResult,
									                Size = pr.Size,
									                ParameterName =
										                CheckParameterName(pr.ParameterName),
									                ParameterType = pr.ParameterType,
									                SystemType = pr.SystemType,
									                DataType = pr.DataType.ToString(),
								                })
								        .ToList(),
						        }
					        };
				        })
				.ToList();

			foreach (var p in procedures)
			{
				if (ReplaceSimilarTables)
				{
					if (p.proc.SimilarTables.Count == 1 ||
						p.proc.SimilarTables.Count(t => !t.IsView) == 1)
					{
						p.proc.ResultTable = p.proc.SimilarTables.Count() == 1
							? p.proc.SimilarTables[0]
							: p.proc.SimilarTables.First(t => !t.IsView);
					}
				}

				Procedures[p.key] = p.proc;
			}

			GenerateTypesFromMetadata();
		}

		private Table MakeResultTable(ProcedureSchema procSchema)
		{
			var columns = MakeResultColumns(procSchema);

			return new Table(procSchema.ResultTable.TypeName)
			{
				TableSchema = procSchema.ResultTable,
				TableName = procSchema.ResultTable.TypeName,
				TypeName = procSchema.ResultTable.TypeName,
				Columns = columns.Unordered,
				OrderedColumns = columns.Ordered
			};
		}

		private DictResults<Column> MakeResultColumns(ProcedureSchema procSchema)
		{
			return ToDictionary(
				procSchema.ResultTable.Columns,
				columnSchema => columnSchema.ColumnName,
				columnSchema => new Column
				{
					ColumnSchema = columnSchema,
					ColumnName = columnSchema.ColumnName,
					ColumnType = columnSchema.ColumnType,
					IsNullable = columnSchema.IsNullable,
					IsIdentity = columnSchema.IsIdentity,
					IsPrimaryKey = columnSchema.IsPrimaryKey,
					PrimaryKeyOrder = columnSchema.PrimaryKeyOrder,
					MemberName =
						CheckColumnName(CheckType(columnSchema.SystemType, columnSchema.MemberName)),
					Type = columnSchema.MemberType,
					SkipOnInsert = columnSchema.SkipOnInsert,
					SkipOnUpdate = columnSchema.SkipOnUpdate,
					Description = columnSchema.Description,
				},
				(c,
				 n) =>
				{
					c.IsDuplicateOrEmpty = true;
					return "$" + (c.MemberName = "Column" + n);
				});
		}

		private Table MakeTable(TableSchema schema)
		{
			var orderedColumns = schema.Columns.Select(MakeColumn).ToList();

			var table = new Table(schema.TableName)
			{
				TableSchema = schema,
				Schema =
					(schema.IsDefaultSchema && !IncludeDefaultSchema) ||
					string.IsNullOrEmpty(schema.SchemaName)
						? null
						: schema.SchemaName,
				BaseClass = BaseEntityClass,
				TableName = schema.TableName,
				TypeName = schema.TypeName,
				DataContextPropertyName = schema.TypeName,
				IsView = schema.IsView,
				IsProviderSpecific = false,
				Description = schema.Description,
				OrderedColumns = orderedColumns,
				Columns = orderedColumns.ToDictionary(
					c => c.ColumnName,
					c => c)
			};

			return table;
		}

		private Column MakeColumn(ColumnSchema colSchema)
		{
			return new Column
			{
				ColumnName = colSchema.ColumnName,
				ColumnType = colSchema.ColumnType,
				DataType = "DataType." + colSchema.DataType,
				Length = colSchema.Length,
				Precision = colSchema.Precision,
				Scale = colSchema.Scale,
				IsNullable = colSchema.IsNullable,
				IsIdentity = colSchema.IsIdentity,
				IsPrimaryKey = colSchema.IsPrimaryKey,
				PrimaryKeyOrder = colSchema.PrimaryKeyOrder,
				MemberName =
					CheckType(colSchema.SystemType, colSchema.MemberName),
				Type = colSchema.MemberType,
				SkipOnInsert = colSchema.SkipOnInsert,
				SkipOnUpdate = colSchema.SkipOnUpdate,
				Description = colSchema.Description,
			};
		}

		private event Action<Property, string, object> SetPropertyValueAction;

		private void SetPropertyValue(Property propertyObject,
		                              string propertyName,
		                              object value)
		{
			if (SetPropertyValueAction != null)
			{
				SetPropertyValueAction(propertyObject, propertyName, value);
			}
		}

		private void GenerateTypesFromMetadata()
		{
			BeforeGenerateLinqToDBModel();

			Model.Usings.Add("LinqToDB");
			Model.Usings.Add("LinqToDB.Mapping");

			if (NamespaceName == null)
			{
				NamespaceName = "DataModel";
			}

			var schemas = CollectSchemaInfo();

			var defProps = new MemberGroup {IsCompact = true};
			var defAliases = new MemberGroup {IsCompact = true, Region = "Alias members"};
			var defTableExtensions = new MemberGroup {};

			MakeConstructors(schemas);

			if (Tables.Count > 0)
			{
				DataContextObject.Members.Insert(0, defProps);
			}

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

			foreach (var table in Tables.Values.OrderBy(tbl => tbl.IsProviderSpecific).ThenBy(tbl => tbl.TypeName))
			{
				Action<Class> addType = tp => Model.Types.Add(tp);
				var props = defProps;
				var aliases = defAliases;
				var tableExtensions = defTableExtensions;

				if (table.IsView &&
					!GenerateViews)
				{
					continue;
				}

				var schema = table.Schema != null && schemas.ContainsKey(table.Schema)
					? schemas[table.Schema]
					: null;

				if (schema != null)
				{
					var si = schemas[table.Schema];

					addType = tp => si.Type.Members.Add(tp);
					props = si.Props;
					aliases = si.Aliases;
					tableExtensions = si.TableExtensions;
				}

				var dcProp = table.IsProviderSpecific
					? GenerateProviderSpecificTable(table)
					: new Property(
						string.Format("ITable<{0}>", table.TypeName),
						table.DataContextPropertyName,
						new[]
						{
							string.Format((schema == null
								              ? "this"
								              : "_dataContext") + ".GetTable<{0}>()", table.TypeName)
						},
						null);

				if (dcProp == null)
				{
					continue;
				}

				table.DataContextProperty = dcProp;

				props.Members.Add(dcProp);

				Property aProp = null;

				if (table.AliasPropertyName != null &&
					table.AliasPropertyName != table.DataContextPropertyName)
				{
					aProp = new Property(
						$"ITable<{table.TypeName}>",
						table.AliasPropertyName,
						new[] {table.DataContextPropertyName});

					if (GenerateObsoleteAttributeForAliases)
					{
						aProp.Attributes.Add(new Attribute("Obsolete", "\"Use " + table.DataContextPropertyName + " instead.\""));
					}

					aliases.Members.Add(aProp);
				}

				var tableAttrs = new List<string>();

				tableAttrs.Add("Name=" + '"' + table.OriginalTableName + '"');
				if (DatabaseName != null)
				{
					tableAttrs.Add("Database=" + '"' + DatabaseName + '"');
				}
				if (table.Schema != null)
				{
					tableAttrs.Add("Schema=" + '"' + table.Schema + '"');
				}

				if (table.IsView)
				{
					tableAttrs.Add("IsView=true");
				}

				table.Attributes.Add(new Attribute("Table", tableAttrs.ToArray()) {IsSeparated = true});

				var comments = new List<string>();

				if (!string.IsNullOrWhiteSpace(table.Description))
				{
					comments.Add("/ <summary>");
					foreach (var line in table.Description.Split('\n'))
					{
						comments.Add("/ " + line.TrimEnd());
					}

					comments.Add("/ </summary>");
				}

				if (comments.Count > 0)
				{
					table.Comment.AddRange(comments);
					dcProp.Comment.AddRange(comments);

					if (aProp != null)
					{
						aProp.Comment.AddRange(comments);
					}
				}

				MakeColumns(table);

				MakeAssociations(table);

				addType(table);

				if (!string.IsNullOrWhiteSpace(table.AliasTypeName))
				{
					var aClass = new Class(table.AliasTypeName)
					{
						BaseClass = table.TypeName
					};

					if (comments.Count > 0)
					{
						aClass.Comment.AddRange(comments);
					}

					if (GenerateObsoleteAttributeForAliases)
					{
						aClass.Attributes.Add(new Attribute("Obsolete", "\"Use " + table.TypeName + " instead.\""));
					}

					Model.Types.Add(aClass);
				}
			}

			if (defAliases.Members.Count > 0)
			{
				DataContextObject.Members.Add(defAliases);
			}

			foreach (var schema in schemas.Values)
			{
				if (schema.Aliases.Members.Count > 0)
				{
					schema.Type.Members.Add(defAliases);
				}
			}

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

					var schema = p.SchemaName != null && schemas.ContainsKey(p.SchemaName)
						? schemas[p.SchemaName]
						: null;

					if (schema != null)
					{
						var si = schemas[p.SchemaName];

						addProcs = tp => si.Procedures.Members.Add(tp);
						addFuncs = tp => si.Functions.Members.Add(tp);
						addTabfs = tp => si.TableFunctions.Members.Add(tp);
						thisDataContext = "_dataContext";
					}

					var proc = new MemberGroup {Region = p.Name};

					if (!p.IsFunction)
					{
						addProcs(proc);
					}
					else if (p.IsTableFunction)
					{
						addTabfs(proc);
					}
					else
					{
						addFuncs(proc);
					}

					if (p.ResultException != null)
					{
						proc.Errors.Add(p.ResultException.Message);
						continue;
					}

					proc.Members.Add(p);

					if (p.IsTableFunction)
					{
						var attr = new Attribute("Sql.TableFunction");

						//if (DatabaseName != null)
						//{
						//	attr.Parameters.Add($"Database=\"{DatabaseName}\"");
						//}
						if (p.Schema != null)
						{
							attr.Parameters.Add($"Schema=\"{p.Schema}\"");
						}

						attr.Parameters.Add($"Name=\"{p.ProcedureName}\"");

						p.Attributes.Add(attr);

						p.Type = "ITable<" + p.ResultTable.TypeName + ">";
					}
					else if (p.IsFunction)
					{
						p.IsStatic = true;
						p.Type = p.ProcParameters.Single(pr => pr.IsResult).ParameterType;
						p.Attributes.Add(new Attribute("Sql.Function", "Name=\"" + p.SchemaName + "." + p.ProcedureName + "\"",
						                               "ServerSideOnly=true"));
					}
					else
					{
						p.IsStatic = true;
						p.Type = p.ResultTable == null
							? "int"
							: "IEnumerable<" + p.ResultTable.TypeName + ">";
						p.Parameters.Add("this DataConnection dataConnection");
					}

					foreach (var pr in p.ProcParameters.Where(par => !par.IsResult))
					{
						p.Parameters.Add($"{(pr.IsOut ? pr.IsIn ? "ref " : "out " : "")}{pr.ParameterType} {pr.ParameterName}");
					}

					if (p.IsTableFunction)
					{
						var body =
							string.Format("return " + thisDataContext + ".GetTable<{0}>(this, (MethodInfo)MethodBase.GetCurrentMethod()",
							              p.ResultTable.TypeName);

						body += p.ProcParameters.Count == 0
							? ");"
							: ",";

						p.Body.Add(body);

						for (var i = 0; i < p.ProcParameters.Count; i++)
						{
							p.Body.Add("\t" + p.ProcParameters[i].ParameterName + (i + 1 == p.ProcParameters.Count
								           ? ");"
								           : ","));
						}
					}
					else if (p.IsFunction)
					{
						p.Body.Add("throw new InvalidOperationException();");
					}
					else
					{
						var spName =
							SqlBuilder.BuildTableName(
								new StringBuilder(),
								(string) SqlBuilder.Convert(DatabaseName, ConvertType.NameToDatabase),
								(string) SqlBuilder.Convert(p.SchemaName, ConvertType.NameToOwner),
								(string) SqlBuilder.Convert(p.ProcedureName, ConvertType.NameToQueryTable)
							).ToString();

						spName = "\"" + spName.Replace("\"", "\\\"") + "\"";

						var inputParameters = p.ProcParameters.Where(pp => pp.IsIn).ToList();
						var outputParameters = p.ProcParameters.Where(pp => pp.IsOut).ToList();

						spName += inputParameters.Count == 0
							? ");"
							: ",";

						var retName = "ret";
						var retNo = 0;

						while (p.ProcParameters.Any(pp => pp.ParameterName == retName))
						{
							retName = "ret" + ++retNo;
						}

						var hasOut = outputParameters.Any(pr => pr.IsOut);
						var prefix = hasOut
							? "var " + retName + " = "
							: "return ";

						if (p.ResultTable == null)
						{
							p.Body.Add(prefix + "dataConnection.ExecuteProc(" + spName);
						}
						else
						{
							if (p.ResultTable.Columns.Values.Any(c => c.IsDuplicateOrEmpty))
							{
								p.Body.Add("var ms = dataConnection.MappingSchema;");
								p.Body.Add("");
								p.Body.Add($"{prefix}dataConnection.QueryProc(dataReader =>");
								p.Body.Add($"\tnew {p.ResultTable.TypeName}");
								p.Body.Add("\t{");

								var n = 0;

								foreach (var c in p.ResultTable.Columns.Values)
								{
									p.Body.Add($"\t\t{c.MemberName} = Converter.ChangeTypeTo<{c.Type}>(dataReader.GetValue({n++}), ms),");
								}

								p.Body.Add("\t},");
								p.Body.Add("\t" + spName);
							}
							else
							{
								p.Body.Add($"{prefix}dataConnection.QueryProc<{p.ResultTable.TypeName}>({spName}");
							}
						}

						for (var i = 0; i < inputParameters.Count; i++)
						{
							var pr = inputParameters[i];

							var str = $"\tnew DataParameter(\"{pr.SchemaName}\", {pr.ParameterName}, {"DataType." + pr.DataType})";

							if (pr.IsOut)
							{
								str += $" {{ Direction = {(pr.IsIn ? "ParameterDirection.InputOutput" : "ParameterDirection.Output")}";

								if (pr.Size != null &&
									pr.Size.Value != 0)
								{
									str += ", Size = " + pr.Size.Value;
								}

								str += " }";
							}

							str += i + 1 == inputParameters.Count
								? ");"
								: ",";

							p.Body.Add(str);
						}

						if (hasOut)
						{
							p.Body.Add("");

							foreach (var pr in p.ProcParameters.Where(_ => _.IsOut))
							{
								var str =
									$"{pr.ParameterName} = Converter.ChangeTypeTo<{pr.ParameterType}>(((IDbDataParameter)dataConnection.Command.Parameters[\"{pr.SchemaName}\"]).Value);";

								p.Body.Add(str);
							}

							p.Body.Add("");
							p.Body.Add($"return {retName};");
						}
					}

					if (p.ResultTable != null &&
						p.ResultTable.DataContextPropertyName == null)
					{
						var columns = new MemberGroup {IsCompact = true};

						foreach (var c in p.ResultTable.Columns.Values)
						{
							if (c.MemberName != c.ColumnName)
							{
								c.Attributes.Add(new Attribute("Column") {Parameters = {'"' + c.ColumnName + '"'}});
							}
							columns.Members.Add(c);
						}

						p.ResultTable.Members.Add(columns);
						proc.Members.Add(p.ResultTable);
					}
				}

				if (procs.Members.Count > 0)
				{
					Model.Types.Add(new Class(DataContextObject.Name + "StoredProcedures", procs) {IsStatic = true});
				}

				if (funcs.Members.Count > 0)
				{
					Model.Types.Add(new Class("SqlFunctions", funcs) {IsStatic = true});
				}

				if (tabfs.Members.Count > 0)
				{
					DataContextObject.Members.Add(tabfs);
				}

				foreach (var schema in schemas.Values)
				{
					if (schema.Procedures.Members.Count > 0)
					{
						schema.Type.Members.Add(new Class(DataContextObject.Name + "StoredProcedures", schema.Procedures)
						                        {
							                        IsStatic = true
						                        });
					}

					if (schema.Functions.Members.Count > 0)
					{
						schema.Type.Members.Add(new Class("SqlFunctions", schema.Functions) {IsStatic = true});
					}

					if (schema.TableFunctions.Members.Count > 0)
					{
						schema.DataContext.Members.Add(schema.TableFunctions);
					}
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

//			Tables.Clear();
//			Procedures.Clear();

			Model.SetParent();

			AfterGenerateLinqToDBModel();
		}

		private void MakeAssociations(Table table)
		{
			if (GenerateAssociations)
			{
				var keys = table.ForeignKeys.Values.ToList();

				if (!GenerateBackReferences)
				{
					keys = keys.Where(k => k.BackReference != null).ToList();
				}

				if (keys.Count > 0)
				{
					var associations = new MemberGroup {Region = "Associations"};

					foreach (var key in keys)
					{
						key.Comment.Add("/ <summary>");
						key.Comment.Add("/ " + key.KeyName);
						key.Comment.Add("/ </summary>");

						if (key.AssociationType == AssociationType.OneToMany)
						{
							key.Type = string.Format(OneToManyAssociationType, key.OtherTable.TypePrefix + key.OtherTable.TypeName);
						}
						else
						{
							key.Type = key.OtherTable.TypePrefix + key.OtherTable.TypeName;
						}

						var aa = new Attribute("Association");

						aa.Parameters.Add("ThisKey=\"" + string.Join(", ", (from c in key.ThisColumns
							                                             select c.MemberName).ToArray()) + "\"");
						aa.Parameters.Add("OtherKey=\"" + string.Join(", ", (from c in key.OtherColumns
							                                              select c.MemberName).ToArray()) + "\"");
						aa.Parameters.Add("CanBeNull=" + (key.CanBeNull
							                  ? "true"
							                  : "false"));

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
							{
								aa.Parameters.Add("KeyName=\"" + key.KeyName + "\"");
							}
							if (GenerateBackReferences && !string.IsNullOrEmpty(key.BackReference.KeyName))
							{
								aa.Parameters.Add("BackReferenceName=\"" + key.BackReference.MemberName + "\"");
							}
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

					table.Members.Add(associations);
				}
			}
		}

		private void MakeColumns(Table table)
		{
			var columns = new MemberGroup {IsCompact = IsCompactColumns};
			var columnAliases = new MemberGroup {IsCompact = IsCompactColumnAliases, Region = "Alias members"};
			var nPKs = table.Columns.Values.Count(c => c.IsPrimaryKey);
			var allNullable = table.Columns.Values.All(c => c.IsNullable || c.IsIdentity);
			var nameMaxLen = table.Columns.Values.Max(c => (int?) (c.MemberName == c.ColumnName
				                                          ? 0
				                                          : c.ColumnName.Length)) ?? 0;
			var dbTypeMaxLen = table.Columns.Values.Max(c => (int?) c.ColumnType.Length) ?? 0;
			var dataTypeMaxLen = table.Columns.Values.Where(c => c.DataType != null).Max(c => (int?) c.DataType.Length) ?? 0;
			var dataTypePrefix = table.Columns.Values.Any(c => c.MemberName == "DataType")
				? "LinqToDB."
				: "";

			foreach (var column in table.Columns.Values)
			{
				// Column.
				//
				var ca = new Attribute("Column");
				var canBeReplaced = true;

				if (column.MemberName != column.ColumnName)
				{
					var space = new string(' ', nameMaxLen - column.ColumnName.Length);

					ca.Parameters.Add('"' + column.ColumnName + '"' + space);
					canBeReplaced = false;
				}
				else if (nameMaxLen > 0)
				{
					ca.Parameters.Add(new string(' ', nameMaxLen + 2));
					canBeReplaced = false;
				}

				if (GenerateDbTypes)
				{
					var space = new string(' ', dbTypeMaxLen - column.ColumnType.Length);

					ca.Parameters.Add("DbType=\"" + column.ColumnType + '"' + space);
					canBeReplaced = false;
				}

				if (GenerateDataTypes)
				{
					var space = new string(' ', dataTypeMaxLen - column.DataType.Length);
					ca.Parameters.Add("DataType=" + dataTypePrefix + column.DataType + space);
					canBeReplaced = false;
				}

				if (GenerateDataTypes && !GenerateLengthProperty.HasValue ||
					GenerateLengthProperty == true)
				{
					if (column.Length != null)
					{
						ca.Parameters.Add("Length=" + (column.Length == int.MaxValue
							                  ? "int.MaxValue"
							                  : column.Length.ToString()));
					}
					canBeReplaced = false;
				}

				if (GenerateDataTypes && !GeneratePrecisionProperty.HasValue ||
					GeneratePrecisionProperty == true)
				{
					if (column.Precision != null)
					{
						ca.Parameters.Add("Precision=" + column.Precision);
					}
					canBeReplaced = false;
				}

				if (GenerateDataTypes && !GenerateScaleProperty.HasValue ||
					GenerateScaleProperty == true)
				{
					if (column.Scale != null)
					{
						ca.Parameters.Add("Scale=" + column.Scale);
					}
					canBeReplaced = false;
				}

				if (column.SkipOnInsert &&
					!column.IsIdentity)
				{
					ca.Parameters.Add("SkipOnInsert=true");
					canBeReplaced = false;
				}

				if (column.SkipOnUpdate &&
					!column.IsIdentity)
				{
					ca.Parameters.Add("SkipOnUpdate=true");
					canBeReplaced = false;
				}

				if (column.IsDiscriminator)
				{
					ca.Parameters.Add("IsDiscriminator=true");
					canBeReplaced = false;
				}

				column.Attributes.Add(ca);

				// PK.
				//
				if (column.IsPrimaryKey)
				{
					var pka = new Attribute("PrimaryKey");

					if (nPKs > 1)
					{
						pka.Parameters.Add(column.PrimaryKeyOrder.ToString());
					}

					if (canBeReplaced)
					{
						column.Attributes[0] = pka;
					}
					else
					{
						column.Attributes.Add(pka);
					}

					canBeReplaced = false;
				}

				// Identity.
				//
				if (column.IsIdentity)
				{
					var ida = new Attribute("Identity");

					if (canBeReplaced)
					{
						column.Attributes[0] = ida;
					}
					else
					{
						column.Attributes.Add(ida);
					}

					canBeReplaced = false;
				}

				// Nullable.
				//
				if (column.IsNullable)
				{
					column.Attributes.Add(new Attribute("Nullable"));
				}
				else if (!column.IsIdentity)
				{
					column.Attributes.Add(new Attribute("NotNull"));
				}

				var columnComments = new List<string>();

				if (!string.IsNullOrWhiteSpace(column.Description))
				{
					columnComments.Add("/ <summary>");
					foreach (var line in column.Description.Split('\n'))
					{
						columnComments.Add("/ " + line.TrimEnd());
					}

					columnComments.Add("/ </summary>");
				}

				if (columnComments.Count > 0)
				{
					column.Comment.AddRange(columnComments);
				}

				// End line comment.
				//
				column.EndLineComment = column.ColumnType;

				SetPropertyValue(column, "IsNotifying", true);
				SetPropertyValue(column, "IsEditable", true);

				columns.Members.Add(column);

				// Alias.
				//
				if (column.AliasName != null &&
					column.AliasName != column.MemberName)
				{
					var caProp = new Property(
						column.Type,
						column.AliasName,
						new[] {column.MemberName},
						new[] {column.MemberName + " = value;"});

					caProp.Comment.AddRange(columnComments);

					if (GenerateObsoleteAttributeForAliases)
					{
						caProp.Attributes.Add(new Attribute("Obsolete", "\"Use " + column.MemberName + " instead.\""));
					}

					caProp.Attributes.Add(new Attribute("ColumnAlias", "\"" + column.MemberName + "\""));

					columnAliases.Members.Add(caProp);
				}
			}

			table.Members.Add(columns);

			if (columnAliases.Members.Count > 0)
			{
				table.Members.Add(columnAliases);
			}
		}

		private void MakeConstructors(Dictionary<string, SchemaInfo> schemas)
		{
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
				{
					DataContextObject.Members.Add(new Method(null, DataContextObject.Name, new string[0], body));
				}
				else
				{
					DataContextObject.Members.Add(new Method(null, DataContextObject.Name, new string[0], body)
					                              {
						                              AfterSignature = {": base(\"" + DefaultConfiguration + "\")"}
					                              });
				}
				DataContextObject.Members.Add(new Method(null, DataContextObject.Name, new[] {"string configuration"}, body)
				                              {
					                              AfterSignature = {": base(configuration)"}
				                              });

				DataContextObject.Members.Add(new MemberGroup
				                              {
					                              IsCompact = true,
					                              Members =
					                              {
						                              new Method("void", "InitDataContext") {AccessModifier = AccessModifier.Partial}
					                              }
				                              });
			}
		}

		private Dictionary<string, SchemaInfo> CollectSchemaInfo()
		{
			string schemaName;

			var schemas =
			(
				from t in Tables.Values
				where GenerateSchemaAsType && t.Schema != null && !t.TableSchema.IsDefaultSchema
				group t by t.Schema
				into gr
				let typeName = SchemaNameMapping.TryGetValue(gr.Key, out schemaName)
					? schemaName
					: gr.Key
				select new SchemaInfo
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

			foreach (var schema in schemas.Values)
			{
				schema.Type.Members.Add(schema.DataContext);
				schema.DataContext.Members.Insert(0, schema.Props);

				schema.DataContext.Members.Add(new Field("IDataContext", "_dataContext")
				{
					AccessModifier = AccessModifier.Private,
					IsReadonly = true
				});
				schema.DataContext.Members.Add(new Method(null, schema.DataContext.Name, new[] { "IDataContext dataContext" },
														  new[] { "_dataContext = dataContext;" }));

				foreach (var t in schema.Tables)
				{
					t.TypePrefix = schema.TypeName + ".";
				}
			}

			return schemas;
		}

		class DictResults<TElement>
		{
			public List<TElement> Ordered;
			public Dictionary<string, TElement> Unordered;
		}

		private DictResults<TR> ToDictionary<T, TR>(IEnumerable<T> source,
		                                                   Func<T, string> keyGetter,
		                                                   Func<T, TR> objGetter,
		                                                   Func<TR, int, string> getKeyName)
		{
			var dict = new Dictionary<string, TR>();
			var list = new List<TR>();

			var current = 1;

			foreach (var item in source)
			{
				var key = keyGetter(item);
				var obj = objGetter(item);

				if (string.IsNullOrEmpty(key) ||
					dict.ContainsKey(key))
				{
					key = getKeyName(obj, current);
				}

				list.Add(obj);
				dict.Add(key, obj);

				current++;
			}

			return new DictResults<TR> {Ordered = list, Unordered = dict};
		}

		private string CheckType(Type type,
		                         string typeName)
		{
			if (!_model.Usings.Contains(type.Namespace))
			{
				_model.Usings.Add(type.Namespace);
			}
			return typeName;
		}

		private string CheckColumnName(string memberName)
		{
			if (string.IsNullOrEmpty(memberName))
			{
				memberName = "Empty";
			}
			else
			{
				{
					memberName = memberName.Replace("%", "Percent");
				}
			}
			return memberName;
		}

		private string CheckParameterName(string parameterName)
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

		private string ConvertToCompilable(string name)
		{
			var query =
				from c in name
				select char.IsLetterOrDigit(c) || c == '@'
					? c
					: '_';

			return new string(query.ToArray());
		}

		private class SchemaInfo
		{
			public MemberGroup Aliases;
			public Class DataContext;
			public MemberGroup Functions;
			public string Name;
			public MemberGroup Procedures;
			public string PropertyName;
			public MemberGroup Props;
			public MemberGroup TableExtensions;
			public MemberGroup TableFunctions;
			public List<Table> Tables;
			public Class Type;
			public string TypeName;
		}
	}
}