
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Network.Event;
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;

// TODO: Remove
public class EntityPlayer : EntityDynamic
{

    [BitSerialize(3)]
    [HideInInspector]
    public ShipData ShipData;

    [Tooltip("The transform which points towards where the forward direction is.")]
    public Transform View;

    [Tooltip("The root of the render object (must be a child/decendent of this root)")]
    public Transform Render;

    public Ship ShipRoot;

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
        this.View.gameObject.SetActive(false);
        this.Steering = null;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        GameManager.Events.Dispatch(new EventPlayerMoved(this));
    }

    public override bool ShouldDeserialize()
    {
        this.ShipData = this.ShipRoot.ShipData;
        return this.OwnerNetworkID != NetworkComponent.GetSession.NetworkID;
    }

    public override void OnDeserializeSuccess()
    {
        if (this.ShipRoot.ShipData.MustBeRebuilt())
        {
            // TODO: Rebuild the ship
            Debug.LogWarning("rebuild ship");
        }
    }

}
