
namespace Skyrates.Util.Serializing
{

    /// <summary>
    /// Implemented for custom use cases which cannot be defined with
    /// the <see cref="BitSerializeAttribute"/> usage.
    /// </summary>
    public interface ISerializing
    {

        /// <summary>
        /// Returns the size of the packet (the size for a potential byte[])
        /// </summary>
        /// <returns>
        /// the integer length of a byte array to hold this event's _gameStateData
        /// </returns>
        /// <remarks>
        /// Author: Dustin Yost
        /// </remarks>
        int GetSize();

        /// <summary>
        /// Serializes _gameStateData from this event into a byte array
        /// </summary>
        /// <param name="data">The _gameStateData.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <remarks>
        /// Author: Dustin Yost
        /// </remarks>
        void Serialize(ref byte[] data, ref int lastIndex);

        /// <summary>
        /// Deserializes _gameStateData from a byte array into this event's _gameStateData
        /// </summary>
        /// <param name="data">The _gameStateData.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <remarks>
        /// Author: Dustin Yost
        /// </remarks>
        void Deserialize(byte[] data, ref int lastIndex);

    }

}
