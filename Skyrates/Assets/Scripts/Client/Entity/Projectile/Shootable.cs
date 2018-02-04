using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Shootable : MonoBehaviour
{

    // The owner of this shootable (destroyed when shot)
    public GameObject rootParent;

    public GameObject loot;

    private bool markedForDestruction = false;

    // Requires that all colliders are triggers
    // Also requires incoming colliders to be rigidbodies

    private void Start()
    {
        // TODO: Potentially expensive, and not required
        this.checkTriggers();
    }

    private void checkTriggers()
    {
        foreach (Collider collider in this.GetComponents<Collider>())
        {
            collider.isTrigger = true;
        }
    }

    // Called when some non-trigger collider with a rigidbody enters
    private void OnTriggerEnter(Collider other)
    {

        bool destroy = false;

        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile != null && !this.markedForDestruction)
        {
            destroy = true;
            // collider is a projectile
            Destroy(projectile.gameObject);
        }

        if (other.CompareTag("Ram"))
        {
            //ShipFigurehead ram = other.GetComponent<ShipFigurehead>();
            destroy = true;
        }

        if (destroy)
        {
            this.markedForDestruction = true;

            Vector3 position = this.transform.position;

            // Deal damage
            Destroy(this.rootParent);
            
            // Spawn loot
            this.SpawnLoot(position);
        }

    }

    private void SpawnLoot(Vector3 position)
    {
        for (int i = 0; i < Random.Range(5, 20); i++)
        {
            GameObject loot = Instantiate(this.loot, position, Quaternion.identity);
            Rigidbody lootPhysics = loot.GetComponent<Rigidbody>();
            float x = Random.Range(-1, 1);
            float y = 1;
            float z = Random.Range(-1, 1);
            Vector3 direction = new Vector3(x, y, z);
            Vector3 force = direction * 5;
            Quaternion rotateForce = Quaternion.RotateTowards(Quaternion.identity, Quaternion.Euler(direction), 5);
            loot.GetComponent<ConstantForce>().torque = direction * 10;
            lootPhysics.AddForce(force, ForceMode.Impulse);
        }
    }
    
}

