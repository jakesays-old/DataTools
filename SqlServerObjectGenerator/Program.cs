using System;
using System.IO;
using System.Linq;
using Std.Tools.Data.CodeModel;
using Std.Tools.Data.Metadata;
using Std.Utility;
using Std.Utility.Process;
// ReSharper disable LocalizableElement

namespace Std.Tools.Data
{
	class Program
	{
		private static void Using(OptionSet options)
		{
			options.WriteOptionDescriptions(Console.Out);
			Console.WriteLine("NOTE: output directories default to current directory if not specified");
			Environment.Exit(1);
		}

		private static void Main(string[] args)
		{
			string[] tables = null;
			string dataObjectOutputDirectory = null;
			string pocoOutputDirectory = null;
			string adapterOutputDirectory = null;
			var showUsing = false;

			string databaseServer = null;
			string databaseUsername = null;
			string databasePassword = null;
			string databaseName = null;
			string connectionString = null;

			var schemaCollector = new SqlServerMetadataCollector();
			var generator = new SqlServerModelGenerator(schemaCollector);

			var options = new OptionSet
			{
				{"tables=", "Comma separated list of tables to generate", (string p) => tables = p.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries)},
				{"tablePattern=", "Regular expression to match table names", p => generator.IncludeTablePattern = p},
				{"p|partial", "Generate partial DataObject classes", p => generator.GeneratePartialClasses=true},
				{"nodc|noDataContext", "Do not generate data context class", p => generator.GenerateDataContextClass = false},
				{"dcb=|dataContextBase=", "Base class for the data context", p => generator.BaseDataContextClass = p},
				{"dc=|dataContext=", "Data context class name. Default is 'DataContext'", p => generator.DataContextName = p},
				{"server=", "Database server", p => databaseServer = p},
				{"user=", "Database username", p => databaseUsername = p},
				{"password=", "Database password", p => databasePassword = p},
				{"database=", "Database name", p => databaseName = p},
				{"connect=", "Database server onnection string", p => connectionString = p},
				{"dons=", "Namespace for generated DataObject classes", p => generator.DataNamespace = p},
				{"pocons=", "Namespace for generated POCO classes", p => generator.PocoNamespace = p},
				{"adapterns=", "Namespace for generated adapter classes (defaults to DataObject ns)", p => generator.AdapterNamespace = p},
				{"dosuffix=", "DataObject class suffix (defaults to 'Data')", p => generator.DataObjectSuffix = p},
				{"pocosuffix=", "POCO class suffix (defaults to no suffix)", p => generator.PocoSuffix = p},
				{"adaptersuffix=", "Adapter class suffix (defaults to 'Adapter')", p => generator.AdapterSuffix = p},
				{"s", "Singularize class names", p => generator.SingularizeClassNames = true},
				{"fk", "Enable generation of foreign key associations (incomplete!)", p => generator.RenderForeignKeys = true},
				{"poco", "Generate POCOs", p => generator.GeneratePocos = true},
				{"adapter", "Generate DataObject <-> POCO adapters", p => generator.GenerateAdapters = true},
				{"do", "Generate DataObjects", p => generator.GenerateDataObjects = true},
				{"dodir=", "Generated DataObject output directory", p => dataObjectOutputDirectory = p},
				{"pocodir=", "Generated POCO output directory", p => pocoOutputDirectory = p},
				{"adapterdir=", "Generated DataObject output directory", p => adapterOutputDirectory = p},
				{"pascal", "Use Pascal casing for type and member names", p => generator.UsePascalCasing = true},
				{"help", "Show Help", _ => showUsing = true}
			};

			var remainingOptions = options.Parse();
			if (showUsing)
			{
				Using(options);
			}

			if (remainingOptions.Count != 0)
			{
				Console.WriteLine("Unrecognized options");
				foreach (var opt in remainingOptions)
				{
					Console.WriteLine(opt);
				}
				Using(options);
			}

			if (dataObjectOutputDirectory.IsNullOrEmpty())
			{
				dataObjectOutputDirectory = Directory.GetCurrentDirectory();
			}

			if (pocoOutputDirectory.IsNullOrEmpty())
			{
				pocoOutputDirectory = Directory.GetCurrentDirectory();
			}

			if (adapterOutputDirectory.IsNullOrEmpty())
			{
				adapterOutputDirectory = Directory.GetCurrentDirectory();
			}
			
			if (!Directory.Exists(dataObjectOutputDirectory))
			{
				Directory.CreateDirectory(dataObjectOutputDirectory);
			}

			if (!Directory.Exists(pocoOutputDirectory))
			{
				Directory.CreateDirectory(pocoOutputDirectory);
			}

			if (!Directory.Exists(adapterOutputDirectory))
			{
				Directory.CreateDirectory(adapterOutputDirectory);
			}

			if (tables != null)
			{
				generator.IncludeTables = tables.ToList();
			}

			if (connectionString.NotNullOrEmpty())
			{
				schemaCollector.LoadMetadata(connectionString);
			}
			else if (databaseServer.NotNullOrEmpty() &&
				databaseName.NotNullOrEmpty() &&
				databasePassword.NotNullOrEmpty() &&
				databaseUsername.NotNullOrEmpty())
			{
				schemaCollector.LoadMetadata(databaseServer, databaseName, databaseUsername, databasePassword);
			}
			else
			{
				Console.WriteLine("Error: no valid database server connection information provided.");
				return;
			}


			var results = generator.Generate(schemaCollector.Model);
			if (results == null)
			{
				Console.WriteLine("No data model generated");
				return;
			}

			if (generator.GenerateDataContextClass &&
				results.DataContextMainPart.NotNullOrEmpty())
			{
				File.WriteAllText(Path.Combine(dataObjectOutputDirectory, generator.DataContextName + ".cs"), results.DataContextMainPart);
				if (results.DataContextProcedurePart.NotNullOrEmpty())
				{
					File.WriteAllText(Path.Combine(dataObjectOutputDirectory, generator.DataContextName + ".Procedures.cs"), results.DataContextProcedurePart);
				}
			}

			foreach (var model in results.Models)
			{
				string outputDirectory = null;
				if (model.Type == CodeType.Adapter)
				{
					outputDirectory = adapterOutputDirectory;
				}
				else if (model.Type == CodeType.DataObject)
				{
					outputDirectory = dataObjectOutputDirectory;
				}
				else if (model.Type == CodeType.Poco)
				{
					outputDirectory = pocoOutputDirectory;
				}
				else
				{
					Console.WriteLine($"Invalid model type for model {model.Name}");
					continue;
				}

				File.WriteAllText(Path.Combine(outputDirectory, model.Name + ".cs"), model.Content);
			}
		}
	}
}
