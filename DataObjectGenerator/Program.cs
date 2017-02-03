using System;
using System.IO;
using System.Linq;
using Std.Utility;
using Std.Utility.Process;

namespace Std.DataTools
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

			var generator = new SqlServerModelGenerator();

			var options = new OptionSet
			{
				{"tables=", "Comma separated list of tables to generate", (string p) => tables = p.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries)},
				{"tablePattern=", "Regular expression to match table names", p => generator.IncludeTablePattern = p},
				{"p|partial", "Generate partial DataObject classes", p => generator.GeneratePartialClasses=true},
				{"nodc|noDataContext", "Do not generate data context class", p => generator.GenerateDataContextClass = false},
				{"dcb=|dataContextBase=", "Base class for the data context", p => generator.BaseDataContextClass = p},
				{"dc=|dataContext=", "Data context class name. Default is 'DataContext'", p => generator.DataContextName = p},
				{"server=", "Database server", p => generator.DatabaseServer = p},
				{"user=", "Database username", p => generator.DatabaseUsername = p},
				{"password=", "Database password", p => generator.DatabasePassword = p},
				{"database=", "Database name", p => generator.DatabaseName = p},
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

			var dataModel = generator.GenerateDataModel();
			if (dataModel == null)
			{
				Console.WriteLine("No data model generated");

				if (generator.Errors.Count != 0)
				{
					foreach (var error in generator.Errors)
					{
						Console.WriteLine(error.ToString());
					}
				}
				return;
			}

			if (generator.GenerateDataContextClass &&
				!dataModel.DataContext.IsNullOrEmpty())
			{
				File.WriteAllText(Path.Combine(dataObjectOutputDirectory, generator.DataContextName + ".cs"), dataModel.DataContext);
			}

			foreach (var model in dataModel.Models)
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
					Console.WriteLine("Invalid model type for model {0}", model.Name);
					continue;
				}

				File.WriteAllText(Path.Combine(outputDirectory, model.Name + ".cs"), model.Content);
			}
		}
	}
}
