
using Skyrates.Client.Network.Event;
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;

// TODO: Remove
public class EntityPlayer : EntityDynamic
{

    [Tooltip("The transform which points towards where the forward direction is.")]
    public Transform View;

    [Tooltip("The root of the render object (must be a child/decendent of this root)")]
    public Transform Render;

    public Ship ShipRoot;

    private bool isDummy = false;

    void Awake()
    {
        this.ShipRoot.Destroy();
        this.ShipRoot.Generate();
    }

    protected override Transform GetView()
    {
        return this.View;
    }

    protected override Transform GetRender()
    {
        return this.Render;
    }

    public void SetDummy()
    {
        this.isDummy = true;
        this.View.gameObject.SetActive(false);
        this.Steering = null;
    }

    public override bool ShouldDeserialize()
    {
        return this.OwnerNetworkID != NetworkComponent.GetSession.NetworkID;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!this.isDummy)
        {
            // TODO: Move this to an event
            // TODO: Reconsider frequency
            NetworkComponent.GetNetwork().Dispatch(new EventRequestSetPlayerPhysics(this.Physics));
        }
    }
}
