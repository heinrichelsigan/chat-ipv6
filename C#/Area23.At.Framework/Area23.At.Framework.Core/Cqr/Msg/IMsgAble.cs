namespace Area23.At.Framework.Core.Cqr.Msg
{
   
    /// <summary>
    /// interface for all en-/decryptable entities derived from <see cref="CMsg"/>
    /// </summary>
    public interface IMsgAble
    {
        /// <summary>
        /// <see cref="SerType"/> is serializing type, for now Json = 0x1000, Xml = 0x2000, maybe protbuf later
        /// </summary>
        SerType Cerializer { get; }

        /// <summary>
        /// basic Message is needed for en-/decryption
        /// </summary>
        string Message { get; }

        /// <summary>
        /// simple hash to know, if the pipe is matching
        /// </summary>
        string Hash { get; }

        /// <summary>
        /// standard md5sum of bytes or message string
        /// </summary>
        string Md5Hash { get; }

        /// <summary>
        /// Serializes the current object to a string representation.
        /// </summary>
        /// <returns>A string that represents the serialized form of the object.</returns>
        string Cerialize();

        /// <summary>
        /// Deserializes the specified string to an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="serializedText">The serializedText string to deserialize. Cannot be null or empty.</param>
        /// <returns>An instance of type T deserialized from the serializedText string, or null if the input is null, empty, or cannot be
        /// deserialized to type T.</returns>
        T? DeCerialize<T>(string serializedText);

    }

}
