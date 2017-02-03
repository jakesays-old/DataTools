using System;
using System.IO;

namespace Std.Utility.IO
{
	public static class StreamHelpers
	{
		private const int ReadBufferSize = 16384;
		private const int InitialMemoryStreamSize = 2048;

		/// <summary>
		/// Read the entire contents of a stream in to a byte array.
		/// </summary>
		/// <param name="source">Source stream.</param>
		/// <returns>byte[] containing the contents of the stream.</returns>
		public static byte[] ReadAllBytes(Stream source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!source.CanRead)
			{
				throw new InvalidOperationException("source stream does not support reading");
			}

			byte[] fileData = null;
			using (var bitStream = new MemoryStream(InitialMemoryStreamSize))
			{
				var readBuffer = new byte[ReadBufferSize];
				var byteCount = 0;
				var bytesRead = 0;
				while ((bytesRead = source.Read(readBuffer, 0, ReadBufferSize)) > 0)
				{
					byteCount += bytesRead;
					bitStream.Write(readBuffer, 0, bytesRead);
				}

				fileData = bitStream.ToArray();
			}

			return fileData;
		}
	}
}