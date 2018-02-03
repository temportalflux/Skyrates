using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializing
{

    /// <summary>
    /// Returns the size of the packet (the size for a potential byte[])
    /// </summary>
    /// <returns>
    /// the integer length of a byte array to hold this event's data
    /// </returns>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    int GetSize();

    /// <summary>
    /// Serializes data from this event into a byte array
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="lastIndex">The last index.</param>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    void Serialize(ref byte[] data, ref int lastIndex);

    /// <summary>
    /// Deserializes data from a byte array into this event's data
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="lastIndex">The last index.</param>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    void Deserialize(byte[] data, ref int lastIndex);

}
