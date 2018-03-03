using UnityEngine;

namespace Skyrates.Client.Entity
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
			transform.rotation = Quaternion.LookRotation(_rigidbody.velocity.normalized);
		}
	}
}
