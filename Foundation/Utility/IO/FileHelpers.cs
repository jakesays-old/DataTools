using System.IO;
using System.Text;

namespace Std.Utility.IO
{
	public static class FileHelpers
	{
		//File.ReadAllText that doesn't lock the file while reading
		public static string ReadTextFile(string sourcePath)
		{
			using (var input = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (var inputStream = new StreamReader(input, Encoding.UTF8))
				{
					var content = inputStream.ReadToEnd();
					return content;
				}
			}
		}

		//File.ReadAllBytes that doesn't lock the file while reading
		public static byte[] ReadBinaryFile(string sourcePath)
		{
			using (var input = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var length = input.Length;
				var data = new byte[length];
				if (length == 0)
				{
					return data;
				}

				//bad juju - if the file is over 4gb
				var bytesRead = input.Read(data, 0, (int) length);

				if (bytesRead != length)
				{
					throw new IOException("Not all of file read");
				}

				return data;
			}
		}
	}
}