using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using UnityEngine;

public class XInputDispatcher : MonoBehaviour
{

    void OnEnable()
    {
        GameManager.Events.EntityShipHitByProjectile += this.OnEntityHitByProjectile;
    }

    void OnDisable()
    {
        GameManager.Events.EntityShipHitByProjectile -= this.OnEntityHitByProjectile;
    }

    void OnEntityHitByProjectile(GameEvent evt)
    {
        EventEntityShipHitByProjectile evtHit = (EventEntityShipHitByProjectile) evt;

        Transform target = evtHit.Ship.GetRender();
        Transform source = evtHit.Projectile.transform;

        Vector3 targetToSource = source.position - target.position;

        // determines orthogonality of
        // target's right to the vector from target to source
        float orthogonality = Vector3.Dot(target.right, targetToSource);

        // if orthogonality is 
        Debug.Log(orthogonality);

    }

}
