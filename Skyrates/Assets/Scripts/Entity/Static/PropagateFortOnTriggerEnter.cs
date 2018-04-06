using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Entity
{
	public class PropagateFortOnTriggerEnter : MonoBehaviour
	{
		void OnTriggerEnter(Collider other)
		{
			transform.root.GetComponent<EntityFort>().OnTriggerEnter(other);
		}
	}
}