
using Skyrates.AI.Formation;

namespace Skyrates.Entity
{

    /// <summary>
    /// Special case of entity ship which is completely controlled by AI.
    /// </summary>
    public class EntityShipMerchant : EntityShipNPC
    {

        public override void OnDetectEntityNearFormation(FormationAgent source, EntityAI other, float distanceFromSource)
        {
            if (other is EntityPlayerShip)
            {
                base.OnDetectEntityNearFormation(source, other, distanceFromSource);
            }
        }

    }

}
