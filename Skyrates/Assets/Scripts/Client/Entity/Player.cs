
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;

// TODO: Remove
public class Player : EntityDynamic
{

    public Ship ShipRoot;

    public void GenerateShip()
    {
        this.ShipRoot.Destroy();
        this.ShipRoot.Generate();
    }

}
