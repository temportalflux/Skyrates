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

        Vector3 forwardTarget = evtHit.Ship.GetRender().forward;
        Vector3 positionSource = evtHit.Projectile.transform.position;



    }

}
