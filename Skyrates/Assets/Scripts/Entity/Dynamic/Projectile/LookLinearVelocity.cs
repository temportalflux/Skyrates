using UnityEngine;

namespace Skyrates.Entity
{
	[RequireComponent(typeof(Rigidbody))]
	public class LookLinearVelocity : MonoBehaviour
	{
		private Rigidbody _rigidbody;

		// Use this for initialization
		void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		// Update is called once per frame
		void Update()
		{
			if(_rigidbody.velocity.sqrMagnitude > 0.0f) transform.rotation = Quaternion.LookRotation(_rigidbody.velocity.normalized);
		}
	}
}
