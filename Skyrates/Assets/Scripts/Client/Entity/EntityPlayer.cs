
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;

// TODO: Remove
public class EntityPlayer : EntityDynamic
{

    public Ship ShipRoot;

    void Awake()
    {
        this.ShipRoot.Destroy();
        this.ShipRoot.Generate();
    }

}
