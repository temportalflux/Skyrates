using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootPivot : MonoBehaviour {

    void OnDrawGizmosSelected()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(this.transform.position, transform.rotation, this.transform.lossyScale * 0.1f);
        Gizmos.matrix = rotationMatrix;
        Gizmos.color = new Color(1, 0, 0, 0.5F);
        Gizmos.DrawCube(Vector3.up * 0.5f, Vector3.one);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
