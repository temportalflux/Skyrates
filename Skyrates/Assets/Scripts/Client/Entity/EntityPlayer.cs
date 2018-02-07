
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;
using UnityEngine;

// TODO: Remove
public class EntityPlayer : EntityDynamic
{

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

}
