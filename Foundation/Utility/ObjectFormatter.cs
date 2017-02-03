using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;

namespace Std.Utility
{
	/// <summary>
	/// An updated version of ObjectSerializer that works around its limitation
	/// of not being able to configure the binary formatter.
	/// </summary>
	public class ObjectFormatter
	{
		private readonly IRemotingFormatter _formatter;

		public ObjectFormatter()
		{
			_formatter = new BinaryFormatter();
		}

		public ObjectFormatter(IRemotingFormatter formatter)
		{
			_formatter = formatter;
		}

		/// <summary>
		/// Creates a deep clone of the object pass in (this means all referenced objects are cloned as well)
		/// </summary>
		/// <typeparam name="TObjectType">Type of the object to clone</typeparam>
		/// <param name="objectToClone">The object to clone</param>
		/// <returns>The deep clone of the object</returns>
		public TObjectType DeepClone<TObjectType>(TObjectType objectToClone)
		{
			var serializedObject = Serialize(objectToClone);
			return Deserialize<TObjectType>(serializedObject);
		}

		/// <summary>
		/// Serializes the specified object to serialize.
		/// </summary>
		/// <typeparam name="TObjectType">Type of the object to serialize.</typeparam>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>Serialized bytes of the object</returns>
		public byte[] Serialize<TObjectType>(TObjectType objectToSerialize)
		{
			byte[] serializedObject;

			using (var memStream = new MemoryStream())
			{
				_formatter.Serialize(memStream, objectToSerialize);
				serializedObject = memStream.ToArray();
			}

			return serializedObject;
		}

		/// <summary>
		/// Deserializes the specified object to deserialize.
		/// </summary>
		/// <typeparam name="TObjectType">Type of the object to deserialize.</typeparam>
		/// <param name="objectToDeserialize">The object to deserialize.</param>
		/// <returns>Deserialized object</returns>
		public TObjectType Deserialize<TObjectType>(byte[] objectToDeserialize)
		{
			TObjectType deserializedObject;
			using (var memStream = new MemoryStream(objectToDeserialize))
			{
				deserializedObject = (TObjectType) _formatter.Deserialize(memStream);
			}

			return deserializedObject;
		}
	}
}
