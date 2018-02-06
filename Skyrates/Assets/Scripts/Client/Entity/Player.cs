
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;

// TODO: Remove
public class Player : EntityDynamic
{

    public Ship ShipRoot;

    void Awake()
    {
        this.ShipRoot.Destroy();
        this.ShipRoot.Generate();
    }

}
