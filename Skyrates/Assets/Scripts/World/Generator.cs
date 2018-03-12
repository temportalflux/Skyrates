using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Generator : MonoBehaviour
{

    public GameObject prefab;

    public Transform root;

    public int amount;

    void Awake()
    {
        //this.Generate();
    }

    public void Generate()
    {
        Bounds bounds = this.GetComponent<BoxCollider>().bounds;
        for (int i = 0; i < this.amount; i++)
        {
            this.Generate(this.prefab, bounds);
        }
    }

    private void Generate(GameObject prefab, Bounds bounds)
    {
        Vector3 pos = UnityEngine.Random.insideUnitSphere;
        pos.Scale(bounds.extents);
        pos += bounds.center;

        Vector3 rot = new Vector3(0, UnityEngine.Random.value * 360, 0);

        GameObject generated = Instantiate(prefab, this.root);
        generated.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));

    }
    
}
