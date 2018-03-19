using Skyrates.Entity;
using UnityEngine;

// TODO: Obselete. Use ForwardCollisionsTo
public class ShipRenderRoot : MonoBehaviour
{

    public EntityShip Owner;

    protected virtual void OnTriggerEnter(Collider other)
    {
        this.Owner.OnTriggerEnter(other);
    }

}
