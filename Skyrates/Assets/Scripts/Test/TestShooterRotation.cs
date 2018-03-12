using UnityEngine;

namespace Skyrates.Client.Entity
{
	public class TestShooterRotation : MonoBehaviour
	{
		//No doxygen.  Only for testing purposes.
		public EntityCannon cannonBase;

		// Update is called once per frame
		void Update()
		{
			Vector3 targetDirection = Quaternion.AngleAxis(cannonBase.ArcAngle, cannonBase.ArcAxis) * (cannonBase.TargetPosition - transform.position).normalized;
			if(!Mathf.Approximately(targetDirection.sqrMagnitude, 0.0f)) transform.rotation = Quaternion.LookRotation(targetDirection.normalized);
		}
	}
}
