using Skyrates.Entity;
using UnityEngine;

namespace Skyrates.Client.Entity
{
    // TODO: Remove, this is a test class
    public class TestCannonRotation : MonoBehaviour
	{
		//No doxygen.  Only for testing purposes.
		public EntityCannon cannonBase;

		// Update is called once per frame
		void Update()
		{
			Vector3 targetDirection = (cannonBase.TargetPosition - transform.position).normalized;
			targetDirection.y = 0.0f;
			if (!Mathf.Approximately(targetDirection.sqrMagnitude, 0.0f)) transform.localRotation = Quaternion.LookRotation(targetDirection.normalized);
		}
	}
}
