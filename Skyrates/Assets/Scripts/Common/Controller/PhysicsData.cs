using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsData
{

    // NOT SERIALIZED - set during GameState integration

    // If this motion _gameStateData is controlled by SOME client (not necessarily this client)
    public bool IsClientControlled;

    // The controller's identifier
    // If IsClientControlled, this references a specific client as the controller
    // Else, this references some GameObject/AI behavior as controller
    public System.Guid ControllerID;

    // SERIALIZED

    [BitSerialize(0)]
    public Vector3 Position;

    [BitSerialize(1)]
    public Vector3 RotationEuler;

}
