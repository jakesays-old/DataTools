using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Std.Utility.Resources
{
	public static class ManifestResourceManager
	{
		public static string[] GetResourceNames(string targetAssemblyPath)
		{
			if (targetAssemblyPath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("targetAssemblyPath");
			}

			var assy = Assembly.ReflectionOnlyLoadFrom(targetAssemblyPath);
			return GetResourceNames(assy);
		}

		public static string[] GetResourceNames(Assembly targetAssembly)
		{
			if (targetAssembly == null)
			{
				throw new ArgumentNullException("targetAssembly");
			}

			var resourceNames = targetAssembly.GetManifestResourceNames();
			return resourceNames;
		}

		public static string ReadStringResource(string targetAssemblyPath, string resourceName)
		{
			if (targetAssemblyPath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("targetAssemblyPath");
			}

			var assy = Assembly.ReflectionOnlyLoadFrom(targetAssemblyPath);

			return ReadStringResource(assy, resourceName);
		}

		public static string ReadStringResource(Assembly targetAssembly, string resourceName)
		{
			if (targetAssembly == null)
			{
				throw new ArgumentNullException("targetAssembly");
			}

			if (resourceName.IsNullOrEmpty())
			{
				throw new ArgumentNullException("resourceName");
			}

			using (var rstream = targetAssembly.GetManifestResourceStream(resourceName))
			{
				if (rstream == null)
				{
					return null;
				}

				using (var reader = new StreamReader(rstream, Encoding.UTF8))
				{
					var data = reader.ReadToEnd();
					return data;
				}
			}
		}

		public static byte[] ReadBinaryResource(string targetAssemblyPath, string resourceName)
		{
			if (targetAssemblyPath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("targetAssemblyPath");
			}

			var assy = Assembly.ReflectionOnlyLoadFrom(targetAssemblyPath);

			return ReadBinaryResource(assy, resourceName);
		}

		public static Stream GetResourceStream(Assembly targetAssembly, string resourceName)
		{
			var rstream = targetAssembly.GetManifestResourceStream(resourceName);
			return rstream;
		}

		public static byte[] ReadBinaryResource(Assembly targetAssembly, string resourceName)
		{
			if (targetAssembly == null)
			{
				throw new ArgumentNullException("targetAssembly");
			}

			if (resourceName.IsNullOrEmpty())
			{
				throw new ArgumentNullException("resourceName");
			}

			using (var rstream = targetAssembly.GetManifestResourceStream(resourceName))
			{
				if (rstream == null)
				{
					return null;
				}

				var resourceLength = rstream.Length;
				var data = new byte[resourceLength];
				rstream.Read(data, 0, (int) resourceLength);

				return data;
			}
		}
	}
}